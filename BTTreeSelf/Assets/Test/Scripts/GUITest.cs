using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUITest : MonoBehaviour
{
    private string Mystring = "MyString";
    private int aa = 0;

    private Vector2 vector = Vector2.zero;

    private bool isToggle = false;

    string[] vs = new string[4]{"AAaa","BBabb","CCcc","DDdd"};

    private void OnGUI()
    {

        //Mystring = GUI.TextArea(new Rect(25, 25, 100, 30), Mystring);

        //aa = GUI.Toolbar(new Rect(25, 70, 500, 50), aa, vs);

        //if (GUILayout.Button("Name"))
        //{
        //    Debug.Log(Mystring + aa.ToString());
        //}

        isToggle = GUI.Toggle(new Rect(25, 130, 500, 50), isToggle, "Too");

        //aa = GUI.SelectionGrid(new Rect(25, 190, 500, 50), aa, vs, 2);

        vector = GUI.BeginScrollView(new Rect(25, 250, 200, 100), vector, new Rect(25, 250, 200, 500));

        GUI.Button(new Rect(25, 270, 100, 100),"Btn1");
        GUI.Button(new Rect(25, 380, 100, 100), "Btn2");
        GUI.Button(new Rect(25, 490, 100, 100), "Btn3");


        GUI.EndScrollView();

        //GUI.Window(0,new Rect(100,100,500,500), WinFunc,"My WIn");

        if (GUI.changed)
        {
            Debug.Log("该笔");
        }
    }

    void WinFunc(int a) 
    {
        Mystring = GUI.TextArea(new Rect(25, 25, 100, 30), Mystring);
    }
}
