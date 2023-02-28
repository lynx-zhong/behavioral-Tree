using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass : MonoBehaviour
{
    public BaseClass()
    {
        Debug.LogError("Base class Info");
    }

    
}

public class ChildClass : BaseClass 
{
    public ChildClass()
    {
        Debug.LogError("Chlid Class Info");
    }

}

public class TestClass : MonoBehaviour
{
    private void Start()
    {
        ChildClass childClass = new ChildClass();
    }
}