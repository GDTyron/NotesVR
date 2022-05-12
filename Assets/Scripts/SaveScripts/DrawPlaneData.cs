using UnityEngine;
//using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class DrawPlaneData
{
    public float[] pos1;
    public float[] pos2;
    public float[] middlePoint;
    public string texPath;
    public int identifier;
    //public float[][][][] layerList;
    public float[,,,] layerList;

    [System.NonSerialized]
    public GameObject RectangleToolObject;
    [System.NonSerialized]
    RectangleTool RectangleToolScript;

    /*
    void Start()
    {
        RectangleToolObject = GameObject.Find("RectangleTool");
        RectangleToolScript = RectangleToolObject.GetComponent<RectangleTool>();
    }
    */

    public DrawPlaneData(DrawPlane drawPlane)
    {
        RectangleToolObject = GameObject.Find("RectangleTool");
        RectangleToolScript = RectangleToolObject.GetComponent<RectangleTool>();

        identifier = drawPlane.identifier;

        texPath = drawPlane.texPath;

        pos1 = new float[] { RectangleToolScript.allAllPos[identifier][0].x, RectangleToolScript.allAllPos[identifier][0].y, RectangleToolScript.allAllPos[identifier][0].z };

        pos2 = new float[] { RectangleToolScript.allAllPos[identifier][1].x, RectangleToolScript.allAllPos[identifier][1].y, RectangleToolScript.allAllPos[identifier][1].z };

        middlePoint = drawPlane.middlePoint;

        List<List<List<Color>>> layerListList = RectangleToolScript.getLayerList();
        List<List<Color>> subLayerList = RectangleToolScript.getLayerList()[0];
        List<Color> subSubLayerList = RectangleToolScript.getLayerList()[0][0];
        //List<List<List<List<float>>>> ListLayerList = new List<List<List<List<float>>>>();

        //layerList = new float[layerListList.Count][][][]; //[subLayerList.Count][subSubLayerList.Count][4]
        layerList = new float[layerListList.Count,subLayerList.Count,subSubLayerList.Count,4]; //[subLayerList.Count][subSubLayerList.Count][4]

        for (int i = 0; i < layerListList.Count; i += 1)
        {
            for (int j = 0; j < layerListList[i].Count; j += 1)
            {
                for (int k = 0; k < layerListList[i][j].Count; k += 1)
                {
                    /*
                    float temp1 = layerListList[i][j][k].r;
                    float temp2 = layerListList[i][j][k].g;
                    float temp3 = layerListList[i][j][k].b;
                    float temp4 = layerListList[i][j][k].a;
                    */
                    
                    layerList[i,j,k,0] = layerListList[i][j][k].r;
                    layerList[i,j,k,1] = layerListList[i][j][k].g;
                    layerList[i,j,k,2] = layerListList[i][j][k].b;
                    layerList[i,j,k,3] = layerListList[i][j][k].a;
                    //layerList[i][j][k][0] = layerListList[i][j][k].r;
                    //layerList[i][j][k][1] = layerListList[i][j][k].g;
                    //layerList[i][j][k][2] = layerListList[i][j][k].b;
                    //layerList[i][j][k][3] = layerListList[i][j][k].a;
                    
                }
            }
        }
    }
}
