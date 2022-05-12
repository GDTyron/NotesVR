using UnityEngine;

[System.Serializable]
public class HandBrushData
{
    public float[][] position;
    //public float[][] position = new float[][];
    public float[][] scale;
    //public float[][] scale = new float[][];
    public int brushCount;

    public HandBrushData(HandBrush handBrush)
    {
        brushCount = handBrush.transform.childCount;

        position = new float[brushCount][];
        scale = new float[brushCount][];

        for (int i = 0; i < brushCount - 1; i += 1)
        {
            Vector3 handBrushPos = handBrush.transform.GetChild(i).transform.position;

            position[i] = new float[]
            {
                handBrushPos.x, handBrushPos.y, handBrushPos.z
            };

            Vector3 handBrushScale = handBrush.transform.GetChild(i).transform.localScale;

            scale[i] = new float[]
            {
                handBrushScale.x, handBrushScale.y, handBrushScale.z
            };
        }
    }
}