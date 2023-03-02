using UnityEngine;
using UnityEditor;

public class TestEditor : EditorWindow
{
    [MenuItem("BehaviourTree/TestWindow")]
    static void OpenTestWindow() 
    {
        GetWindow<TestEditor>(typeof(TestEditor).Name);
    }

    private GameObject gameObject;

    private Rect rect1 = new Rect(20, 20, 100, 100);
    private Rect rect2 = new Rect(200, 20, 100, 100);

    private void OnGUI()
    {
        BeginWindows();

        rect1 = GUI.Window(1, rect1, Func1,"Func1");
        rect2 = GUI.Window(2, rect2, Func2,"Func2");

        gameObject = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), false);

        EndWindows();
    }

    void Func1(int aa) 
    {
        gameObject = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), false);
        GUI.DragWindow();

        
    }

    void Func2(int bb) 
    {
        GUI.DragWindow();
    }
}
