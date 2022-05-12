using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System;
using System.Text;

public class SaveSystem : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Uwc_window uwc_window_Prefab;
    [SerializeField] Eraser handBrush_Prefab;
    [SerializeField] bruhsh Drawable_Prefab;

    public static List<Uwc_window> uwc_windowList = new List<Uwc_window>();
    public static List<DrawPlane> drawPlaneList = new List<DrawPlane>();
    public static List<HandBrush> handBrushList = new List<HandBrush>();

    const string UWC_WINDOW_SUB = "/uwc_window";
    const string UWC_WINDOW_COUNT_SUB = "/uwc_window.count";
    const string DRAWPLANE_SUB = "/drawplane";
    const string DRAWPLANE_COUNT_SUB = "/drawplane.count";
    const string HANDBRUSH_SUB = "/handbrush";
    const string HANDBRUSH_COUNT_SUB = "/handbrush.count";

    public static Texture2D textureTemp;

    RectangleTool RectangleToolScript;
    bruhsh bruhshScript;

    public GameObject RectangleTool;
    public GameObject bruhsh;

    /*
    void Awake()
    {
        LoadUwcWindow();
    }

    void OnApplicationQuit()
    {
        SaveUwcWindow();
    }
    */

    void Start()
    {
        RectangleToolScript = RectangleTool.GetComponent<RectangleTool>();
        bruhshScript = bruhsh.GetComponent<bruhsh>();
    }

    public void SaveAllSavableObjects() 
    {
        SaveUwcWindow();
        SaveDrawPlane();
        SaveHandBrush();
    }

    public void LoadAllSavableObjects()
    {
        LoadUwcWindow();
        LoadDrawPlane();
        LoadHandBrush();
    }

    public void SaveUwcWindow()
    {
        
        Debug.Log("Saving...");
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + UWC_WINDOW_SUB + SceneManager.GetActiveScene().buildIndex;
        string countPath = Application.persistentDataPath + UWC_WINDOW_COUNT_SUB + SceneManager.GetActiveScene().buildIndex;

        FileStream countStream = new FileStream(countPath, FileMode.Create);

        formatter.Serialize(countStream, uwc_windowList.Count);
        countStream.Close();

        for (int i = 0; i < uwc_windowList.Count; i++)
        {
            FileStream stream = new FileStream(path + i, FileMode.Create);
            Uwc_windowData data = new Uwc_windowData(uwc_windowList[i]);

            Debug.Log("[SaveData] TextField: " + data.TextField + " position: " + data.position + " rotation: " + data.rotation + " scale: " + data.scale);

            //byte[] bytes = new byte[stream.Length];
            //Debug.Log("Saved: " + stream.Read(bytes, (int)stream.Length, 0));
            formatter.Serialize(stream, data);
            stream.Close();
            Debug.Log("Saved object to " + (path + i));
        }
        UnityEngine.Debug.Log("Saved all uwc_windows");
    }

    public void LoadUwcWindow()
    {
        Debug.Log("Loading...");
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + UWC_WINDOW_SUB + SceneManager.GetActiveScene().buildIndex;
        string countPath = Application.persistentDataPath + UWC_WINDOW_COUNT_SUB + SceneManager.GetActiveScene().buildIndex;
        int uwc_windowCount = 0;

        if (File.Exists(countPath))
        {
            FileStream countStream = new FileStream(countPath, FileMode.Open);

            uwc_windowCount = (int)formatter.Deserialize(countStream);
            countStream.Close();
        }
        else
        {
            Debug.LogError("Path not found in " + countPath);
        }

        if (File.Exists(path + 0)) 
        {
            //Uwc_window[] objList = GameObject.FindObjectsOfType<Uwc_window>;
            Uwc_window[] objList = GameObject.FindObjectsOfType<Uwc_window>();
            //GameObject[] objList = GameObject.FindGameObjectsWithTag("uWindowCapture window");
            for (int i = 0; i < objList.Length; i++)
            {
                uwc_windowList.Remove(objList[i]);
                Destroy(objList[i]);
            }
        }

        for (int i = 0; i < uwc_windowCount; i++)
        {
            if (File.Exists(path + i))
            {
                FileStream stream = new FileStream(path + i, FileMode.Open);
                Uwc_windowData data = formatter.Deserialize(stream) as Uwc_windowData;

                stream.Close();

                Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);
                Quaternion rotation = new Quaternion(data.rotation[0], data.rotation[1], data.rotation[2], data.rotation[3]);
                //Vector3 localScale = new Vector3(data.scale[0], data.scale[1], data.scale[2]);

                Uwc_window uwc_window = Instantiate(uwc_window_Prefab, position, rotation); //Quaternion.identity
                //uwc_window.transform.localScale = localScale;

                //uwc_window.TextField = data.TextField;
                uwc_window.GetComponentInChildren<UnityEngine.UI.Text>().text = data.TextField;
                uwc_window.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
                uwc_window.transform.rotation = new Quaternion(data.rotation[0], data.rotation[1], data.rotation[2], data.rotation[3]);
                uwc_window.transform.localScale = new Vector3(data.scale[0], data.scale[1], data.scale[2]);

                Debug.Log("[LoadData] TextField: " + data.TextField + " position: " + data.position + " rotation: " + data.rotation + " scale: " + data.scale);

                Debug.Log("Loaded a uwc_window from a save called " + (path + i));

                if (data.type == "Desktop")
                {
                    uwc_window.GetComponent<uWindowCapture.UwcWindowTexture>().type = uWindowCapture.WindowTextureType.Desktop;
                }
                else if (data.type == "Window")
                {
                    uwc_window.GetComponent<uWindowCapture.UwcWindowTexture>().type = uWindowCapture.WindowTextureType.Window;
                    uwc_window.GetComponent<uWindowCapture.UwcWindowTexture>().partialWindowTitle = data.partialWindowTitle;
                }
                else
                {
                    uwc_window.GetComponent<uWindowCapture.UwcWindowTexture>().type = uWindowCapture.WindowTextureType.Window;
                    UnityEngine.Debug.Log("Didn't recognize UwcWindowTexture.type");
                }
            }
            else
            {
                Debug.LogError("Path not found in " + (path + i));
            }
        }
        Debug.Log("Loaded all uwc_windows from save");
        
    }

    public void SaveDrawPlane()
    {
        Debug.Log("Saving...");
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + DRAWPLANE_SUB + SceneManager.GetActiveScene().buildIndex;
        string countPath = Application.persistentDataPath + DRAWPLANE_COUNT_SUB + SceneManager.GetActiveScene().buildIndex;

        FileStream countStream = new FileStream(countPath, FileMode.Create);

        formatter.Serialize(countStream, drawPlaneList.Count);
        countStream.Close();

        for (int i = 0; i < drawPlaneList.Count; i++)
        {
            FileStream stream = new FileStream(path + i, FileMode.Create);
            DrawPlaneData data = new DrawPlaneData(drawPlaneList[i]);

            var drawSprite = drawPlaneList[i].GetComponent<SpriteRenderer>().sprite;

            textureTemp = new Texture2D((int)drawSprite.rect.width, (int)drawSprite.rect.height);
            var pixels = drawSprite.texture.GetPixels((int)drawSprite.textureRect.x,
                                                    (int)drawSprite.textureRect.y,
                                                    (int)drawSprite.textureRect.width,
                                                    (int)drawSprite.textureRect.height);
            textureTemp.SetPixels(pixels);
            textureTemp.Apply();

            data.texPath = bruhshScript.SaveTexture(textureTemp, i);

            data.identifier = i;

            Debug.Log("[SaveData] texPath: " + data.texPath + " pos1: " + data.pos1 + " pos2: " + data.pos2 + " identifier: " + data.identifier);

            //byte[] bytes = new byte[stream.Length];
            //Debug.Log("Saved: " + stream.Read(bytes, (int)stream.Length, 0));
            formatter.Serialize(stream, data);
            stream.Close();
            Debug.Log("Saved drawPlane to " + (path + i));
        }
        UnityEngine.Debug.Log("Saved all drawPlanes");
    }

    public void LoadDrawPlane()
    {
        Debug.Log("Loading...");
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + DRAWPLANE_SUB + SceneManager.GetActiveScene().buildIndex;
        string countPath = Application.persistentDataPath + DRAWPLANE_COUNT_SUB + SceneManager.GetActiveScene().buildIndex;
        int drawPlaneCount = 0;

        if (File.Exists(countPath))
        {
            FileStream countStream = new FileStream(countPath, FileMode.Open);

            drawPlaneCount = (int)formatter.Deserialize(countStream);
            countStream.Close();
        }
        else
        {
            Debug.LogError("Path not found in " + countPath);
        }

        if (File.Exists(path + 0))
        {
            //GameObject[] objList = GameObject.FindGameObjectsWithTag("drawPlane");
            DrawPlane[] objList = GameObject.FindObjectsOfType<DrawPlane>();

            for (int i = 0; i < objList.Length; i++)
            {
                drawPlaneList.Remove(objList[i]);
                Destroy(objList[i]);
            }
        }

        for (int i = 0; i < drawPlaneCount; i++)
        {
            if (File.Exists(path + i))
            {
                FileStream stream = new FileStream(path + i, FileMode.Open);
                DrawPlaneData data = formatter.Deserialize(stream) as DrawPlaneData;

                stream.Close();

                Vector3 pos1 = new Vector3(data.pos1[0], data.pos1[1], data.pos1[2]);

                Vector3 pos2 = new Vector3(data.pos2[0], data.pos2[1], data.pos2[2]);

                string texPath = data.texPath;

                int identifier = data.identifier;

                Vector3 middlePoint = new Vector3(data.middlePoint[0], data.middlePoint[1], data.middlePoint[2]);

                DrawPlane drawPlane = RectangleToolScript.loadFactualPlane(pos1, pos2, texPath, i, middlePoint, data.layerList) as DrawPlane;

                //drawPlane.GetComponent<ValueScript>().setFourDFloatArrayValue(data.layerList);

                Debug.Log("[LoadData] texPath: " + data.texPath + " pos1: " + data.pos1 + " pos2: " + data.pos2 + " identifier: " + data.identifier);

                Debug.Log("Loaded a drawPlane from a save called " + (path + i));
            }
            else
            {
                Debug.LogError("Path not found in " + (path + i));
            }
        }
        Debug.Log("Loaded all drawPlanes from save");
    }

    public void SaveHandBrush()
    {

        Debug.Log("Saving...");
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + HANDBRUSH_SUB + SceneManager.GetActiveScene().buildIndex;
        string countPath = Application.persistentDataPath + HANDBRUSH_COUNT_SUB + SceneManager.GetActiveScene().buildIndex;

        FileStream countStream = new FileStream(countPath, FileMode.Create);

        formatter.Serialize(countStream, handBrushList.Count);
        countStream.Close();

        UnityEngine.Debug.Log("handBrushList.Count = " + handBrushList.Count);

        for (int i = 0; i < handBrushList.Count; i++)
        {
            FileStream stream = new FileStream(path + i, FileMode.Create);
            HandBrushData data = new HandBrushData(handBrushList[i]);
            for (int j = 0; j < data.brushCount - 1; j += 1)
            {
                Debug.Log("[SaveData] position: " + data.position[j] + " size: " + data.scale[j] + " brushCount: " + data.brushCount);
            }
            formatter.Serialize(stream, data);
            stream.Close();
            Debug.Log("Saved object to " + (path + i));
        }
        UnityEngine.Debug.Log("Saved all 3D handBrushes");
    }

    public void LoadHandBrush()
    {
        Debug.Log("Loading...");
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + HANDBRUSH_SUB + SceneManager.GetActiveScene().buildIndex;
        string countPath = Application.persistentDataPath + HANDBRUSH_COUNT_SUB + SceneManager.GetActiveScene().buildIndex;
        int handBrushCount = 0;

        if (File.Exists(countPath))
        {
            FileStream countStream = new FileStream(countPath, FileMode.Open);

            handBrushCount = (int)formatter.Deserialize(countStream);
            countStream.Close();
        }
        else
        {
            Debug.LogError("Path not found in " + countPath);
        }

        if (File.Exists(path + 0))
        {
            GameObject[] objList = GameObject.FindGameObjectsWithTag("handBrush");
            for (int i = 0; i < objList.Length; i++)
            {
                Destroy(objList[i]);
            }

            HandBrush[] objList2 = GameObject.FindObjectsOfType<HandBrush>();
            for (int i = 0; i < objList2.Length; i++)
            {
                handBrushList.Remove(objList2[i]);
                Destroy(objList2[i]);
            }

            /*
            foreach (HandBrush item in handBrushList)
            {
                Destroy(item);
            }
            */
        }

        for (int i = 0; i < handBrushCount; i++)
        {
            if (File.Exists(path + i))
            {
                FileStream stream = new FileStream(path + i, FileMode.Open);
                HandBrushData data = formatter.Deserialize(stream) as HandBrushData;

                stream.Close();

                Vector3[] position = new Vector3[data.position.Length];
                Vector3[] scale = new Vector3[data.scale.Length];

                //GameObject Drawable = gameObject.Find("Drawable");
                //GameObject Drawable = new GameObject("Drawable");
                bruhsh Drawable = Instantiate(Drawable_Prefab);

                //Drawable.AddComponent<HandBrush>();

                for (int j = 0; j < data.position.Length - 1; j += 1)
                {
                    UnityEngine.Debug.Log("data.position = " + data.position);
                    position[j] = new Vector3(data.position[j][0], data.position[j][1], data.position[j][2]);
                    scale[j] = new Vector3(data.scale[j][0], data.scale[j][1], data.scale[j][2]);
                    
                    Eraser handBrush = Instantiate(handBrush_Prefab, position[j], Quaternion.identity);

                    handBrush.transform.localScale = new Vector3(scale[j][0], scale[j][1], scale[j][2]);
                    handBrush.transform.position = new Vector3(position[j][0], position[j][1], position[j][2]);

                    handBrush.transform.parent = Drawable.transform;
                    handBrush.tag = "handBrush";

                    Debug.Log("[LoadData] position: " + data.position[j] + " size: " + data.scale[j] + " brushCount: " + data.brushCount);

                    Debug.Log("Loaded a handBrush number " + j + " from a save called " + (path + i));
                }
            }
            else
            {
                Debug.LogError("Path not found in " + (path + i));
            }
        }
        Debug.Log("Loaded all handBrushes from save");

    }

}