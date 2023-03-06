using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

namespace BehaviourTreeEditor
{
    public class BTNodeEditor: EditorWindow
    {
        #region 节点icon尺寸相关常量
        float nodeWidth { get { return 115 / 11 * className.Length > 115 ? 115 / 11 * className.Length : 115; } }
        const int nodeHeight = 100;
        const float nodeBgSpace = 7.5f;
        const int nodeConnectIconWidth = 42;
        const int nodeConnectIconHeight = 16;
        const int centerIconLength = 44;

        public string ItemTopIconPath = "DarkTaskConnectionTop";
        public string NodeCenterIconPath = "DarkTaskCompact";
        public string NodeSelectedCenterIconPath = "DarkTaskSelectedCompact";
        public string ItemDownIconPath = "DarkTaskConnectionBottom";
        public string IconBoxPath = "DarkTaskBorder";
        public string RootIconPath = "DarkEntryIcon";

        #endregion

        #region 节点自身属性
        public int windowID = 0;
        public Rect windowRect;
        public Vector2 nodePos = Vector2.zero;
        #endregion



        #region 节点icon相关
        private float GlobalSizeScale { get { return BTEditorGlobalUtil.GlobalSizeScale; } }
        public Vector2 ItemEmptySize { get { return new Vector2(nodeWidth, nodeHeight); } }
        public Rect ItemBgRect { get { return new Rect(0, nodeBgSpace * GlobalSizeScale, nodeWidth * GlobalSizeScale, (nodeHeight - nodeBgSpace*2) * GlobalSizeScale); } }
        public Rect NodeTopConnectIconRect { get { return new Rect((nodeWidth / 2 - nodeConnectIconWidth / 2) * GlobalSizeScale, 0 * GlobalSizeScale, nodeConnectIconWidth * GlobalSizeScale, nodeConnectIconHeight * GlobalSizeScale); } }
        public Rect NodeDownConnectIconRect { get { return new Rect((nodeWidth / 2 - nodeConnectIconWidth / 2) * GlobalSizeScale, (nodeHeight - nodeBgSpace * 2) * GlobalSizeScale, nodeConnectIconWidth * GlobalSizeScale, nodeConnectIconHeight * GlobalSizeScale); } }

        public Rect CenterIconRect { get { return new Rect((nodeWidth / 2 - centerIconLength/2) * GlobalSizeScale, 18 * GlobalSizeScale, centerIconLength * GlobalSizeScale, centerIconLength * GlobalSizeScale); } }
        public Rect IconBoxRect { get { return new Rect((nodeWidth / 2 - 22)  * GlobalSizeScale, 18 * GlobalSizeScale, centerIconLength * GlobalSizeScale, centerIconLength * GlobalSizeScale); } }
        public Rect TitleLableRect { get { return new Rect(0, 58 * GlobalSizeScale, nodeWidth * GlobalSizeScale, 30 * GlobalSizeScale); } }
        #endregion

        string nodeCenterIconPath;
        public string className;

        public BTNodeEditor parentNode;
        public List<BTNodeEditor> childsNode;


        public BTNodeEditor(Vector2 nodePos, string className)
        {
            Init();

            this.nodePos = nodePos;
            this.className = className;
        }

        void Init() 
        {
            windowID = (int)DateTime.Now.ToFileTimeUtc();
            parentNode = null;
            childsNode = new List<BTNodeEditor>();
        }

        public void AddChildNode(BTNodeEditor childNode)
        {
            childsNode.Add(childNode);
            childNode.parentNode = this;
        }

        /// <summary>
        /// 绘制行为树节点弹窗
        /// </summary>
        /// <param name="curSelectedNode"> 当前选中的节点 </param>
        public virtual void DrawNodeWindow(Vector2 mousePos,BTNodeEditor curSelectedNode) 
        {
            windowRect = new Rect(nodePos, ItemEmptySize * BTEditorGlobalUtil.GlobalSizeScale);
            windowRect = GUI.Window(windowID, windowRect, WindowCreateCallBackDrawInfo, "", "RL Element");

            if (curSelectedNode != null && curSelectedNode.windowID == windowID)
                nodeCenterIconPath = NodeSelectedCenterIconPath;
            else
                nodeCenterIconPath = NodeCenterIconPath;
        }

        /// <summary>
        /// 窗口创建回调 开始绘制窗口
        /// </summary>
        public virtual void WindowCreateCallBackDrawInfo(int selfWindowID)
        {
            // 上下帽子
            BTEditorGlobalUtil.EditorLoadTexture(ItemTopIconPath, NodeTopConnectIconRect);
            BTEditorGlobalUtil.EditorLoadTexture(ItemDownIconPath, NodeDownConnectIconRect);

            // 中间图形
            BTEditorGlobalUtil.EditorLoadTexture(nodeCenterIconPath, new Rect(ItemBgRect.position, new Vector2(ItemBgRect.width, ItemBgRect.height)));
            BTEditorGlobalUtil.EditorLoadTexture(IconBoxPath, IconBoxRect);

            BTEditorGlobalUtil.EditorLoadTexture(RootIconPath, CenterIconRect);
            GUIStyle iconNameStyle = BTEditorGlobalUtil.GetNodeNameGUIStyle();
            EditorGUI.LabelField(TitleLableRect, className, iconNameStyle);
        }


        #region 画线
        /// <summary>
        /// 画节点的连接线
        /// </summary>
        public virtual void DrawNodeConnectLine()
        {
            // 父物体到自己的线
            if (parentNode != null)
            {
                Vector2 startPos = new Vector2(parentNode.windowRect.x + parentNode.windowRect.width / 2, parentNode.windowRect.y + parentNode.windowRect.height - 20);
                Vector2 endPos = new Vector2(windowRect.x + windowRect.width / 2, windowRect.y + 20);
                DrawLineTool(startPos, endPos);
            }
        }

        public void DrawLineTool(Vector2 startPos, Vector2 endPos)
        {
            Vector2 centerPos1;
            Vector2 centerPos2;

            if (Mathf.Abs(endPos.x - startPos.x) <= 10)
            {
                endPos.x = startPos.x;
                centerPos1 = new Vector2(startPos.x, (startPos.y + endPos.y) / 2);
                centerPos2 = new Vector2(endPos.x, (startPos.y + endPos.y) / 2);
            }
            else
            {
                Rect centerRect = GetCenterTheSameParentCenter();
                if (centerRect.y < (parentNode.windowRect.y + ItemEmptySize.y))
                    centerRect.y = (parentNode.windowRect.y + ItemEmptySize.y);

                centerPos1 = new Vector2(startPos.x, (startPos.y + centerRect.y) / 2);
                centerPos2 = new Vector2(endPos.x, (startPos.y + centerRect.y) / 2);
            }
            Handles.color = Color.white;

            Handles.DrawLine(startPos, centerPos1);
            Handles.DrawLine(centerPos1, centerPos2);
            Handles.DrawLine(centerPos2, endPos);
        }

        public Rect GetCenterTheSameParentCenter()
        {
            if (parentNode != null && childsNode != null)
            {
                Rect maxHightRect = new Rect(0, 1000000, 0, 0);
                for (int i = 0; i < parentNode.childsNode.Count; i++)
                {
                    if (parentNode.childsNode[i].windowRect.y <= maxHightRect.y)
                        maxHightRect = parentNode.childsNode[i].windowRect;
                }

                return maxHightRect;
            }
            return new Rect(0, 0, 0, 0);
        }
        #endregion
    }
}
