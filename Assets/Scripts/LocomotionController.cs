using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionController : MonoBehaviour
{
    public XRController leftTeleportRay;
    public XRController rightTeleportRay;
    public InputHelpers.Button teleportActivationButton;
    public float activationThreshold = 0.1f;

    //RectTool
    RectangleTool RectTool;
    HandPresence RightHandScript;
    HandPresence LeftHandScript;
    public GameObject RectToolG;
    [SerializeField]
    public int isInRectTool;

    public GameObject LReticle;
    public GameObject RReticle;
    public GameObject RightHand;
    public GameObject LeftHand;

    public XRRayInteractor leftInteractorRay;
    public XRRayInteractor rightInteractorRay;

    public bool EnableLeftTeleport { get; set; } = true;
    public bool EnableRightTeleport { get; set; } = true;

    void Start() 
    {
        RectTool = RectToolG.GetComponent<RectangleTool>();
        RightHandScript = RightHand.GetComponent<HandPresence>();
        LeftHandScript = LeftHand.GetComponent<HandPresence>();
    }

    void Update()
    {
        //public bool SendHapticImpulse(float amplitude, float duration)    could be cool
        //you can also make an object grab at a certain place if you grab it, by making an empty gameObject the child of that object
        //and then setting it as Attach Transform in the XRGRabInteractor
        isInRectTool = RectTool.isInRectTool;

        Vector3 pos = new Vector3();
        Vector3 norm = new Vector3();
        int index = 0;
        bool validTarget = false;

        if (isInRectTool != 0)
        {
            LReticle.SetActive(false);
            RReticle.SetActive(false);
        }
        else if (isInRectTool == 0 && (LeftHandScript.getPressedTrigger() && !RightHandScript.getPressedTrigger()))
        {
            LReticle.SetActive(true);
            RReticle.SetActive(false);
        }
        else if (isInRectTool == 0 && (!LeftHandScript.getPressedTrigger() && RightHandScript.getPressedTrigger()))
        {
            LReticle.SetActive(false);
            RReticle.SetActive(true);
        }
        else
        {
            LReticle.SetActive(false);
            RReticle.SetActive(false);
        }

        if (leftTeleportRay)
        {
            bool isLeftInteractorRayHovering = leftInteractorRay.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget);
            leftTeleportRay.gameObject.SetActive(isInRectTool == 0 && EnableLeftTeleport && CheckIfActivated(leftTeleportRay) && !isLeftInteractorRayHovering);
        }
        if (rightTeleportRay)
        {
            bool isRightInteractorRayHovering = rightInteractorRay.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget);
            rightTeleportRay.gameObject.SetActive(isInRectTool == 0 && EnableRightTeleport && CheckIfActivated(rightTeleportRay) && !isRightInteractorRayHovering);
        }
    }

    public bool CheckIfActivated(XRController controller) {
        InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActivated, activationThreshold);
        return isActivated;
    }
}
