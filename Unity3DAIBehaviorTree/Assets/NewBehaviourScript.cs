using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Type t = Type.GetType("NewTs");
        TestBase testBase = Activator.CreateInstance(t) as TestBase;

        Debug.LogError("ss:  " + testBase.NewHp);
    }

}
