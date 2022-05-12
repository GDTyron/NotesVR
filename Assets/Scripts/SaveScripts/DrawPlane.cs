using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawPlane : MonoBehaviour
{
    public float[] pos1;
    public float[] pos2;
    public float[] middlePoint;
    public string texPath;
    public int identifier;
    //public List<List<List<List<float>>>> layerList = new List<List<List<List<float>>>>();
    public float[,,,] layerList;

    void Awake()
    {
        SaveSystem.drawPlaneList.Add(this);

        middlePoint = new float[] { GameObject.Find("RightHand").transform.position.x, GameObject.Find("RightHand").transform.position.y, GameObject.Find("RightHand").transform.position.z };
    }
}
