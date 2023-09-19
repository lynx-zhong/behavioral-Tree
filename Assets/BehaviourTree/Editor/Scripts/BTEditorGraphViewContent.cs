using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTreeEditor
{
    public class BTEditorGraphViewContent : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BTEditorGraphViewContent,UxmlTraits> { }

        string styleSheetPath = @"Assets\BehaviourTree\Editor\Scripts\BTEdiotrGraphViewContent.uss";

        public BTEditorGraphViewContent()
        {
            Insert(0,new GridBackground());
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);
            styleSheets.Add(styleSheet);
        }
    }
}
