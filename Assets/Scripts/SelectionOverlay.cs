using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using uWindowCapture;

public class SelectionOverlay : MonoBehaviour
{
    HandPresence RightHandScript;
    HandPresence LeftHandScript;
    planeCreation planeMaker;
    SpriteGrab leftSpriteGrabber;
    SpriteGrab rightSpriteGrabber;
    RectangleTool RectTool;

    private GameObject planeDraw;
    private GameObject collideObj;
    public GameObject RightHand;
    public GameObject LeftHand;
    public GameObject planeCreator;
    public GameObject SelectorPlane;
    public GameObject LeftHandGrab;
    public GameObject RightHandGrab;
    public GameObject RectToolG;
    public GameObject RightHandG;
    public GameObject LeftHandG;

    public Sprite DrawableSprite;

    [SerializeField]
    private Vector2 right2DAxis;
    [SerializeField]
    private Vector2 left2DAxis;
    [SerializeField]
    Vector2 IdealPointLeft = new Vector2(-1f, 0f);
    [SerializeField]
    Vector2 IdealPointRight = new Vector2(1f, 0f);
    [SerializeField]
    Vector2 IdealPointUp = new Vector2(0f, 1f);
    [SerializeField]
    Vector2 IdealPointDown = new Vector2(0f, -1f);
    [SerializeField]
    Vector2 ZeroPoint = new Vector2(0f, 0f);
    [SerializeField]
    private bool touchpadTouch;
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
    public float leftButtonHoldTime = 0;
    [SerializeField]
    public float rightButtonHoldTime = 0;
    [SerializeField]
    public bool leftWasHeld = false;
    [SerializeField]
    public bool rightWasHeld = false;
    [SerializeField]
    public Vector2 rightTouchpadLast;
    [SerializeField]
    public Vector2 leftTouchpadLast;
    [SerializeField]
    public int selectOverlay;
    [SerializeField]
    public int isInRectTool;

    [SerializeField]
    public Material highlightMaterial;

    void Start()
    {
        RightHandScript = RightHand.GetComponent<HandPresence>();
        LeftHandScript = LeftHand.GetComponent<HandPresence>();
        rightSpriteGrabber = RightHandGrab.GetComponent<SpriteGrab>();
        leftSpriteGrabber = LeftHandGrab.GetComponent<SpriteGrab>();

        RectTool = RectToolG.GetComponent<RectangleTool>();
    }

    void Update()
    {
        selectOverlay = RectTool.selectOverlay;
        isInRectTool = RectTool.isInRectTool;

        if (isInRectTool == 3)
        {
            if (selectOverlay == 0)
            {
                //Ray Selector
                if (RightHandScript.getPressedGrip()) {
                    Vector3 ROrigin = RightHandG.transform.position;
                    Vector3 RDirection = RightHandG.transform.TransformDirection(Vector3.forward);
                    RaycastHit hit;

                    Debug.DrawRay(ROrigin, RDirection);     //for testing where the Ray actually goes
                    if (Physics.Raycast(ROrigin, RDirection, out hit)) //, maxDistance, layerMask
                    {
                        UnityEngine.Debug.Log("Raycast hit something. It's called " + hit.transform.name);
                        var selection = hit.transform;
                        var selectionRenderer = selection.GetComponent<Renderer>();
                        if (selectionRenderer != null)
                        {
                            selectionRenderer.material = highlightMaterial;
                        }
                    }
                }

                if (LeftHandScript.getPressedGrip())
                {
                    Vector3 LOrigin = LeftHandG.transform.position;
                    Vector3 LDirection = LeftHandG.transform.TransformDirection(Vector3.forward);
                    RaycastHit hit;
                    Debug.DrawRay(LOrigin, LDirection);     //for testing where the Ray actually goes
                    if (Physics.Raycast(LOrigin, LDirection, out hit)) //, maxDistance, layerMask
                    {
                        UnityEngine.Debug.Log("Raycast hit something. It's called " + hit.transform.name);
                        var selection = hit.transform;
                        var selectionRenderer = selection.GetComponent<Renderer>();
                        if (selectionRenderer != null)
                        {
                            selectionRenderer.material = highlightMaterial;
                        }
                    }
                }

                //QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
                //var ray = (ROrigin, RDirection, maxDistance, layerMask, queryTriggerInteraction);

                //creating new selectPlanes
                if (!rightWasPressed && !leftWasPressed)
                {
                    if (RightHandScript.getPressedTrigger())
                    {
                        pos2 = Vector3.zero;
                        pos1 = GameObject.Find("RightHand").transform.position;
                        rightWasPressed = true;
                    }
                    else if (LeftHandScript.getPressedTrigger())
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
                        pos2 = GameObject.Find("RightHand").transform.position;
                        rightWasPressed = false;
                        SelectorPlane = new GameObject("SelectorPlane");
                        SelectorPlane.AddComponent<planeCreation>();
                        SelectorPlane.tag = "selectPlane";
                        SelectorPlane.layer = 8;
                        planeMaker = SelectorPlane.GetComponent<planeCreation>();
                        allPos = planeMaker.recalcPos(pos1, pos2);
                        pos1 = allPos[0];
                        pos2 = allPos[1];
                        planeMaker.CreateQuadMeshWOTexture(pos1, pos2, SelectorPlane);
                        GameObject testTemp = GameObject.Find("Cube");
                        MeshRenderer testTempRend = gameObject.GetComponent<MeshRenderer>();
                        SelectorPlane.AddComponent<BoxCollider>();
                        SelectorPlane.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, 0.2f);
                        SelectorPlane.GetComponent<BoxCollider>().isTrigger = true;
                        SelectorPlane.AddComponent<XRGrabInteractable>();
                        SelectorPlane.GetComponent<Rigidbody>().useGravity = false;
                        SelectorPlane.GetComponent<XRGrabInteractable>().throwOnDetach = false;
                        SelectorPlane.AddComponent<UwcWindowTexture>();
                        //SelectorPlane.GetComponent<UwcWindowTexture>().desktopIndex = 1;
                        SelectorPlane.GetComponent<UwcWindowTexture>().type = uWindowCapture.WindowTextureType.Desktop;
                    }
                    else if (!LeftHandScript.getPressedTrigger() && leftWasPressed)
                    {
                        pos2 = GameObject.Find("RightHand").transform.position;
                        rightWasPressed = false;
                        SelectorPlane = new GameObject("SelectorPlane");
                        SelectorPlane.AddComponent<planeCreation>();
                        SelectorPlane.tag = "selectPlane";
                        SelectorPlane.layer = 8;
                        planeMaker = SelectorPlane.GetComponent<planeCreation>();
                        allPos = planeMaker.recalcPos(pos1, pos2);
                        pos1 = allPos[0];
                        pos2 = allPos[1];
                        planeMaker.CreateQuadMeshWOTexture(pos1, pos2, SelectorPlane);
                        GameObject testTemp = GameObject.Find("Cube");
                        MeshRenderer testTempRend = gameObject.GetComponent<MeshRenderer>();
                        SelectorPlane.AddComponent<BoxCollider>();
                        SelectorPlane.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, 0.2f);
                        SelectorPlane.GetComponent<BoxCollider>().isTrigger = true;
                        SelectorPlane.AddComponent<XRGrabInteractable>();
                        SelectorPlane.GetComponent<Rigidbody>().useGravity = false;
                        SelectorPlane.GetComponent<XRGrabInteractable>().throwOnDetach = false;
                        SelectorPlane.AddComponent<UwcWindowTexture>();
                        //SelectorPlane.GetComponent<UwcWindowTexture>().desktopIndex = 1;
                        SelectorPlane.GetComponent<UwcWindowTexture>().type = uWindowCapture.WindowTextureType.Desktop;
                    }
                }
                //UnityEngine.Debug.Log("getPressedGrip got called in the script with left grip being " + LeftHandScript.getPressedGrip());
                //UnityEngine.Debug.Log("getPressedGrip got called in the script with right grip being " + RightHandScript.getPressedGrip());
                if (LeftHandScript.getPressedGrip())
                {
                    //UnityEngine.Debug.Log("LeftHand grip active in mode 1");
                    collideObj = leftSpriteGrabber.getCollidingObject(LeftHand);
                    if (collideObj != null)
                    {
                        //UnityEngine.Debug.Log("collideObj is titled " + collideObj.name);
                        collideObj.transform.position = LeftHand.transform.position;
                        //collideObj.transform.eulerAngles = new Vector3(LeftHand.transform.eulerAngles.x, LeftHand.transform.eulerAngles.y, LeftHand.transform.eulerAngles.z);
                        collideObj.transform.rotation = LeftHand.transform.rotation;
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
                        //UnityEngine.Debug.Log("collideObj is null");
                    }
                }
            }
        }
    }
}
