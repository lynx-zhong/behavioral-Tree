using UnityEditor;
using UnityEngine;
using BT_BehaviourTree;

namespace BehaviourTreeEditor
{
    public class BTLogicEditorNode : EditorWindow
    {
        //public string className;

        //string nodeCenterIconPath = BTEditorGlobalUtil.NodeCenterIconPath;

        //public BTLogicEditorNode(Rect rect)
        //{
        //    nodePos = rect.position;
        //    windowRect = rect;
        //}

        //public override void DrawWindow(Vector2 mousePos, BTNodeEditor tNodeEditorBase)
        //{
        //    base.DrawWindow(mousePos, tNodeEditorBase);

        //    // 选中状态
        //    if (tNodeEditorBase != null && tNodeEditorBase.windowID == windowID)
        //        nodeCenterIconPath = BTEditorGlobalUtil.NodeSelectedCenterIconPath;
        //    else
        //        nodeCenterIconPath = BTEditorGlobalUtil.NodeCenterIconPath;
        //}


        ///// <summary>
        ///// WindowCallBack 是被DrawWindow 调用的
        ///// </summary>
        //public override void WindowCallBack(int selfWindowID)
        //{
        //    BTEditorGlobalUtil.EditorLoadTexture(BTEditorGlobalUtil.ItemTopIconPath, BTEditorGlobalUtil.ItemTitleTopRect);
        //    BTEditorGlobalUtil.EditorLoadTexture(BTEditorGlobalUtil.ItemDownIconPath, BTEditorGlobalUtil.ItemTitleDownRect);
        //    BTEditorGlobalUtil.EditorLoadTexture(nodeCenterIconPath, BTEditorGlobalUtil.ItemCenterRect);
        //    BTEditorGlobalUtil.EditorLoadTexture(BTEditorGlobalUtil.DefualtIconPath, BTEditorGlobalUtil.EnterIconRect);
        //    BTEditorGlobalUtil.EditorLoadTexture(BTEditorGlobalUtil.IconBoxPath, BTEditorGlobalUtil.IconBoxRect);

        //    GUIStyle iconNameStyle = BTEditorGlobalUtil.GetNodeNameGUIStyle();
        //    EditorGUI.LabelField(BTEditorGlobalUtil.TitleLableRect, "Enter", iconNameStyle);
        //}

    }
}
