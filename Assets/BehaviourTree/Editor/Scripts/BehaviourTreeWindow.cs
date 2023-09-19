using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTreeEditor
{
    public class BehaviourTreeWindow : EditorWindow
    {
        static BTEditorGraphViewContent contentGraphView;

        [MenuItem("ZzWindow/BehaviourTreeWindow")]
        public static void ShowExample()
        {
            BehaviourTreeWindow wnd = GetWindow<BehaviourTreeWindow>("BehaviourTreeWindow");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviourTree/Editor/Scripts/BehaviourTreeWindow.uxml");
            visualTree.CloneTree(root);

            root.Add(contentGraphView);
        }
    }
}
