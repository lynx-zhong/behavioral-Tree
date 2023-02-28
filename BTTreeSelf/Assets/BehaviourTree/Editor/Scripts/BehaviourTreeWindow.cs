using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using BT_BehaviourTree;
using System.Reflection;
using System;

namespace BehaviourTreeEditor
{
    public enum EMouseButton 
    {
        left = 0,
        right = 1,
        middle = 2,
    }

    public enum OperationType 
    {
        addState,
        addChildState,
        deleteNode,
    }

    public class BehaviourTreeWindow : EditorWindow
    {
        #region 变量
        public static BehaviourTreeWindow behaviourTreeEditor;

        /// <summary>
        /// 展示树节点布局的工作空间 右边
        /// </summary>
        Rect treeStructShowRect;

        /// <summary>
        /// 左边的操作空间 行为树的详细操作
        /// </summary>
        Rect leftBehaviourOperationRect;

        /// <summary>
        /// 页面所有的 Node
        /// </summary>
        List<BTNodeEditor> nodes = new List<BTNodeEditor>();

        /// <summary>
        /// 鼠标右键 或 鼠标左键 选择的物体
        /// </summary>
        BTNodeEditor selectedNode;

        Vector2 mousePosition = Vector2.zero;

        /// <summary>
        /// 检测是否点击在窗口
        /// </summary>
        bool clickedOnWindow;
        #endregion


        [MenuItem("BehaviourTree/OpenEditor")]
        public static void OpenEditor() 
        {
            behaviourTreeEditor = GetWindow<BehaviourTreeWindow>(typeof(BehaviourTreeWindow).Name);
        }


        private void OnEnable()
        {
            // 初始化
            {
                nodes.Clear();
                selectedNode = null;
                clickedOnWindow = false;
                mousePosition = Vector2.zero;
                BTEditorGlobalUtil.GlobalSizeScale = 1;
            }
        }

        private void OnGUI()
        {
            DrawWorkSpace();

            GUILayout.BeginArea(treeStructShowRect);

            DrawGrid();

            // 检测点击事件
            UserInput(Event.current);

            DrawWindows();

            GUILayout.EndArea();
        }

        #region 用户输入
        private void UserInput(Event _event) 
        {
            // 初始化输入
            BTEditorGlobalUtil.GlobalPosScale = 1;
            mousePosition = Event.current.mousePosition;

            if (_event.button == (int)EMouseButton.right && _event.type == EventType.MouseDown)
                RightMouseClick(_event);

            else if (_event.type == EventType.ScrollWheel)
                ScrollWheel(_event);

            else if (_event.button == (int)EMouseButton.middle && _event.type == EventType.MouseDrag)
                ScrollWheelDrag(_event);

            else if (_event.button == (int)EMouseButton.left && _event.type == EventType.MouseDown)
                LeftMouseClick(_event);

            else if (_event.button == (int)EMouseButton.left && _event.type == EventType.MouseDrag)
                OnLeftMouseDrag(_event);

            else if (_event.button == (int)EMouseButton.left && _event.type == EventType.MouseUp){

            }

            clickedOnWindow = false;
        }

        /// <summary>
        /// 点击背景 操作（添加节点）
        /// </summary>
        void HandleBackgroudAddNode(Event e)
        {
            GenericMenu menu = new GenericMenu();

            // 逻辑节点
            {
                GetLocalSpritsShowToMenu<BTNode>(menu, "逻辑节点/",OperationType.addState);
            }

            // 行为节点
            {
                GetLocalSpritsShowToMenu<BTAction>( menu, "行为节点/", OperationType.addState);
            }

            menu.ShowAsContext();
        }

        /// <summary>
        /// 点击节点，修改当前节点
        /// </summary>
        void NodeOnClick(Event e)
        {
            GenericMenu menu = new GenericMenu();

            // 添加子节点
            {
                GetLocalSpritsShowToMenu<BTNode>(menu, "添加子节点/逻辑节点/",OperationType.addChildState);
                GetLocalSpritsShowToMenu<BTAction>(menu, "添加子节点/行为节点/", OperationType.addChildState);
            }

            // 替换节点
            {
                GetLocalSpritsShowToMenu<BTNode>( menu, "替换节点/逻辑节点/", OperationType.addChildState);
                GetLocalSpritsShowToMenu<BTAction>( menu, "替换节点/行为节点/",OperationType.addChildState);
            }

            // 删除节点
            MenuItemData nodeData = new MenuItemData("删除节点");
            nodeData.action = OperationType.deleteNode;
            menu.AddItem(new GUIContent("删除节点"), false, ContextCallback, nodeData);

            menu.ShowAsContext();

            e.Use();
        }


        /// <summary>
        /// 右键功能 获取本地的 行为树逻辑代码 展示到菜单栏
        /// </summary>
        private void GetLocalSpritsShowToMenu<T>(GenericMenu menu, string nodeParentName,OperationType operationType) where T:class
        {
            Type[] types = GetAssemblyTargetCs<T>();

            var targetType = typeof(T);

            List<string> allNames = new List<string>();
            foreach (Type type in types)
            {
                Type baseType = type.BaseType;
                if (baseType != null && baseType.Name == targetType.Name && baseType.Namespace == "BT_BehaviourTree")
                {
                    string name = nodeParentName + type.Name;
                    MenuItemData nodeData = new MenuItemData(type.Name);
                    nodeData.action = operationType;
                    menu.AddItem(new GUIContent(name), false, ContextCallback, nodeData);
                    allNames.Add(type.Name);
                }
            }
        }

        Type[] GetAssemblyTargetCs<T>() 
        {
            string csAssemblyPath = Application.dataPath + "/../Library/ScriptAssemblies/Assembly-CSharp.dll";
            Assembly assembly = Assembly.LoadFile(csAssemblyPath);
            Type[] types = assembly.GetTypes();

            return types;
        }


        void ContextCallback(object _nodeData)
        {
            MenuItemData nodeData = (MenuItemData)_nodeData;

            switch (nodeData.action)
            {
                case OperationType.addState:
                    BTNodeEditor bTStateGUIBase = new BTNodeEditor(mousePosition, nodeData.nodeName);
                    nodes.Add(bTStateGUIBase);
                    break;
                case OperationType.addChildState:
                    BTNodeEditor bTStateGUI = new BTNodeEditor(mousePosition,nodeData.nodeName);
                    selectedNode.childsNode.Add(bTStateGUI);
                    bTStateGUI.parentNode = selectedNode;
                    nodes.Add(bTStateGUI);
                    break;
                case OperationType.deleteNode:         // 删除节点
                    if (selectedNode != null)
                    {
                        nodes.Remove(selectedNode);
                        for (int i = 0; i < selectedNode.childsNode.Count; i++)
                        {
                            selectedNode.childsNode[i].parentNode = null;
                        }
                    }
                    break;
            }
        }

        private void RightMouseClick(Event _event)
        {
            selectedNode = null;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].windowRect.Contains(_event.mousePosition))
                {
                    clickedOnWindow = true;
                    selectedNode = nodes[i];
                    break;
                }
            }

            if (!clickedOnWindow)
                HandleBackgroudAddNode(_event);
            else 
                NodeOnClick(_event);
        }


        // 设置全局比例
        float zoomSpeed = 0.05f;

        private void ScrollWheel(Event _event) 
        {

            BTEditorGlobalUtil.GlobalSizeScale = _event.delta.y < 0 ? BTEditorGlobalUtil.GlobalSizeScale += zoomSpeed : BTEditorGlobalUtil.GlobalSizeScale -= zoomSpeed;
            if (BTEditorGlobalUtil.GlobalSizeScale >= 1 || BTEditorGlobalUtil.GlobalSizeScale <= 0.2f)
            {
                Repaint();
                return;
            }

            // 设置缩放比例
            BTEditorGlobalUtil.GlobalPosScale = _event.delta.y < 0 ? BTEditorGlobalUtil.GlobalPosScale += zoomSpeed : BTEditorGlobalUtil.GlobalPosScale -= zoomSpeed;
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].nodePos = _event.mousePosition + (nodes[i].nodePos - _event.mousePosition) * BTEditorGlobalUtil.GlobalPosScale;
            }

            Repaint();
        }

        private void ScrollWheelDrag(Event _event) 
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].nodePos += _event.delta;
            }

            Repaint();
        }

        private void LeftMouseClick(Event _event)
        {
            selectedNode = null;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].windowRect.Contains(_event.mousePosition))
                {
                    clickedOnWindow = true;
                    selectedNode = nodes[i];
                    break;
                }
            }
            Repaint();
        }

        private void OnLeftMouseDrag(Event _event) 
        {
            if (selectedNode != null)
            {
                selectedNode.nodePos += _event.delta;
            }

            Repaint();
        }
        #endregion

        private int titleSelected = 0;

        private void DrawWorkSpace() 
        {
            leftBehaviourOperationRect = new Rect(2,22,300,position.height - 25);

            // 标题
            {
                Rect titleRect = EditorGUILayout.BeginHorizontal();

                GUIContent[] rightGUIConytent = new GUIContent[4] { new GUIContent("Behavior"), new GUIContent("Tasks"), new GUIContent("Variables"), new GUIContent("Inspector") };
                titleSelected = GUILayout.Toolbar(titleSelected, rightGUIConytent, "toolbarbutton", GUILayout.Width(leftBehaviourOperationRect.width));

                GUILayout.Label(" ", "toolbarbutton");

                if (GUILayout.Button("SaveBT_Tree", "toolbarbutton", GUILayout.Width(100)))
                {
                    Debug.Log("SaveBT_Tree");
                }

                EditorGUILayout.EndHorizontal();
            }

            // 左边工作区
            {
                GUILayout.BeginArea(leftBehaviourOperationRect);
                switch (titleSelected)
                {
                    case 0:
                        BehaviorInfoShow();
                        break;
                    case 1:
                        BehaviorAllAction();
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    default:
                        break;
                }
                GUILayout.EndArea();
            }

            // 右边工作区
            {
                treeStructShowRect = new Rect(300, 22, position.width - 4, position.height - 25);
                GUI.Label(treeStructShowRect, "Scale： " + BTEditorGlobalUtil.GlobalSizeScale.ToString(), "CurveEditorBackground");
            }
        }

        /// <summary>
        /// 展示行为树信息的窗口
        /// </summary>
        private void BehaviorInfoShow() 
        {
            GUILayout.Label("左边的  按到");
        }

        private string allActionSearch = string.Empty;
        private Vector2 sollviewPos = Vector2.zero;
        /// <summary>
        /// 展示所有的行为代码
        /// </summary>
        private void BehaviorAllAction() 
        {
            sollviewPos = GUILayout.BeginScrollView(sollviewPos);
            allActionSearch = GUILayout.TextField("", "SearchTextField");

            GUILayout.EndScrollView();
        }

        private void DrawWindows()
        {
            BeginWindows();

            foreach (BTNodeEditor bTNodeGUI in nodes)
            {
                bTNodeGUI.DrawCurve();
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].DrawWindow(mousePosition,selectedNode);
            }

            EndWindows();
        }

        /// <summary>
        /// 画格子
        /// </summary>
        private void DrawGrid() 
        {
            float gridSize = 60 * BTEditorGlobalUtil.GlobalSizeScale;

            Handles.color = new Color(0.2f,0.2f,0.2f,1);

            for (int i = 1; i < Mathf.CeilToInt(treeStructShowRect .width/ gridSize); i++)
            {
                Handles.DrawLine(new Vector2(i * gridSize,0),new Vector2(i * gridSize, treeStructShowRect.height));
            }
            for (int i = 1; i < Mathf.CeilToInt(treeStructShowRect.height/gridSize); i++)
            {
                Handles.DrawLine(new Vector2(0,i * gridSize), new Vector2(treeStructShowRect.width,i * gridSize));
            }
        }
    }
}