using System;
using UnityEditor;
using UnityEngine;

public class EditorGUITest : EditorWindow
{
    public string[] iconNames = { "level", "scene", "cutscene", "camera", "actor" };
    int seleIndex = 0;
    private Vector2 scrollPos = Vector2.zero;

    private GUIContent[] gUIContent;

    [MenuItem("Zzz/CreatWindow")]
    static void CreatWindow() 
    {
        GetWindow<EditorGUITest>(false,typeof(EditorGUITest).Name);
    }

    public EditorGUITest()
    {
        Init();
    }

    void Init() 
    {

    }

    private void Awake()
    {
        gUIContent = new GUIContent[iconNames.Length];

        for (int i = 0; i < iconNames.Length; i++)
        {
            Texture2D texture2D = (Texture2D)Resources.Load(iconNames[i]);

            GUIContent GUIContent = new GUIContent(texture2D, iconNames[i]);

            gUIContent.SetValue(GUIContent, i);
        }
    }

    Rect window = new Rect(50,100,200,200);

    private void OnGUI()
    {
        seleIndex = GUI.Toolbar(new Rect(50, 50, 300, 50), seleIndex, gUIContent);



        //GUILayout.BeginArea(new Rect(5,60, position.width - 5, position.height - 5));
        //{
        //    GUILayout.BeginHorizontal();
        //    {
        //        for (int i = 0; i < iconNames.Length; i++)
        //        {
        //            GUILayout.Button(iconNames[i]);
        //        }
        //    }
        //}

        //GUILayout.EndArea();

        Rect clirect = new Rect(200, 200, 1000, 500);
        GUILayout.BeginArea(clirect,"", "CurveEditorBackground");

        BeginWindows();
        window = GUI.Window(1, window, CallBack, "哈哈哈");


        EndWindows();

        GUILayout.EndArea();

        if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
        {
            Debug.LogError("点击");
            bool isHave = window.Contains(Event.current.mousePosition);
            Debug.LogError("window:  " + window);
            Debug.LogError("mousePosition:  " + Event.current.mousePosition);
            Debug.LogError("isHave:  " + isHave);
        }
    }

    private void CallBack(int id)
    {

    }
}
