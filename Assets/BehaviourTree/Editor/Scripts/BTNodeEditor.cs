using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace BehaviourTreeEditor
{
    public class BTNodeEditor: EditorWindow
    {
        #region 节点自身属性
        public int windowID = 0;    // 无用
        public Rect windowRect;
        public Vector2 nodePos = Vector2.zero;
        #endregion

        #region 节点icon相关
        public string ItemTopIconPath = "DarkTaskConnectionTop";
        public string NodeCenterIconPath = "DarkTaskCompact";
        public string NodeSelectedCenterIconPath = "DarkTaskSelectedCompact";
        public string ItemDownIconPath = "DarkTaskConnectionBottom";


        private float GlobalSizeScale { get { return BTEditorGlobalUtil.GlobalSizeScale; } }

        public Rect ItemTitleTopRect { get { return new Rect(37 * GlobalSizeScale, 0 * GlobalSizeScale, 42 * GlobalSizeScale, 16 * GlobalSizeScale); } }

        public Rect ItemTitleDownRect { get { return new Rect(37 * GlobalSizeScale, 84 * GlobalSizeScale, 42 * GlobalSizeScale, 16 * GlobalSizeScale); } }

        public string RootIconPath = "DarkEntryIcon";

        public Rect DefualtIconRect { get { return new Rect(37 * GlobalSizeScale, 18 * GlobalSizeScale, 44 * GlobalSizeScale, 44 * GlobalSizeScale); } }

        public string IconBoxPath = "DarkTaskBorder";

        //public Rect IconBoxRect { get { return new Rect(37 * GlobalSizeScale, 18 * GlobalSizeScale, 44 * GlobalSizeScale, 44 * GlobalSizeScale); } }
        public Rect IconBoxRect { get { return new Rect((itemWidth / 2 - 22)  * GlobalSizeScale, 18 * GlobalSizeScale, 44 * GlobalSizeScale, 44 * GlobalSizeScale); } }

        public Rect TitleLableRect { get { return new Rect(0, 58 * GlobalSizeScale, 115 * GlobalSizeScale, 30 * GlobalSizeScale); } }

        public Rect ItemCenterRect { get { return new Rect(0, 7.5f * GlobalSizeScale, itemWidth * GlobalSizeScale, 85 * GlobalSizeScale); } }
        public Vector2 ItemEmptySize { get { return new Vector2(itemWidth, 100); } }

        private float itemWidth { get { return 115 / 11 * className.Length > 115 ? 115 / 11 * className.Length : 115; } }
        #endregion

        string nodeCenterIconPath;
        public string className;

        public BTNodeEditor parentNode;
        public List<BTNodeEditor> childsNode;


        public BTNodeEditor(Vector2 _nodePos, string ClassName)
        {
            Init();

            nodePos = _nodePos;
            className = ClassName;
        }

        void Init() 
        {
            windowID = (int)DateTime.Now.ToFileTimeUtc();
            parentNode = null;
            childsNode = new List<BTNodeEditor>();
        }


        // 根据改变了的比例绘制
        public virtual void DrawWindow(Vector2 mousePos,BTNodeEditor tNodeEditorBase) 
        {
            windowRect = new Rect(nodePos, ItemEmptySize * BTEditorGlobalUtil.GlobalSizeScale);
            windowRect = GUI.Window(windowID, windowRect, WindowCallBack, "", "RL Element");

            if (tNodeEditorBase != null && tNodeEditorBase.windowID == windowID)
                nodeCenterIconPath = NodeSelectedCenterIconPath;
            else
                nodeCenterIconPath = NodeCenterIconPath;
        }

        public virtual void WindowCallBack(int selfWindowID)
        {
            // 上下帽子
            BTEditorGlobalUtil.EditorLoadTexture(ItemTopIconPath, ItemTitleTopRect);
            BTEditorGlobalUtil.EditorLoadTexture(ItemDownIconPath, ItemTitleDownRect);

            // 中间图形
            BTEditorGlobalUtil.EditorLoadTexture(nodeCenterIconPath, new Rect(ItemCenterRect.position, new Vector2(ItemCenterRect.width,ItemCenterRect.height)));
            BTEditorGlobalUtil.EditorLoadTexture(IconBoxPath, IconBoxRect);

            BTEditorGlobalUtil.EditorLoadTexture(RootIconPath, DefualtIconRect);
            GUIStyle iconNameStyle = BTEditorGlobalUtil.GetNodeNameGUIStyle();
            EditorGUI.LabelField(TitleLableRect, className, iconNameStyle);
        }


        #region 画线
        public virtual void DrawCurve()
        {
            // 父物体到自己的线
            if (parentNode != null)
            {
                Vector2 startPos = new Vector2(parentNode.windowRect.x + parentNode.windowRect.width / 2, parentNode.windowRect.y + parentNode.windowRect.height - 20);
                Vector2 endPos = new Vector2(windowRect.x + windowRect.width / 2, windowRect.y + 20);
                DrawCurveTool(startPos, endPos);
            }
        }

        public void DrawCurveTool(Vector2 startPos, Vector2 endPos)
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
