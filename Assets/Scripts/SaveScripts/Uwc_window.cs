using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Uwc_window : MonoBehaviour
{
    //public string TextField = this.GetComponentInChildren<Text>().text;
    public string TextField;
    public float[] position;
    public float[] rotation;
    public float[] scale;
    public string partialWindowTitle;
    public string type;

    void Awake()
    {
        SaveSystem.uwc_windowList.Add(this);
    }

    /*
    void Update()
    {
        TextField = this.GetComponentInChildren<UnityEngine.UI.Text>().text;
        //position, rotation and scale are gotten through Uwc_windowData
    }
    */
}
