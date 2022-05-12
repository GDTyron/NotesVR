using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using UnityEngine;
using System.Collections.Specialized;
using System.ComponentModel.Design.Serialization;
using System.Threading;
using System;
using System.Linq;

public class planeCreation : MonoBehaviour
{
    public Material planeMat;
    public Sprite Drawable;

    [SerializeField]
    public static Vector3 posi1;
    [SerializeField]
    public static Vector3 posi2;
    [SerializeField]
    public static Vector3 posi3;
    [SerializeField]
    public static Vector3 posi4;
    [SerializeField]
    private Vector3 pos3;
    [SerializeField]
    private Vector3 pos4;
    [SerializeField]
    private static Vector3[] planeNormals;

    public Vector3[] recalcPos(Vector3 pos1, Vector3 pos2) {
        Vector3 pos4 = new Vector3(pos1.x, pos2.y, pos1.z);
        Vector3 pos3 = new Vector3(pos2.x, pos1.y, pos2.z);

        //1 3
        //4 2
        //Position movement
        if (Vector3.Distance(pos1, pos4) / 9 > Vector3.Distance(pos1, pos3) / 16)
        {
            //UnityEngine.Debug.Log("too tall. drawable will be shortened to fit 16:9");
            if (pos1.y > pos2.y)
            {
                //UnityEngine.Debug.Log("Actuated 1. if statement of tall");
                pos4 = new Vector3(pos4.x, pos1.y - ((Vector3.Distance(pos1, pos3) / 16) * 9), pos4.z);
                pos2 = new Vector3(pos2.x, pos1.y - ((Vector3.Distance(pos1, pos3) / 16) * 9), pos2.z);
            }
            else if (pos1.y < pos2.y)
            {
                //UnityEngine.Debug.Log("Actuated 2. if statement of tall");
                pos4 = new Vector3(pos4.x, pos1.y + ((Vector3.Distance(pos1, pos3) / 16) * 9), pos4.z);
                pos2 = new Vector3(pos2.x, pos1.y + ((Vector3.Distance(pos1, pos3) / 16) * 9), pos2.z);
            }
        }
        else if (Vector3.Distance(pos1, pos4) / 9 < Vector3.Distance(pos1, pos3) / 16)
        {
            //UnityEngine.Debug.Log("too wide. drawable will be shortened to fit 16:9");
            if (pos1.x < pos2.x)
            {// a' = (c' * a)/c
                if (pos1.z > pos2.z)
                {
                    //UnityEngine.Debug.Log("Actuated 1. if statement of wide");
                    pos3 = new Vector3(pos1.x + (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.x - pos1.x) / Vector3.Distance(pos1, pos3))), pos3.y,
                        pos1.z + (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.z - pos1.z) / Vector3.Distance(pos1, pos3))));
                    pos2 = new Vector3(pos1.x + (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.x - pos1.x) / Vector3.Distance(pos1, pos3))), pos2.y,
                        pos1.z + (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.z - pos1.z) / Vector3.Distance(pos1, pos3))));
                }
                else if (pos1.z < pos2.z)
                {
                    //UnityEngine.Debug.Log("Actuated 2. if statement of wide");
                    pos3 = new Vector3(pos1.x + (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.x - pos1.x) / Vector3.Distance(pos1, pos3))), pos3.y,
                        pos1.z - (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.z - pos1.z) / Vector3.Distance(pos1, pos3))));
                    pos2 = new Vector3(pos1.x + (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.x - pos1.x) / Vector3.Distance(pos1, pos3))), pos2.y,
                        pos1.z - (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.z - pos1.z) / Vector3.Distance(pos1, pos3))));
                }
            }
            else if (pos1.x > pos2.x)
            {// a' = (c' * a)/c
                if (pos1.z > pos2.z)
                {
                    //UnityEngine.Debug.Log("Actuated 3. if statement of wide");
                    pos3 = new Vector3(pos1.x - (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.x - pos1.x) / Vector3.Distance(pos1, pos3))), pos3.y,
                        pos1.z + (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.z - pos1.z) / Vector3.Distance(pos1, pos3))));
                    pos2 = new Vector3(pos1.x - (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.x - pos1.x) / Vector3.Distance(pos1, pos3))), pos2.y,
                        pos1.z + (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.z - pos1.z) / Vector3.Distance(pos1, pos3))));
                }
                else if (pos1.z < pos2.z)
                {
                    //UnityEngine.Debug.Log("Actuated 4. if statement of wide");
                    pos3 = new Vector3(pos1.x - (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.x - pos1.x) / Vector3.Distance(pos1, pos3))), pos3.y,
                        pos1.z - (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.z - pos1.z) / Vector3.Distance(pos1, pos3))));
                    pos2 = new Vector3(pos1.x - (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.x - pos1.x) / Vector3.Distance(pos1, pos3))), pos2.y,
                        pos1.z - (((Vector3.Distance(pos1, pos4) / 9) * 16) * (Mathf.Abs(pos3.z - pos1.z) / Vector3.Distance(pos1, pos3))));
                }
            }
        }
        Vector3[] allPos = { pos1, pos2, pos3, pos4 };
        //UnityEngine.Debug.Log("Thus new width and height metrics are " + Vector3.Distance(allPos[0], allPos[2]) /16 + "x" + Vector3.Distance(allPos[0], allPos[3]) /9);
        return allPos;
    }

    //You can do undo with SerializedObject
    //Lets do Sprites instead of Meshes
    public void CreateSpriteWPos(Vector3 pos1, Vector3 pos2, Texture2D pixelTexArea, GameObject FactualPlane) {
        //UnityEngine.Debug.Log("Creating Sprite...");
        Vector3 pos4 = new Vector3(pos1.x, pos2.y, pos1.z);
        Vector3 pos3 = new Vector3(pos2.x, pos1.y, pos2.z);

        Drawable = Sprite.Create(pixelTexArea, new Rect(0.0f, 0.0f, pixelTexArea.width, pixelTexArea.height), new Vector3(0.5f, 0.5f, 0.5f), 1000f);
        //Drawable = Sprite.Create(pixelTexArea, new Rect(0.0f, 0.0f, (int)((100) * Vector3.Distance(pos1, pos3)), (int)((100) * Vector3.Distance(pos1, pos4))), new Vector3(0.5f, 0.5f, 0.5f), 100f);
        //Drawable = Sprite.Create(pixelTexArea, new Rect(0.0f, 0.0f, 1920f/4, 1080f/4), new Vector3(0.5f, 0.5f, 0.5f), 100.0f);
        //UnityEngine.Debug.Log("Drawable width should be " + (int)(1920 * Vector3.Distance(pos1, pos3)) + " and height is " + (int)(1080 * Vector3.Distance(pos1, pos4)));
        Drawable.name = "DrawableSprite";

        FactualPlane.AddComponent<SpriteRenderer>();

        SpriteRenderer DrawableRenderer = FactualPlane.GetComponent<SpriteRenderer>();

        DrawableRenderer.sprite = Drawable;

        FactualPlane.transform.position = new Vector3((pos1.x + pos2.x) / 2, (pos1.y + pos2.y) / 2, (pos1.z + pos2.z) / 2);
        //UnityEngine.Debug.Log("VIBE CHECK. Teiler und Nenner sind: " + Mathf.Abs(pos1.x - pos3.x) + " und " + Mathf.Abs(pos1.z - pos3.z));
        FactualPlane.transform.Rotate(0f,
            Mathf.Acos((Mathf.Abs(pos1.x - pos3.x) / Mathf.Abs(pos1.z - pos3.z)) % 1) * 360,
            0f);
        //rot is 360f / (Mathf.Acos((Mathf.Abs(pos1.x - pos3.x)) / (Vector3.Distance(pos1, pos3)))
        //let rotx = 0, roty = , rotz = 0

        posi1 = pos1;
        posi2 = pos2;
        posi3 = pos3;
        posi4 = pos4;
    }

    public void CreateSpriteWPosWOTexture(float SWidth, float SHeight, Vector3 pos1, Vector3 pos2, GameObject FactualPlane)
    {
        //UnityEngine.Debug.Log("Creating Sprite...");
        Vector3 pos4 = new Vector3(pos1.x, pos2.y, pos1.z);
        Vector3 pos3 = new Vector3(pos2.x, pos1.y, pos2.z);

        Drawable = Sprite.Create(Texture2D.whiteTexture, new Rect(0.0f, 0.0f, SWidth, SHeight), new Vector3(0.5f, 0.5f, 0.5f), 1000f);
        //Drawable = Sprite.Create(pixelTexArea, new Rect(0.0f, 0.0f, (int)((100) * Vector3.Distance(pos1, pos3)), (int)((100) * Vector3.Distance(pos1, pos4))), new Vector3(0.5f, 0.5f, 0.5f), 100f);
        //Drawable = Sprite.Create(pixelTexArea, new Rect(0.0f, 0.0f, 1920f/4, 1080f/4), new Vector3(0.5f, 0.5f, 0.5f), 100.0f);
        //UnityEngine.Debug.Log("Drawable width should be " + (int)(1920 * Vector3.Distance(pos1, pos3)) + " and height is " + (int)(1080 * Vector3.Distance(pos1, pos4)));
        Drawable.name = "DrawableSprite";

        FactualPlane.AddComponent<SpriteRenderer>();

        SpriteRenderer DrawableRenderer = FactualPlane.GetComponent<SpriteRenderer>();

        DrawableRenderer.sprite = Drawable;

        FactualPlane.transform.position = new Vector3((pos1.x + pos2.x) / 2, (pos1.y + pos2.y) / 2, (pos1.z + pos2.z) / 2);
        //UnityEngine.Debug.Log("VIBE CHECK. Teiler und Nenner sind: " + Mathf.Abs(pos1.x - pos3.x) + " und " + Mathf.Abs(pos1.z - pos3.z));
        FactualPlane.transform.Rotate(0f,
            Mathf.Acos((Mathf.Abs(pos1.x - pos3.x) / Mathf.Abs(pos1.z - pos3.z)) % 1) * 360,
            0f);
        //rot is 360f / (Mathf.Acos((Mathf.Abs(pos1.x - pos3.x)) / (Vector3.Distance(pos1, pos3)))
        //let rotx = 0, roty = , rotz = 0

        posi1 = pos1;
        posi2 = pos2;
        posi3 = pos3;
        posi4 = pos4;
    }

    public void CreateQuadMeshWOTexture(Vector3 pos1, Vector3 pos2, GameObject selectorPlane)
    {
        UnityEngine.Debug.Log("Creating Mesh...");
        MeshRenderer meshRenderer = selectorPlane.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = (Material)Resources.Load("Ball");
        meshRenderer.material.mainTexture = Texture2D.whiteTexture;

        MeshFilter meshFilter = selectorPlane.gameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        Vector3 pos4 = new Vector3(pos1.x, pos2.y, pos1.z);
        Vector3 pos3 = new Vector3(pos2.x, pos1.y, pos2.z);

        if (Vector3.Distance(pos1, pos4) / 16 < Vector3.Distance(pos2, pos4) / 9)
        {
            if (pos1.y < pos2.y)
            {
                pos4 = new Vector3((float)pos4.x, (pos1.y + Vector3.Distance(pos1, pos3) / 16) * 9, (float)pos4.z);
                pos2 = new Vector3((float)pos2.x, (pos3.y + Vector3.Distance(pos3, pos1) / 16) * 9, (float)pos2.z);
                UnityEngine.Debug.Log("Y von 4 und 2 wurden angepasst");
            }
            else if (pos1.y > pos2.y)
            {
                pos4 = new Vector3((float)pos4.x, (pos1.y - Vector3.Distance(pos1, pos3) / 16) * 9, (float)pos4.z);
                pos2 = new Vector3((float)pos2.x, (pos3.y - Vector3.Distance(pos1, pos3) / 16) * 9, (float)pos2.z);
                UnityEngine.Debug.Log("Y von 4 und 2 wurden angepasst");
            }
        }
        else if (Vector3.Distance(pos1, pos4) / 16 > Vector3.Distance(pos2, pos4) / 9)
        {
            if (pos1.x < pos2.x)
            {
                pos3 = new Vector3(pos3.x + Vector3.Distance(pos1, pos4) / 9 * 16, pos3.y, pos3.z);
                pos2 = new Vector3(pos2.x + Vector3.Distance(pos4, pos1) / 9 * 16, pos2.y, pos2.z);
                UnityEngine.Debug.Log("X von 3 und 2 wurden angepasst");
            }
            else if (pos1.x > pos2.x)
            {
                pos3 = new Vector3(pos3.x - Vector3.Distance(pos1, pos4) / 9 * 16, pos3.y, pos3.z);
                pos2 = new Vector3(pos2.x - Vector3.Distance(pos4, pos1) / 9 * 16, pos2.y, pos2.z);
                UnityEngine.Debug.Log("X von 3 und 2 wurden angepasst");
            }
        }

        Vector3[] vertices = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        vertices[0] = new Vector3(pos1.x, pos1.y, pos1.z); //le up 1
        vertices[1] = new Vector3(pos4.x, pos4.y, pos4.z); //ri up 3
        vertices[2] = new Vector3(pos3.x, pos3.y, pos3.z); //le do 4
        vertices[3] = new Vector3(pos2.x, pos2.y, pos2.z); //ri do 2

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 3;
        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 2;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        planeNormals = mesh.normals;

        if (pos1.y < pos2.y && pos1.x > pos2.x)
        {
            Vector3[] normals = mesh.normals;
            //UnityEngine.Debug.Log(normals);

            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -1 * normals[i];
            }
        }
        UnityEngine.Debug.Log("pos1 is " + pos1.ToString());
        UnityEngine.Debug.Log("pos2 is " + pos2.ToString());
        UnityEngine.Debug.Log("pos3 is " + pos3.ToString());
        UnityEngine.Debug.Log("pos4 is " + pos4.ToString());

        posi1 = pos1;
        posi2 = pos2;
        posi3 = pos3;
        posi4 = pos4;

        meshFilter.mesh = mesh;
    }

    public void CreateQuadMeshWPos(Vector3 pos1, Vector3 pos2, Texture2D pixelTexArea)
    {
        UnityEngine.Debug.Log("Creating Mesh...");
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = (Material)Resources.Load("Ball");
        meshRenderer.material.mainTexture = pixelTexArea;

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        Vector3 pos4 = new Vector3(pos1.x, pos2.y, pos1.z);
        Vector3 pos3 = new Vector3(pos2.x, pos1.y, pos2.z);
        
        if (Vector3.Distance(pos1, pos4) / 16 < Vector3.Distance(pos2, pos4) / 9)
        {
            if (pos1.y < pos2.y)
            {
                pos4 = new Vector3((float)pos4.x, (pos1.y + Vector3.Distance(pos1, pos3) / 16) * 9, (float)pos4.z);
                pos2 = new Vector3((float)pos2.x, (pos3.y + Vector3.Distance(pos3, pos1) / 16) * 9, (float)pos2.z);
                UnityEngine.Debug.Log("Y von 4 und 2 wurden angepasst");
            } else if (pos1.y > pos2.y)
            {
                pos4 = new Vector3((float)pos4.x, (pos1.y - Vector3.Distance(pos1, pos3) / 16) * 9, (float)pos4.z);
                pos2 = new Vector3((float)pos2.x, (pos3.y - Vector3.Distance(pos1, pos3) / 16) * 9, (float)pos2.z);
                UnityEngine.Debug.Log("Y von 4 und 2 wurden angepasst");
            }
        }
        else if (Vector3.Distance(pos1, pos4) / 16 > Vector3.Distance(pos2, pos4) / 9)
        {
            if (pos1.x < pos2.x)
            {
                pos3 = new Vector3(pos3.x + Vector3.Distance(pos1, pos4) / 9 * 16, pos3.y, pos3.z);
                pos2 = new Vector3(pos2.x + Vector3.Distance(pos4, pos1) / 9 * 16, pos2.y, pos2.z);
                UnityEngine.Debug.Log("X von 3 und 2 wurden angepasst");
            } else if (pos1.x > pos2.x)
            {
                pos3 = new Vector3(pos3.x - Vector3.Distance(pos1, pos4) / 9 * 16, pos3.y, pos3.z);
                pos2 = new Vector3(pos2.x - Vector3.Distance(pos4, pos1) / 9 * 16, pos2.y, pos2.z);
                UnityEngine.Debug.Log("X von 3 und 2 wurden angepasst");
            }
        }
        
        Vector3[] vertices = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        vertices[0] = new Vector3(pos1.x, pos1.y, pos1.z); //le up 1
        vertices[1] = new Vector3(pos4.x, pos4.y, pos4.z); //ri up 3
        vertices[2] = new Vector3(pos3.x, pos3.y, pos3.z); //le do 4
        vertices[3] = new Vector3(pos2.x, pos2.y, pos2.z); //ri do 2

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 3;
        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 2;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        planeNormals = mesh.normals;

        if (pos1.y < pos2.y && pos1.x > pos2.x)
        {
            Vector3[] normals = mesh.normals;
            //UnityEngine.Debug.Log(normals);

            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -1 * normals[i];
            }
        }
        UnityEngine.Debug.Log("pos1 is " + pos1.ToString());
        UnityEngine.Debug.Log("pos2 is " + pos2.ToString());
        UnityEngine.Debug.Log("pos3 is " + pos3.ToString());
        UnityEngine.Debug.Log("pos4 is " + pos4.ToString());

        posi1 = pos1;
        posi2 = pos2;
        posi3 = pos3;
        posi4 = pos4;

        meshFilter.mesh = mesh;
    }

    public Vector3[] getNormals()
    {
        return planeNormals;
    }

    public Vector3 getPos1()
    {
        return posi1;
    }
    public Vector3 getPos2()
    {
        return posi2;
    }
    public Vector3 getPos3()
    {
        return posi3;
    }
    public Vector3 getPos4()
    {
        return posi4;
    }
}
