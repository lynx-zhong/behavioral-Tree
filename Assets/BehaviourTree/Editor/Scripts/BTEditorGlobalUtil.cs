using UnityEngine;
using System;
using UnityEditor;

namespace BehaviourTreeEditor
{
    public class BTEditorGlobalUtil
    {
        #region 单例
        private static BTEditorGlobalUtil instance;
        public static BTEditorGlobalUtil Instance 
        {
            get
            {
                if (instance == null)
                {
                    instance = new BTEditorGlobalUtil();

                }
                return instance;
            }
        }
        private BTEditorGlobalUtil()
        {

        }
        #endregion

        public static GUIStyle GetNodeNameGUIStyle()
        {
            GUIStyle iconNameStyle = new GUIStyle() { };
            iconNameStyle.normal.textColor = Color.white;
            iconNameStyle.fontSize = (int)Math.Floor(12 * GlobalSizeScale);
            iconNameStyle.alignment = TextAnchor.MiddleCenter;

            return iconNameStyle;
        }

        private static float globalSizeScale = 1;
        /// <summary>
        /// 全局大小比例
        /// </summary>
        public static float GlobalSizeScale 
        {
            get 
            {
                float value = globalSizeScale > 1 ? 1 : globalSizeScale;
                value = value <= 0.2f ? 0.2f : value;
                return value; 
            }
            set { globalSizeScale = value; }
        }


        private static float globalPosScale = 1;
        /// <summary>
        /// 计算位置用的比例 滑动滚轮时的变化比例
        /// </summary>
        public static float GlobalPosScale 
        {
            get { return globalPosScale; }
            set { globalPosScale = value; }
        }


        public static string DefalutIconFloderPath = "BehaviourTree/Editor/Icon";
        public static void EditorLoadTexture(string path, Rect rect)
        {
            string iconPath = string.Format("Assets/{0}/{1}.png", DefalutIconFloderPath, path);
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(iconPath);
            if (texture == null)
            {
                Debug.LogError("load texture is error path: " + iconPath + "   check defalutIconFolderPath is  correct");
                return;
            }
            GUI.DrawTexture(rect, texture, ScaleMode.StretchToFill, true,10000);
        }
    }
}
