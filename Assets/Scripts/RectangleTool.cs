using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using UnityEngine;
using System.Collections.Specialized;
using System.ComponentModel.Design.Serialization;
using System.Threading;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Text;

//isInRectTool Overview:
//[0 UP]: Teleportation Curve
//[1 LEFT]: drawable planeMaker + turning left
//[2 RIGHT]: draw in 3d + turning right
//[3 DOWN]: SelectOverlay (primarly used for tools)
//
//[SO 0 UP UP]: Create Desktop Mirror
//[SO 1 UP RIGHT]: SelectNewDesktop()
//[SO 2 RIGHT RIGHT]: (unallocated)
//[SO 3 Down RIGHT]: (unallocated)
//[SO 4 Down Down]: (unallocated)
//[SO 5 Down LEFT]: (unallocated)
//[SO 6 LEFT LEFT]: (unallocated)
//[SO 7 UP LEFT]: return to normal select

public class RectangleTool : MonoBehaviour
{
    HandPresence RightHandScript;
    HandPresence LeftHandScript;
    planeCreation planeMaker;
    bruhsh bruhshScript;
    SpriteGrab leftSpriteGrabber;
    SpriteGrab rightSpriteGrabber;
    Keyboard KeyboardScript;

    public GameObject RightHand;
    public GameObject LeftHand;
    private GameObject planeDraw;
    public GameObject RectangleToolSelf;
    public GameObject planeCreator;
    public GameObject bruhsher;
    public GameObject FactualPlane;
    private GameObject collideObj;
    public GameObject LeftHandGrab;
    public GameObject RightHandGrab;
    public GameObject VRRig;
    public GameObject Camera;
    public GameObject LSelectionOverlay;
    public GameObject RSelectionOverlay;
    public GameObject LeftDesktopSelectorRay;
    public GameObject RightDesktopSelectorRay;
    public GameObject LeftRayUI;
    public GameObject RightRayUI;
    public GameObject Keyboard;
    //public GameObject LeftDrawPlaneRay;
    //public GameObject RightDrawPlaneRay;

    //public XRRayInteractor LRayInt;
    //public XRRayInteractor RRayInt;

    public Sprite DrawableSprite;

    [SerializeField]
    public int isInRectTool = 0;
    [SerializeField]
    private Vector2 right2DAxis;
    [SerializeField]
    private Vector2 left2DAxis;
    [SerializeField]
    static Vector2 IdealPointLeft = new Vector2(-1f, 0f);
    [SerializeField]
    static Vector2 IdealPointRight = new Vector2(1f, 0f);
    [SerializeField]
    static Vector2 IdealPointUp = new Vector2(0f, 1f);
    [SerializeField]
    static Vector2 IdealPointDown = new Vector2(0f, -1f);
    [SerializeField]
    static Vector2 ZeroPoint = new Vector2(0f, 0f);
    [SerializeField]
    static Vector2 IdealPointUpRight = new Vector2(0.70710678118f, 0.70710678118f);
    [SerializeField]
    static Vector2 IdealPointDownRight = new Vector2(0.70710678118f, -0.70710678118f);
    [SerializeField]
    static Vector2 IdealPointDownLeft = new Vector2(-0.70710678118f, -0.70710678118f);
    [SerializeField]
    static Vector2 IdealPointUpLeft = new Vector2(-0.70710678118f, 0.70710678118f);
    [SerializeField]
    private bool touchpadPress;
    [SerializeField]
    private bool leftWasPressed = false;
    [SerializeField]
    private bool rightWasPressed = false;
    [SerializeField]
    public Vector3 pos1;
    [SerializeField]
    public Vector3 pos2;
    [SerializeField]
    public Vector3[] allPos;
    [SerializeField]
    public List<Vector3[]> allAllPos = new List<Vector3[]>();
    [SerializeField]
    public float leftButtonHoldTime = 0;
    [SerializeField]
    public float rightButtonHoldTime = 0;
    [SerializeField]
    public bool leftWasHeld = false;
    [SerializeField]
    public bool leftTriggerWasHeld = false;
    [SerializeField]
    public bool rightTriggerWasHeld = false;
    [SerializeField]
    public bool rightWasHeld = false;
    [SerializeField]
    public Vector2 rightTouchpadLast;
    [SerializeField]
    public Vector2 leftTouchpadLast;
    [SerializeField]
    public int selectOverlay;
    [SerializeField]
    static Vector2[] selectOverlayPosList = new Vector2[] { IdealPointUp, IdealPointUpRight, IdealPointRight, IdealPointDownRight, IdealPointDown, IdealPointDownLeft, IdealPointLeft, IdealPointUpLeft };
    [SerializeField]
    public bool rightGripWasPressed = false;
    [SerializeField]
    public bool leftGripWasPressed = false;
    [SerializeField]
    public Vector3 initialLeftHandPosition;
    [SerializeField]
    public Vector3 initialRightHandPosition;
    [SerializeField]
    public Vector3 leftStart;
    [SerializeField]
    public Vector3 leftDirection;
    [SerializeField]
    public Vector3 rightStart;
    [SerializeField]
    public Vector3 rightDirection;
    [SerializeField]
    public Vector3 initialRightHandRotation;
    [SerializeField]
    public Vector3 initialLeftHandRotation;
    [SerializeField]
    public bool LeftHandEnabled;
    [SerializeField]
    public bool RightHandEnabled;

    Gradient gradientA;
    Gradient gradientB;
    GradientColorKey[] colorKeyA;
    GradientAlphaKey[] alphaKeyA;
    GradientColorKey[] colorKeyB;
    GradientAlphaKey[] alphaKeyB;

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    //public event EventHandler onHoverEnter;

    [Header("Prefabs")]
    [SerializeField] GameObject uwc_window_Prefab;
    //[SerializeField] GameObject XRGrabInteractable_Child_Prefab;

    void Start()
    {
        RightHandScript = RightHand.GetComponent<HandPresence>();
        LeftHandScript = LeftHand.GetComponent<HandPresence>();
        bruhshScript = bruhsher.GetComponent<bruhsh>();
        rightSpriteGrabber = RightHandGrab.GetComponent<SpriteGrab>();
        leftSpriteGrabber = LeftHandGrab.GetComponent<SpriteGrab>();
        KeyboardScript = Keyboard.GetComponent<Keyboard>();
        //onHoverEnter += setMaterial_onHoverEnter;
        //FactualPlane.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>().onHoverEnter += setMaterial_onHoverEnter;

        gradientA = new Gradient();
        gradientB = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKeyA = new GradientColorKey[2];
        colorKeyA[0].color = Color.red;
        colorKeyA[0].time = 0.0f;
        colorKeyA[1].color = Color.red;
        colorKeyA[1].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKeyA = new GradientAlphaKey[2];
        alphaKeyA[0].alpha = 1.0f;
        alphaKeyA[0].time = 0.0f;
        alphaKeyA[1].alpha = 0.0f;
        alphaKeyA[1].time = 1.0f;

        gradientA.SetKeys(colorKeyA, alphaKeyA);


        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKeyB = new GradientColorKey[2];
        colorKeyB[0].color = Color.red;
        colorKeyB[0].time = 0.0f;
        colorKeyB[1].color = Color.red;
        colorKeyB[1].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKeyB = new GradientAlphaKey[2];
        alphaKeyB[0].alpha = 0.0f;
        alphaKeyB[0].time = 0.0f;
        alphaKeyB[1].alpha = 0.0f;
        alphaKeyB[1].time = 1.0f;

        gradientB.SetKeys(colorKeyB, alphaKeyB);

        DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath + "/BlenderImports");
        System.IO.FileInfo[] myFileList = (from f in directory.GetFiles("*.obj") orderby f.LastWriteTime descending select f).ToArray();

        for (int i = 0; i < 5; i++)
        {
            if (i < myFileList.Length)
            {
                //REDO THE IMPORTER AND REMOVE OBJLOADER FROM ASSETS

                //works only for roughly 65k vertices. For higher vert count try another importer
                //Mesh holderMesh = new Mesh();
                //ObjImporter newMesh = new ObjImporter();
                //var textStream = new MemoryStream(Encoding.UTF8.GetBytes(myFileList[i].ToString()));
                var textStream = new MemoryStream(Encoding.UTF8.GetBytes(string.Join("", System.IO.File.ReadAllLines(myFileList[i].FullName))));
                //Dummiesman.OBJLoader newMesh = new Dummiesman.OBJLoader();
                //holderMesh = newMesh.ImportFile(myFileList[i]);
                GameObject newObj = new Dummiesman.OBJLoader().Load(textStream);

                //MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
                //MeshFilter filter = gameObject.AddComponent<MeshFilter>();
                //filter.mesh = holderMesh;
            }
        }
    }

    void Update()
    {
        //XRRayInteractor for selecting planes
        //Vector3 pos = new Vector3();
        //Vector3 norm = new Vector3();
        //int index = 0;
        //bool validTarget = false;

        /*
        if (Input.GetKeyDown(KeyCode.Space)) {
            onHoverEnter?.Invoke(this, EventArgs.Empty);
        }*/

        //TypeToLookFor[] firstList = GameObject.FindObjectsOfType<TypeToLookFor>();

        if (isInRectTool != 1 || bruhshScript.getToolSelect() == 9) //(bruhshScript.getToolSelect() == 9 || bruhshScript.getCropActive())
        {
            GameObject[] objList = GameObject.FindGameObjectsWithTag("drawPlane");
            /*
            for (int i = 0; i < objList.Length; i++)
            {
                if (objList[i].transform.childCount != 0)
                {
                    objList[i].transform.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
                    objList[i].transform.GetChild(0).rotation = objList[i].transform.rotation; //Quaternion.identity
                }
                //objList[i].transform.GetChild(0).position = objList[i].transform.position; //new Vector3(0f, 0f, 0f)
                //UnityEngine.Debug.Log("objList " + i + " is called " + objList[i]);
            }
            */

            if (LeftHandScript.getPressedTouchpad())
            {
                touchpadPress = true;
            }
            else if (RightHandScript.getPressedTouchpad())
            {
                touchpadPress = true;
            }
            else
            {
                touchpadPress = false;
            }

            right2DAxis = RightHandScript.getPrimary2DAxis();
            left2DAxis = LeftHandScript.getPrimary2DAxis();

            ////RectTool 0 = Teleport        RectTool 3 = Multiscreen Editor
            //if (Vector2.Distance(IdealPointUp, right2DAxis) <= 0.5 && right2DAxis != ZeroPoint && touchpadPress && isInRectTool != 3 && isInRectTool != 1)
            //{
            //    isInRectTool = 0;
            //}

            //if (Vector2.Distance(IdealPointUp, left2DAxis) <= 0.5 && left2DAxis != ZeroPoint && touchpadPress && isInRectTool != 3 && isInRectTool != 1)
            //{
            //    isInRectTool = 0;
            //}

            //if (Vector2.Distance(IdealPointDown, right2DAxis) <= 0.5 && right2DAxis != ZeroPoint && touchpadPress && isInRectTool != 1)
            //{
            //    isInRectTool = 3;
            //}

            //if (Vector2.Distance(IdealPointDown, left2DAxis) <= 0.5 && left2DAxis != ZeroPoint && touchpadPress && isInRectTool != 1)
            //{
            //    isInRectTool = 3;
            //}

            if (rightWasHeld)
            {
                ////rightTouchpad, leftPoint -> RectMode 1
                //if (Vector2.Distance(IdealPointLeft, right2DAxis) <= 0.5 && right2DAxis != ZeroPoint && touchpadPress)
                //{
                //    if (rightButtonHoldTime == 0)
                //    {
                //        rightButtonHoldTime = UnityEngine.Time.time;
                //    }
                //}
                if (rightButtonHoldTime == 0)
                {
                    rightButtonHoldTime = UnityEngine.Time.time;
                }
                else if (!touchpadPress && Vector2.Distance(IdealPointLeft, rightTouchpadLast) <= 0.5)
                {
                    if (UnityEngine.Time.time - rightButtonHoldTime > 0.8 && isInRectTool != 3 && isInRectTool != 1)
                    {
                        isInRectTool = 1;
                        bruhshScript.setToolSelect(9);
                    }
                    else if (UnityEngine.Time.time - leftButtonHoldTime > 0.8 && (isInRectTool == 3 || isInRectTool == 1))
                    {
                        //nothing
                    }
                    else
                    {
                        VRRig.transform.Rotate(new Vector3(0f, -45f, 0f));
                    }
                    rightButtonHoldTime = 0;
                }
                else if (!touchpadPress && Vector2.Distance(IdealPointUp, rightTouchpadLast) <= 0.5)
                {
                    if (UnityEngine.Time.time - rightButtonHoldTime > 0.8)
                    {
                        //UnityEngine.Debug.Log("" + (UnityEngine.Time.time - rightButtonHoldTime));
                        isInRectTool = 0;
                    }
                    else
                    {
                        //nothing
                    }
                    rightButtonHoldTime = 0;
                }
                else if (!touchpadPress && Vector2.Distance(IdealPointDown, rightTouchpadLast) <= 0.5)
                {
                    if (UnityEngine.Time.time - rightButtonHoldTime > 0.8)
                    {
                        isInRectTool = 3;
                    }
                    else
                    {
                        //nothing
                    }
                    rightButtonHoldTime = 0;
                }

                //rightTouchpad, rightPoint -> RectMode 2
                if (Vector2.Distance(IdealPointRight, right2DAxis) <= 0.5 && right2DAxis != ZeroPoint && touchpadPress)
                {
                    if (rightButtonHoldTime == 0)
                    {
                        rightButtonHoldTime = UnityEngine.Time.time;
                    }
                }
                else if (!touchpadPress && Vector2.Distance(IdealPointRight, rightTouchpadLast) <= 0.5)
                {
                    if (UnityEngine.Time.time - rightButtonHoldTime > 0.8 && isInRectTool != 3 && isInRectTool != 1)
                    {
                        isInRectTool = 2;
                    }
                    else if (UnityEngine.Time.time - leftButtonHoldTime > 0.8 && (isInRectTool == 3 || isInRectTool == 1))
                    {
                        //nothing
                    }
                    else
                    {
                        VRRig.transform.Rotate(new Vector3(0f, 45f, 0f));
                    }
                    rightButtonHoldTime = 0;
                }
            }

            if (leftWasHeld)
            {
                ////leftTouchpad, leftPoint -> RectMode 1
                //if (Vector2.Distance(IdealPointLeft, left2DAxis) <= 0.5 && left2DAxis != ZeroPoint && touchpadPress)
                //{
                //    if (leftButtonHoldTime == 0)
                //    {
                //        leftButtonHoldTime = UnityEngine.Time.time;
                //    }
                //}
                if (leftButtonHoldTime == 0)
                {
                    leftButtonHoldTime = UnityEngine.Time.time;
                }
                else if (!touchpadPress && Vector2.Distance(IdealPointLeft, leftTouchpadLast) <= 0.5)
                {
                    if (UnityEngine.Time.time - leftButtonHoldTime > 0.8 && isInRectTool != 3 && isInRectTool != 1)
                    {
                        isInRectTool = 1;
                        bruhshScript.setToolSelect(9);
                    }
                    else if (UnityEngine.Time.time - leftButtonHoldTime > 0.8 && (isInRectTool == 3 || isInRectTool == 1))
                    {
                        //nothing
                    }
                    else if (!KeyboardScript.getKeyboardsActive())
                    {
                        VRRig.transform.Rotate(new Vector3(0f, -45f, 0f));
                    }
                    leftButtonHoldTime = 0;
                }

                //leftTouchpad, rightPoint -> RectMode 2
                if (Vector2.Distance(IdealPointRight, left2DAxis) <= 0.5 && left2DAxis != ZeroPoint && touchpadPress)
                {
                    if (leftButtonHoldTime == 0)
                    {
                        leftButtonHoldTime = UnityEngine.Time.time;
                    }
                }
                else if (!touchpadPress && Vector2.Distance(IdealPointRight, leftTouchpadLast) <= 0.5)
                {
                    if (UnityEngine.Time.time - leftButtonHoldTime > 0.8 && isInRectTool != 3 && isInRectTool != 1)
                    {
                        isInRectTool = 2;
                    }
                    else if (UnityEngine.Time.time - leftButtonHoldTime > 0.8 && (isInRectTool == 3 || isInRectTool == 1))
                    {
                        //nothing
                    }
                    else if (!KeyboardScript.getKeyboardsActive())
                    {
                        VRRig.transform.Rotate(new Vector3(0f, 45f, 0f));
                    }
                    leftButtonHoldTime = 0;
                }
                else if (!touchpadPress && Vector2.Distance(IdealPointUp, leftTouchpadLast) <= 0.5)
                {
                    if (UnityEngine.Time.time - leftButtonHoldTime > 0.8)
                    {
                        isInRectTool = 0;
                    }
                    else
                    {
                        //nothing
                    }
                    leftButtonHoldTime = 0;
                }
                else if (!touchpadPress && Vector2.Distance(IdealPointDown, leftTouchpadLast) <= 0.5)
                {
                    if (UnityEngine.Time.time - leftButtonHoldTime > 0.8)
                    {
                        isInRectTool = 3;
                    }
                    else
                    {
                        //nothing
                    }
                    leftButtonHoldTime = 0;
                }
            }

            //bool isRightInteractorRayHovering = RRayInt.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget);
            //bool isLeftInteractorRayHovering = RRayInt.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget);

            if (!rightWasHeld && !leftWasHeld)
            {
                if (RightHandScript.getPressedTrigger()) // && !RRayInt.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget) && !RightRayUI.GetComponent<XRRayInteractor>().TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget)
                {
                    pos2 = Vector3.zero;
                    pos1 = GameObject.Find("RightHand").transform.position;
                    rightWasHeld = true;
                }
                else if (LeftHandScript.getPressedTrigger()) // && !LRayInt.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget) && !LeftRayUI.GetComponent<XRRayInteractor>().TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget)
                {
                    pos2 = Vector3.zero;
                    pos1 = GameObject.Find("LeftHand").transform.position;
                    leftWasHeld = true;
                }
                else if (pos2 == Vector3.zero)
                {
                    pos1 = Vector3.zero;
                }
            }

            if (leftTriggerWasHeld)
            {
                if (LeftHandScript.getPressedTrigger() && isInRectTool == 1)
                {
                    if (leftButtonHoldTime == 0)
                    {
                        leftButtonHoldTime = UnityEngine.Time.time;
                    }
                }
                else if (!LeftHandScript.getPressedTrigger() && isInRectTool == 1)
                {
                    if (UnityEngine.Time.time - leftButtonHoldTime > 0.4)
                    {
                        createFactualPlane(pos1, "LeftHand");
                        leftTriggerWasHeld = false;
                    }
                    else if (UnityEngine.Time.time - leftButtonHoldTime <= 0.4)
                    {
                        //LeftDrawPlaneRay.SetActive(true);
                        bruhshScript.selectRayDrawPlane(LeftHand, "drawPlane");
                        //UnityEngine.Debug.Log("tried selecting a new drawPlane");
                    }
                    leftButtonHoldTime = 0;
                }
            }
            else
            {
                //LeftDrawPlaneRay.SetActive(false);
            }

            if (rightTriggerWasHeld)
            {
                //if (RightHandScript.getPressedTrigger() && isInRectTool == 1)
                //{
                //    if (rightButtonHoldTime == 0)
                //    {
                //        rightButtonHoldTime = UnityEngine.Time.time;
                //    }
                //}
                if (rightButtonHoldTime == 0)
                {
                    rightButtonHoldTime = UnityEngine.Time.time;
                }
                else if (!RightHandScript.getPressedTrigger() && isInRectTool == 1)
                {
                    if (UnityEngine.Time.time - rightButtonHoldTime > 0.4)
                    {
                        createFactualPlane(pos1, "RightHand");
                        rightTriggerWasHeld = false;
                    }
                    else if (UnityEngine.Time.time - rightButtonHoldTime <= 0.4)
                    {
                        //RightDrawPlaneRay.SetActive(true);
                        bruhshScript.selectRayDrawPlane(RightHand, "drawPlane");
                        //UnityEngine.Debug.Log("tried selecting a new drawPlane");
                    }
                    rightButtonHoldTime = 0;
                }
            }
        }
        else
        {
            rightButtonHoldTime = UnityEngine.Time.time;
            leftButtonHoldTime = UnityEngine.Time.time;
        }
        //UnityEngine.Debug.Log("RightDrawPlaneRay is currently active: " + RightDrawPlaneRay.active);

        if (isInRectTool == 1)
        {
            LSelectionOverlay.SetActive(false);
            RSelectionOverlay.SetActive(false);
            //LeftDrawPlaneRay.SetActive(true);
            //RightDrawPlaneRay.SetActive(true);
            RightRayUI.GetComponent<XRInteractorLineVisual>().invalidColorGradient = gradientA;
            LeftRayUI.GetComponent<XRInteractorLineVisual>().invalidColorGradient = gradientA;

            if (RightHandScript.getPressedGrip())
            {
                if (!rightGripWasPressed)
                {
                    rightGripWasPressed = true;

                    rightStart = RightHand.transform.position;
                    rightDirection = -RightHand.transform.forward;

                    //Debug.DrawLine(rightStart, rightDirection + rightStart, Color.blue);

                    /*
                    if (Physics.Raycast(rightStart, rightDirection, out rightHit))
                    {
                        if (rightHit.collider.gameObject != null)
                        {
                            if (rightHit.collider.gameObject.transform.parent != null)
                            {
                                if (rightHit.collider.gameObject.transform.parent.gameObject.tag == "drawPlane")
                                {
                                    //Debug.Log("Selected drawPlane with raycast");
                                    selectedObj = rightHit.collider.gameObject.transform.parent.gameObject;
                                }
                            }
                            //Debug.Log("Selected " + rightHit.collider.gameObject.name);
                        }
                    }
                    */
                }
                else if(bruhshScript.getSelectedDrawPlane() != null) //bruhshScript.getToolSelect() != 9 && 
                {
                    OnRightGripHoldDrawPlane(bruhshScript.getSelectedDrawPlane()); //KeyboardScript.getSelectedDesktop()
                }
            }
            else
            {
                rightGripWasPressed = false;
            }

            if (LeftHandScript.getPressedGrip())
            {
                if (!leftGripWasPressed)
                {
                    leftStart = LeftHand.transform.position;
                    leftDirection = -LeftHand.transform.forward;

                    //Debug.DrawLine(leftStart, leftDirection + leftStart, Color.blue);

                    /*
                    if (leftHit.collider.gameObject != null)
                    {
                        if (leftHit.collider.gameObject.transform.parent != null)
                        {
                            if (Physics.Raycast(leftStart, leftDirection, out leftHit))
                            {
                                //Debug.Log("Selected " + leftHit.collider.gameObject.name);

                                if (leftHit.collider.gameObject.transform.parent.gameObject.tag == "drawPlane")
                                {
                                    //Debug.Log("Selected drawPlane with raycast");
                                    selectedObj = leftHit.collider.gameObject.transform.parent.gameObject;
                                }
                            }
                        }
                    }
                    */
                }
                else if (bruhshScript.getToolSelect() != 9 && bruhshScript.getSelectedDrawPlane() != null)
                {
                    OnLeftGripHoldDrawPlane(bruhshScript.getSelectedDrawPlane()); //KeyboardScript.getSelectedDesktop()
                }
            }
            else
            {
                leftGripWasPressed = false;
            }

            //FactualPlane.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>().onHoverEnter?.Invoke(this, EventArgs.Empty);

            //bool isRightInteractorRayHovering = RRayInt.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget);
            //bool isLeftInteractorRayHovering = RRayInt.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget);

            //idk make this select things
            //UnityEngine.Debug.Log("List of: " + RRayInt.GetValidTargets()); it's all protected
            //try !LeftRayUI.GetComponent<XRRayInteractor>().TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget)
            /*
            if (!rightWasPressed && !leftWasPressed)
            {
                if (RightHandScript.getPressedTrigger() && !RRayInt.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget) && !RightRayUI.GetComponent<XRRayInteractor>().TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget))
                {
                    pos2 = Vector3.zero;
                    pos1 = GameObject.Find("RightHand").transform.position;
                    rightWasPressed = true;
                }
                else if (LeftHandScript.getPressedTrigger() && !LRayInt.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget) && !LeftRayUI.GetComponent<XRRayInteractor>().TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget))
                {
                    pos2 = Vector3.zero;
                    pos1 = GameObject.Find("LeftHand").transform.position;
                    leftWasPressed = true;
                }
                else if (pos2 == Vector3.zero)
                {
                    pos1 = Vector3.zero;
                }
            }
            else if (rightWasPressed || leftWasPressed)
            {
                if (!RightHandScript.getPressedTrigger() && rightWasPressed)
                {
                    createFactualPlane(pos1, "RightHand");
                    rightWasPressed = false;
                    
                    pos2 = GameObject.Find("RightHand").transform.position;
                    FactualPlane = new GameObject("FactualPlane");
                    FactualPlane.AddComponent<planeCreation>();
                    FactualPlane.tag = "drawPlane";
                    FactualPlane.layer = 8;
                    planeMaker = FactualPlane.GetComponent<planeCreation>();
                    allPos = planeMaker.recalcPos(pos1, pos2);
                    pos1 = allPos[0];
                    pos2 = allPos[1];
                    Texture2D pixelTexArea = bruhshScript.newTex(Vector3.Distance(pos1, new Vector3(pos2.x, pos1.y, pos2.z)), Vector3.Distance(pos1, new Vector3(pos1.x, pos2.y, pos1.z)));
                    planeMaker.CreateSpriteWPos(pos1, pos2, pixelTexArea, FactualPlane);
                    GameObject testTemp = GameObject.Find("Cube");
                    MeshRenderer testTempRend = gameObject.GetComponent<MeshRenderer>();
                    FactualPlane.AddComponent<BoxCollider>();
                    FactualPlane.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, 0.2f);
                    FactualPlane.GetComponent<BoxCollider>().isTrigger = true;
                    FactualPlane.AddComponent<XRGrabInteractable>();
                    FactualPlane.GetComponent<Rigidbody>().useGravity = false;
                    FactualPlane.GetComponent<XRGrabInteractable>().throwOnDetach = false;
                    
                }
                else if (!LeftHandScript.getPressedTrigger() && leftWasPressed)
                {
                    createFactualPlane(pos1, "LeftHand");
                    leftWasPressed = false;
                    
                    pos2 = GameObject.Find("LeftHand").transform.position;
                    leftWasPressed = false;
                    FactualPlane = new GameObject("FactualPlane");
                    FactualPlane.AddComponent<planeCreation>();
                    FactualPlane.tag = "drawPlane";
                    FactualPlane.layer = 8;
                    planeMaker = FactualPlane.GetComponent<planeCreation>();
                    allPos = planeMaker.recalcPos(pos1, pos2);
                    //UnityEngine.Debug.Log("Pos1 and Pos2 before are " + pos1.ToString() + "   " + pos2.ToString());
                    pos1 = allPos[0];
                    pos2 = allPos[1];
                    //UnityEngine.Debug.Log("Pos1 and Pos2 after are " + pos1.ToString() + "   " + pos2.ToString());
                    //UnityEngine.Debug.Log("Ratio is in reality " + Vector3.Distance(pos1, new Vector3(pos2.x, pos1.y, pos2.z))/16 + "x" + Vector3.Distance(pos1, new Vector3(pos1.x, pos2.y, pos1.z)) / 9);
                    Texture2D pixelTexArea = bruhshScript.newTex(Vector3.Distance(pos1, new Vector3(pos2.x, pos1.y, pos2.z)), Vector3.Distance(pos1, new Vector3(pos1.x, pos2.y, pos1.z)));
                    //UnityEngine.Debug.Log("pixelTexArea should be created with " + pixelTexArea.width + " pixels and also " + pixelTexArea.height);
                    //UnityEngine.Debug.Log("pixelTexArea has a ratio of " + pixelTexArea.width/16 + "x" + pixelTexArea.height/9);
                    planeMaker.CreateSpriteWPos(pos1, pos2, pixelTexArea, FactualPlane);
                    //FactualPlane.AddComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
                    //FactualPlane.GetComponent<UnityEngine.Rigidbody>().useGravity = false;
                    GameObject testTemp = GameObject.Find("Cube");
                    MeshRenderer testTempRend = gameObject.GetComponent<MeshRenderer>();
                    //FactualPlane.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>().onHoverEnter.AddListener(setMaterial_onHoverEnter());
                    //FactualPlane.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>().onHoverEnter = (testTempRend.material = (Material)Resources.Load("DryWall_Mat"));
                    //FactualPlane.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>().onHoverEnter += setMaterial();
                    //FactualPlane.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>().OnHoverEnter() = (testTempRend.material = (Material)Resources.Load("DryWall_Mat"));
                    //FactualPlane.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>().OnHoverExit() = (testTempRend.material = (Material)Resources.Load("DryWallPainted_Mat"));
                    FactualPlane.AddComponent<BoxCollider>();
                    FactualPlane.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, 0.2f);
                    FactualPlane.GetComponent<BoxCollider>().isTrigger = true;
                    FactualPlane.AddComponent<XRGrabInteractable>();
                    FactualPlane.GetComponent<Rigidbody>().useGravity = false;
                    FactualPlane.GetComponent<XRGrabInteractable>().throwOnDetach = false;
                    
                }
            }
            */
            //UnityEngine.Debug.Log("getPressedGrip got called in the script with left grip being " + LeftHandScript.getPressedGrip());
            //UnityEngine.Debug.Log("getPressedGrip got called in the script with right grip being " + RightHandScript.getPressedGrip());
            /*
            if (LeftHandScript.getPressedGrip())
            {
                //UnityEngine.Debug.Log("LeftHand grip active in mode 1");
                collideObj = leftSpriteGrabber.getCollidingObject(LeftHand);
                if (collideObj != null)
                {
                    //UnityEngine.Debug.Log("collideObj is titled " + collideObj.name);
                    collideObj.transform.position = LeftHand.transform.position;
                    //collideObj.transform.eulerAngles = new Vector3(LeftHand.transform.eulerAngles.x, LeftHand.transform.eulerAngles.y, LeftHand.transform.eulerAngles.z);
                    collideObj.transform.rotation = RightHand.transform.rotation;
                    collideObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
                    collideObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
                    collideObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
                    collideObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX;
                    collideObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
                    collideObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ;
                    // rigidbody.constraints = RigidbodyConstraints.None;   to unfreeze
                }
                else if (collideObj == null)
                {
                    UnityEngine.Debug.Log("collideObj is null");
                }
            }
            else if (RightHandScript.getPressedGrip())
            {
                //UnityEngine.Debug.Log("RightHand grip active in mode 1");
                collideObj = rightSpriteGrabber.getCollidingObject(RightHand);
                if (collideObj != null)
                {
                    //UnityEngine.Debug.Log("collideObj is titled " + collideObj.name);
                    collideObj.transform.position = RightHand.transform.position;
                    //collideObj.transform.eulerAngles = new Vector3(RightHand.transform.eulerAngles.x, RightHand.transform.eulerAngles.y, RightHand.transform.eulerAngles.z);
                    collideObj.transform.rotation = RightHand.transform.rotation;
                    collideObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
                    collideObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
                    collideObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
                    collideObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX;
                    collideObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
                    collideObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ;
                    // rigidbody.constraints = RigidbodyConstraints.None;   to unfreeze
                }
                else if (collideObj == null)
                {
                    UnityEngine.Debug.Log("collideObj is null");
                }
            }
            */
        }
        else
        {
            //LeftDrawPlaneRay.SetActive(false);
            //RightDrawPlaneRay.SetActive(false);
            RightRayUI.GetComponent<XRInteractorLineVisual>().invalidColorGradient = gradientB;
            LeftRayUI.GetComponent<XRInteractorLineVisual>().invalidColorGradient = gradientB;
        }

        if (isInRectTool == 3 && !KeyboardScript.getKeyboardsActive())
        {
            LeftHandScript.transform.GetChild(0).gameObject.SetActive(false);
            RightHandScript.transform.GetChild(0).gameObject.SetActive(false);

            if (KeyboardScript.getKeyboardsActive())
            {
                LeftDesktopSelectorRay.SetActive(false);
                RightDesktopSelectorRay.SetActive(false);
            }

            if (leftWasHeld && !LeftHandScript.getPressedTouchpad())
            {
                LSelectionOverlay.SetActive(true);
                RSelectionOverlay.SetActive(false);

                var closestSelectOverlay = new Vector2(0f, 0f);
                int tempSelectOverlayPos = 0;
                foreach (Vector2 selectOverlayPos in selectOverlayPosList)
                {
                    if (Vector2.Distance(selectOverlayPos, left2DAxis) < Vector2.Distance(closestSelectOverlay, left2DAxis))
                    {
                        closestSelectOverlay = selectOverlayPos;
                        selectOverlay = tempSelectOverlayPos;
                    }
                    tempSelectOverlayPos += 1;
                }
            }
            else if (rightWasHeld && !RightHandScript.getPressedTouchpad())
            {
                RSelectionOverlay.SetActive(true);
                LSelectionOverlay.SetActive(false);

                var closestSelectOverlay = new Vector2(0f, 0f);
                int tempSelectOverlayPos = 0;
                foreach (Vector2 selectOverlayPos in selectOverlayPosList)
                {
                    if (Vector2.Distance(selectOverlayPos, right2DAxis) < Vector2.Distance(closestSelectOverlay, right2DAxis))
                    {
                        closestSelectOverlay = selectOverlayPos;
                        selectOverlay = tempSelectOverlayPos;
                    }
                    tempSelectOverlayPos += 1;
                }
            }

            if (!rightWasPressed && !leftWasPressed)
            {
                if (RightHandScript.getPressedTrigger())
                {
                    rightWasPressed = true;
                }
                if (LeftHandScript.getPressedTrigger())
                {
                    leftWasPressed = true;
                }
            }
            if (rightWasPressed && !leftWasPressed && !RightHandScript.getPressedTrigger())
            {
                LeftDesktopSelectorRay.SetActive(false);
                RightDesktopSelectorRay.SetActive(false);

                switch (selectOverlay)
                {
                    case 0:
                        //GameObject Desktop = GameObject.Find("TestCube4WindowTest Window"); //put in whatever the name
                        GameObject clone = Instantiate(uwc_window_Prefab);
                        clone.transform.position = GameObject.Find("RightHand").transform.position;
                        clone.transform.rotation = GameObject.Find("RightHand").transform.rotation;
                        clone.layer = 15;
                        break;
                    case 1:
                        RightDesktopSelectorRay.SetActive(true);
                        LeftRayUI.SetActive(false);
                        RightRayUI.SetActive(false);
                        //SelectNewDesktop ist eine eingebaute Funktion in Keyboard, also bleibt das hier leer
                        break;
                    case 7:
                        isInRectTool = 0;
                        LSelectionOverlay.SetActive(false);
                        RSelectionOverlay.SetActive(false);
                        LeftRayUI.SetActive(true);
                        RightRayUI.SetActive(true);
                        LeftHandScript.transform.GetChild(0).gameObject.SetActive(true);
                        RightHandScript.transform.GetChild(0).gameObject.SetActive(true);
                        break;
                    default:
                        UnityEngine.Debug.Log("Right Trigger was pressed in RectMode 3 but had no selectOverlay");
                        break;
                }
                rightWasPressed = false;
            }
            if (leftWasPressed && !rightWasPressed && !LeftHandScript.getPressedTrigger())
            {
                LeftDesktopSelectorRay.SetActive(false);
                RightDesktopSelectorRay.SetActive(false);
                switch (selectOverlay)
                {
                    case 0:
                        //GameObject Desktop = GameObject.Find("TestCube4WindowTest Window"); //put in whatever the name
                        GameObject clone = Instantiate(uwc_window_Prefab);
                        clone.transform.position = GameObject.Find("LeftHand").transform.position;
                        clone.transform.rotation = GameObject.Find("LeftHand").transform.rotation;
                        clone.layer = 15;
                        break;
                    case 1:
                        LeftDesktopSelectorRay.SetActive(true);
                        LeftRayUI.SetActive(false);
                        RightRayUI.SetActive(false);
                        //SelectNewDesktop ist eine eingebaute Funktion in Keyboard, also bleibt das hier leer
                        break;
                    case 7:
                        isInRectTool = 0;
                        LSelectionOverlay.SetActive(false);
                        RSelectionOverlay.SetActive(false);
                        LeftHandScript.transform.GetChild(0).gameObject.SetActive(true);
                        RightHandScript.transform.GetChild(0).gameObject.SetActive(true);
                        break;
                    default:
                        UnityEngine.Debug.Log("Right Trigger was pressed in RectMode 3 but had no selectOverlay");
                        break;
                }
                leftWasPressed = false;
            }

            if (LeftHandScript.getPressedGrip())
            {
                OnLeftGripHoldUwc(KeyboardScript.getSelectedDesktop());
            }
            if (RightHandScript.getPressedGrip())
            {
                OnRightGripHoldUwc(KeyboardScript.getSelectedDesktop());
            }
        }

        rightWasHeld = RightHandScript.getPressedTouchpad();
        leftWasHeld = LeftHandScript.getPressedTouchpad();
        leftTriggerWasHeld = LeftHandScript.getPressedTrigger();
        rightTriggerWasHeld = RightHandScript.getPressedTrigger();
        rightTouchpadLast = right2DAxis;
        leftTouchpadLast = left2DAxis;
    }

    public GameObject createFactualPlane(Vector3 pos1t, string Hand)
    {
        Vector3 pos2t = GameObject.Find(Hand).transform.position;
        FactualPlane = new GameObject("FactualPlane");
        FactualPlane.AddComponent<planeCreation>();
        FactualPlane.AddComponent<DrawPlane>();
        FactualPlane.tag = "drawPlane";
        FactualPlane.layer = 10;
        planeMaker = FactualPlane.GetComponent<planeCreation>();
        allPos = planeMaker.recalcPos(pos1t, pos2t);
        //pos1t = allPos[0];
        //pos2t = allPos[1];
        pos1t = (allPos[0] - allPos[1]) / 2;
        pos2t = (allPos[1] - allPos[0]) / 2;
        //UnityEngine.Debug.Log("allAllPos.Count = " + allAllPos.Count);
        allAllPos.Add(allPos);
        Texture2D pixelTexArea = bruhshScript.newTex(Vector3.Distance(new Vector3(allPos[0].x, allPos[0].y, allPos[0].z), new Vector3(allPos[1].x, allPos[0].y, allPos[1].z)), Vector3.Distance(new Vector3(allPos[0].x, allPos[0].y, allPos[0].z), new Vector3(allPos[0].x, allPos[1].y, allPos[0].z)), Color.white);
        //Texture2D pixelTexArea = bruhshScript.newTex(Vector3.Distance(pos1t, new Vector3(pos2t.x, pos1t.y, pos2t.z)), Vector3.Distance(pos1t, new Vector3(pos1t.x, pos2t.y, pos1t.z)));
        planeMaker.CreateSpriteWPos(pos1t, pos2t, pixelTexArea, FactualPlane);
        //GameObject testTemp = GameObject.Find("Cube");
        //MeshRenderer testTempRend = gameObject.GetComponent<MeshRenderer>();
        //FactualPlane.AddComponent<BoxCollider>();
        //FactualPlane.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, 0.2f);
        //FactualPlane.GetComponent<BoxCollider>().size = new Vector3(2 * (new Vector3(pos1t.x, 0f, pos1t.z)).magnitude, 2 * pos1t.y, 0.1f);
        //FactualPlane.GetComponent<BoxCollider>().isTrigger = true;
        //FactualPlane.AddComponent<XRGrabInteractable>();
        //FactualPlane.GetComponent<Rigidbody>().useGravity = false;
        //FactualPlane.GetComponent<XRGrabInteractable>().throwOnDetach = false;
        FactualPlane.AddComponent<ValueScript>();
        FactualPlane.GetComponent<ValueScript>().setIntValue(allAllPos.Count - 1);
        //GameObject XRGrabInteractable_Child = Instantiate(XRGrabInteractable_Child_Prefab);
        //XRGrabInteractable_Child.transform.position = new Vector3(0f, 0f, 0f);
        //XRGrabInteractable_Child.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        //XRGrabInteractable_Child.transform.parent = FactualPlane.transform;
        //FactualPlane.transform.position = allPos[0] + (allPos[1] - allPos[0]) / 2;
        FactualPlane.transform.position = GameObject.Find(Hand).transform.position - pos1;
        //XRGrabInteractable_Child.GetComponent<BoxCollider>().size = new Vector3(2 * (new Vector3(pos1t.x, 0f, pos1t.z)).magnitude, 2 * pos1t.y, 0.1f);
        FactualPlane.AddComponent<BoxCollider>();
        FactualPlane.GetComponent<BoxCollider>().size = new Vector3(2 * (new Vector3(pos1t.x, 0f, pos1t.z)).magnitude, 2 * pos1t.y, 0.1f);
        bruhshScript.changeSelPlane(allAllPos.Count - 1);

        List<List<List<Color>>> layerListList = new List<List<List<Color>>>();

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
        layerListList.Add(tempList1);

        List<List<Color>> subLayerList = layerListList[0];
        List<Color> subSubLayerList = layerListList[0][0];

        //layerList = new float[layerListList.Count][][][]; //[subLayerList.Count][subSubLayerList.Count][4]
        float[,,,] layerList = new float[layerListList.Count, subLayerList.Count, subSubLayerList.Count, 4]; //[subLayerList.Count][subSubLayerList.Count][4]

        for (int i = 0; i < layerListList.Count; i += 1)
        {
            for (int j = 0; j < layerListList[i].Count; j += 1)
            {
                for (int k = 0; k < layerListList[i][j].Count; k += 1)
                {
                    layerList[i, j, k, 0] = layerListList[i][j][k].r;
                    layerList[i, j, k, 1] = layerListList[i][j][k].g;
                    layerList[i, j, k, 2] = layerListList[i][j][k].b;
                    layerList[i, j, k, 3] = layerListList[i][j][k].a;
                }
            }
        }

        FactualPlane.GetComponent<ValueScript>().setFourDFloatArrayValue(layerList);
        bruhshScript.createNewLayerList();
        bruhshScript.selectLayer(1);

        bruhshScript.selectEverything();
        UnityEngine.Debug.Log("Created a FactualPlane through createFactualPlane"); //this here needs disabling if Ray

        return FactualPlane;
    }

    public UnityEngine.Object loadFactualPlane(Vector3 pos1t, Vector3 pos2t, string path, int identifier, Vector3 middlePoint, float[,,,] layerList)
    {
        FactualPlane = new GameObject("FactualPlane");
        FactualPlane.AddComponent<planeCreation>();
        FactualPlane.AddComponent<DrawPlane>();
        FactualPlane.tag = "drawPlane";
        FactualPlane.layer = 10;
        planeMaker = FactualPlane.GetComponent<planeCreation>();
        allPos = planeMaker.recalcPos(pos1t, pos2t);
        //pos1t = allPos[0];
        //pos2t = allPos[1];
        pos1t = (allPos[0] - allPos[1]) / 2;
        pos2t = (allPos[1] - allPos[0]) / 2;
        var rawData = System.IO.File.ReadAllBytes(path);
        Texture2D pixelTexArea = new Texture2D(2, 2);
        pixelTexArea.LoadImage(rawData);
        planeMaker.CreateSpriteWPos(pos1t, pos2t, pixelTexArea, FactualPlane);
        //GameObject testTemp = GameObject.Find("Cube");
        //MeshRenderer testTempRend = gameObject.GetComponent<MeshRenderer>();
        
        //FactualPlane.AddComponent<BoxCollider>();
        //FactualPlane.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, 0.2f);
        //FactualPlane.GetComponent<BoxCollider>().size = new Vector3(2 * (new Vector3(pos1t.x, 0f, pos1t.z)).magnitude, 2 * pos1t.y, 0.1f);
        //FactualPlane.GetComponent<BoxCollider>().isTrigger = true;
        FactualPlane.AddComponent<ValueScript>();
        FactualPlane.GetComponent<ValueScript>().setIntValue(identifier);
        //CopyComponent(KeyboardScript.getSelectedDesktop().GetComponent<XRGrabInteractable>(), FactualPlane);
        //GameObject XRGrabInteractable_Child = Instantiate(XRGrabInteractable_Child_Prefab);
        //XRGrabInteractable_Child.transform.position = new Vector3(0f, 0f, 0f);
        //XRGrabInteractable_Child.transform.rotation = Quaternion.identity;
        //XRGrabInteractable_Child.transform.parent = FactualPlane.transform;
        //FactualPlane.transform.position = allPos[0] + (allPos[1] - allPos[0]) / 2;
        FactualPlane.transform.position = middlePoint;
        //XRGrabInteractable_Child.GetComponent<BoxCollider>().size = new Vector3(2 * (new Vector3(pos1t.x, 0f, pos1t.z)).magnitude, 2 * pos1t.y, 0.1f);
        FactualPlane.AddComponent<BoxCollider>();
        FactualPlane.GetComponent<BoxCollider>().size = new Vector3(2 * (new Vector3(pos1t.x, 0f, pos1t.z)).magnitude, 2 * pos1t.y, 0.1f);

        FactualPlane.GetComponent<ValueScript>().setFourDFloatArrayValue(layerList);

        UnityEngine.Debug.Log("Created a FactualPlane through loadFactualPlane");

        return FactualPlane;
    }

    public void cropFactualPlane(Vector3 pos1t, Vector3 pos2t, Vector3 pos3t, Vector3 pos4t, Texture2D tempPixelTexArea, DrawPlane factualPlaneSelf, Vector3 middlePoint, float[,,,] layerList)
    {
        int identifier = factualPlaneSelf.GetComponent<ValueScript>().getIntValue();

        UnityEngine.Debug.Log("Created a FactualPlane through cropFactualPlane");

        FactualPlane = new GameObject("FactualPlane");
        FactualPlane.AddComponent<planeCreation>();
        FactualPlane.AddComponent<DrawPlane>();
        FactualPlane.tag = "drawPlane";
        FactualPlane.layer = 10;
        planeMaker = FactualPlane.GetComponent<planeCreation>();
        //allPos = planeMaker.recalcPos(pos1t, pos2t);
        allPos = new Vector3[4] { pos1t, pos2t, pos3t, pos4t };
        pos1t = (allPos[0] - allPos[1]) / 2;
        pos2t = (allPos[1] - allPos[0]) / 2;
        planeMaker.CreateSpriteWPos(pos1t, pos2t, tempPixelTexArea, FactualPlane);
        FactualPlane.AddComponent<ValueScript>();
        FactualPlane.GetComponent<ValueScript>().setIntValue(identifier);
        FactualPlane.transform.position = middlePoint;
        FactualPlane.AddComponent<BoxCollider>();
        FactualPlane.GetComponent<BoxCollider>().size = new Vector3(2 * (new Vector3(pos1t.x, 0f, pos1t.z)).magnitude, 2 * pos1t.y, 0.1f);

        FactualPlane.GetComponent<ValueScript>().setFourDFloatArrayValue(layerList);

        GameObject[] listOfChildren = new GameObject[factualPlaneSelf.transform.GetChild(0).childCount];
        GameObject cropParent = factualPlaneSelf.transform.GetChild(0).gameObject;

        for (int i = 0; i < factualPlaneSelf.transform.GetChild(0).childCount; i++)
        {
            listOfChildren[i] = factualPlaneSelf.transform.GetChild(0).GetChild(i).gameObject;
        }

        factualPlaneSelf.transform.DetachChildren();
        //Destroy(factualPlaneSelf);
        //DestroyImmediate(factualPlaneSelf);

        cropParent.transform.SetParent(FactualPlane.transform);

        for (int j = 0; j < listOfChildren.Length; j++)
        {
            if (listOfChildren[j] != null)
            {
                listOfChildren[j].transform.SetParent(cropParent.transform);
            }
        }
    }

    //object sender, EventArgs e
    private void setMaterial_onHoverEnter() {
        //GameObject testTemp = GameObject.Find("Cube");
        //MeshRenderer testTempRend = gameObject.GetComponent<MeshRenderer>();
        //testTempRend.material = (Material)Resources.Load("DryWall_Mat");
        UnityEngine.Debug.Log("Triggered the event!");
    }

    /*
    public UnityEngine.Component CopyComponent(UnityEngine.Component original, GameObject destination)
    {
        System.Type type = original.GetType();
        UnityEngine.Component copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }
    */

    void OnLeftGripHoldDrawPlane(DrawPlane selectedObj) //GameObject collidingObj
    {
        if (selectedObj != null && bruhshScript.getToolSelect() != 2)
        {
            //Debug.Log("We hit " + selectedObj.name);

            if (!leftGripWasPressed)
            {
                leftGripWasPressed = true;
                initialLeftHandPosition = LeftHand.transform.position;
                initialLeftHandRotation = LeftHand.transform.eulerAngles;
            }

            leftGripWasPressed = true;

            //selectedObj.transform.position += LeftHand.transform.position - initialLeftHandPosition;
            //selectedObj.transform.eulerAngles += LeftHand.transform.eulerAngles - initialLeftHandRotation;

            selectedObj.transform.position = LeftHand.transform.position - LeftHand.transform.forward;
            //selectedObj.transform.eulerAngles += LeftHand.transform.eulerAngles;
            selectedObj.transform.eulerAngles = new Vector3(-LeftHand.transform.eulerAngles.x, LeftHand.transform.eulerAngles.y + 180f, 0f);
        }
    }

    void OnRightGripHoldDrawPlane(DrawPlane selectedObj) //GameObject collidingObj
    {
        if (selectedObj != null && bruhshScript.getToolSelect() != 2)
        {
            //Debug.Log("We hit " + selectedObj.name);

            if (!rightGripWasPressed)
            {
                rightGripWasPressed = true;
                initialRightHandPosition = RightHand.transform.position;
                initialRightHandRotation = RightHand.transform.eulerAngles;
            }

            rightGripWasPressed = true;

            //selectedObj.transform.position += RightHand.transform.position - initialRightHandPosition;
            //selectedObj.transform.eulerAngles += RightHand.transform.eulerAngles - initialRightHandRotation;

            selectedObj.transform.position = RightHand.transform.position - RightHand.transform.forward;
            //selectedObj.transform.eulerAngles = RightHand.transform.eulerAngles;
            selectedObj.transform.eulerAngles = new Vector3(-RightHand.transform.eulerAngles.x, RightHand.transform.eulerAngles.y + 180f, 0f);
        }
    }

    void OnLeftGripHoldUwc(GameObject selectedObj) //GameObject collidingObj
    {
        if (selectedObj != null && bruhshScript.getToolSelect() == 1)
        {
            //Debug.Log("We hit " + selectedObj.name);

            if (!leftGripWasPressed)
            {
                leftGripWasPressed = true;
                initialLeftHandPosition = LeftHand.transform.position;
                initialLeftHandRotation = LeftHand.transform.eulerAngles;
            }

            leftGripWasPressed = true;

            //selectedObj.transform.position += LeftHand.transform.position - initialLeftHandPosition;
            //selectedObj.transform.eulerAngles += LeftHand.transform.eulerAngles - initialLeftHandRotation;

            selectedObj.transform.position = LeftHand.transform.position - LeftHand.transform.forward;
            //selectedObj.transform.eulerAngles += LeftHand.transform.eulerAngles;
            selectedObj.transform.eulerAngles = new Vector3(-LeftHand.transform.eulerAngles.x, LeftHand.transform.eulerAngles.y + 180f, 0f);
        }
    }

    void OnRightGripHoldUwc(GameObject selectedObj) //GameObject collidingObj
    {
        if (selectedObj != null)
        {
            //Debug.Log("We hit " + selectedObj.name);

            if (!rightGripWasPressed)
            {
                rightGripWasPressed = true;
                initialRightHandPosition = RightHand.transform.position;
                initialRightHandRotation = RightHand.transform.eulerAngles;
            }

            rightGripWasPressed = true;

            //selectedObj.transform.position += RightHand.transform.position - initialRightHandPosition;
            //selectedObj.transform.eulerAngles += RightHand.transform.eulerAngles - initialRightHandRotation;

            selectedObj.transform.position = RightHand.transform.position - RightHand.transform.forward;
            //selectedObj.transform.eulerAngles = RightHand.transform.eulerAngles;
            selectedObj.transform.eulerAngles = new Vector3(-RightHand.transform.eulerAngles.x, RightHand.transform.eulerAngles.y + 180f, 0f);
        }
    }

    public int getMode()
    {
        return isInRectTool;
    }

    public Vector3 getPos1()
    {
        return pos1;
    }

    public Vector3 getPos2()
    {
        return pos2;
    }

    public int getSelectOverlay()
    {
        return selectOverlay;
    }

    public List<List<List<Color>>> getLayerList()
    {
        return bruhshScript.getLayerList();
    }

    public void DeviceDetection()
    {
        //InputSystem.onDeviceChange +=
        //        (device, change) =>
        //        {
        //            switch (change)
        //            {
        //                case InputDeviceChange.Added:
        //                    //if(device == XRInputV1::HTC::OpenVRControllerViveControllerMV:/ OpenVRControllerViveControllerMV)
        //                    break;

        //                case InputDeviceChange.Removed:
        //                    break;
        //                case InputDeviceChange.Disconnected:
        //                    break;
        //            }
        //        };
    }

    public void BlenderStart()
    {
        try
        {
            using (Process myProcess = new Process())
            {
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = "C:\\Program Files\\Blender Foundation\\Blender 3.0\\blender-launcher.exe";
                myProcess.StartInfo.CreateNoWindow = false; //true
                myProcess.Start();

                //needs VR launching
                System.Threading.Thread.Sleep(4000);

                KeyboardScript.FocusProcess("Blender");
                KeyboardScript.DoMouseClick(960, 200);

                System.Threading.Thread.Sleep(2000);

                KeyboardScript.pressKey("N", myProcess);

                System.Threading.Thread.Sleep(1000);

                SetCursorPos(1576, 249);
                System.Threading.Thread.Sleep(500);
                KeyboardScript.DoMouseClick(1576, 249);

                System.Threading.Thread.Sleep(1000);

                SetCursorPos(1471, 140);
                System.Threading.Thread.Sleep(500);
                KeyboardScript.DoMouseClick(1471, 140);

                // save any game data here
                #if UNITY_EDITOR
                    // Application.Quit() does not work in the editor so
                    // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
        }
    }
}
