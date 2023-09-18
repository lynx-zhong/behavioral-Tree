using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class BehaviourTreeWindow : EditorWindow
{
    [MenuItem("ZzWindow/BehaviourTreeWindow")]
    public static void ShowExample()
    {
        BehaviourTreeWindow wnd = GetWindow<BehaviourTreeWindow>();
        wnd.titleContent = new GUIContent("BehaviourTreeWindow");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviourTree/Editor/Scripts/BehaviourTreeWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);
    }
}