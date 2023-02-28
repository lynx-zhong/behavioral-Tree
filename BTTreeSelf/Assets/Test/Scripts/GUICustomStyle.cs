using UnityEngine;
using System.Collections;

public class GUICustomStyle : MonoBehaviour
{
    public GUISkin mySkin;

    void OnGUI()
    {
        // 将该皮肤指定为当前使用的皮肤。
        GUI.skin = mySkin;

        // 创建按钮。此时将从分配给 mySkin 的皮肤获得默认的 "button" 样式。
        GUI.Button(new Rect(10, 10, 150, 20), "Skinned Button");
    }

}
