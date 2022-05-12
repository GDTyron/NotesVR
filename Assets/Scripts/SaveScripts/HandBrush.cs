using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandBrush : MonoBehaviour
{
    public float[][] position;
    public float[][] scale;
    public int brushCount;

    //[System.NonSerialized]
    public GameObject VRCamera;

    void Awake()
    {
        SaveSystem.handBrushList.Add(this);
        VRCamera = GameObject.Find("VRCamera");
    }

    void Update()
    {
        for (int i = 0; i < this.transform.childCount; i += 1)
        { 
            this.transform.GetChild(i).transform.LookAt(VRCamera.transform);
            this.transform.GetChild(i).transform.rotation *= Quaternion.Euler(90, 0, 0);
        }
        
    }
}
