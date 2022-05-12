using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using System.Linq;
using System.Globalization;
using System.Drawing;

public class bruhsh : MonoBehaviour
{
    public GameObject drawable;
    public GameObject Brush;
    private GameObject FactualPlane;
    public GameObject planeCreator;
    public GameObject LeftSelectorG;
    public GameObject RightSelectorG;
    public GameObject RectoTool;
    public GameObject RightHand;
    public GameObject LeftHand;
    //public GameObject[] transPlanes;
    public List<DrawPlane> transPlanes = new List<DrawPlane>();
    //private GameObject cTPlane;
    public GameObject EraserR;
    public GameObject EraserL;
    public Texture2D[] texturePlanes;
    public Texture2D pixelTexArea;
    public GameObject VRCamera;
    public GameObject toolSelectSprite;
    //public GameObject scrollBar;
    public GameObject UICanvas;
    public GameObject layerSelectHover;
    public GameObject ColorWheel;
    public GameObject ColorCursor;
    public GameObject pointerObj;
    public GameObject Keyboard;
    public GameObject ScreenShotCam;
    public GameObject toolSelectHover;
    public GameObject cropParent;
    public GameObject BrushSizeText;
    public GameObject BrushSizeSlider;
    public GameObject EraserModeToggle;
    public GameObject BrushSoftnessText;
    public GameObject BrushSoftnessSlider;
    public GameObject LerpCountText;
    public GameObject LerpCountSlider;

    //public int BrushSize = 5;
    private static bool penExists = false;
    public bool foundPlane = false;
    public bool isHoveringRight = false;
    public bool isHoveringLeft = false;
    public bool GripIsActive = false;
    public int selPlane = 0;
    public int numPlanes = 0;
    public float Distance13;
    public float Distance14;
    //public float[] colorStrength;
    public float[] allPos;
    //public Vector3[] allN;
    //public Vector3[] allP;
    //Vector3 hitPoint = new Vector3(0f, 0f, 0f);
    Vector3 handHitPoint = new Vector3(0f, 0f, 0f);
    public static Pen Wacom = null;
    public int weirdOffsetX = 853;
    public int weirdOffsetY = 61;
    public float weirdOffsetYt = 0.0101522842639593908629f;
    public int toolSelect = 10;
    public Color backgroundColor = Color.white;
    public int selectedLayer = 1;
    public Color selectedColor = Color.black;
    public bool eraserMode = false;
    Vector2 right2DAxis;
    Vector2 left2DAxis;
    public Texture2D ColorWheelTex;
    public List<List<bool>> selectedPixels = new List<List<bool>>();
    bool selectRectangleActive = false;
    (int, int) firstCoordinate;
    (int, int) secondCoordinate;
    //public Vector3 vectorStraight;
    public Vector2 lastVectorPoint;
    public Vector2 currentVectorPoint;
    List<(int, int)> currentBrushMemory = new List<(int, int)>();
    List<(int, int)> lastBrushMemory = new List<(int, int)>();
    List<(int, int)> lastLastBrushMemory = new List<(int, int)>();
    public Vector2 lastLastVectorPoint;
    bool lineToolActive = false;
    public Material ColorViewMaterial;
    Color contrast;
    bool lassoActive = false;
    Vector2 mostLeftPoint;
    bool textActive = false;
    ////string textInput = "";
    //float FontSize = 12.0f;
    //Texture2D pixelTextArea;
    //Graphics pixelTexGraph;
    int selectedTextLayer;
    public Texture2D TextScreenshot;
    //string TextInput = "";
    //public Material MovingMaterial;
    bool firstMove = true;
    Vector3 initialHandPos;
    int planeCount = 0;
    bool rayHitSomething = false;
    Collider lastRayHit = null;
    Vector3 tempCropPos1 = new Vector3();
    Vector3 tempCropPos2 = new Vector3();
    Vector3 tempCropPos3 = new Vector3();
    Vector3 tempCropPos4 = new Vector3();
    bool cropIsActive = false;
    bool gotToolSelect = false;
    int[] brushSizeList = new int[12]; //it can be made to remember sizes from last launch
    double brushSoftness = 1;
    int lerpCount = 10; //how many Brush instances should be performed inbetween frames. Must be > 0

    public List<List<List<Color>>> layerList = new List<List<List<Color>>>(); //static
    public List<GameObject> UIButtons = new List<GameObject>();
    //public List<(int, Graphics, string)> textLayers = new List<(int, Graphics, string)>();
    List<(int, string, Texture2D, Rect)> textLayers = new List<(int, string, Texture2D, Rect)>(); //int layer, string InputText, Texture2D tex, Rect hitBox

    [SerializeField] GameObject layerButton;

    [SerializeField]
    private Vector3 pos1;
    [SerializeField]
    private Vector3 pos2;
    [SerializeField]
    private Vector3 pos3;
    [SerializeField]
    private Vector3 pos4;
    //[SerializeField]
    //private static Vector3 p;
    //[SerializeField]
    //private Vector3 n;
    [SerializeField]
    Vector3 cropPos1;
    [SerializeField]
    Vector3 cropPos2;
    [SerializeField]
    Vector3 cropPos3;
    [SerializeField]
    Vector3 cropPos4;

    planeCreation planeMaker;
    RectangleTool RectTool;
    HandPresence RightHandScript;
    HandPresence LeftHandScript;
    Keyboard KeyboardScript;
    ScreenShot ScreenShotScript;
    //SphereCollider EraserScript;
    //XRRayInteractor LeftSelectorS;
    //RightSelector RightSelectorS;
    //XRDirectInteractor RightSelectorDI;
    //XRBaseInteractor RightSelectorDI;

    /*
     * layerList< layer < y < x > > >
     * 
     * selectLayer:
     * 0 = background (inaccessible)
     * >0 = layers
     * 
     * toolSelect:
     * 0  = Move ✔
     * 1  = SelectRectangle ✔
     * 2  = Crop ✔
     * 3  = Pipette ✔
     * 4  = Bucket ✔
     * 5  = Lasso ✔
     * 6  = Select Brush ✔ (intentionally no vectoring)
     * 7  = Pencil ✔
     * 8  = Eraser ✔
     * 9  = Text ✔ (discontinued) is now createFactualPlane
     * 10 = Brush ✔
     * 11 = Line Tool ✔
     * 
     * eraserMode:
     * false = eraser pencil
     * true = eraser brush
     * 
     * Features to add:
     * make the brush prettier ❌ (?)
     * 
     *  I ended up doing it with a RenderTexture. Here were the steps I took.

        Duplicate the main game camera, call it "ScreenshotCam" and set it to a lower depth (not sure if that was required), create a ScreenShotScript.cs (follow)

        Make a new render texture in the project panel and set it to a decent size

        Make the render texture the "target Texture" of the ScreenshotCam

        Add some UI text for score and a UI image for the game logo. Put these both on a layer that is only rendered by the ScreenshotCam (ie uncheck the layer in the culling mask dropdown of the main camera)

        output the current score to the UI text box as the game is being player

        Make the render texture created in step 2 active while taking the screenshot.

        Here is the capture function I'm using:

     function CaptureScreen() {
         yield  WaitForEndOfFrame();
         var currentRT = RenderTexture.active;
         RenderTexture.active = scoreRenderTexture;
         var tex = new Texture2D(screenshotScoreCamera.pixelWidth, screenshotScoreCamera.pixelHeight, TextureFormat.RGB24, false);
         //Read screen contents into the texture
         tex.ReadPixels(screenshotScoreCamera.pixelRect, 0, 0);
         tex.Apply();
     
             RenderTexture.active = currentRT;
         return tex;
     }
    */

    void Start()
    {
        planeMaker = planeCreator.GetComponent<planeCreation>();
        RectTool = RectoTool.GetComponent<RectangleTool>();
        RightHandScript = RightHand.GetComponent<HandPresence>();
        LeftHandScript = LeftHand.GetComponent<HandPresence>();
        KeyboardScript = Keyboard.GetComponent<Keyboard>();
        ScreenShotScript = ScreenShotCam.GetComponent<ScreenShot>();

        brushSizeList = new int[] { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5};

        findTablet();
    }

    void Awake()
    {
        List<List<Color>> tempList1 = new List<List<Color>>();
        List<Color> tempList2 = new List<Color>();
        tempList2.Add(new Color(1f, 1f, 1f, 1f));
        layerList.Add(tempList1);

        UIButtons.Add(layerButton); //background button

        GameObject newLayerButton = Instantiate(layerButton, UICanvas.transform.position, UICanvas.transform.rotation, UICanvas.transform);
        UIButtons.Add(newLayerButton);
        newLayerButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "Layer 1";
        newLayerButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { selectLayer(1); });

        //private PictureBox pictureBox1 = new PictureBox();
        //pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.paintText);
    }

    void Update()
    {
        //HandBrush[] handBrushList = GameObject.FindObjectsOfType<HandBrush>();
        /*
        foreach (GameObject handBrush in handBrushList)
        {
            for (int i = 0; i < handBrush.transform.childCount; i += 1)
            {
                handBrush.transform.GetChild(i).transform.LookAt(VRCamera.transform);
                handBrush.transform.GetChild(i).transform.rotation *= Quaternion.Euler(90, 0, 0);
            }
        }
        */
        /*
        GameObject[] handBrushList = GameObject.FindGameObjectsWithTag("handBrush");

        foreach (GameObject handBrush in handBrushList)
        {
            handBrush.transform.LookAt(VRCamera.transform);
            handBrush.transform.rotation *= Quaternion.Euler(90, 0, 0);
        }
        */
        brushSoftness = 0.01 * Math.Pow(Math.E, 9.2103 * BrushSoftnessSlider.GetComponent<UnityEngine.UI.Slider>().value);
        BrushSoftnessText.GetComponent<UnityEngine.UI.Text>().text = "Exponent: " + brushSoftness.ToString().Substring(0, 4);

        pos1 = planeMaker.getPos1();
        pos2 = planeMaker.getPos2();
        pos3 = planeMaker.getPos3();
        pos4 = planeMaker.getPos4();

        bool foundNumber = false;
        //bool foundN = false;
        int tempj = 0;
        //int tempn = 0;

        if (planeCount != transPlanes.Count)
        {
            cropPos1 = pos1;
            cropPos2 = pos2;
            cropPos3 = pos3;
            cropPos4 = pos4;

            tempCropPos1 = pos1;
            tempCropPos2 = pos2;
            tempCropPos3 = pos3;
            tempCropPos4 = pos4;

            planeCount = transPlanes.Count;
        }

        right2DAxis = RightHandScript.getPrimary2DAxis();
        left2DAxis = LeftHandScript.getPrimary2DAxis();

        transPlanes = SaveSystem.drawPlaneList;

        EraserR.SetActive(false);
        EraserL.SetActive(false);

        if (allPos.Length > 11 && pos1.x == allPos[allPos.Length - 12])
        {
            foundNumber = true;
        }
        if (!foundNumber)
        {
            foreach (float numPlane in allPos)
            {
                if (tempj % 12 == 0 && !foundNumber)
                {
                    //Currently only checks x, for better accuracy check all
                    if (pos1.x == numPlane)
                    {
                        foundNumber = true;
                        break;
                    }
                }
                tempj++;
            }
        }

        if (foundNumber == false && pos1.x != 0f)
        {
            foundNumber = true;
            //UnityEngine.Debug.Log("newly added values are " + pos1 + " and " + pos2 + " and " + pos3 + " and " + pos4);
            allPos = allPos.Concat(new float[] { pos1.x }).ToArray(); // 0
            allPos = allPos.Concat(new float[] { pos1.y }).ToArray(); // 1
            allPos = allPos.Concat(new float[] { pos1.z }).ToArray(); // 2
            allPos = allPos.Concat(new float[] { pos2.x }).ToArray(); // 3
            allPos = allPos.Concat(new float[] { pos2.y }).ToArray(); // 4
            allPos = allPos.Concat(new float[] { pos2.z }).ToArray(); // 5
            allPos = allPos.Concat(new float[] { pos3.x }).ToArray(); // 6
            allPos = allPos.Concat(new float[] { pos3.y }).ToArray(); // 7
            allPos = allPos.Concat(new float[] { pos3.z }).ToArray(); // 8
            allPos = allPos.Concat(new float[] { pos4.x }).ToArray(); // 9
            allPos = allPos.Concat(new float[] { pos4.y }).ToArray(); // 10
            allPos = allPos.Concat(new float[] { pos4.z }).ToArray(); // 11
        }

        //Selector
        if (RectTool.getMode() == 1)
        {
            LeftSelectorG.SetActive(true);
            RightSelectorG.SetActive(true);
            //toolSelectSprite.SetActive(true);
            UICanvas.SetActive(true);

            /*
            if (RightHandScript.getPressedGrip() && isHoveringRight)
            {
                //UnityEngine.Debug.Log("" + RightSelectorG.XRBaseInteractor.GetValidTargets());
                //FactualPlane.AddComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();   this will help
               // UnityEngine.Debug.Log("Try and make this selected");
            }
            else if (LeftHandScript.getPressedGrip() && isHoveringLeft)
            {

            }
            */

            if (RightHandScript.getPressedTouchpad() && toolSelect != 9)
            {
                selectedColor = ColorWheelTex.GetPixel((int)(((right2DAxis.x + 1) / 2) * ColorWheelTex.width), (int)(((right2DAxis.y + 1) / 2) * ColorWheelTex.height));

                ColorViewMaterial.color = selectedColor;
            }
            else if (LeftHandScript.getPressedTouchpad() && toolSelect != 9)
            {
                selectedColor = ColorWheelTex.GetPixel((int)(((left2DAxis.x + 1) / 2) * ColorWheelTex.width), (int)(((left2DAxis.y + 1) / 2) * ColorWheelTex.height));

                ColorViewMaterial.color = selectedColor;
            }

            if (RightHandScript.touchedTouchpad && toolSelect != 9)
            {
                ColorCursor.SetActive(true);
                ColorCursor.transform.localPosition = new Vector3(-right2DAxis.x + 1f, right2DAxis.y * 1f, 0f); //1f is whatever the size idk
            }
            else if (LeftHandScript.touchedTouchpad && toolSelect != 9)
            {
                ColorCursor.SetActive(true);
                ColorCursor.transform.localPosition = new Vector3(-left2DAxis.x + 1f, left2DAxis.y * 1f, 0f); //1f is whatever the size idk
            }
            else
            {
                ColorCursor.SetActive(false);
            }

            if (RightHandScript.getPressedTrigger() && !gotToolSelect)
            {
                RaycastHit rayToolSelect;
                if (Physics.Raycast(RightHand.transform.position, -RightHand.transform.forward, out rayToolSelect, Mathf.Infinity)) //, 17
                {
                    Debug.DrawRay(RightHand.transform.position, -RightHand.transform.forward * rayToolSelect.distance, Color.yellow);

                    if (rayToolSelect.collider.gameObject != null)
                    {
                        if (rayToolSelect.collider.gameObject.name == "1") { toolSelect = 0; gotToolSelect = true; }
                        else if (rayToolSelect.collider.gameObject.name == "2") { toolSelect = 1; gotToolSelect = true; }
                        else if (rayToolSelect.collider.gameObject.name == "3") { toolSelect = 2; gotToolSelect = true; }
                        else if (rayToolSelect.collider.gameObject.name == "4") { toolSelect = 3; gotToolSelect = true; }
                        else if (rayToolSelect.collider.gameObject.name == "5") { toolSelect = 4; gotToolSelect = true; }
                        else if (rayToolSelect.collider.gameObject.name == "6") { toolSelect = 5; gotToolSelect = true; }
                        else if (rayToolSelect.collider.gameObject.name == "7") { toolSelect = 6; gotToolSelect = true; }
                        else if (rayToolSelect.collider.gameObject.name == "8") { toolSelect = 7; gotToolSelect = true; }
                        else if (rayToolSelect.collider.gameObject.name == "9") { toolSelect = 8; gotToolSelect = true; }
                        else if (rayToolSelect.collider.gameObject.name == "10") { toolSelect = 9; gotToolSelect = true; }
                        else if (rayToolSelect.collider.gameObject.name == "11") { toolSelect = 10; gotToolSelect = true; }
                        else if (rayToolSelect.collider.gameObject.name == "12") { toolSelect = 11; gotToolSelect = true; }

                        BrushSizeSlider.GetComponent<UnityEngine.UI.Slider>().value = (float)Math.Sqrt(brushSizeList[toolSelect] - 1) / 25;
                        BrushSizeText.GetComponent<UnityEngine.UI.Text>().text = "Brush Size: " + brushSizeList[toolSelect];
                        
                        toolSelectHover.transform.localPosition = new Vector3(0.10304f, 0.921f - 0.155f * toolSelect, 0.001f);
                    }
                }
            }
            else if(!RightHandScript.getPressedTrigger())
            {
                gotToolSelect = false;
            }

            if (toolSelect == 2 && RightHandScript.getPressedGrip()) //Crop
            {
                if (!GripIsActive)
                {
                    enterCrop();
                }
                setGripActive(true);

                RaycastHit rayCrop;
                if (Physics.Raycast(RightHand.transform.position, -RightHand.transform.forward, out rayCrop, Mathf.Infinity) || rayHitSomething)
                {
                    Debug.DrawRay(RightHand.transform.position, -RightHand.transform.forward * rayCrop.distance, Color.yellow);
                    rayHitSomething = true;

                    if (rayCrop.collider != null && rayCrop.collider.gameObject.tag == "CropTool")
                    {
                        lastRayHit = rayCrop.collider;
                    }

                    if (lastRayHit != null)
                    {
                        if (lastRayHit.gameObject.name == "TopLeft") changeCropPosRel(0); //rayCrop.collider.gameObject.name
                        else if (lastRayHit.gameObject.name == "Top") changeCropPosRel(1);
                        else if (lastRayHit.gameObject.name == "TopRight") changeCropPosRel(2);
                        else if (lastRayHit.gameObject.name == "Right") changeCropPosRel(3);
                        else if (lastRayHit.gameObject.name == "BottomRight") changeCropPosRel(4);
                        else if (lastRayHit.gameObject.name == "Bottom") changeCropPosRel(5);
                        else if (lastRayHit.gameObject.name == "BottomLeft") changeCropPosRel(6);
                        else if (lastRayHit.gameObject.name == "Left") changeCropPosRel(7);
                    }
                }
            }
            else if (toolSelect == 2 && GripIsActive && !RightHandScript.getPressedGrip())
            {
                exitCrop();
                setGripActive(false);
                rayHitSomething = false;
            }
            /*
            if (scrollBar.active)
            {
                for (int i = 0; i < UIButtons.Count; i += 1)
                {
                    UIButtons[i].transform.localPosition = new Vector3(0.064f, scrollBar.GetComponent<UnityEngine.UI.Scrollbar>().value, 0f);
                }
            }
            */

            for (int i = 0; i < UIButtons.Count; i += 1) // - 1
            {
                UIButtons[i].transform.localPosition = new Vector3(0.064f, i * 0.3f, 0f);
                UIButtons[i].transform.localScale = new Vector3(0.007483f, 0.007483f, 0.007483f);
                UIButtons[i].transform.localRotation = Quaternion.identity;
            }

            //Debug.Log("UIButtons " + UIButtons.Count + ", layerList " + layerList.Count);
            //if(transPlanes[selPlane].GetComponent<ValueScript>().getFourDFloatArrayValue().Length > 1) Debug.Log("saved count: " + convertFloatToList(transPlanes[selPlane].GetComponent<ValueScript>().getFourDFloatArrayValue()).Count);

            if (UIButtons.Count != layerList.Count)
            {
                int UIButtonsCount = UIButtons.Count;
                //destroy for
                for (int i = 0; i < UIButtonsCount - 2; i++) //not destroying background and layer 1
                {
                    //Debug.Log("destroyed place " + (UIButtonsCount - 1 - i));
                    Destroy(UIButtons[UIButtonsCount - 1 - i]); //going in reverse
                    UIButtons.RemoveAt(UIButtonsCount - 1 - i);
                }

                //create for
                for (int j = 0; j < layerList.Count - 2; j++)
                {
                    GameObject newLayerButton = Instantiate(layerButton, UICanvas.transform.position, UICanvas.transform.rotation, UICanvas.transform);
                    UIButtons.Add(newLayerButton);
                    newLayerButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "Layer " + (j + 2);
                    int tempUIButtonsCount = j + 2;
                    newLayerButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { selectLayer(tempUIButtonsCount); });
                }
            }
        }
        else
        {
            LeftSelectorG.SetActive(false);
            RightSelectorG.SetActive(false);
            //toolSelectSprite.SetActive(false);
            UICanvas.SetActive(false);
        }

        if (RectTool.getMode() == 2)
        {
            float HandBrushSize = 0.005f;
            if (RightHandScript.getPressedTrigger())
            {
                handHitPoint = new Vector3(RightHandScript.transform.position.x, RightHandScript.transform.position.y, RightHandScript.transform.position.z);
                var bru = Instantiate(Brush, handHitPoint, Quaternion.identity, drawable.transform);
                //Vector3 scale = bru.transform.localScale;
                //scale.Set(HandBrushSize, HandBrushSize, HandBrushSize);
                //bru.transform.localScale = scale;
                bru.SetActive(true);
                bru.transform.localScale = Vector3.one * HandBrushSize;
                bru.transform.Rotate(new Vector3(RightHandScript.transform.rotation.x * 360f, (RightHandScript.transform.rotation.y * 360f) + 45f, (RightHandScript.transform.rotation.z * 360f) + 45f));
                //UnityEngine.Debug.Log("Made a bru with rotation " + bru.transform.rotation);
                bru.tag = "handBrush";
            }
            else if (LeftHandScript.getPressedTrigger())
            {
                handHitPoint = new Vector3(LeftHandScript.transform.position.x, LeftHandScript.transform.position.y, LeftHandScript.transform.position.z);
                var bru = Instantiate(Brush, handHitPoint, Quaternion.identity, drawable.transform);
                //Vector3 scale = bru.transform.localScale;
                //scale.Set(HandBrushSize, HandBrushSize, HandBrushSize);
                //bru.transform.localScale = scale;
                bru.SetActive(true);
                bru.transform.localScale = Vector3.one * HandBrushSize;
                bru.transform.Rotate(new Vector3(LeftHandScript.transform.rotation.x * 360f, (LeftHandScript.transform.rotation.y * 360f) + 45f, (LeftHandScript.transform.rotation.z * 360f) + 45f));
                //UnityEngine.Debug.Log("Made a bru with rotation " + bru.transform.rotation);
                bru.tag = "handBrush";
            }
            else if (RightHandScript.getPressedGrip())
            {
                EraserR.SetActive(true);
                EraserL.SetActive(false);
                setGripActive(true);
            }
            else if (LeftHandScript.getPressedGrip())
            {
                EraserL.SetActive(true);
                EraserR.SetActive(false);
                setGripActive(true);
            }
            else
            {
                setGripActive(false);
            }
        }

        if (penExists && transPlanes.Count != 0)
        {
            if (cropIsActive && toolSelect != 2)
            {
                float originalWidth = pixelTexArea.width / Vector3.Distance(pos1, pos3);
                float originalHeight = pixelTexArea.height / Vector3.Distance(pos1, pos4);

                float newWidth = originalWidth * Vector3.Distance(tempCropPos1, tempCropPos3);
                float newHeight = originalHeight * Vector3.Distance(tempCropPos1, tempCropPos4);


                //(pixelTexArea.width / Vector3.Distance(pos1, pos3)) = Resolution, [pixel per unit]
                Vector2 upperLeftPoint = new Vector2((pixelTexArea.width / Vector3.Distance(pos1, pos3)) * Vector3.Distance(new Vector3(tempCropPos1.x, 0f, tempCropPos1.z), new Vector3(pos1.x, 0f, pos1.z)),
                                                    (pixelTexArea.height / Vector3.Distance(pos1, pos4)) * Vector3.Distance(new Vector3(0f, tempCropPos1.y, 0f), new Vector3(0f, pos1.y, 0f)));

                layerList = extendLayerList(layerList, (int)newWidth, (int)newHeight, upperLeftPoint);
                Texture2D newPixelTexArea = ExtendTex(pixelTexArea, (int)newWidth, (int)newHeight, upperLeftPoint);
                RectTool.cropFactualPlane(tempCropPos1, tempCropPos2, tempCropPos4, tempCropPos4, newPixelTexArea, transPlanes[selPlane], RightHand.transform.position, convertListToFloat(layerList));
                Destroy(SaveSystem.drawPlaneList[selPlane].gameObject);
                transPlanes[selPlane] = SaveSystem.drawPlaneList[SaveSystem.drawPlaneList.Count - 1]; //from here on, it's bogged
                texturePlanes[selPlane] = newPixelTexArea;
                //Destroy(SaveSystem.drawPlaneList[SaveSystem.drawPlaneList.Count - 1].gameObject);
                SaveSystem.drawPlaneList.RemoveAt(SaveSystem.drawPlaneList.Count - 1);
                //SaveSystem.drawPlaneList[selPlane] = transPlanes[selPlane]; //this probably just a pointer

                if (transPlanes.Count != 0)
                {
                    layerList = convertFloatToList(transPlanes[selPlane].GetComponent<ValueScript>().getFourDFloatArrayValue());
                }
                //pixelTexArea = texturePlanes[selPlane]; //the above seems to work fine except Destroy. The pixelTexArea is truly unruly
                pixelTexArea = newPixelTexArea;

                //currently selects everything
                selectedPixels = new List<List<bool>>();
                //for (int i = 0; i < pixelTexArea.width; i++)
                for (int i = 0; i < layerList[0].Count; i++)
                {
                    List<bool> tempList = new List<bool>();
                    //for (int j = 0; j < pixelTexArea.height; j++)
                    for (int j = 0; j < layerList[0][i].Count; j++)
                    {
                        tempList.Add(true);
                    }
                    selectedPixels.Add(tempList);
                }
            }

            pointerObj.transform.parent.transform.position = transPlanes[selPlane].transform.position;
            pointerObj.transform.parent.transform.rotation = transPlanes[selPlane].transform.rotation;

            changeBrushSize(BrushSizeSlider.GetComponent<UnityEngine.UI.Slider>().value);
            changeLerpCount(LerpCountSlider.GetComponent<UnityEngine.UI.Slider>().value);

            if (toolSelect == 8)
            {
                EraserModeToggle.SetActive(true);
                eraserMode = EraserModeToggle.GetComponent<UnityEngine.UI.Toggle>().isOn;
            }
            else if (toolSelect == 10)
            {
                BrushSoftnessText.SetActive(true);
                BrushSoftnessSlider.SetActive(true);
                //brushSoftness = 0.01 + BrushSoftnessSlider.GetComponent<UnityEngine.UI.Slider>().value;
                brushSoftness = 0.01 * Math.Pow(Math.E, 9.2103 * BrushSoftnessSlider.GetComponent<UnityEngine.UI.Slider>().value);
                BrushSoftnessText.GetComponent<UnityEngine.UI.Text>().text = "Exponent: " + brushSoftness.ToString().Substring(0, 3);
                //0.01e^{9.2103x}
            }
            else
            {
                EraserModeToggle.SetActive(false);
            }

            if (toolSelect < 6) //012345 don't need brushSize
            {
                BrushSizeText.SetActive(false);
                BrushSizeSlider.SetActive(false);
            }
            else
            {
                BrushSizeText.SetActive(true);
                BrushSizeSlider.SetActive(true);
            }
            if (toolSelect != 10)
            {
                BrushSoftnessText.SetActive(false);
                BrushSoftnessSlider.SetActive(false);
            }
            if (toolSelect == 10 || toolSelect == 7)
            {
                LerpCountText.SetActive(true);
                LerpCountSlider.SetActive(true);
            }
            else
            {
                LerpCountText.SetActive(false);
                LerpCountSlider.SetActive(false);
            }
            if (toolSelect == 9 && RightHandScript.getPressedGrip()) //literally only made for right hand
            {
                selectRayDrawPlane(RightHand, "DrawPlane");
            }

            pointerObj.transform.localPosition = new Vector3(
                        (((Wacom.position.x.ReadValue() - 103) / 1920) * pixelTexArea.width) / 1000, // - BrushSize,  + weirdOffsetX,  - 960
                        (((Wacom.position.y.ReadValue() - 450) / 1080) * pixelTexArea.height) / 1000, //  + weirdOffsetYt * pixelTexArea.height, + weirdOffsetY
                        0f
                        );

            if (!LeftHandScript.getPressedGrip() && !RightHandScript.getPressedGrip())
            {
                if (layerList.Count > 1) // && UIButtons.Count - 1 >= selectedLayer //UIButtons.Count == layerList.Count && 
                {
                    //toolSelectSprite.transform.GetChild(0).transform.position = transPlanes[selPlane].transform.position
                    //- 0.65f * transPlanes[selPlane].transform.right * transPlanes[selPlane].GetComponent<BoxCollider>().size.x;

                    UICanvas.transform.position = transPlanes[selPlane].transform.position
                    + 1f * transPlanes[selPlane].transform.right * transPlanes[selPlane].GetComponent<BoxCollider>().size.x;

                    UICanvas.transform.eulerAngles = transPlanes[selPlane].transform.eulerAngles;

                    //toolSelectSprite.transform.GetChild(0).transform.eulerAngles = new Vector3(-transPlanes[selPlane].transform.eulerAngles.x, transPlanes[selPlane].transform.eulerAngles.y + 180f, transPlanes[selPlane].transform.eulerAngles.z); //  + new Vector3(-transPlanes[selPlane].transform.eulerAngles.x, 180f, 0f);

                    if(UIButtons.Count - 1 >= selectedLayer) layerSelectHover.transform.localPosition = UIButtons[selectedLayer].transform.localPosition + new Vector3(0.651f, 0f, 0f);
                    //layerSelectHover.transform.eulerAngles = UICanvas.transform.eulerAngles + new Vector3(0f, 180f, 0f);

                    ColorWheel.transform.localPosition = new Vector3(2f, 0f, 0f);
                    //ColorWheel.transform.eulerAngles = UICanvas.transform.eulerAngles + new Vector3(0f, 180f, 0f);
                    ColorWheel.transform.localScale = UICanvas.transform.localScale + new Vector3(0.3f, 0.3f, 0.3f);// + new Vector3(0.6f, 0.6f, 0.6f);
                }
            }

            if (layerList.Count - 1 < selectedLayer)
            {
                //UnityEngine.Debug.Log("layerList.Count - 1: " + (layerList.Count - 1) + " selectedLayer: " + selectedLayer);

                /*
                List<List<Color>> tempList1 = new List<List<Color>>();
                for (int i = 0; i < pixelTexArea.height; i += 1)
                {
                    List<Color> tempList2 = new List<Color>();
                    for (int j = 0; j < pixelTexArea.width; j += 1)
                    {
                        tempList2.Add(new Color(1f, 1f, 1f, 0f));
                    }
                    tempList1.Add(tempList2);
                }
                layerList.Add(tempList1);
                */

                createNewLayer();
                //selectedLayer = layerList.Count - 1;

                //UnityEngine.Debug.Log("Just added tempList1 to layerList and now it has a count = " + layerList.Count);
            }

            if (layerList.Count == 0)
            {
                createNewLayerList();
                UnityEngine.Debug.Log("createNewLayerList() 1");
                selectEverything();
            }
            else if (layerList[0].Count == 0)
            {
                createNewLayerList();
                UnityEngine.Debug.Log("createNewLayerList() 2");
                selectEverything();
            }
            else if (layerList[0][0].Count == 0)
            {
                createNewLayerList();
                UnityEngine.Debug.Log("createNewLayerList() 3");
                selectEverything();
            }

            if (Wacom.tip.isPressed || Wacom.eraser.isPressed)
            {
                currentBrushMemory = new List<(int, int)>();

                if (toolSelect != 9) { textActive = false; KeyboardScript.setKeyboardsActive(false); }

                //float t = 0; //step through straight
                //currentVectorPoint = new Vector2(Wacom.position.x.ReadValue(), Wacom.position.y.ReadValue());
                currentVectorPoint = new Vector2((int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width)),
                        (int)((((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)
                        );

                if (lastVectorPoint == new Vector2())
                {
                    lastVectorPoint = currentVectorPoint;
                }
                if (lastLastVectorPoint == new Vector2())
                {
                    lastLastVectorPoint = lastVectorPoint;
                }

                if (lastLastVectorPoint == new Vector2())
                {
                    lastLastVectorPoint = lastVectorPoint;
                }

                if (lastBrushMemory.Count == 0)
                {
                    lastBrushMemory = new List<(int, int)>();
                }
                if (lastLastBrushMemory.Count == 0)
                {
                    lastLastBrushMemory = new List<(int, int)>();
                }

                //vectorStraight = lastVectorPoint + t * (currentVectorPoint - lastVectorPoint);

                //if (layerList.Count == 0)
                //{
                //    createNewLayerList();
                //    UnityEngine.Debug.Log("createNewLayerList() 1");
                //}
                //else if (layerList[0].Count == 0)
                //{
                //    createNewLayerList();
                //    UnityEngine.Debug.Log("createNewLayerList() 2");
                //}
                //else if (layerList[0][0].Count == 0)
                //{
                //    createNewLayerList();
                //    UnityEngine.Debug.Log("createNewLayerList() 3");
                //}

                if (toolSelect == 10) //Brush
                {
                    //old Brush
                    float penPressure = Wacom.pressure.ReadValue();

                    if (currentVectorPoint != lastVectorPoint)
                    {
                        //y for
                        for (int tempy = 0; tempy < 2 * brushSizeList[toolSelect] + 1; tempy++)
                        {
                            //x for
                            for (int tempx = 0; tempx < 2 * brushSizeList[toolSelect] + 1; tempx++)
                            {
                                //if ((int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height) > 0 && (int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - BrushSize) > 0 && (int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height) < layerList[selectedLayer].Count && (int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - BrushSize) < layerList[selectedLayer][(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)].Count)

                                //Color tempColor = layerList[selectedLayer][(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - BrushSize)];
                                float distance = (float)Math.Sqrt((tempx - brushSizeList[toolSelect]) * (tempx - brushSizeList[toolSelect]) + (tempy - brushSizeList[toolSelect]) * (tempy - brushSizeList[toolSelect]));
                                //float distanceNorm = (brushSizeList[toolSelect] - distance) / brushSizeList[toolSelect]; //linear DistanceNorm
                                float distanceNorm = (float)Math.Pow((brushSizeList[toolSelect] - distance) / brushSizeList[toolSelect], brushSoftness);

                                for (int i = 0; i < lerpCount; i++)
                                {
                                    Vector2 lerpPos = Vector2.Lerp(currentVectorPoint, lastVectorPoint, (float)(i + 1) / lerpCount) + new Vector2(tempy - brushSizeList[toolSelect], tempx - brushSizeList[toolSelect]);
                                    //Debug.Log(Vector2.Lerp(currentVectorPoint, lastVectorPoint, (float)(i + 1) / lerpCount) + ", (i + 1) / lerpCount = " + ((i + 1) / lerpCount));

                                    if (distance < brushSizeList[toolSelect] && checkOOB((int)lerpPos.y, (int)lerpPos.x))
                                    {
                                        Color tempColor = layerList[selectedLayer][(int)lerpPos.y][(int)lerpPos.x];

                                        tempColor.a = selectedColor.a * penPressure * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm * penPressure);
                                        tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                                        tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                                        tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                                        if (tempColor.r > 1f) tempColor.r = 1f;
                                        if (tempColor.g > 1f) tempColor.g = 1f;
                                        if (tempColor.b > 1f) tempColor.b = 1f;
                                        if (tempColor.a > 1f) tempColor.a = 1f;
                                        if (tempColor.r < 0f) tempColor.r = 0f;
                                        if (tempColor.g < 0f) tempColor.g = 0f;
                                        if (tempColor.b < 0f) tempColor.b = 0f;
                                        if (tempColor.a < 0f) tempColor.a = 0f;

                                        //if (selectedPixels[(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - BrushSize)])
                                        if (selectedPixels[(int)lerpPos.y][(int)lerpPos.x])
                                        {
                                            layerList[selectedLayer][(int)lerpPos.y][(int)lerpPos.x] = tempColor;

                                            renderPixelToTex((int)lerpPos.y, (int)lerpPos.x);
                                        }

                                    }
                                }

                            }

                        }
                    }
                    else
                    {
                        //y for
                        for (int tempy = 0; tempy < 2 * brushSizeList[toolSelect] + 1; tempy++)
                        {
                            //x for
                            for (int tempx = 0; tempx < 2 * brushSizeList[toolSelect] + 1; tempx++)
                            {
                                Vector2 lerpPos = currentVectorPoint + new Vector2(tempy - brushSizeList[toolSelect], tempx - brushSizeList[toolSelect]);

                                float distance = (float)Math.Sqrt((tempx - brushSizeList[toolSelect]) * (tempx - brushSizeList[toolSelect]) + (tempy - brushSizeList[toolSelect]) * (tempy - brushSizeList[toolSelect]));
                                float distanceNorm = (brushSizeList[toolSelect] - distance) / brushSizeList[toolSelect];

                                if (distance < brushSizeList[toolSelect] && checkOOB((int)lerpPos.y, (int)lerpPos.x))
                                {
                                    Color tempColor = layerList[selectedLayer][(int)lerpPos.y][(int)lerpPos.x];

                                    tempColor.a = selectedColor.a * penPressure * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm * penPressure);
                                    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                                    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                                    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                                    if (tempColor.r > 1f) tempColor.r = 1f;
                                    if (tempColor.g > 1f) tempColor.g = 1f;
                                    if (tempColor.b > 1f) tempColor.b = 1f;
                                    if (tempColor.a > 1f) tempColor.a = 1f;
                                    if (tempColor.r < 0f) tempColor.r = 0f;
                                    if (tempColor.g < 0f) tempColor.g = 0f;
                                    if (tempColor.b < 0f) tempColor.b = 0f;
                                    if (tempColor.a < 0f) tempColor.a = 0f;

                                    //if (selectedPixels[(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - BrushSize)])
                                    if (selectedPixels[(int)lerpPos.y][(int)lerpPos.x])
                                    {
                                        layerList[selectedLayer][(int)lerpPos.y][(int)lerpPos.x] = tempColor;

                                        renderPixelToTex((int)lerpPos.y, (int)lerpPos.x);
                                    }

                                }
                            }
                        }
                    }

                    ///////////////////////////

                    //List<(int, int)> brushingCoords = new List<(int, int)>();

                    //Vector2 brushingStraightA = vectorStraight + new Vector2(Math.Sin(Vector2.Angle(vectorStraight, Vector2.right)) * BrushSize, Math.Cos(Vector2.Angle(vectorStraight, Vector2.right)) * BrushSize);
                    //Vector2 brushingStraightB = vectorStraight - new Vector2(Math.Sin(Vector2.Angle(vectorStraight, Vector2.right)) * BrushSize, Math.Cos(Vector2.Angle(vectorStraight, Vector2.right)) * BrushSize);

                    ////y for
                    //for (int tempy = 0; tempy < 2 * BrushSize + 1; tempy++)
                    //{
                    //    //x for
                    //    for (int tempx = 0; tempx < 2 * BrushSize + 1; tempx++)
                    //    {
                    //        if (tempy + currentVectorPoint.y > 0 && tempx + currentVectorPoint.x > 0 && tempy + currentVectorPoint.y < layerList[selectedLayer].Count && tempx + currentVectorPoint.x < layerList[selectedLayer][tempy + (int)currentVectorPoint.y].Count)
                    //        {

                    //                //if (!brushingCoords.Contains((currentVectorPoint.x - tempx - BrushSize, tempy + currentVectorPoint.y + weirdOffsetYt * pixelTexArea.height)))
                    //                //{
                    //                //    brushingCoords.Add((currentVectorPoint.x - tempx - BrushSize, tempy + currentVectorPoint.y + weirdOffsetYt * pixelTexArea.height));
                    //                //}

                    //            Color tempColor = layerList[selectedLayer][(int)(tempy + currentVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(currentVectorPoint.x - tempx - BrushSize)];
                    //            float distance = (float)Math.Sqrt((tempx - BrushSize) * (tempx - BrushSize) + (tempy - BrushSize) * (tempy - BrushSize));
                    //            float distanceNorm = (BrushSize - distance) / BrushSize;

                    //            if (distance < BrushSize)
                    //            {
                    //                tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                if (tempColor.r > 1f) tempColor.r = 1f;
                    //                if (tempColor.g > 1f) tempColor.g = 1f;
                    //                if (tempColor.b > 1f) tempColor.b = 1f;
                    //                if (tempColor.a > 1f) tempColor.a = 1f;

                    //                if (selectedPixels[(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - BrushSize)])
                    //                {
                    //                    layerList[selectedLayer][(int)(tempy + currentVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(currentVectorPoint.x - tempx - BrushSize)] = tempColor;

                    //                    renderPixelToTex((int)(tempy + currentVectorPoint.y + weirdOffsetYt * pixelTexArea.height), (int)(currentVectorPoint.x - tempx - BrushSize));
                    //                }
                    //            }
                    //        }

                    //        if (tempy + lastVectorPoint.y > 0 && tempx + lastVectorPoint.x > 0 && tempy + lastVectorPoint.y < layerList[selectedLayer].Count && tempx + lastVectorPoint.x < layerList[selectedLayer][tempy + (int)lastVectorPoint.y].Count)
                    //        {
                    //            Color tempColor = layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)];
                    //            float distance = (float)Math.Sqrt((tempx - BrushSize) * (tempx - BrushSize) + (tempy - BrushSize) * (tempy - BrushSize));
                    //            float distanceNorm = (BrushSize - distance) / BrushSize;

                    //            if (distance < BrushSize)
                    //            {
                    //                tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                if (tempColor.r > 1f) tempColor.r = 1f;
                    //                if (tempColor.g > 1f) tempColor.g = 1f;
                    //                if (tempColor.b > 1f) tempColor.b = 1f;
                    //                if (tempColor.a > 1f) tempColor.a = 1f;

                    //                if (selectedPixels[(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - BrushSize)])
                    //                {
                    //                    layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)] = tempColor;

                    //                    renderPixelToTex((int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height), (int)(lastVectorPoint.x - tempx - BrushSize));
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    //while (true)
                    //{
                    //    int s = 0; // step through adjunctions

                    //    while (true)
                    //    {
                    //        if (Vector2.Distance(vectorStraight, new Vector2(vectorStraight.x + s, vectorStraight.y)) <= BrushSize)
                    //        {
                    //            if (!brushingCoords.Contains((vectorStraight.x + s, vectorStraight.y)))
                    //            {
                    //                brushingCoords.Add((vectorStraight.x + s, vectorStraight.y));
                    //                s += 1;
                    //            }
                    //        }
                    //        else
                    //        { s = 0; break; }
                    //    }
                    //    while (true)
                    //    {
                    //        if (Vector2.Distance(vectorStraight, new Vector2(vectorStraight.x, vectorStraight.y + s)) <= BrushSize)
                    //        {
                    //            if (!brushingCoords.Contains((vectorStraight.x, vectorStraight.y + s)))
                    //            {
                    //                brushingCoords.Add((vectorStraight.x, vectorStraight.y + s));
                    //                s += 1;
                    //            }
                    //        }
                    //        else
                    //        { s = 0; break; }
                    //    }
                    //    while (true)
                    //    {
                    //        if (Vector2.Distance(vectorStraight, new Vector2(vectorStraight.x - s, vectorStraight.y)) <= BrushSize)
                    //        {
                    //            if (!brushingCoords.Contains((vectorStraight.x - s, vectorStraight.y)))
                    //            {
                    //                brushingCoords.Add((vectorStraight.x - s, vectorStraight.y));
                    //                s += 1;
                    //            }
                    //        }
                    //        else
                    //        { s = 0; break; }
                    //    }
                    //    while (true)
                    //    {
                    //        if (Vector2.Distance(vectorStraight, new Vector2(vectorStraight.x, vectorStraight.y - s)) <= BrushSize)
                    //        {
                    //            if (!brushingCoords.Contains((vectorStraight.x, vectorStraight.y - s)))
                    //            {
                    //                brushingCoords.Add((vectorStraight.x, vectorStraight.y - s));
                    //                s += 1;
                    //            }
                    //        }
                    //        else
                    //        { s = 0; break; }
                    //    }

                    //    if (t <= 1)
                    //    {
                    //        t += 1 / (currentVectorPoint - lastVectorPoint).Magnitude;
                    //    }
                    //    else
                    //    { t = 0; break; }
                    //}

                    //foreach (int i = 0; i < brushingCoords.Count; i += 1)
                    //{
                    //    (int, int) currentPoint = brushingCoords[i];

                    //    Color tempColor = layerList[selectedLayer][currentPoint.Item2][currentPoint.Item1];
                    //    float distance = (float)Math.Sqrt((tempx - BrushSize) * (tempx - BrushSize) + (tempy - BrushSize) * (tempy - BrushSize));
                    //    float distanceNorm = (BrushSize - distance) / BrushSize;

                    //    tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //    if (tempColor.r > 1f) tempColor.r = 1f;
                    //    if (tempColor.g > 1f) tempColor.g = 1f;
                    //    if (tempColor.b > 1f) tempColor.b = 1f;
                    //    if (tempColor.a > 1f) tempColor.a = 1f;

                    //    if (selectedPixels[(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - BrushSize)])
                    //    {
                    //        layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)] = tempColor;

                    //        renderPixelToTex((int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height), (int)(lastVectorPoint.x - tempx - BrushSize));
                    //    }
                    //}

                    //if (currentVectorPoint == lastVectorPoint) //enable if you need for memory
                    //{
                    //    ////y for
                    //    //for (int tempy = 0; tempy < 2 * BrushSize + 1; tempy++)
                    //    //{
                    //    //    //x for
                    //    //    for (int tempx = 0; tempx < 2 * BrushSize + 1; tempx++)
                    //    //    {
                    //    //        if ((int)(tempy + currentVectorPoint.y) > 0 && (int)(tempx + currentVectorPoint.x) > 0 && (int)(tempy + currentVectorPoint.y) < layerList[selectedLayer].Count && (int)(tempx + currentVectorPoint.x) < layerList[selectedLayer][(int)(tempy + currentVectorPoint.y)].Count)
                    //    //        {
                    //    //            Color tempColor = layerList[selectedLayer][(int)(tempy + currentVectorPoint.y)][(int)(tempx + currentVectorPoint.x)];
                    //    //            float distance = (float)Math.Sqrt((tempx - BrushSize) * (tempx - BrushSize) + (tempy - BrushSize) * (tempy - BrushSize));
                    //    //            float distanceNorm = (BrushSize - distance) / BrushSize;

                    //    //            if (distance < BrushSize)
                    //    //            {
                    //    //                tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //    //                tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //    //                tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //    //                tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //    //                if (tempColor.r > 1f) tempColor.r = 1f;
                    //    //                if (tempColor.g > 1f) tempColor.g = 1f;
                    //    //                if (tempColor.b > 1f) tempColor.b = 1f;
                    //    //                if (tempColor.a > 1f) tempColor.a = 1f;
                    //    //                if (tempColor.r < 0f) tempColor.r = 0f;
                    //    //                if (tempColor.g < 0f) tempColor.g = 0f;
                    //    //                if (tempColor.b < 0f) tempColor.b = 0f;
                    //    //                if (tempColor.a < 0f) tempColor.a = 0f;

                    //    //                if (selectedPixels[(int)(tempy + currentVectorPoint.y)][(int)(tempx + currentVectorPoint.x)])
                    //    //                {
                    //    //                    layerList[selectedLayer][(int)(tempy + currentVectorPoint.y)][(int)(tempx + currentVectorPoint.x)] = tempColor;

                    //    //                    renderPixelToTex((int)(tempy + currentVectorPoint.y), (int)(tempx + currentVectorPoint.x));
                    //    //                }
                    //    //            }
                    //    //        }
                    //    //    }
                    //    //}
                    //}
                    //else
                    //{
                    //    Vector3 direction = new Vector3(currentVectorPoint.x - lastVectorPoint.x, currentVectorPoint.y - lastVectorPoint.y, 0f);
                    //    Vector3 startingPoint = new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f);
                    //    //Vector3 lastDirection = new Vector3(lastVectorPoint.x - lastLastVectorPoint.x, lastVectorPoint.y - lastLastVectorPoint.y, 0f);
                    //    //Vector3 LastStartingPoint = new Vector3(lastLastVectorPoint.x, lastLastVectorPoint.y, 0f);

                    //    Ray vectorStraight = new Ray(startingPoint, direction.normalized);
                    //    //Ray lastVectorStraight = new Ray(LastStartingPoint, lastDirection.normalized);

                    //    //Math.Max(Math.Abs(currentVectorPoint.x - lastVectorPoint.x), Math.Abs(lastVectorPoint.x - lastLastVectorPoint.x))
                    //    //Math.Abs(currentVectorPoint.x - lastVectorPoint.x)

                    //    for (int tempx = 0; tempx < 2 * BrushSize + 1 + Math.Max(Math.Abs(currentVectorPoint.x - lastVectorPoint.x), Math.Abs(currentVectorPoint.x - lastLastVectorPoint.x)); tempx += 1) //lastVectorPoint
                    //    //for (int tempx = 0; tempx < 2 * BrushSize + 1 + Math.Abs(currentVectorPoint.x - lastVectorPoint.x); tempx += 1) //lastVectorPoint
                    //    {
                    //        for (int tempy = 0; tempy < 2 * BrushSize + 1 + Math.Max(Math.Abs(currentVectorPoint.y - lastVectorPoint.y), Math.Abs(currentVectorPoint.y - lastLastVectorPoint.y)); tempy += 1) //lastVectorPoint
                    //        //for (int tempy = 0; tempy < 2 * BrushSize + 1 + Math.Abs(currentVectorPoint.y - lastVectorPoint.y); tempy += 1) //lastVectorPoint
                    //        {
                    //            Vector3 currentPoint = new Vector3(0f, 0f);

                    //            if (currentVectorPoint.x >= lastVectorPoint.x) // move right
                    //            {
                    //                if (currentVectorPoint.y >= lastVectorPoint.y)  // move up
                    //                {
                    //                    currentPoint = new Vector3(lastVectorPoint.x + tempx - BrushSize, lastVectorPoint.y + tempy - BrushSize, 0f);
                    //                }
                    //                if (currentVectorPoint.y < lastVectorPoint.y)  // move down
                    //                {
                    //                    currentPoint = new Vector3(lastVectorPoint.x + tempx - BrushSize, lastVectorPoint.y - tempy + BrushSize, 0f);
                    //                }
                    //            }
                    //            else if (currentVectorPoint.x < lastVectorPoint.x) // move left
                    //            {
                    //                if (currentVectorPoint.y >= lastVectorPoint.y)  // move up
                    //                {
                    //                    currentPoint = new Vector3(lastVectorPoint.x - tempx + BrushSize, lastVectorPoint.y + tempy - BrushSize, 0f);
                    //                }
                    //                if (currentVectorPoint.y < lastVectorPoint.y)  // move down
                    //                {
                    //                    currentPoint = new Vector3(lastVectorPoint.x - tempx + BrushSize, lastVectorPoint.y - tempy + BrushSize, 0f);
                    //                }
                    //            }

                    //            //Color tempColor = layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x];

                    //            //mathematical my version
                    //            //float b = (-(currentVectorPoint.y / perpendicularLine().y) + (currentVectorPoint.x * (currentVectorPoint.y - lastVectorPoint.y)) / ((currentVectorPoint.x - lastVectorPoint.x) * perpendicularLine().y) - (lastVectorPoint.x * (currentVectorPoint.y - lastVectorPoint.y)) / ((currentVectorPoint.x - lastVectorPoint.x) * perpendicularLine().y) + lastVectorPoint.y / perpendicularLine().y)/ (1 - ((currentVectorPoint.y - lastVectorPoint.y) * perpendicularLine().x) / ((currentVectorPoint.x - lastVectorPoint.x) * perpendicularLine().y));
                    //            //float a = (currentVectorPoint.x / (currentVectorPoint.x - lastVectorPoint.x) - lastVectorPoint.x / (currentVectorPoint.x - lastVectorPoint.x) - (currentVectorPoint.y * perpendicularLine().x) / ((currentVectorPoint.x - lastVectorPoint.x) * perpendicularLine().y) + (lastVectorPoint.y * perpendicularLine().x) / ((currentVectorPoint.x - lastVectorPoint.x) * perpendicularLine().y)) / (1 - ((currentVectorPoint.y - lastVectorPoint.y) * perpendicularLine().x) / ((currentVectorPoint.x - lastVectorPoint.x) * perpendicularLine().y));

                    //            //if (a <= 1f && distanceToSegment(a, b, currentPoint) < BrushSize && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x)))
                    //            //{
                    //            //    Color tempColor = layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x];

                    //            //    float distanceNorm = (BrushSize - distanceToSegment(a, b, currentPoint)) / BrushSize;

                    //            //    tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //            //    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //            //    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //            //    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //            //    if (tempColor.r > 1f) tempColor.r = 1f;
                    //            //    if (tempColor.g > 1f) tempColor.g = 1f;
                    //            //    if (tempColor.b > 1f) tempColor.b = 1f;
                    //            //    if (tempColor.a > 1f) tempColor.a = 1f;

                    //            //    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //            //    {
                    //            //        layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x] = tempColor;

                    //            //        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //            //        currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //            //    }
                    //            //}

                    //            ////new mathematical version
                    //            ///*
                    //            // * for example: (x1, y1) = currentVectorPoint
                    //            // * (x2, y2) = lastVectorPoint
                    //            // * 
                    //            // * mx + b = y
                    //            // * m = (y2 - y1)/(x2 - x1)
                    //            // * 
                    //            // * b = mx1 - y1
                    //            // * do this for both lines
                    //            // * 
                    //            // float delta = m2 - m1;

                    //            //if (delta == 0) 
                    //            //    throw new ArgumentException("Lines are parallel");

                    //            //float x = (b2 - b1) / delta;
                    //            //float y = (m1 * b2 - m2 * b1) / delta;
                    //            // */

                    //            ////Vector2 perpendicularPoint = currentPoint + new Vector2(currentVectorPoint.x - lastVectorPoint.x, lastVectorPoint.y - currentVectorPoint.y);
                    //            //Vector2 perpendicularPoint = currentPoint + new Vector2(lastVectorPoint.y - currentVectorPoint.y, currentVectorPoint.x - lastVectorPoint.x);

                    //            //float mA = (currentVectorPoint.y - lastVectorPoint.y)/(currentVectorPoint.x - lastVectorPoint.x);
                    //            //float mB = (currentPoint.y - perpendicularPoint.y)/(currentPoint.x - perpendicularPoint.x);

                    //            //float bA = -mA * currentVectorPoint.x + currentVectorPoint.y;
                    //            //float bB = -mB * currentPoint.x + currentPoint.y;

                    //            //float delta = mB - mA;

                    //            ////if (delta == 0)
                    //            ////    throw new ArgumentException("Lines are parallel");
                    //            ////else

                    //            ////UnityEngine.Debug.Log("Angle cVp-lVp, cP-pP is " + Vector2.Angle(currentVectorPoint - lastVectorPoint, currentPoint - perpendicularPoint));

                    //            //if (delta != 0)
                    //            //{
                    //            //    float xInter = -(bB - bA) / delta;
                    //            //    float yInter = -(mA * bB - mB * bA) / delta;
                    //            //    //everything works fine until here. Test distance calcs and rectangle calcs for mistake

                    //            //    float distanceNorm = (BrushSize - Vector2.Distance(currentPoint, new Vector2(xInter, yInter))) / BrushSize;
                    //            //    float distanceLastNorm = (BrushSize - Vector2.Distance(currentPoint, lastVectorPoint)) / BrushSize;

                    //            //    //if at line segment
                    //            //    if (Vector2.Distance(currentPoint, new Vector2(xInter, yInter)) <= BrushSize &&
                    //            //        //Vector2.Distance(new Vector2(xInter, yInter), currentVectorPoint) < Vector2.Distance(lastVectorPoint, currentVectorPoint)
                    //            //        //&& Vector2.Distance(new Vector2(xInter, yInter), lastVectorPoint) < Vector2.Distance(lastVectorPoint, currentVectorPoint)
                    //            //        Vector2.Angle((currentPoint - lastVectorPoint), (currentVectorPoint - lastVectorPoint)) <= 90f
                    //            //        && Vector2.Angle((currentPoint - currentVectorPoint), (lastVectorPoint - currentVectorPoint)) <= 90f
                    //            //        //&& !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))
                    //            //        )
                    //            //    {
                    //            //        Color tempColor = layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x];

                    //            //        tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //            //        tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //            //        tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //            //        tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //            //        if (tempColor.r > 1f) tempColor.r = 1f;
                    //            //        if (tempColor.g > 1f) tempColor.g = 1f;
                    //            //        if (tempColor.b > 1f) tempColor.b = 1f;
                    //            //        if (tempColor.a > 1f) tempColor.a = 1f;

                    //            //        if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //            //        {
                    //            //            layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x] = tempColor;

                    //            //            renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //            //            currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //            //        }
                    //            //    }
                    //            //    //else if at lastVectorPoint
                    //            //    else if (Vector2.Distance(currentPoint, lastVectorPoint) <= BrushSize //distanceLastNorm <= 1f
                    //            //        && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))
                    //            //        && !(Vector2.Angle((currentPoint - lastVectorPoint), (lastLastVectorPoint - lastVectorPoint)) <= 90f
                    //            //        && Vector2.Angle((currentPoint - lastLastVectorPoint), (lastVectorPoint - lastLastVectorPoint)) <= 90f)
                    //            //        )
                    //            //    {
                    //            //        Color tempColor = layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x];

                    //            //        tempColor.a = selectedColor.a * distanceLastNorm + tempColor.a * (1f - selectedColor.a * distanceLastNorm);
                    //            //        tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //            //        tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //            //        tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //            //        if (tempColor.r > 1f) tempColor.r = 1f;
                    //            //        if (tempColor.g > 1f) tempColor.g = 1f;
                    //            //        if (tempColor.b > 1f) tempColor.b = 1f;
                    //            //        if (tempColor.a > 1f) tempColor.a = 1f;

                    //            //        if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //            //        {
                    //            //            //layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x] = tempColor;

                    //            //            //renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //            //            currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //            //        }
                    //            //    }
                    //            //}
                    //            //else
                    //            //{
                    //            //    UnityEngine.Debug.Log("delta is 0");
                    //            //}

                    //            //some other method
                    //            float distanceToLine = Vector3.Cross(vectorStraight.direction, currentPoint - vectorStraight.origin).magnitude;
                    //            //float distanceToCurrent = Vector3.Distance(currentPoint, new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f));
                    //            float distanceToLast = Vector3.Distance(currentPoint, new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f));


                    //            if (distanceToLine <= BrushSize && checkOOB((int)currentPoint.y, (int)currentPoint.x))
                    //            {
                    //                Color tempColor = layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x];

                    //                if (Vector3.Angle((currentPoint - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f)), (new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f) - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f))) <= 90f
                    //                    && Vector3.Angle((currentPoint - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f)), (new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f) - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f))) <= 90f
                    //                    && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))) //line
                    //                {
                    //                    float distanceNorm = (BrushSize - distanceToLine) / BrushSize;

                    //                    tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                    if (tempColor.r > 1f) tempColor.r = 1f;
                    //                    if (tempColor.g > 1f) tempColor.g = 1f;
                    //                    if (tempColor.b > 1f) tempColor.b = 1f;
                    //                    if (tempColor.a > 1f) tempColor.a = 1f;

                    //                    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                    {
                    //                        layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x] = tempColor;

                    //                        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                        currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //                    }
                    //                }
                    //                ////else if (Vector3.Angle((currentPoint - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f)), (new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f) - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f))) <= 90f
                    //                ////    && Vector3.Angle((currentPoint - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f)), (new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f) - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f))) <= 90f
                    //                ////    && distanceToLast < BrushSize)//&& lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))) //line
                    //                ////{
                    //                ////    float distanceNorm = (BrushSize - distanceToLine) / BrushSize;

                    //                ////    tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                ////    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                ////    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                ////    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                ////    if (tempColor.r > 1f) tempColor.r = 1f;
                    //                ////    if (tempColor.g > 1f) tempColor.g = 1f;
                    //                ////    if (tempColor.b > 1f) tempColor.b = 1f;
                    //                ////    if (tempColor.a > 1f) tempColor.a = 1f;

                    //                ////    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                ////    {
                    //                ////        layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x] = tempColor;

                    //                ////        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                ////        currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //                ////    }
                    //                ////}

                    //                else if (
                    //                    //distanceToLine < BrushSize
                    //                    //(Vector3.Angle((currentPoint - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f)), (new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f) - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f))) > 90f
                    //                    //|| Vector3.Angle((currentPoint - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f)), (new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f) - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f))) > 90f)
                    //                    Vector3.Angle((currentPoint - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f)), (new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f) - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f))) > 90f
                    //                    && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))
                    //                    && distanceToLast < BrushSize
                    //                    && !lastLastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))
                    //                    )
                    //                {
                    //                    float distanceNorm = (BrushSize - distanceToLast) / BrushSize;

                    //                    tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                    if (tempColor.r > 1f) tempColor.r = 1f;
                    //                    if (tempColor.g > 1f) tempColor.g = 1f;
                    //                    if (tempColor.b > 1f) tempColor.b = 1f;
                    //                    if (tempColor.a > 1f) tempColor.a = 1f;

                    //                    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                    {
                    //                        layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x] = tempColor;

                    //                        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                        currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //                    }
                    //                }

                    //                //else if (
                    //                //    distanceToLine < BrushSize
                    //                //    && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x)) && distanceToCurrent < BrushSize) //&& !lastLastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))
                    //                //{
                    //                //    float distanceNorm = (BrushSize - distanceToCurrent) / BrushSize;

                    //                //    tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                //    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                //    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                //    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                //    if (tempColor.r > 1f) tempColor.r = 1f;
                    //                //    if (tempColor.g > 1f) tempColor.g = 1f;
                    //                //    if (tempColor.b > 1f) tempColor.b = 1f;
                    //                //    if (tempColor.a > 1f) tempColor.a = 1f;

                    //                //    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                //    {
                    //                //        layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x] = tempColor;

                    //                //        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                //        currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //                //    }
                    //                //}
                    //                /* (*uncomment for whole line. this is for testing if the angle tracking works right*)
                    //                else if (Vector3.Angle((currentPoint - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f)), (new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f) - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f))) <= 90f
                    //                    && Vector3.Angle((currentPoint - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f)), (new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f) - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f))) <= 90f
                    //                    && lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x)) && !currentBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))) //line 2 && !currentBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))
                    //                {
                    //                    float distanceNorm = (BrushSize - distanceToLine) / BrushSize;

                    //                    tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                    if (tempColor.r > 1f) tempColor.r = 1f;
                    //                    if (tempColor.g > 1f) tempColor.g = 1f;
                    //                    if (tempColor.b > 1f) tempColor.b = 1f;
                    //                    if (tempColor.a > 1f) tempColor.a = 1f;

                    //                    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                    {
                    //                        UnityEngine.Debug.Log(layerList[selectedLayer].Count);
                    //                        layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)] = tempColor;
                    //                        layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x] = tempColor;

                    //                        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                        currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //                    }
                    //                }
                    //                else if (distanceToLast < distanceToCurrent && distanceToLast < BrushSize && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))) //last
                    //                {
                    //                    currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //                    float distanceNorm = (BrushSize - distanceToLast) / BrushSize;

                    //                    tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                    if (tempColor.r > 1f) tempColor.r = 1f;
                    //                    if (tempColor.g > 1f) tempColor.g = 1f;
                    //                    if (tempColor.b > 1f) tempColor.b = 1f;
                    //                    if (tempColor.a > 1f) tempColor.a = 1f;

                    //                    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                    {
                    //                        layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)] = tempColor;

                    //                        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                    }
                    //                }
                    //                else if (Vector3.Angle((currentPoint - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f)), (new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f) - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f))) <= 90f
                    //                    && Vector3.Angle((currentPoint - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f)), (new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f) - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f))) <= 90f
                    //                    && lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x)))
                    //                {
                    //                    float distanceNorm = (BrushSize - distanceToLast) / BrushSize;

                    //                    tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                    if (tempColor.r > 1f) tempColor.r = 1f;
                    //                    if (tempColor.g > 1f) tempColor.g = 1f;
                    //                    if (tempColor.b > 1f) tempColor.b = 1f;
                    //                    if (tempColor.a > 1f) tempColor.a = 1f;

                    //                    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                    {
                    //                        layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)] = tempColor;

                    //                        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                        currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //                    }
                    //                }
                    //                else if (distanceToLast > distanceToCurrent && distanceToCurrent < BrushSize && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x)) && !currentBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))) //current
                    //                {
                    //                    float distanceNorm = (BrushSize - distanceToCurrent) / BrushSize;

                    //                    tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                    if (tempColor.r > 1f) tempColor.r = 1f;
                    //                    if (tempColor.g > 1f) tempColor.g = 1f;
                    //                    if (tempColor.b > 1f) tempColor.b = 1f;
                    //                    if (tempColor.a > 1f) tempColor.a = 1f;

                    //                    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                    {
                    //                        layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)] = tempColor;

                    //                        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                        currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //                    }
                    //                }
                    //                else if (distanceToLast > distanceToCurrent && distanceToCurrent < BrushSize) //current
                    //                {
                    //                    float distanceNorm = (BrushSize - distanceToCurrent) / BrushSize;

                    //                    tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                    if (tempColor.r > 1f) tempColor.r = 1f;
                    //                    if (tempColor.g > 1f) tempColor.g = 1f;
                    //                    if (tempColor.b > 1f) tempColor.b = 1f;
                    //                    if (tempColor.a > 1f) tempColor.a = 1f;

                    //                    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                    {
                    //                        layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)] = tempColor;

                    //                        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                        currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //                    }
                    //                }
                    //                */

                    //                ////new Rules
                    //                //if (Vector3.Angle((currentPoint - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f)), (new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f) - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f))) <= 90f
                    //                //    && Vector3.Angle((currentPoint - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f)), (new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f) - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f))) <= 90f
                    //                //    && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x)) && !currentBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x)))
                    //                //{ //non-overlapping in line, probably won't overwrite overlapping one
                    //                //    /*Color calc for line*/
                    //                //    currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //                //    float distanceNorm = (BrushSize - distanceToLine) / BrushSize;

                    //                //    tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                //    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                //    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                //    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                //    if (tempColor.r > 1f) tempColor.r = 1f;
                    //                //    if (tempColor.g > 1f) tempColor.g = 1f;
                    //                //    if (tempColor.b > 1f) tempColor.b = 1f;
                    //                //    if (tempColor.a > 1f) tempColor.a = 1f;

                    //                //    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                //    {
                    //                //        layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)] = tempColor;

                    //                //        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                //    }
                    //                //}
                    //                //else if (distanceToLast > distanceToCurrent && distanceToCurrent < BrushSize && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x)) && !currentBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x)))
                    //                //{ //ankle of current
                    //                //    /*Color calc for current*/
                    //                //    currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //                //    float distanceNorm = (BrushSize - distanceToCurrent) / BrushSize;

                    //                //    tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                //    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                //    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                //    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                //    if (tempColor.r > 1f) tempColor.r = 1f;
                    //                //    if (tempColor.g > 1f) tempColor.g = 1f;
                    //                //    if (tempColor.b > 1f) tempColor.b = 1f;
                    //                //    if (tempColor.a > 1f) tempColor.a = 1f;

                    //                //    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                //    {
                    //                //        layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)] = tempColor;

                    //                //        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                //    }
                    //                //}
                    //                //else if (distanceToLast < distanceToCurrent && distanceToLast < BrushSize && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x)) && !currentBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x)))
                    //                //{ //ankle of last
                    //                //    /*Color calc for last*/
                    //                //    currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //                //    float distanceNorm = (BrushSize - distanceToLast) / BrushSize;

                    //                //    tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                //    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                //    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                //    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                //    if (tempColor.r > 1f) tempColor.r = 1f;
                    //                //    if (tempColor.g > 1f) tempColor.g = 1f;
                    //                //    if (tempColor.b > 1f) tempColor.b = 1f;
                    //                //    if (tempColor.a > 1f) tempColor.a = 1f;

                    //                //    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                //    {
                    //                //        layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)] = tempColor;

                    //                //        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                //    }
                    //                //}

                    //                //new rules end

                    //                //if (pointDistanceA < BrushSize)
                    //                //{
                    //                //    tempColor.a = selectedColor.a * pointDistanceANorm + tempColor.a * (1f - selectedColor.a * pointDistanceANorm);
                    //                //    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                //    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                //    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                //    if (tempColor.r > 1f) tempColor.r = 1f;
                    //                //    if (tempColor.g > 1f) tempColor.g = 1f;
                    //                //    if (tempColor.b > 1f) tempColor.b = 1f;
                    //                //    if (tempColor.a > 1f) tempColor.a = 1f;

                    //                //    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                //    {
                    //                //        layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)] = tempColor;

                    //                //        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                //    }
                    //                //}
                    //                //else if (pointDistanceB < BrushSize)
                    //                //{
                    //                //    //no second brush >:(
                    //                //}
                    //                //else
                    //                //{
                    //                //    if (currentVectorPoint.x > lastVectorPoint.x) // move right
                    //                //    {
                    //                //        if (currentVectorPoint.y > lastVectorPoint.y)  // move up
                    //                //        {
                    //                //            if (!(currentPoint.x > currentVectorPoint.x && currentPoint.y > currentVectorPoint.y) && !(currentPoint.x < lastVectorPoint.x && currentPoint.y < lastVectorPoint.y))
                    //                //            {
                    //                //                tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                //                tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                //                tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                //                tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                //                if (tempColor.r > 1f) tempColor.r = 1f;
                    //                //                if (tempColor.g > 1f) tempColor.g = 1f;
                    //                //                if (tempColor.b > 1f) tempColor.b = 1f;
                    //                //                if (tempColor.a > 1f) tempColor.a = 1f;

                    //                //                if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                //                {
                    //                //                    layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)] = tempColor;

                    //                //                    renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                //                }
                    //                //            }
                    //                //        }
                    //                //        if (currentVectorPoint.y < lastVectorPoint.y)  // move down
                    //                //        {
                    //                //            if (!(currentPoint.x > currentVectorPoint.x && currentPoint.y < currentVectorPoint.y) && !(currentPoint.x < lastVectorPoint.x && currentPoint.y > lastVectorPoint.y))
                    //                //            {
                    //                //                tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                //                tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                //                tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                //                tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                //                if (tempColor.r > 1f) tempColor.r = 1f;
                    //                //                if (tempColor.g > 1f) tempColor.g = 1f;
                    //                //                if (tempColor.b > 1f) tempColor.b = 1f;
                    //                //                if (tempColor.a > 1f) tempColor.a = 1f;

                    //                //                if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                //                {
                    //                //                    layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)] = tempColor;

                    //                //                    renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                //                }
                    //                //            }
                    //                //        }
                    //                //    }
                    //                //    else if (currentVectorPoint.x < lastVectorPoint.x) // move left
                    //                //    {
                    //                //        if (currentVectorPoint.y > lastVectorPoint.y)  // move up
                    //                //        {
                    //                //            if (!(currentPoint.x < currentVectorPoint.x && currentPoint.y > currentVectorPoint.y) && !(currentPoint.x > lastVectorPoint.x && currentPoint.y < lastVectorPoint.y))
                    //                //            {
                    //                //                tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                //                tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                //                tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                //                tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                //                if (tempColor.r > 1f) tempColor.r = 1f;
                    //                //                if (tempColor.g > 1f) tempColor.g = 1f;
                    //                //                if (tempColor.b > 1f) tempColor.b = 1f;
                    //                //                if (tempColor.a > 1f) tempColor.a = 1f;

                    //                //                if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                //                {
                    //                //                    layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)] = tempColor;

                    //                //                    renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                //                }
                    //                //            }
                    //                //        }
                    //                //        if (currentVectorPoint.y < lastVectorPoint.y)  // move down
                    //                //        {
                    //                //            if (!(currentPoint.x < currentVectorPoint.x && currentPoint.y < currentVectorPoint.y) && !(currentPoint.x > lastVectorPoint.x && currentPoint.y > lastVectorPoint.y))
                    //                //            {
                    //                //                tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                    //                //                tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                    //                //                tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                    //                //                tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                    //                //                if (tempColor.r > 1f) tempColor.r = 1f;
                    //                //                if (tempColor.g > 1f) tempColor.g = 1f;
                    //                //                if (tempColor.b > 1f) tempColor.b = 1f;
                    //                //                if (tempColor.a > 1f) tempColor.a = 1f;

                    //                //                if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                //                {
                    //                //                    layerList[selectedLayer][(int)(tempy + lastVectorPoint.y + weirdOffsetYt * pixelTexArea.height)][(int)(lastVectorPoint.x - tempx - BrushSize)] = tempColor;

                    //                //                    renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                //                }
                    //                //            }
                    //                //        }
                    //                //    }
                    //                //}

                    //            }
                    //        }
                    //    }
                    //}

                }
                else if (toolSelect == 8 || Wacom.eraser.isPressed) //Eraser
                {
                    if (eraserMode)
                    {
                        if (currentVectorPoint == lastVectorPoint) //enable if you need for memory
                        {
                            //some stationary stuff
                        }
                        else
                        {
                            Vector3 direction = new Vector3(currentVectorPoint.x - lastVectorPoint.x, currentVectorPoint.y - lastVectorPoint.y, 0f);
                            Vector3 startingPoint = new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f);

                            Ray vectorStraight = new Ray(startingPoint, direction.normalized);

                            //y for
                            for (int tempy = 0; tempy < 2 * brushSizeList[toolSelect] + 1; tempy++)
                            {
                                //x for
                                for (int tempx = 0; tempx < 2 * brushSizeList[toolSelect] + 1; tempx++)
                                {
                                    //old Eraser
                                    if ((int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height) > 0 && (int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect]) > 0 && (int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height) < layerList[selectedLayer].Count && (int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect]) < layerList[selectedLayer][(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)].Count)
                                    {
                                        Color tempColor = layerList[selectedLayer][(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect])];
                                        float distance = (float)Math.Sqrt((tempx - brushSizeList[toolSelect]) * (tempx - brushSizeList[toolSelect]) + (tempy - brushSizeList[toolSelect]) * (tempy - brushSizeList[toolSelect]));
                                        float distanceNorm = (brushSizeList[toolSelect] - distance) / brushSizeList[toolSelect];

                                        if (distance < brushSizeList[toolSelect])
                                        {
                                            tempColor.a = 1f - (distanceNorm * tempColor.a);

                                            if (selectedPixels[(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect])])
                                            {
                                                layerList[selectedLayer][(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect])] = tempColor;

                                                renderPixelToTex((int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height), (int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect]));
                                            }
                                        }
                                    }
                                    ////new Eraser
                                    //Vector3 currentPoint = new Vector3(0f, 0f);

                                    //if (currentVectorPoint.x >= lastVectorPoint.x) // move right
                                    //{
                                    //    if (currentVectorPoint.y >= lastVectorPoint.y)  // move up
                                    //    {
                                    //        currentPoint = new Vector3(lastVectorPoint.x + tempx - BrushSize, lastVectorPoint.y + tempy - BrushSize, 0f);
                                    //    }
                                    //    if (currentVectorPoint.y < lastVectorPoint.y)  // move down
                                    //    {
                                    //        currentPoint = new Vector3(lastVectorPoint.x + tempx - BrushSize, lastVectorPoint.y - tempy + BrushSize, 0f);
                                    //    }
                                    //}
                                    //else if (currentVectorPoint.x < lastVectorPoint.x) // move left
                                    //{
                                    //    if (currentVectorPoint.y >= lastVectorPoint.y)  // move up
                                    //    {
                                    //        currentPoint = new Vector3(lastVectorPoint.x - tempx + BrushSize, lastVectorPoint.y + tempy - BrushSize, 0f);
                                    //    }
                                    //    if (currentVectorPoint.y < lastVectorPoint.y)  // move down
                                    //    {
                                    //        currentPoint = new Vector3(lastVectorPoint.x - tempx + BrushSize, lastVectorPoint.y - tempy + BrushSize, 0f);
                                    //    }
                                    //}

                                    //float distanceToLine = Vector3.Cross(vectorStraight.direction, currentPoint - vectorStraight.origin).magnitude;
                                    //float distanceToLast = Vector3.Distance(currentPoint, new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f));


                                    //if (distanceToLine <= BrushSize && checkOOB((int)currentPoint.y, (int)currentPoint.x))
                                    //{
                                    //    Color tempColor = layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x];

                                    //    if (Vector3.Angle((currentPoint - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f)), (new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f) - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f))) <= 90f
                                    //        && Vector3.Angle((currentPoint - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f)), (new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f) - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f))) <= 90f
                                    //        && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x)))
                                    //    {
                                    //        float distanceNorm = (BrushSize - distanceToLine) / BrushSize;

                                    //        tempColor.a = 1f - (distanceNorm * tempColor.a);

                                    //        if (tempColor.a < 0f) tempColor.a = 0f;

                                    //        if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                                    //        {
                                    //            layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x] = tempColor;

                                    //            renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                                    //            currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                                    //        }
                                    //    }

                                    //    else if (
                                    //        Vector3.Angle((currentPoint - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f)), (new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f) - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f))) > 90f
                                    //        && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))
                                    //        && distanceToLast < BrushSize
                                    //        && !lastLastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))
                                    //        )
                                    //    {
                                    //        float distanceNorm = (BrushSize - distanceToLast) / BrushSize;

                                    //        tempColor.a = 1f - (distanceNorm * tempColor.a);

                                    //        if (tempColor.a < 0f) tempColor.a = 0f;

                                    //        if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                                    //        {
                                    //            layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x] = tempColor;

                                    //            renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                                    //            currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                                    //        }
                                    //    }
                                    //}
                                }
                            }
                        }
                    }
                    else
                    {
                        if (currentVectorPoint == lastVectorPoint) //enable if you need for memory
                        {
                            //some stationary stuff
                        }
                        else
                        {
                            Vector3 direction = new Vector3(currentVectorPoint.x - lastVectorPoint.x, currentVectorPoint.y - lastVectorPoint.y, 0f);
                            Vector3 startingPoint = new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f);

                            Ray vectorStraight = new Ray(startingPoint, direction.normalized);

                            //y for
                            for (int tempy = 0; tempy < 2 * brushSizeList[toolSelect] + 1; tempy++)
                            {
                                //x for
                                for (int tempx = 0; tempx < 2 * brushSizeList[toolSelect] + 1; tempx++)
                                {
                                    //old eraser
                                    if ((int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height) > 0 && (int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect]) > 0 && (int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height) < layerList[selectedLayer].Count && (int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect]) < layerList[selectedLayer][(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)].Count)
                                    {
                                        Color tempColor = layerList[selectedLayer][(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect])];
                                        float distance = (float)Math.Sqrt((tempx - brushSizeList[toolSelect]) * (tempx - brushSizeList[toolSelect]) + (tempy - brushSizeList[toolSelect]) * (tempy - brushSizeList[toolSelect]));

                                        if (distance < brushSizeList[toolSelect])
                                        {
                                            tempColor.r = backgroundColor.r;
                                            tempColor.g = backgroundColor.g;
                                            tempColor.b = backgroundColor.b;
                                            tempColor.a = 0f;

                                            if (selectedPixels[(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect])])
                                            {
                                                layerList[selectedLayer][(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect])] = tempColor;

                                                renderPixelToTex((int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height), (int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect]));
                                            }
                                        }
                                    }
                                    ////new Eraser
                                    //Vector3 currentPoint = new Vector3(0f, 0f);

                                    //if (currentVectorPoint.x >= lastVectorPoint.x) // move right
                                    //{
                                    //    if (currentVectorPoint.y >= lastVectorPoint.y)  // move up
                                    //    {
                                    //        currentPoint = new Vector3(lastVectorPoint.x + tempx - BrushSize, lastVectorPoint.y + tempy - BrushSize, 0f);
                                    //    }
                                    //    if (currentVectorPoint.y < lastVectorPoint.y)  // move down
                                    //    {
                                    //        currentPoint = new Vector3(lastVectorPoint.x + tempx - BrushSize, lastVectorPoint.y - tempy + BrushSize, 0f);
                                    //    }
                                    //}
                                    //else if (currentVectorPoint.x < lastVectorPoint.x) // move left
                                    //{
                                    //    if (currentVectorPoint.y >= lastVectorPoint.y)  // move up
                                    //    {
                                    //        currentPoint = new Vector3(lastVectorPoint.x - tempx + BrushSize, lastVectorPoint.y + tempy - BrushSize, 0f);
                                    //    }
                                    //    if (currentVectorPoint.y < lastVectorPoint.y)  // move down
                                    //    {
                                    //        currentPoint = new Vector3(lastVectorPoint.x - tempx + BrushSize, lastVectorPoint.y - tempy + BrushSize, 0f);
                                    //    }
                                    //}

                                    //float distanceToLine = Vector3.Cross(vectorStraight.direction, currentPoint - vectorStraight.origin).magnitude;
                                    ////float distanceToCurrent = Vector3.Distance(currentPoint, new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f));
                                    //float distanceToLast = Vector3.Distance(currentPoint, new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f));


                                    //if (distanceToLine <= BrushSize && checkOOB((int)currentPoint.y, (int)currentPoint.x))
                                    //{
                                    //    Color tempColor = layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x];

                                    //    if (Vector3.Angle((currentPoint - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f)), (new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f) - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f))) <= 90f
                                    //        && Vector3.Angle((currentPoint - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f)), (new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f) - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f))) <= 90f
                                    //        && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x)))
                                    //    {
                                    //        tempColor.a = 0f;

                                    //        if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                                    //        {
                                    //            layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x] = tempColor;

                                    //            renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                                    //            currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                                    //        }
                                    //    }

                                    //    else if (
                                    //        Vector3.Angle((currentPoint - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f)), (new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f) - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f))) > 90f
                                    //        && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))
                                    //        && distanceToLast < BrushSize
                                    //        && !lastLastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))
                                    //        )
                                    //    {
                                    //        float distanceNorm = (BrushSize - distanceToLast) / BrushSize;

                                    //        tempColor.a = 0f;

                                    //        if (tempColor.a < 0f) tempColor.a = 0f;

                                    //        if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                                    //        {
                                    //            layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x] = tempColor;

                                    //            renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                                    //            currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                                    //        }
                                    //    }
                                    //}
                                }
                            }
                        }
                    }
                }
                else if (toolSelect == 7) //Pencil
                {
                    //old Pencil
                    //y for
                    for (int tempy = 0; tempy < 2 * brushSizeList[toolSelect] + 1; tempy++)
                    {
                        //x for
                        for (int tempx = 0; tempx < 2 * brushSizeList[toolSelect] + 1; tempx++)
                        {
                            //if ((int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height) > 0 && (int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect]) > 0 && (int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height) < layerList[selectedLayer].Count && (int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect]) < layerList[selectedLayer][(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)].Count)
                            //{
                            //    if (Math.Sqrt((tempx - brushSizeList[toolSelect]) * (tempx - brushSizeList[toolSelect]) + (tempy - brushSizeList[toolSelect]) * (tempy - brushSizeList[toolSelect])) < brushSizeList[toolSelect])
                            //    {
                            //        if ((int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height) >= 0 && (int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height) < pixelTexArea.height &&
                            //            (int)(tempx + (((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - brushSizeList[toolSelect]) >= 0 && (int)(tempx + (((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - brushSizeList[toolSelect]) < pixelTexArea.width)
                            //        {
                            //            if (selectedPixels[(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)(tempx + (((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - brushSizeList[toolSelect])])
                            //            {
                            //                layerList[selectedLayer][(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)(tempx + (((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - brushSizeList[toolSelect])] = selectedColor; // - BrushSize
                            //                                                                                                                                                                                                                                                                                                         //UnityEngine.Debug.Log("Setpixel at the spot " + (int)(tempx + (((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - BrushSize) + "   " + (int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height + weirdOffsetYt * pixelTexArea.height))); // - BrushSize
                            //                renderPixelToTex((int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height), (int)(tempx + (((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - brushSizeList[toolSelect]));
                            //            }
                            //        }
                            //    }
                            //}

                            float distance = (float)Math.Sqrt((tempx - brushSizeList[toolSelect]) * (tempx - brushSizeList[toolSelect]) + (tempy - brushSizeList[toolSelect]) * (tempy - brushSizeList[toolSelect]));

                            for (int i = 0; i < lerpCount; i++)
                            {
                                Vector2 lerpPos = Vector2.Lerp(currentVectorPoint, lastVectorPoint, (float)(i + 1) / lerpCount) + new Vector2(tempy - brushSizeList[toolSelect], tempx - brushSizeList[toolSelect]);
                                //Debug.Log(Vector2.Lerp(currentVectorPoint, lastVectorPoint, (float)(i + 1) / lerpCount) + ", (i + 1) / lerpCount = " + ((i + 1) / lerpCount));

                                if (distance < brushSizeList[toolSelect] && checkOOB((int)lerpPos.y, (int)lerpPos.x))
                                {
                                    //if (selectedPixels[(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - BrushSize)])
                                    if (selectedPixels[(int)lerpPos.y][(int)lerpPos.x])
                                    {
                                        layerList[selectedLayer][(int)lerpPos.y][(int)lerpPos.x] = selectedColor;

                                        renderPixelToTex((int)lerpPos.y, (int)lerpPos.x);
                                    }
                                }
                            }
                        }
                    }
                    ////new Pencil
                    //if (currentVectorPoint == lastVectorPoint) //enable if you need for memory
                    //{
                    //    //some stationary stuff
                    //}
                    //else
                    //{
                    //    Vector3 direction = new Vector3(currentVectorPoint.x - lastVectorPoint.x, currentVectorPoint.y - lastVectorPoint.y, 0f);
                    //    Vector3 startingPoint = new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f);

                    //    Ray vectorStraight = new Ray(startingPoint, direction.normalized);

                    //    //y for
                    //    for (int tempy = 0; tempy < 2 * BrushSize + 1; tempy++)
                    //    {
                    //        //x for
                    //        for (int tempx = 0; tempx < 2 * BrushSize + 1; tempx++)
                    //        {
                    //            Vector3 currentPoint = new Vector3(0f, 0f);

                    //            if (currentVectorPoint.x >= lastVectorPoint.x) // move right
                    //            {
                    //                if (currentVectorPoint.y >= lastVectorPoint.y)  // move up
                    //                {
                    //                    currentPoint = new Vector3(lastVectorPoint.x + tempx - BrushSize, lastVectorPoint.y + tempy - BrushSize, 0f);
                    //                }
                    //                if (currentVectorPoint.y < lastVectorPoint.y)  // move down
                    //                {
                    //                    currentPoint = new Vector3(lastVectorPoint.x + tempx - BrushSize, lastVectorPoint.y - tempy + BrushSize, 0f);
                    //                }
                    //            }
                    //            else if (currentVectorPoint.x < lastVectorPoint.x) // move left
                    //            {
                    //                if (currentVectorPoint.y >= lastVectorPoint.y)  // move up
                    //                {
                    //                    currentPoint = new Vector3(lastVectorPoint.x - tempx + BrushSize, lastVectorPoint.y + tempy - BrushSize, 0f);
                    //                }
                    //                if (currentVectorPoint.y < lastVectorPoint.y)  // move down
                    //                {
                    //                    currentPoint = new Vector3(lastVectorPoint.x - tempx + BrushSize, lastVectorPoint.y - tempy + BrushSize, 0f);
                    //                }
                    //            }

                    //            float distanceToLine = Vector3.Cross(vectorStraight.direction, currentPoint - vectorStraight.origin).magnitude;
                    //            float distanceToLast = Vector3.Distance(currentPoint, new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f));

                    //            if (distanceToLine <= BrushSize && checkOOB((int)currentPoint.y, (int)currentPoint.x))
                    //            {
                    //                if (Vector3.Angle((currentPoint - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f)), (new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f) - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f))) <= 90f
                    //                    && Vector3.Angle((currentPoint - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f)), (new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f) - new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f))) <= 90f
                    //                    && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x)))
                    //                {
                    //                    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                    {
                    //                        layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x] = selectedColor;

                    //                        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                        currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //                    }
                    //                }

                    //                else if (
                    //                    Vector3.Angle((currentPoint - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f)), (new Vector3(currentVectorPoint.x, currentVectorPoint.y, 0f) - new Vector3(lastVectorPoint.x, lastVectorPoint.y, 0f))) > 90f
                    //                    && !lastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))
                    //                    && distanceToLast < BrushSize
                    //                    && !lastLastBrushMemory.Contains(((int)currentPoint.y, (int)currentPoint.x))
                    //                    )
                    //                {
                    //                    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                    //                    {
                    //                        layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x] = selectedColor;

                    //                        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                    //                        currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }

                else if (toolSelect == 4) //Bucket
                {
                    if (layerList[selectedLayer][(int)((((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - brushSizeList[toolSelect])] != selectedColor)
                    {
                        floodFill((int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width)), // - BrushSize
                        (int)((((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height),
                        selectedColor,
                        layerList[selectedLayer][(int)((((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - brushSizeList[toolSelect])]);
                        //renderAllPixelsToTex();
                    }
                }

                else if (toolSelect == 6) //SelectBrush
                {
                    for (int i = 0; i < selectedPixels.Count; i += 1)
                    {
                        if (selectedPixels[i].All(o => o == selectedPixels[i].First()))
                        {
                            for (int j = 0; j < selectedPixels[i].Count; j += 1)
                            {
                                selectedPixels[i][j] = false;
                            }
                        }
                    }

                    //y for
                    for (int tempy = 0; tempy < 2 * brushSizeList[toolSelect] + 1; tempy++)
                    {
                        //x for
                        for (int tempx = 0; tempx < 2 * brushSizeList[toolSelect] + 1; tempx++)
                        {
                            if ((int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height) > 0 && (int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect]) > 0 && (int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height) < selectedPixels.Count && (int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect]) < selectedPixels[(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)].Count)
                            {
                                float distance = (float)Math.Sqrt((tempx - brushSizeList[toolSelect]) * (tempx - brushSizeList[toolSelect]) + (tempy - brushSizeList[toolSelect]) * (tempy - brushSizeList[toolSelect]));

                                if (distance < brushSizeList[toolSelect])
                                {
                                    selectedPixels[(int)(tempy + (((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)][(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width) - tempx - brushSizeList[toolSelect])] = true;
                                }
                            }
                        }
                    }

                    updateSelectionBorder();
                }

                else if (toolSelect == 1) //SelectRectangle
                {
                    if (!selectRectangleActive)
                    {
                        firstCoordinate = ((int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width)), // - BrushSize
                        (int)((((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height));

                        //y for
                        for (int tempy = 0; tempy < selectedPixels.Count; tempy++)
                        {
                            //x for
                            for (int tempx = 0; tempx < selectedPixels[tempy].Count; tempx++)
                            {
                                selectedPixels[tempy][tempx] = false;
                            }
                        }
                    }

                    selectRectangleActive = true;
                }

                else if (toolSelect == 3) //Pipette
                {
                    selectedColor = pixelTexArea.GetPixel((int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width)), // - BrushSize
                        (int)((((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height));

                    ColorViewMaterial.color = selectedColor;
                }

                else if (toolSelect == 11) //Line Tool
                {
                    if (!lineToolActive)
                    {
                        lineToolActive = true;

                        firstCoordinate = ((int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width)),
                        (int)((((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height));
                    }
                }

                else if (toolSelect == 5) //Lasso
                {
                    if (!lassoActive)
                    {
                        //y for
                        for (int tempy = 0; tempy < selectedPixels.Count; tempy++)
                        {
                            //x for
                            for (int tempx = 0; tempx < selectedPixels[tempy].Count; tempx++)
                            {
                                selectedPixels[tempy][tempx] = false;
                            }
                        }

                        firstCoordinate = ((int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width)),
                        (int)((((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height));

                        mostLeftPoint = new Vector2(firstCoordinate.Item1, firstCoordinate.Item2);
                    }

                    lassoActive = true;

                    selectedPixels[(int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width))][(int)((((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height)] = true;
                }

                else if (toolSelect == 9) //prior text, now createFactualPlane
                {
                    //if (!textActive)
                    //{
                    //    textActive = true;

                    //    KeyboardScript.setKeyboardsActive(true);

                    //    ////new textHitBox or check if it hit the hitbox
                    //    //KeyboardScript.SelectNewDrawText();
                    //    //selectedTextLayer = 0; //make this select text

                    //    //pixelTextArea = new Texture2D(pixelTexArea.width, pixelTexArea.height);
                    //    //pixelTexGraph = pixelTextArea.graphicsFormat;
                    //    //textLayers.Add((selectedLayer, pixelTexGraph, "")); //is this updated dynamically?

                    //    //paintText(pixelTexGraph, pixelTextArea);

                    //    //if (hitBox)
                    //    //then select text -> selectedTextLayer = ...

                    //    int failed = 0;

                    //    for (int j = 0; j < textLayers.Count; j += 1)
                    //    {
                    //        if (currentVectorPoint.x > textLayers[j].Item4.x &&
                    //        currentVectorPoint.y > textLayers[j].Item4.y &&
                    //        currentVectorPoint.x < textLayers[j].Item4.x + textLayers[j].Item4.width &&
                    //        currentVectorPoint.x < textLayers[j].Item4.y + textLayers[j].Item4.height &&
                    //        selectedLayer == textLayers[j].Item1
                    //        )
                    //        {
                    //            //hit the latest text box on layer
                    //            selectedTextLayer = j;
                    //        }
                    //        else failed += 1;
                    //    }

                    //    if (failed == textLayers.Count)
                    //    {
                    //        textLayers.Add((selectedLayer, "", Texture2D.grayTexture, new Rect(currentVectorPoint.x, currentVectorPoint.y, pixelTexArea.width / 3, pixelTexArea.height / 3)));
                    //        //needs proper scaling instead of width and height of pixelTexArea
                    //        Debug.Log("Added a textbox with properties: " + textLayers[textLayers.Count - 1]);
                    //    }
                    //    //also needs position on screen and world

                    //    //if keychange then:
                    //    if (textLayers.Count != 0 && KeyboardScript.getTextInput() != textLayers[selectedTextLayer].Item2)
                    //    {
                    //        //gets changed in method setTextInput
                    //        //textLayers[selectedTextLayer] = (textLayers[selectedTextLayer].Item1, KeyboardScript.getTextInput(), textLayers[selectedTextLayer].Item3, textLayers[selectedTextLayer].Item4);

                    //        ScreenShotScript.changeText(textLayers[selectedTextLayer].Item2);

                    //        Texture2D tex = ScreenShotScript.CaptureScreen();

                    //        textLayers[selectedTextLayer] = (textLayers[selectedTextLayer].Item1, textLayers[selectedTextLayer].Item2, tex, textLayers[selectedTextLayer].Item4);

                    //        //y for
                    //        for (int tempy = 0; tempy < layerList[selectedLayer].Count; tempy++)
                    //        {
                    //            //x for
                    //            for (int tempx = 0; tempx < layerList[selectedLayer][tempy].Count; tempx++)
                    //            {
                    //                renderPixelToTex(tempy, tempx); //renders every pixel to tex because idk the dimensions
                    //            }
                    //        }
                    //    }
                    //}
                }

                else if (toolSelect == 0) //Move Tool
                {
                    if (firstMove)
                    {
                        firstMove = false;

                        //find Kontur, set selectedPixels, then find Kontur again after render

                        //y for
                        for (int tempy = 0; tempy < selectedPixels.Count; tempy++)
                        {
                            //x for
                            for (int tempx = 0; tempx < selectedPixels[tempy].Count; tempx++)
                            {
                                if (selectedPixels[tempy][tempx] && layerList[selectedLayer][tempy][tempx].a == 0f) selectedPixels[tempy][tempx] = false;
                            }
                        }
                    }

                    int dx = (int)currentVectorPoint.x - (int)lastVectorPoint.x;
                    int dy = (int)currentVectorPoint.y - (int)lastVectorPoint.y;

                    if (dx != 0 || dy != 0)
                    {
                        List<List<Color>> copyLayer = new List<List<Color>>();

                        //y for
                        for (int tempy = 0; tempy < selectedPixels.Count; tempy++)
                        {
                            List<Color> copyTempLayer = new List<Color>();

                            //x for
                            for (int tempx = 0; tempx < selectedPixels[tempy].Count; tempx++)
                            {
                                if (selectedPixels[tempy][tempx])
                                {
                                    copyTempLayer.Add(layerList[selectedLayer][tempy][tempx]);

                                    layerList[selectedLayer][tempy][tempx] = new Color(1f, 1f, 1f, 0f);
                                    selectedPixels[tempy][tempx] = false;

                                    renderPixelToTex(tempy, tempx);
                                }
                                else
                                {
                                    copyTempLayer.Add(new Color(1f, 1f, 1f, 0f));
                                }
                            }

                            copyLayer.Add(copyTempLayer);
                        }

                        //y for
                        for (int tempy = 0; tempy < selectedPixels.Count; tempy++)
                        {
                            //x for
                            for (int tempx = 0; tempx < selectedPixels[tempy].Count; tempx++)
                            {
                                Color tempColor = copyLayer[tempy][tempx];

                                if (checkOOB(tempy + dy, tempx + dx) && tempColor.a != 0f)
                                {
                                    layerList[selectedLayer][tempy + dy][tempx + dx] = tempColor;
                                    selectedPixels[tempy + dy][tempx + dx] = true;

                                    renderPixelToTex(tempy + dy, tempx + dx);
                                }
                            }
                        }

                        //updateSelectionBorder();
                    }
                }

                lastLastVectorPoint = lastVectorPoint;
                lastVectorPoint = currentVectorPoint;
                lastLastBrushMemory = lastBrushMemory;
                lastBrushMemory = currentBrushMemory;

                pixelTexArea.Apply();

                //UnityEngine.Debug.Log("Got trough tex appliance");
            }
            else //!Wacom.tip.isPressed
            {

                lastVectorPoint = new Vector2();
                lastLastVectorPoint = new Vector2();
                currentBrushMemory = new List<(int, int)>();
                lastBrushMemory = new List<(int, int)>();
                lastLastBrushMemory = new List<(int, int)>();

                if (!firstMove) updateSelectionBorder();

                textActive = false;
                firstMove = true;

                if (toolSelect == 1 && selectRectangleActive) //Select Rectangle
                {
                    selectRectangleActive = false;

                    secondCoordinate = ((int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width)), // - BrushSize
                        (int)((((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height));

                    for (int i = 0; i < selectedPixels.Count; i += 1)
                    {
                        if (selectedPixels[i].All(o => o == selectedPixels[i].First()))
                        {
                            for (int j = 0; j < selectedPixels[i].Count; j += 1)
                            {
                                selectedPixels[i][j] = false;
                            }
                        }
                    }

                    for (int i = 0; i < Math.Abs(firstCoordinate.Item1 - secondCoordinate.Item1); i += 1)
                    {
                        for (int j = 0; j < Math.Abs(firstCoordinate.Item2 - secondCoordinate.Item2); j += 1)
                        {
                            selectedPixels[j + Math.Min(firstCoordinate.Item2, secondCoordinate.Item2)][i + Math.Min(firstCoordinate.Item1, secondCoordinate.Item1)] = true;
                        }
                    }

                    updateSelectionBorder();
                }

                else if (toolSelect == 11 && lineToolActive) //Line Tool
                {
                    lineToolActive = false;

                    secondCoordinate = ((int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width)),
                    (int)((((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height));

                    Vector3 direction = new Vector3(firstCoordinate.Item1 - secondCoordinate.Item1, firstCoordinate.Item2 - secondCoordinate.Item2, 0f);
                    Vector3 startingPoint = new Vector3(secondCoordinate.Item1, secondCoordinate.Item2, 0f);

                    Ray vectorStraight = new Ray(startingPoint, direction.normalized);

                    for (int tempx = 0; tempx < 2 * brushSizeList[toolSelect] + 1 + Math.Abs(firstCoordinate.Item1 - secondCoordinate.Item1); tempx += 1)
                    {
                        for (int tempy = 0; tempy < 2 * brushSizeList[toolSelect] + 1 + Math.Abs(firstCoordinate.Item2 - secondCoordinate.Item2); tempy += 1)
                        {
                            Vector3 currentPoint = new Vector3(0f, 0f);

                            if (firstCoordinate.Item1 >= secondCoordinate.Item1) // move right
                            {
                                if (firstCoordinate.Item2 >= secondCoordinate.Item2)  // move up
                                {
                                    currentPoint = new Vector3(secondCoordinate.Item1 + tempx - brushSizeList[toolSelect], secondCoordinate.Item2 + tempy - brushSizeList[toolSelect], 0f);
                                }
                                if (firstCoordinate.Item2 < secondCoordinate.Item2)  // move down
                                {
                                    currentPoint = new Vector3(secondCoordinate.Item1 + tempx - brushSizeList[toolSelect], secondCoordinate.Item2 - tempy + brushSizeList[toolSelect], 0f);
                                }
                            }
                            else if (firstCoordinate.Item1 < secondCoordinate.Item1) // move left
                            {
                                if (firstCoordinate.Item2 >= secondCoordinate.Item2)  // move up
                                {
                                    currentPoint = new Vector3(secondCoordinate.Item1 - tempx + brushSizeList[toolSelect], secondCoordinate.Item2 + tempy - brushSizeList[toolSelect], 0f);
                                }
                                if (firstCoordinate.Item2 < secondCoordinate.Item2)  // move down
                                {
                                    currentPoint = new Vector3(secondCoordinate.Item1 - tempx + brushSizeList[toolSelect], secondCoordinate.Item2 - tempy + brushSizeList[toolSelect], 0f);
                                }
                            }

                            float distanceToLine = Vector3.Cross(vectorStraight.direction, currentPoint - vectorStraight.origin).magnitude;

                            if (distanceToLine <= brushSizeList[toolSelect] && checkOOB((int)currentPoint.y, (int)currentPoint.x))
                            {
                                Color tempColor = layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x];

                                if (Vector3.Angle((currentPoint - new Vector3(secondCoordinate.Item1, secondCoordinate.Item2, 0f)), (new Vector3(firstCoordinate.Item1, firstCoordinate.Item2, 0f) - new Vector3(secondCoordinate.Item1, secondCoordinate.Item2, 0f))) <= 90f
                                    && Vector3.Angle((currentPoint - new Vector3(firstCoordinate.Item1, firstCoordinate.Item2, 0f)), (new Vector3(secondCoordinate.Item1, secondCoordinate.Item2, 0f) - new Vector3(firstCoordinate.Item1, firstCoordinate.Item2, 0f))) <= 90f)
                                {
                                    float distanceNorm = (brushSizeList[toolSelect] - distanceToLine) / brushSizeList[toolSelect];

                                    tempColor.a = selectedColor.a * distanceNorm + tempColor.a * (1f - selectedColor.a * distanceNorm);
                                    tempColor.r = selectedColor.a * selectedColor.r + tempColor.r * tempColor.a * (1f - selectedColor.a);
                                    tempColor.g = selectedColor.a * selectedColor.g + tempColor.g * tempColor.a * (1f - selectedColor.a);
                                    tempColor.b = selectedColor.a * selectedColor.b + tempColor.b * tempColor.a * (1f - selectedColor.a);

                                    if (tempColor.r > 1f) tempColor.r = 1f;
                                    if (tempColor.g > 1f) tempColor.g = 1f;
                                    if (tempColor.b > 1f) tempColor.b = 1f;
                                    if (tempColor.a > 1f) tempColor.a = 1f;

                                    if (selectedPixels[(int)currentPoint.y][(int)currentPoint.x])
                                    {
                                        layerList[selectedLayer][(int)currentPoint.y][(int)currentPoint.x] = tempColor;

                                        renderPixelToTex((int)currentPoint.y, (int)currentPoint.x);
                                        currentBrushMemory.Add(((int)currentPoint.y, (int)currentPoint.x));
                                    }
                                }
                            }
                        }
                    }
                    pixelTexArea.Apply();
                }

                else if (toolSelect == 5 && lassoActive) //Lasso
                {
                    lassoActive = false;

                    //secondCoordinate = ((int)((((Wacom.position.x.ReadValue() + weirdOffsetX) / 1920) * pixelTexArea.width)),
                    //    (int)((((Wacom.position.y.ReadValue() + weirdOffsetY) / 1080) * pixelTexArea.height) + weirdOffsetYt * pixelTexArea.height));

                    //if (secondCoordinate != firstCoordinate)
                    //{
                    //    double m = (secondCoordinate.Item2 - firstCoordinate.Item2) / (secondCoordinate.Item1 - firstCoordinate.Item1);
                    //    //double length = Math.Sqrt(Math.Pow(secondCoordinate.Item2 - firstCoordinate.Item2, 2) + Math.Pow(secondCoordinate.Item1 - firstCoordinate.Item1, 2));


                    //    for (int tempx = 0; tempx < secondCoordinate.Item1 - firstCoordinate.Item1; tempx += 1)
                    //    {
                    //        //y = mx + b
                    //        for (int tempy = 0; m - tempy > 0; tempy += 1)
                    //        {
                    //            selectedPixels[(int)(firstCoordinate.Item2 + m * tempx + tempy)][tempx] = true;
                    //        }
                    //    }
                    //}

                    if (secondCoordinate != firstCoordinate)
                    {
                        Vector3 direction = new Vector3(firstCoordinate.Item1 - secondCoordinate.Item1, firstCoordinate.Item2 - secondCoordinate.Item2, 0f);
                        Vector3 startingPoint = new Vector3(secondCoordinate.Item1, secondCoordinate.Item2, 0f);

                        Ray vectorStraight = new Ray(startingPoint, direction.normalized);

                        for (int tempx = 0; tempx < Math.Abs(firstCoordinate.Item1 - secondCoordinate.Item1); tempx += 1)
                        {
                            for (int tempy = 0; tempy < Math.Abs(firstCoordinate.Item2 - secondCoordinate.Item2); tempy += 1)
                            {
                                Vector3 currentPoint = new Vector3(0f, 0f);

                                if (firstCoordinate.Item1 >= secondCoordinate.Item1) // move right
                                {
                                    if (firstCoordinate.Item2 >= secondCoordinate.Item2)  // move up
                                    {
                                        currentPoint = new Vector3(secondCoordinate.Item1 + tempx, secondCoordinate.Item2 + tempy, 0f);
                                    }
                                    if (firstCoordinate.Item2 < secondCoordinate.Item2)  // move down
                                    {
                                        currentPoint = new Vector3(secondCoordinate.Item1 + tempx, secondCoordinate.Item2 - tempy, 0f);
                                    }
                                }
                                else if (firstCoordinate.Item1 < secondCoordinate.Item1) // move left
                                {
                                    if (firstCoordinate.Item2 >= secondCoordinate.Item2)  // move up
                                    {
                                        currentPoint = new Vector3(secondCoordinate.Item1 - tempx, secondCoordinate.Item2 + tempy, 0f);
                                    }
                                    if (firstCoordinate.Item2 < secondCoordinate.Item2)  // move down
                                    {
                                        currentPoint = new Vector3(secondCoordinate.Item1 - tempx, secondCoordinate.Item2 - tempy, 0f);
                                    }
                                }

                                float distanceToLine = Vector3.Cross(vectorStraight.direction, currentPoint - vectorStraight.origin).magnitude;

                                if (distanceToLine <= 1f && checkOOB((int)currentPoint.y, (int)currentPoint.x))
                                {
                                    if (Vector3.Angle((currentPoint - new Vector3(secondCoordinate.Item1, secondCoordinate.Item2, 0f)), (new Vector3(firstCoordinate.Item1, firstCoordinate.Item2, 0f) - new Vector3(secondCoordinate.Item1, secondCoordinate.Item2, 0f))) <= 90f
                                        && Vector3.Angle((currentPoint - new Vector3(firstCoordinate.Item1, firstCoordinate.Item2, 0f)), (new Vector3(secondCoordinate.Item1, secondCoordinate.Item2, 0f) - new Vector3(firstCoordinate.Item1, firstCoordinate.Item2, 0f))) <= 90f)
                                    {
                                        selectedPixels[(int)currentPoint.y][(int)currentPoint.x] = true;
                                    }
                                }
                            }
                        }

                        //if (firstCoordinate.Item2 < secondCoordinate.Item2) //up
                        //{
                        //    if (Vector2.Angle(new Vector2(secondCoordinate.Item1 - firstCoordinate.Item1, secondCoordinate.Item2 - firstCoordinate.Item2), Vector2.right) > 0f)
                        //    {

                        //    }
                        //}

                        floodFillSelect((int)mostLeftPoint.x + 2, (int)mostLeftPoint.y); //the 2 is arbitrary
                    }

                    mostLeftPoint = new Vector2(0f, 0f);

                    updateSelectionBorder();
                }
                //else if (textActive && toolSelect == 9) //Text
                //{
                    ////Graphics pixelTexGraph = pixelTexArea.graphicsFormat;
                    //Texture2D pixelTextArea = new Texture2D(pixelTexArea.width, pixelTexArea.height);
                    //Graphics pixelTexGraph = pixelTextArea.graphicsFormat;
                    //textLayers.Add((selectedLayer, pixelTexGraph)); //is this updated dynamically?

                    //Font arialBold = new Font("Arial", FontSize);
                    //Size textSize = TextRenderer.MeasureText(textInput, arialBold);
                    //TextRenderer.DrawText(pixelTexGraph, textInput, arialBold, new Rectangle(new Point(10, 10), textSize), selectedColor);

                    //textLayers[selectedTextLayer].Item3 = KeyboardScript.TextInput;
                    //paintText(pixelTextArea, textLayers[selectedTextLayer].Item2);
                //}
            }

            if (toolSelect == 2)
            {
                UpdateCropBorder();
                cropIsActive = true;
                cropParent.SetActive(true);
            }
            else
            {
                cropIsActive = false;
                cropParent.SetActive(false);
            }
        }
    }

    /*
    public float[] getBrushArea(float radius)
    {
        float[] areaList = new float[(int)Mathf.Pow(2 * radius + 1, 2)];
        Vector2 relativeOrigin = new Vector2(radius, radius);   //maybe need radius + 1
        float[] strengthInOrder = new float[areaList.Length];

        //UnityEngine.Debug.Log("AreaList has a length of " + areaList.Length);

        //y for
        for (int i = 0; i < 2 * radius + 1; i++) {
            //x for
            for (int j = 0; j < 2 * radius + 1; j++) {
                if (j > relativeOrigin.x && i > relativeOrigin.y)
                {
                    areaList[(int)(i * (2 * radius + 1) + j)] = ((1- ((j - relativeOrigin.x) / relativeOrigin.x)) + (1 - ((i - relativeOrigin.y) / relativeOrigin.y))) / 2;
                }
                else if (j <= relativeOrigin.x && i > relativeOrigin.y)
                {
                    areaList[(int)(i * (2 * radius + 1) + j)] = ((j / relativeOrigin.x) + (1 - ((i - relativeOrigin.y) / relativeOrigin.y))) / 2;
                }
                else if (j > relativeOrigin.x && i <= relativeOrigin.y)
                {
                    areaList[(int)(i * (2 * radius + 1) + j)] = ((1 - ((j - relativeOrigin.x) / relativeOrigin.x)) + (i / relativeOrigin.y)) / 2;
                }
                else if (j <= relativeOrigin.x && i <= relativeOrigin.y)
                {
                    areaList[(int)(i * (2 * radius + 1) + j)] = ((j / relativeOrigin.x) + (i / relativeOrigin.y)) / 2;
                }
                else {
                    areaList[(int)(i * (2 * radius + 1) + j)] = 1f;
                }
                //UnityEngine.Debug.Log("looking at " + (i * (2 * radius + 1) + j) + " with the value of " + areaList[(int)(i * (2 * radius + 1) + j)]);
            }
        }
        return areaList;
    }
    */

    public Texture2D newTex(float width, float height, Color background) {
        pixelTexArea = new Texture2D((int)(width * 1000), (int)(height * 1000));
        pixelTexArea.SetPixels( Enumerable.Repeat(background, pixelTexArea.width * pixelTexArea.height).ToArray() );
        texturePlanes = texturePlanes.Concat(new Texture2D[] { pixelTexArea }).ToArray();
        backgroundColor = background;
        return pixelTexArea;
    }

    public void findTablet()
    {
        //InputSystem.onDeviceChange +=
        //        (device, change) =>
        //        {
        //            switch (change)
        //            {
        //                case InputDeviceChange.Added:
        //                    Debug.Log("New device added: " + device + ", ID: " + device.deviceId);

        //                    if (device.deviceId == 41)
        //                    {
        //                        Debug.Log("The right controller has been added!");
        //                    }

        //                    break;

        //                case InputDeviceChange.Removed:
        //                    Debug.Log("Device removed: " + device + ", ID: " + device.deviceId);

        //                    if (device.deviceId == 41)
        //                    {
        //                        Debug.Log("The right controller has been removed!");
        //                    }

        //                    break;
        //                case InputDeviceChange.Disconnected:
        //                    Debug.Log("Device disconnected: " + device + ", ID: " + device.deviceId);

        //                    if (device.deviceId == 41)
        //                    {
        //                        Debug.Log("The right controller has been disconnected!");
        //                    }
        //                    break;
        //            }
        //        };

        Wacom = Pen.current;
        if (Wacom == null)
        {
            UnityEngine.Debug.Log("Couldn't find any tablet or pen device.");
            return;
        }
        else
        {
            penExists = true;
        }
    }

    public string SaveTexture(Texture2D texture, int identifier)
    {
        byte[] bytes = texture.EncodeToPNG();
        var dirPath = Application.persistentDataPath + "/RenderOutput";

        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }

        System.IO.File.WriteAllBytes(dirPath + "/drawPlane_" + identifier + ".png", bytes);
        Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + dirPath);

        #if UNITY_EDITOR
             UnityEditor.AssetDatabase.Refresh();
        #endif
        return (string)(dirPath + "/drawPlane_" + identifier + ".png");
    }

    /*
    public bool[] CheckColorAdj(Texture2D texArea, int x, int y, Color tempColor)
    {
        return { texArea.GetPixel(x + 1, y) == tempColor , texArea.GetPixel(x, y + 1) == tempColor , texArea.GetPixel(x - 1, y) == tempColor , texArea.GetPixel(x, y - 1) == tempColor};
    }
    */

    public void floodFill(int x, int y, Color newColor, Color oldColor)
    {
        List<Tuple<int, int>> cellsToColor1 = new List<Tuple<int, int>>();

        cellsToColor1.Add(Tuple.Create(x, y));

        while (cellsToColor1.Count > 0)
        {
            List<Tuple<int, int>> cellsToColor2 = new List<Tuple<int, int>>();

            for (int i = 0; i < cellsToColor1.Count; i += 1)
            {
                if (layerList[selectedLayer][cellsToColor1[i].Item2][cellsToColor1[i].Item1] == oldColor && selectedPixels[cellsToColor1[i].Item2][cellsToColor1[i].Item1])
                {
                    layerList[selectedLayer][cellsToColor1[i].Item2][cellsToColor1[i].Item1] = newColor;

                    renderPixelToTex(cellsToColor1[i].Item2, cellsToColor1[i].Item1);

                    if (cellsToColor1[i].Item1 + 1 >= 0 && cellsToColor1[i].Item1 + 1 < pixelTexArea.width && cellsToColor1[i].Item2 + 0 > 0 && cellsToColor1[i].Item2 + 0 < pixelTexArea.height)
                    {
                        cellsToColor2.Add(Tuple.Create(cellsToColor1[i].Item1 + 1, cellsToColor1[i].Item2));
                    }
                    if (cellsToColor1[i].Item1 + 0 >= 0 && cellsToColor1[i].Item1 + 0 < pixelTexArea.width && cellsToColor1[i].Item2 + 1 > 0 && cellsToColor1[i].Item2 + 1 < pixelTexArea.height)
                    {
                        cellsToColor2.Add(Tuple.Create(cellsToColor1[i].Item1, cellsToColor1[i].Item2 + 1));
                    }
                    if (cellsToColor1[i].Item1 - 1 >= 0 && cellsToColor1[i].Item1 - 1 < pixelTexArea.width && cellsToColor1[i].Item2 + 0 > 0 && cellsToColor1[i].Item2 + 0 < pixelTexArea.height)
                    {
                        cellsToColor2.Add(Tuple.Create(cellsToColor1[i].Item1 - 1, cellsToColor1[i].Item2));
                    }
                    if (cellsToColor1[i].Item1 + 0 >= 0 && cellsToColor1[i].Item1 + 0 < pixelTexArea.width && cellsToColor1[i].Item2 - 1 > 0 && cellsToColor1[i].Item2 - 1 < pixelTexArea.height)
                    {
                        cellsToColor2.Add(Tuple.Create(cellsToColor1[i].Item1, cellsToColor1[i].Item2 - 1));
                    }
                }
            }
            cellsToColor1 = cellsToColor2;
        }
    }

    public void floodFillSelect(int x, int y)
    {
        List<Tuple<int, int>> cellsToSelect1 = new List<Tuple<int, int>>();

        cellsToSelect1.Add(Tuple.Create(x, y));

        while (cellsToSelect1.Count > 0)
        {
            List<Tuple<int, int>> cellsToSelect2 = new List<Tuple<int, int>>();

            for (int i = 0; i < cellsToSelect1.Count; i += 1)
            {
                if (selectedPixels[cellsToSelect1[i].Item2][cellsToSelect1[i].Item1] == false)
                {
                    selectedPixels[cellsToSelect1[i].Item2][cellsToSelect1[i].Item1] = true;

                    if (cellsToSelect1[i].Item1 + 1 >= 0 && cellsToSelect1[i].Item1 + 1 < pixelTexArea.width && cellsToSelect1[i].Item2 + 0 > 0 && cellsToSelect1[i].Item2 + 0 < pixelTexArea.height)
                    {
                        cellsToSelect2.Add(Tuple.Create(cellsToSelect1[i].Item1 + 1, cellsToSelect1[i].Item2));
                    }
                    if (cellsToSelect1[i].Item1 + 0 >= 0 && cellsToSelect1[i].Item1 + 0 < pixelTexArea.width && cellsToSelect1[i].Item2 + 1 > 0 && cellsToSelect1[i].Item2 + 1 < pixelTexArea.height)
                    {
                        cellsToSelect2.Add(Tuple.Create(cellsToSelect1[i].Item1, cellsToSelect1[i].Item2 + 1));
                    }
                    if (cellsToSelect1[i].Item1 - 1 >= 0 && cellsToSelect1[i].Item1 - 1 < pixelTexArea.width && cellsToSelect1[i].Item2 + 0 > 0 && cellsToSelect1[i].Item2 + 0 < pixelTexArea.height)
                    {
                        cellsToSelect2.Add(Tuple.Create(cellsToSelect1[i].Item1 - 1, cellsToSelect1[i].Item2));
                    }
                    if (cellsToSelect1[i].Item1 + 0 >= 0 && cellsToSelect1[i].Item1 + 0 < pixelTexArea.width && cellsToSelect1[i].Item2 - 1 > 0 && cellsToSelect1[i].Item2 - 1 < pixelTexArea.height)
                    {
                        cellsToSelect2.Add(Tuple.Create(cellsToSelect1[i].Item1, cellsToSelect1[i].Item2 - 1));
                    }
                }
            }
            cellsToSelect1 = cellsToSelect2;
        }
    }

    /*
    public void SelectNewDrawPlane(GameObject sender)
    {
        selPlane = sender.transform.parent.gameObject.GetComponent<ValueScript>().getIntValue();

        UnityEngine.Debug.Log("selected drawPlane number " + selPlane);

        return;
    }
    */

    public void selectRayDrawPlane(GameObject Hand, String tag)
    {
        Vector3 rayDirection = -Hand.transform.forward;
        Vector3 rayStart = Hand.transform.position;
        RaycastHit rayHit;

        if (toolSelect == 9 && Physics.Raycast(rayStart, rayDirection, out rayHit))
        {
            /*
            if (rayHit.collider.gameObject.tag == tag)
            {
                selPlane = rayHit.collider.gameObject.GetComponent<ValueScript>().getIntValue();

                UnityEngine.Debug.Log("selected drawPlane number " + selPlane);
            }
            */
            if (rayHit.collider.gameObject != null)
            {
                //if (rayHit.collider.gameObject.transform.parent != null)
                //{
                //    if (rayHit.collider.gameObject.transform.parent.gameObject.tag == tag)
                if (rayHit.collider.gameObject.tag == tag)
                {
                    if (transPlanes.Count != 0) transPlanes[selPlane].GetComponent<ValueScript>().setFourDFloatArrayValue(convertListToFloat(layerList));
                    //selPlane = rayHit.collider.gameObject.transform.parent.GetComponent<ValueScript>().getIntValue();
                    selPlane = rayHit.collider.gameObject.GetComponent<ValueScript>().getIntValue();

                    if (transPlanes.Count != 0)
                    {
                        layerList = convertFloatToList(transPlanes[selPlane].GetComponent<ValueScript>().getFourDFloatArrayValue());
                    }

                    setLayerList(transPlanes[selPlane].GetComponent<ValueScript>().getFourDFloatArrayValue());
                    pixelTexArea = texturePlanes[selPlane];

                    UnityEngine.Debug.Log("selected drawPlane number " + selPlane);
                }
                else
                {
                    //UnityEngine.Debug.Log("tag of " + rayHit.collider.gameObject.name + " is not drawPlane");
                }
                //}
                //else
                //{
                //    UnityEngine.Debug.Log("parent " + rayHit.collider.gameObject.name + " is null");
                //}
            }
            //else
            //{
            //    UnityEngine.Debug.Log("gameObject " + rayHit.collider.gameObject.name + " is null");
            //}
        }

        return;
    }

    public void renderPixelToTex(int y, int x, bool unselected = false)
    {
        Color tempColor = layerList[layerList.Count - 1][y][x];

        //A Over B partially transparent (wikipedia Alpha compositing)
        for (int k = 0; k < layerList.Count - 2; k += 1)
        {
            tempColor.a += layerList[layerList.Count - k - 1][y][x].a * (1f - tempColor.a);
            tempColor.r = tempColor.a * tempColor.r + layerList[layerList.Count - k - 1][y][x].r * layerList[layerList.Count - k - 1][y][x].a * (1f - tempColor.a);
            tempColor.g = tempColor.a * tempColor.g + layerList[layerList.Count - k - 1][y][x].g * layerList[layerList.Count - k - 1][y][x].a * (1f - tempColor.a);
            tempColor.b = tempColor.a * tempColor.b + layerList[layerList.Count - k - 1][y][x].b * layerList[layerList.Count - k - 1][y][x].a * (1f - tempColor.a);

            for (int i = 0; i < textLayers.Count; i += 1)
            {
                if (textLayers[i].Item1 == layerList.Count - k - 1)
                {
                    double formFitX = pixelTexArea.width / textLayers[i].Item3.width;
                    double formFitY = pixelTexArea.height / textLayers[i].Item3.height;

                    if (textLayers[i].Item3.GetPixel((int)(x * formFitX), (int)(y * formFitY)) == new Color(0f, 0f, 0f, 0f))
                    {
                        //draw x,y pixel of tex to tempColor
                    }
                }
            }
        }

        if (unselected)
        {
            tempColor.a += contrast.a * (1f - tempColor.a);
            tempColor.r = tempColor.a * tempColor.r + contrast.r * contrast.a * (1f - tempColor.a);
            tempColor.g = tempColor.a * tempColor.g + contrast.r * contrast.a * (1f - tempColor.a);
            tempColor.b = tempColor.a * tempColor.b + contrast.r * contrast.a * (1f - tempColor.a);
        }

        if (tempColor.a != 1f)
        {
            //tempColor.r += layerList[0][y][x].r * (1f - tempColor.a);
            //tempColor.g += layerList[0][y][x].g * (1f - tempColor.a);
            //tempColor.b += layerList[0][y][x].b * (1f - tempColor.a);
            tempColor.r = tempColor.a * tempColor.r + layerList[0][y][x].r * layerList[0][y][x].a * (1f - tempColor.a);
            tempColor.g = tempColor.a * tempColor.g + layerList[0][y][x].r * layerList[0][y][x].a * (1f - tempColor.a);
            tempColor.b = tempColor.a * tempColor.b + layerList[0][y][x].r * layerList[0][y][x].a * (1f - tempColor.a);
            tempColor.a = 1f;

            if (tempColor.r > 1f) tempColor.r = 1f;
            if (tempColor.g > 1f) tempColor.g = 1f;
            if (tempColor.b > 1f) tempColor.b = 1f;
            if (tempColor.r < 0f) tempColor.r = 0f;
            if (tempColor.g < 0f) tempColor.g = 0f;
            if (tempColor.b < 0f) tempColor.b = 0f;
        }

        pixelTexArea.SetPixel(x, y, tempColor);
    }

    /*
    public void renderAllPixelsToTex()
    {
        //for y
        for (int i = 0; i < pixelTexArea.height; i += 1)
        {
            //for x
            for (int j = 0; j < pixelTexArea.width; j += 1)
            {
                Color previousColor = new Color(0f, 0f, 0f, 0f);
                for (int k = 0; k < layerList.Count; k += 1) // shouldn't it be layerList.Count - 1?
                {
                    Color tempColor = layerList[layerList.Count - k - 1][i][j];
                    if (tempColor.a >= 1f)
                    {
                        tempColor.a = 1f;
                        pixelTexArea.SetPixel(j, i, tempColor);
                        break;
                    }
                    else
                    {
                        if (layerList.Count - 1 == k || tempColor.a == 1f)
                        {
                            pixelTexArea.SetPixel(j, i, tempColor);
                            break;
                        }
                        else
                        {
                            tempColor.r = previousColor.r * previousColor.a + tempColor.r * (1f - previousColor.a); // tempColor.a
                            tempColor.g = previousColor.g * previousColor.a + tempColor.g * (1f - previousColor.a); // tempColor.a
                            tempColor.b = previousColor.b * previousColor.a + tempColor.b * (1f - previousColor.a); // tempColor.a
                            tempColor.a += previousColor.a;
                            if (tempColor.r > 1f) tempColor.r = 1f;
                            if (tempColor.g > 1f) tempColor.g = 1f;
                            if (tempColor.b > 1f) tempColor.b = 1f;
                            if (tempColor.r < 0f) tempColor.r = 0f;
                            if (tempColor.g < 0f) tempColor.g = 0f;
                            if (tempColor.b < 0f) tempColor.b = 0f;
                            if (tempColor.a > 1f) tempColor.a = 1f;
                            previousColor = tempColor;
                        }
                    }
                }
            }
        }
    }
    */

    public DrawPlane getSelectedDrawPlane()
    {
        if(SaveSystem.drawPlaneList.Count != 0) return SaveSystem.drawPlaneList[selPlane];
        return null;
    }

    public void setHoveringRightTrue()
    {
        isHoveringRight = true;
    }
    public void setHoveringRightFalse()
    {
        isHoveringRight = false;
    }
    public void setHoveringLeftTrue()
    {
        isHoveringLeft = true;
    }
    public void setHoveringLeftFalse()
    {
        isHoveringLeft = false;
    }
    public bool getGripIsActive()
    {
        return GripIsActive;
    }
    public void setGripActive(bool isActive)
    {
        GripIsActive = isActive;
    }
    public Color getBackgroundColor()
    {
        return backgroundColor;
    }
    /*
    public void findPlane()
    {
        //Plane finding
        int tempi = 0;
        foreach (GameObject drawPlane in GameObject.FindGameObjectsWithTag("drawPlane"))
        {
            if (drawPlane.name == "FactualPlane" && tempi == selPlane)
            {
                FactualPlane = drawPlane;
                foundPlane = true;

                InputSystem.onDeviceChange +=
                (device, change) =>
                {
                    switch (change)
                    {
                        case InputDeviceChange.Added:
                            Debug.Log("New device added: " + device);
                            break;

                        case InputDeviceChange.Removed:
                            Debug.Log("Device removed: " + device);
                            break;
                    }
                };
            }
            else if (drawPlane.name == "FactualPlane") { tempi++; }
        }
    }
    */

    public void selectNewColor(Color newColor)
    {
        selectedColor = newColor;
    }

    public void selectLayer(int layer)
    {
        selectedLayer = layer;
    }

    public void createNewLayer()
    {
        GameObject newLayerButton = Instantiate(layerButton, UICanvas.transform.position, UICanvas.transform.rotation, UICanvas.transform);
        UIButtons.Add(newLayerButton);
        //newLayerButton.transform.parent = UICanvas.transform;
        //newLayerButton.transform.rotation = Quaternion.identity;
        //newLayerButton.transform.localPosition = new Vector3(0.064f, UIButtons.Count, 0f);
        newLayerButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "Layer " + (UIButtons.Count - 1);
        int tempUIButtonsCount = UIButtons.Count - 1;
        newLayerButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { selectLayer(tempUIButtonsCount); });//AddListener(() => selectLayer(tempUIButtonsCount))
        //newLayerButton.transform.localScale = new Vector3(1f, 1f, 1f);

        /*
        if (UIButtons.Count > 3)
        {
            scrollBar.SetActive(true);
        }
        */

        List<List<Color>> tempList1 = new List<List<Color>>();
        for (int i = 0; i < pixelTexArea.height; i += 1)
        {
            List<Color> tempList2 = new List<Color>();
            for (int j = 0; j < pixelTexArea.width; j += 1)
            {
                tempList2.Add(new Color(1f, 1f, 1f, 0f));
            }
            tempList1.Add(tempList2);
        }
        layerList.Add(tempList1);

        selectedLayer = layerList.Count - 1;
        if (layerList.Count > 2) //this one fucks
        {
            transPlanes[selPlane].GetComponent<ValueScript>().setFourDFloatArrayValue(convertListToFloat(layerList));
        }
    }

    public List<List<List<Color>>> getLayerList()
    {
        return layerList;
    }

    public void setLayerList(float[,,,] floatLayerList)
    {
        layerList = new List<List<List<Color>>>();
        UnityEngine.Debug.Log("Created a new layerList through setLayerList");

        for (int i = 0; i < floatLayerList.GetLength(0); i += 1)
        {
            List<List<Color>> subLayerList = new List<List<Color>>();
            for (int j = 0; j < floatLayerList.GetLength(1); j += 1)
            {
                List<Color> subSubLayerList = new List<Color>();
                for (int k = 0; k < floatLayerList.GetLength(2); k += 1)
                {
                    subSubLayerList.Add(new Color(
                            floatLayerList[i, j, k, 0],
                            floatLayerList[i, j, k, 1],
                            floatLayerList[i, j, k, 2],
                            floatLayerList[i, j, k, 3]
                        ));

                    /*
                    layerList[i][j][k].r = floatLayerList[i, j, k, 0];
                    layerList[i][j][k].g = floatLayerList[i, j, k, 1];
                    layerList[i][j][k].b = floatLayerList[i, j, k, 2];
                    layerList[i][j][k].a = floatLayerList[i, j, k, 3];
                    */
                    //could go out of bounds
                }
                subLayerList.Add(subSubLayerList);
            }
            layerList.Add(subLayerList);
        }
    }

    public List<List<List<Color>>> extendLayerList(List<List<List<Color>>> oldLayerList, int newWidth, int newHeight, Vector2 upperLeftPoint)
    {
        List<List<List<Color>>> newLayerList = new List<List<List<Color>>>();
        //UnityEngine.Debug.Log("Created an extended layerList through extendLayerList");

        for (int i = 0; i < oldLayerList.Count; i += 1) // -1 OOB
        {
            List<List<Color>> subLayerList = new List<List<Color>>();
            for (int j = 0; j < newHeight; j += 1)
            {
                List<Color> subSubLayerList = new List<Color>();
                for (int k = 0; k < newWidth; k += 1)
                {
                    //Debug.Log("j - upperLeftPoint.y >= 0 : " + (j - upperLeftPoint.y) + "\n" +
                    //    "k - upperLeftPoint.x >= 0 : " + (k - upperLeftPoint.x) + "\n" +
                    //    "i toMax layerCount : " + i + ", " + oldLayerList.Count + "\n" +
                    //    "j toMax newHeight : " + j + ", " + oldLayerList[i].Count + "\n" +
                    //    "k toMax newWidth : " + k + ", " + oldLayerList[i][j].Count);

                    if (j - upperLeftPoint.y >= 0 
                        && k - upperLeftPoint.x >= 0
                        //&& i < oldLayerList.Count //these three suck cockass
                        //&& j < oldLayerList[i].Count
                        //&& k < oldLayerList[i][j].Count
                        //&& j - upperLeftPoint.y < oldLayerList[i].Count - 1 
                        //&& k - upperLeftPoint.x < oldLayerList[i][j].Count - 1)
                        && j - upperLeftPoint.y < oldLayerList[i].Count - 1
                        && k - upperLeftPoint.x < oldLayerList[i][j - (int)upperLeftPoint.y].Count - 1)
                    {
                        //subSubLayerList.Add(oldLayerList[i][j][k]);
                        subSubLayerList.Add(oldLayerList[i][j - (int)upperLeftPoint.y][k - (int)upperLeftPoint.x]);
                    }
                    else if (i == 0)
                    {
                        subSubLayerList.Add(new Color(1f, 1f, 1f, 1f));
                    }
                    else
                    {
                        subSubLayerList.Add(new Color(1f, 1f, 1f, 0f));
                    }
                }
                subLayerList.Add(subSubLayerList);
                //Debug.Log("Added subSubLayerList to subLayerList!");
            }
            newLayerList.Add(subLayerList); //maybe you need to return it and THEN set it instead?
            //Debug.Log("Added subLayerList to layerList!");
        }

        return newLayerList;
    }

    public void createNewLayerList()
    {
        selectedPixels = new List<List<bool>>();

        layerList = new List<List<List<Color>>>();
        List<List<Color>> tempListA = new List<List<Color>>();
        //y for
        for (int i = 0; i < pixelTexArea.height; i += 1)
        {
            List<Color> tempListB = new List<Color>();
            //x for
            for (int j = 0; j < pixelTexArea.width; j += 1)
            {
                tempListB.Add(new Color(1f, 1f, 1f, 1f));
            }
            tempListA.Add(tempListB);
        }
        layerList.Add(tempListA);

        List<List<Color>> tempListC = new List<List<Color>>();
        //y for
        for (int i = 0; i < pixelTexArea.height; i += 1)
        {
            List<Color> tempListD = new List<Color>();
            //x for
            for (int j = 0; j < pixelTexArea.width; j += 1)
            {
                tempListD.Add(new Color(1f, 1f, 1f, 0f));

                if (selectedPixels.Count - 1 < i)
                {
                    selectedPixels.Add(new List<bool>());
                }
                selectedPixels[i].Add(true);
            }
            tempListC.Add(tempListD);
        }
        layerList.Add(tempListC);
    }

    public float[,,,] convertListToFloat(List<List<List<Color>>> colorList)
    {
        float[,,,] tempFourDFloatArray = new float[colorList.Count, colorList[0].Count, colorList[0][0].Count, 4];

        for (int i = 0; i < colorList.Count; i += 1)
        {
            for (int j = 0; j < colorList[i].Count; j += 1)
            {
                for (int k = 0; k < colorList[i][j].Count; k += 1)
                {
                    tempFourDFloatArray[i, j, k, 0] = colorList[i][j][k].r;
                    tempFourDFloatArray[i, j, k, 1] = colorList[i][j][k].g;
                    tempFourDFloatArray[i, j, k, 2] = colorList[i][j][k].b;
                    tempFourDFloatArray[i, j, k, 3] = colorList[i][j][k].a;
                }
            }
        }
        return tempFourDFloatArray;
    }

    public List<List<List<Color>>> convertFloatToList(float[,,,] colorFloat)
    {
        List<List<List<Color>>> tempFourDList = new List<List<List<Color>>>();

        List<List<Color>> tempListA = new List<List<Color>>();

        //y for
        for (int i = 0; i < colorFloat.GetLength(1); i += 1)
        {
            List<Color> tempListB = new List<Color>();
            //x for
            for (int j = 0; j < colorFloat.GetLength(2); j += 1)
            {
                //layer for
                for (int k = 0; k < colorFloat.GetLength(0); k += 1)
                {
                    tempListB.Add(new Color(colorFloat[k, i, j, 0], colorFloat[k, i, j, 1], colorFloat[k, i, j, 2], colorFloat[k, i, j, 3]));
                }
            }
            tempListA.Add(tempListB);
        }
        tempFourDList.Add(tempListA);

        return tempFourDList;
    }

    //Vector2 perpendicularLine()
    //{
    //    return new Vector2(currentVectorPoint.x - lastVectorPoint.x, lastVectorPoint.y - currentVectorPoint.y);
    //}

    //float distanceToSegment(float a, float b, Vector2 currentPoint)
    //{
    //    if(a < 0) return Vector2.Distance(lastVectorPoint, currentPoint);
    //    else if(a > 1) return Vector2.Distance(currentVectorPoint, currentPoint);
    //    else return perpendicularLine().magnitude * b;
    //}

    public bool checkOOB(int y, int x)
    {
        return (x > 0) && (y > 0) && (y < layerList[selectedLayer].Count - 1) && (x < layerList[selectedLayer][y].Count - 1);
    }

    public Color getContrastColor()
    {
        //RMS Contrast wikipedia
        //Color contrast;
        double[] colorSum = new double[4];
        Color currentCol;
        Color averageCol;

        for (int tempy = 0; tempy < pixelTexArea.height; tempy += 1)
        {
            for (int tempx = 0; tempx < pixelTexArea.width; tempx += 1)
            {
                currentCol = pixelTexArea.GetPixel(tempx, tempy);
                colorSum[0] += (double)currentCol.r;
                colorSum[1] += (double)currentCol.g;
                colorSum[2] += (double)currentCol.b;
                colorSum[3] += (double)currentCol.a;
            }
        }

        averageCol = new Color((float)colorSum[0] / (pixelTexArea.height * pixelTexArea.width), (float)colorSum[1] / (pixelTexArea.height * pixelTexArea.width), (float)colorSum[2] / (pixelTexArea.height * pixelTexArea.width), (float)colorSum[3] / (pixelTexArea.height * pixelTexArea.width));

        colorSum = new double[3];

        for (int tempy = 0; tempy < pixelTexArea.height; tempy += 1)
        {
            for (int tempx = 0; tempx < pixelTexArea.width; tempx += 1)
            {
                currentCol = pixelTexArea.GetPixel(tempx, tempy);

                colorSum[0] += Math.Pow((double)currentCol.r - (double)averageCol.r, 2);
                colorSum[1] += Math.Pow((double)currentCol.g - (double)averageCol.g, 2);
                colorSum[2] += Math.Pow((double)currentCol.b - (double)averageCol.b, 2);
            }
        }

        contrast = new Color((float)Math.Sqrt(colorSum[0] / (pixelTexArea.height * pixelTexArea.width)), (float)Math.Sqrt(colorSum[1] / (pixelTexArea.height * pixelTexArea.width)), (float)Math.Sqrt(colorSum[2] / (pixelTexArea.height * pixelTexArea.width)), 0.5f); //(float)Math.Sqrt(colorSum[3] / (pixelTexArea.height * pixelTexArea.width))
        
        return contrast;
    }

    public void updateSelectionBorder()
    {
        contrast = getContrastColor();

        for (int tempy = 0; tempy < selectedPixels.Count; tempy += 1)
        {
            for (int tempx = 0; tempx < selectedPixels[tempy].Count; tempx += 1)
            {
                if (!selectedPixels[tempy][tempx])
                {
                    renderPixelToTex(tempy, tempx, true);
                }
                else if (selectedPixels[tempy][tempx])
                {
                    renderPixelToTex(tempy, tempx, false);
                }
            }
        }

        pixelTexArea.Apply();
    }

    //public void paintText(object sender, System.Windows.Forms.PaintEventArgs e)
    //{
    //    //paint with e
    //    String text1 = "Measure this text";
    //    float FontSize = 12.0f;
    //    Font arialBold = new Font("Arial", FontSize);
    //    Size textSize = TextRenderer.MeasureText(text1, arialBold);
    //    TextRenderer.DrawText(e.Graphics, text1, arialBold, new Rectangle(new Point(10, 10), textSize), Color.Red);
    //}

    //public void paintText(Graphics pixelTexGraph, Texture2D pixelTextArea)
    //{
    //    Font arialBold = new Font("Arial", FontSize);
    //    Size textSize = TextRenderer.MeasureText(textLayers[selectedTextLayer].Item3, arialBold);
    //    TextRenderer.DrawText(pixelTexGraph, textLayers[selectedTextLayer].Item3, arialBold, new Rectangle(new Point(10, 10), textSize), selectedColor);
    //}

    public void selectEverything()
    {
        //y for
        for (int tempy = 0; tempy < selectedPixels.Count; tempy++)
        {
            //x for
            for (int tempx = 0; tempx < selectedPixels[tempy].Count; tempx++)
            {
                selectedPixels[tempy][tempx] = true;
            }
        }

        updateSelectionBorder();
    }

    public void UpdateCropBorder()
    {
        /*child order:
         *0: top left
         *1: top
         *2: top right
         *3: right
         *4: bottom right
         *5: bottom
         *6: bottom left
         *7: left
         */
        if (transPlanes.Count != 0)
        {
            cropParent.transform.SetParent(transPlanes[selPlane].transform);
            cropParent.transform.localPosition = new Vector3(0f, 0f, 0f);
            cropParent.transform.localEulerAngles = new Vector3(0f, 90f, 180f);
        }

        //cropParent.transform.GetChild(0).localPosition = cropPos1;
        //cropParent.transform.GetChild(1).localPosition = Vector3.Lerp(cropPos1, cropPos3, 0.5f);
        //cropParent.transform.GetChild(2).localPosition = cropPos3;
        //cropParent.transform.GetChild(3).localPosition = Vector3.Lerp(cropPos3, cropPos2, 0.5f);
        //cropParent.transform.GetChild(4).localPosition = cropPos2;
        //cropParent.transform.GetChild(5).localPosition = Vector3.Lerp(cropPos2, cropPos4, 0.5f);
        //cropParent.transform.GetChild(6).localPosition = cropPos4;
        //cropParent.transform.GetChild(7).localPosition = Vector3.Lerp(cropPos4, cropPos1, 0.5f);

        cropParent.transform.GetChild(0).localPosition = tempCropPos1;
        cropParent.transform.GetChild(1).localPosition = Vector3.Lerp(tempCropPos1, tempCropPos3, 0.5f);
        cropParent.transform.GetChild(2).localPosition = tempCropPos3;
        cropParent.transform.GetChild(3).localPosition = Vector3.Lerp(tempCropPos3, tempCropPos2, 0.5f);
        cropParent.transform.GetChild(4).localPosition = tempCropPos2;
        cropParent.transform.GetChild(5).localPosition = Vector3.Lerp(tempCropPos2, tempCropPos4, 0.5f);
        cropParent.transform.GetChild(6).localPosition = tempCropPos4;
        cropParent.transform.GetChild(7).localPosition = Vector3.Lerp(tempCropPos4, tempCropPos1, 0.5f);
    }

    public void changeCropPosRel(int index)
    {
        Vector3 headingDirection = initialHandPos - RightHand.transform.position;
        headingDirection = new Vector3(-headingDirection.z, headingDirection.y, -headingDirection.x);
        if (index == 0)
        {
            tempCropPos1 = headingDirection + cropPos1;
            tempCropPos3 = new Vector3(cropPos2.x, tempCropPos1.y, cropPos2.z);
            tempCropPos4 = new Vector3(tempCropPos1.x, cropPos2.y, tempCropPos1.z);
        }
        else if (index == 2)
        {
            tempCropPos3 = headingDirection + cropPos3;
            tempCropPos1 = new Vector3(cropPos1.x, tempCropPos3.y, cropPos1.z);
            tempCropPos2 = new Vector3(tempCropPos3.x, cropPos2.y, tempCropPos3.z);
        }
        else if (index == 4)
        {
            tempCropPos2 = headingDirection + cropPos2;
            tempCropPos3 = new Vector3(tempCropPos2.x, cropPos1.y, tempCropPos2.z);
            tempCropPos4 = new Vector3(cropPos1.x, tempCropPos2.y, cropPos1.z);
        }
        else if (index == 6)
        {
            tempCropPos4 = headingDirection + cropPos4;
            tempCropPos1 = new Vector3(tempCropPos4.x, cropPos1.y, tempCropPos4.z);
            tempCropPos2 = new Vector3(cropPos2.x, tempCropPos4.y, cropPos2.z);
        }
        else if (index == 1)
        {
            tempCropPos1 = new Vector3(cropPos1.x, cropPos1.y + headingDirection.y, cropPos1.z);
            tempCropPos3 = new Vector3(cropPos3.x, cropPos3.y + headingDirection.y, cropPos3.z);
        }
        else if (index == 3)
        {
            tempCropPos3 = new Vector3(cropPos3.x, cropPos3.y, cropPos3.z + headingDirection.z);
            tempCropPos2 = new Vector3(cropPos2.x, cropPos2.y, cropPos2.z + headingDirection.z);
        }
        else if (index == 5)
        {
            tempCropPos2 = new Vector3(cropPos2.x, cropPos2.y + headingDirection.y, cropPos2.z);
            tempCropPos4 = new Vector3(cropPos4.x, cropPos4.y + headingDirection.y, cropPos4.z);
        }
        else if (index == 7)
        {
            tempCropPos4 = new Vector3(cropPos4.x, cropPos4.y, cropPos4.z + headingDirection.z);
            tempCropPos1 = new Vector3(cropPos1.x, cropPos1.y, cropPos1.z + headingDirection.z);
        }

        tempCropPos1.x = 0f;
        tempCropPos2.x = 0f;
        tempCropPos3.x = 0f;
        tempCropPos4.x = 0f;
    }

    public void enterCrop()
    {
        initialHandPos = RightHand.transform.position;

        //Debug.Log("enterCrop(): initialHandPos = " + initialHandPos);
    }

    public void exitCrop()
    {
        initialHandPos = new Vector3();

        //Debug.Log("exitCrop(): initialHandPos = " + initialHandPos);
    }

    public Texture2D ExtendTex(Texture2D tempPixelTexArea, int newWidth, int newHeight, Vector2 upperLeftPoint)
    {
        ////originial scale: (pixel per unit)
        //float originalWidth = tempPixelTexArea.width / Vector3.Distance(pos1, pos3);
        //float originalHeight = tempPixelTexArea.height / Vector3.Distance(pos1, pos4);

        //float newWidth = originalWidth * Vector3.Distance(tempCropPos1, tempCropPos3);
        //float newHeight = originalHeight * Vector3.Distance(tempCropPos1, tempCropPos4);

        Texture2D newPixelTexArea = new Texture2D(newWidth, newHeight);

        //Debug.Log("new pixels scale: " + newPixelTexArea.width + "x" + newPixelTexArea.height + "\n vs: old pixels scale: " + tempPixelTexArea.width + "x" + tempPixelTexArea.height);
        //newPixelTexArea.SetPixels(tempPixelTexArea.GetPixels());

        ////y for
        //for (int tempy = 0; tempy < tempPixelTexArea.height; tempy++)
        //{
        //    //x for
        //    for (int tempx = 0; tempx < tempPixelTexArea.width; tempx++)
        //    {
        //        newPixelTexArea.SetPixel(tempx, tempy, tempPixelTexArea.GetPixel(tempx, tempy));
        //    }
        //}

        for (int i = 0; i < newWidth; i += 1)
        {
            for (int j = 0; j < newHeight; j += 1)
            {
                if (j - upperLeftPoint.y >= 0
                    && i - upperLeftPoint.x >= 0
                    && j - upperLeftPoint.y < tempPixelTexArea.height
                    && i - upperLeftPoint.x < tempPixelTexArea.width)
                {
                    newPixelTexArea.SetPixel(i, j, tempPixelTexArea.GetPixel(i - (int)upperLeftPoint.x, j - (int)upperLeftPoint.y));
                }
                else
                {
                    newPixelTexArea.SetPixel(i, j, new Color(1f, 1f, 1f, 1f));
                }
            }
        }

        //texturePlanes[selPlane] = newPixelTexArea;
        //texturePlanes = texturePlanes.Concat(new Texture2D[] { newPixelTexArea }).ToArray();

        return newPixelTexArea;
    }

    public void renderPixelsToTex(int x, int y, int width, int height)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                renderPixelToTex(y + j, x + i);
            }
        }
    }

    public void setTextInput(string inputText)
    {
        textLayers[selectedTextLayer] = (textLayers[selectedTextLayer].Item1, inputText, textLayers[selectedTextLayer].Item3, textLayers[selectedTextLayer].Item4);
    }

    public int getToolSelect()
    {
        return toolSelect;
    }

    public void setToolSelect(int value)
    {
        toolSelect = value;
    }

    public bool getCropActive()
    {
        return cropIsActive;
    }

    public void changeBrushSize(float value)
    {
        brushSizeList[toolSelect] = 1 + (int)Math.Pow(value * 25, 2);
        BrushSizeText.GetComponent<UnityEngine.UI.Text>().text = "Brush Size: " + brushSizeList[toolSelect];
    }

    public void changeLerpCount(float value)
    {
        lerpCount = (int)(49 * Math.Pow(value, 4) + 1);
        LerpCountText.GetComponent<UnityEngine.UI.Text>().text = "lerpCount: " + lerpCount;
    }

    public void changeSelPlane(int value)
    {
        selPlane = value;
    }
}
