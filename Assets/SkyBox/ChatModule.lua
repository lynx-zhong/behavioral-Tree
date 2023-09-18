--[[********************************************************************
* author:   zhonglinxin
* purpose:  聊天 module 层
* ********************************************************************--]]

require "UI.UIChat.Data.ChatData"
local chatMessageData = require "UI.UIChat.Data.ChatMessageData"

---@class ChatModule:BaseClass
ChatModule = BaseClass("ChatModule")

function ChatModule:Init()
    ChatData:OnInit()

    -- data
    self._isReceiveStrangerChat = false

    self._events = {}
    self._events[EventName.ChatLoginSuccessEvent] = Bind(self, self.ChatLoginSuccessEvent)
    self._events[EventName.NTF_ROLE_DATA_GET_SUCCES] = Bind(self,self.OnRoleDataGetSucces)
    GameEvent.AddMultiple(self._events)

    self._rspPack = {}
	self._rspPack[MsgIds.PROTO_PUSHCHATMESSAGERES] = Bind(self,self.PushChatMessageRes)
    self._rspPack[MsgIds.PROTO_REQUESTCHATREPORTRSP] = Bind(self,self.RequestChatReportRsp)
    self._rspPack[MsgIds.PROTO_REQUESTGETCHATTOKENRSP] = Bind(self,self.RequestGetChatTokenRsp)
    self._rspPack[MsgIds.PROTO_REQUESTGETSHIELDSTRANGERRSP] = Bind(self,self.RequestGetShieldStrangerRsp)
    self._rspPack[MsgIds.PROTO_REQUESTSETSHIELDSTRANGERRSP] = Bind(self,self.RequestSetShieldStrangerRsp)
	HallConnector:OnRegisterNetEvent(self._rspPack)
end

---@return ChatModule
function ChatModule:GetInstance()
    return ChatModule
end

function ChatModule:Destroy()
    ChatData:OnDelete()

    self._isReceiveStrangerChat = nil

    GameEvent.DelMultiple(self._events)
    HallConnector:OnRemoveNetEvent(self._rspPack)
end

---服务器那边在进入大厅的时候 请求token
function ChatModule:OnRoleDataGetSucces()
    self:RequestGetChatTokenReq()
    self:RequestGetShieldStrangerReq()
end

function ChatModule:ChatLoginSuccessEvent()
    ChatData:InitChatChannel()

    -- 请求离线聊天的频道
    local cacheChatMidID = ChatSystem:GetLastChatMidCache()
    self:RequestGetOfflineMessage(cacheChatMidID)
    
    -- self:RequestGetOfflineMessage('cj6dc165m59gcbkhl460')
end

---发送聊天消息 Http 请求
---@param message SendChatMessageReq
function ChatModule:RequestSendMessageReq(message)
    local fromDic = {}
    fromDic["target_id"] = tostring(message.target_id)
    fromDic["content"] = message.content

    local header = {}
    local content = "Bearer " .. HallConnector:GetChatToken()
    header["Authorization"] = content

    local endStr = (message.chatType and message.chatType == MsgType.Room) and "/send/room" or "/send/private"
    local chatUrl = HallConnector:GetChatSvrHttpUrl() .. endStr

    ---@type ChatChannelClass
    local channelData = {}
    channelData.channelType = message.chatType
    ChatData:SetLastSendChatTime(channelData)

    CSSimpleHttp.HttpPost(chatUrl,fromDic,header,function (webRequest)
        if webRequest.result == CS.UnityEngine.Networking.UnityWebRequest.Result.Success then
            GameEvent.Dispatch(EventName.NTF_CHAT_MESSAGE_SEND_SUCCES,message.content)
            Logger.Log(string.format("<color=#2BC328> send chat message </color> is ok %s",message.content))

        elseif webRequest.result == CS.UnityEngine.Networking.UnityWebRequest.Result.InProgress then

        else
            local dataTab = ljson.decode(webRequest.downloadHandler.text)
            if dataTab and dataTab.error == "target_shield_strangers" then
                GlobalTipService:OpenMessageTip({contentKey = "chat_defure_strange"})
            
            elseif dataTab and dataTab.error == "CD..." then
                ChatData:SetLastSendChatTime(channelData)
                GlobalTipService:OpenMessageTip({contentKey = "chat_in_cd"})

            elseif dataTab and dataTab.error == "in_black_list" then
                ChatData:SetLastSendChatTime(channelData)
                GlobalTipService:OpenMessageTip({contentKey = "chat_in_black_list"})

            elseif dataTab and dataTab.error == "chat_content_max_characters" then
                GlobalTipService:OpenMessageTip({contentKey = "chat_content_is_limit"})

            else
                Logger.LogError("send chat message error ! svr info:  " .. webRequest.downloadHandler.text)
                GlobalTipService:OpenMessageTip({content = "发送失败：  " .. webRequest.downloadHandler.text})
            end
        end
    end,3)
end

---接收聊天推送消息
---@param rsp proto.PushChatMessageRes
function ChatModule:PushChatMessageRes(rsp, req, client)
    -- 收到消息，把消息转成客户端使用得data
    local messageData = nil
    local chatMain = UIManager:GetInstance():GetWindow(UIWindowNames.UIChatMain)

    -- 界面没打开的时候 就不接受 世界聊天了
    if chatMain or (not chatMain and rsp.type == MsgType.Private) then
        messageData = chatMessageData.New(rsp)

        -- 添加进list
        -- ChatData:SetMessagePosY(messageData)
        ChatData:AddChatMessage(messageData)
        ChatData:CheckAndAddChatChannel(messageData)
    end

    -- 红点
    self:ReceiveNewMessageCheckRed(rsp,chatMain,messageData)

    if chatMain then
        ChatSystem:SetLastChatMidCache(rsp.mid)
    end

    -- 事件
    if messageData then
        GameEvent.Dispatch(EventName.NTF_CHAT_NEW_MESSAGE_REFRESH,messageData)
    end
end

---获取聊天消息记录 Http
---@param arg ChatRefreshType
function ChatModule:RequestGetHistoryMessage(channelID,channelType,num,min_mid,arg)
    local endStr = (channelType == MsgType.Room) and "/message/room" or "/message/private"
    local chatUrl = HallConnector:GetChatSvrHttpUrl() .. endStr

    if min_mid then
        chatUrl = string.format("%s?target_id=%s&limit=%s&min_mid=%s",chatUrl,channelID,num,min_mid)
    else
        ChatData:ClearTargetChannelMessage(channelID)
        chatUrl = string.format("%s?target_id=%s&limit=%s",chatUrl,channelID,num)
    end

    local header = {}
    local content = "Bearer " .. HallConnector:GetChatToken()
    header["Authorization"] = content

    CSSimpleHttp.HttpGet(chatUrl,header,function (webRequest)
        if webRequest.result == CS.UnityEngine.Networking.UnityWebRequest.Result.Success then
            Logger.Log(string.format("<color=#2BC328> get chat message channelID %s </color> is ok  info:  %s",channelID,webRequest.downloadHandler.text))

            ---@type HTTPChatMessageList
            local result = ljson.decode(webRequest.downloadHandler.text)
            if result and result.code == EChatHttpCode.Ok and result.result and result.result ~= "null" then
                ChatData:AddMessageList(result.result)
            end
            GameEvent.Dispatch(EventName.NTF_CHAT_MESSAGE_REQUEST_SUCCES,arg)

            self:CheckIsShowChatRedPoint()

        elseif webRequest.result == CS.UnityEngine.Networking.UnityWebRequest.Result.InProgress then

        else
            Logger.LogError("get chat message error ! svr info:  " .. webRequest.downloadHandler.text)
        end
    end,3)
end

---获取离线私聊的玩家频道 Http
function ChatModule:RequestGetOfflineMessage(mid)
    local chatUrl = HallConnector:GetChatSvrHttpUrl() .. "/message/private/offline"

    if mid ~= "" then
        chatUrl = string.format("%s?mid=%s",chatUrl,mid)
    end

    local header = {}
    local content = "Bearer " .. HallConnector:GetChatToken()
    header["Authorization"] = content

    CSSimpleHttp.HttpGet(chatUrl,header,function (webRequest)
        if webRequest.result == CS.UnityEngine.Networking.UnityWebRequest.Result.Success then
            -- Logger.Log(string.format("<color=#2BC328>  请求聊天离线消息成功 </color> info:  " ..  webRequest.downloadHandler.text))

            local result = ljson.decode(webRequest.downloadHandler.text)
            if result and result.code == EChatHttpCode.Ok and result.result and result.result ~= "null" then
                self:RequestOffLineMessage(result.result)
            end

        elseif webRequest.result == CS.UnityEngine.Networking.UnityWebRequest.Result.InProgress then

        else
            Logger.LogError(" get offline chat message error ! svr info:  " .. webRequest.downloadHandler.text)
            GlobalTipService:OpenMessageTip({content = "请求离线聊天信息失败  " .. webRequest.downloadHandler.text})
        end
    end,3)
end

---@param channelIDs string[]
function ChatModule:RequestOffLineMessage(channelIDs)
    for key, value in pairs(channelIDs) do
        ChatData:AddChatChannel(key)
    end

    -- 通过聊天频道请求所有私聊数据，和该对象最近的20条私聊（世界聊天打开界面的时候 再请求）
    local channelList = ChatData:GetChatChannelList()
    for key, value in pairs(channelList) do
        if value.channelType == MsgType.Private then
            local num = ChatSystem:GetSingleRequestMaxMessageNum(value)
            ChatModule:RequestGetHistoryMessage(value.channelID,value.channelType,num,false)
        end
    end
end

---聊天举报 req
function ChatModule:RequestChatReportReq(arg)
    HallConnector:SendMessage(MsgIds.PROTO_REQUESTCHATREPORTREQ,arg)
end

---聊天举报 rsp
function ChatModule:RequestChatReportRsp(rsp, req, client)
    FriendModule:RequestBlackListReq()
end

---进入大厅获取聊天token req
function ChatModule:RequestGetChatTokenReq()
    HallConnector:SendMessage(MsgIds.PROTO_REQUESTGETCHATTOKENREQ)
end

---获取聊天token 获取成功后 连接websocket
---@param rsp proto.RequestGetChatTokenRsp
function ChatModule:RequestGetChatTokenRsp(rsp, req, client)
    HallConnector:SetChatToken(rsp.token)
end

---获取是否屏蔽陌生人私聊
function ChatModule:RequestGetShieldStrangerReq()
    HallConnector:SendMessage(MsgIds.PROTO_REQUESTGETSHIELDSTRANGERREQ)
end

---获取是否屏蔽陌生人私聊
---@param rsp proto.RequestGetShieldStrangerRsp
function ChatModule:RequestGetShieldStrangerRsp(rsp,req)
    self._isReceiveStrangerChat = rsp.shield
end

---设置是否屏蔽陌生人私聊
---@param arg proto.RequestSetShieldStrangerReq
function ChatModule:RequestSetShieldStrangerReq(arg)
    HallConnector:SendMessage(MsgIds.PROTO_REQUESTSETSHIELDSTRANGERREQ,arg)
end

---设置是否屏蔽陌生人私聊
---@param rsp proto.RequestSetShieldStrangerRsp
---@param req proto.RequestSetShieldStrangerReq
function ChatModule:RequestSetShieldStrangerRsp(rsp,req)
    self._isReceiveStrangerChat = req.shield
end

function ChatModule:GetIsReceiveStrangerMessage()
    return self._isReceiveStrangerChat
end

---------------------------------------------------- 红点检测逻辑 ----------------------------------------------------

---收到新消息 判断是否展示红点 不走统一计算逻辑，统一计算逻辑更复杂
---@param rsp proto.PushChatMessageRes
---@param UIChatMain UIWindow
---@param chatMessageData ChatMessageData
function ChatModule:ReceiveNewMessageCheckRed(rsp,chatMain,chatMessageData)
    if rsp.type == MsgType.Private then
        if not chatMain then
            RedDotManager:ChangeValue(RedDotConfig.ChatMessage,true)
        elseif chatMain and chatMain.View and chatMessageData then
            local curSelectedChannelData = ChatData:GetSelectedChatChannelData()
            if curSelectedChannelData and curSelectedChannelData.channelID ~= chatMessageData:GetMessageChannelID() then
                RedDotManager:ChangeValue(RedDotConfig.ChatMessage,true)
            end
        end
    end
end

---检测是否显示红点
function ChatModule:CheckIsShowChatRedPoint()
    local channelList = ChatData:GetChatChannelList()
    ---@type table<string,ChatChannelClass>
    local channelDic = {}
    for key, value in pairs(channelList) do
        channelDic[value.channelID] = value
    end

    local privateChatCacheLastMid = ChatSystem:GetChatPrivateRedPointDataList()
    local chatDatas = ChatData:GetAllChatMessageData()
    for channelID, ChatMessageDataList in pairs(chatDatas) do
        local channelData = channelDic[channelID]

        if channelData and channelData.channelType == MsgType.Private then
            local lastMid = privateChatCacheLastMid[channelID]
            if not lastMid then
                RedDotManager:ChangeValue(RedDotConfig.ChatMessage,true)
                return
            end

            -- Logger.LogError('lastMid:  ',lastMid,'   -------------->',ChatMessageDataList[#ChatMessageDataList]:GetMessageMid())
            if #ChatMessageDataList > 0 and ChatMessageDataList[#ChatMessageDataList] and lastMid < ChatMessageDataList[#ChatMessageDataList]:GetMessageMid() then
                RedDotManager:ChangeValue(RedDotConfig.ChatMessage,true)
                return
            end
        end
    end

    RedDotManager:ChangeValue(RedDotConfig.ChatMessage,false)
end

---------------------------------------------------- 红点检测逻辑 ----------------------------------------------------