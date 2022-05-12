using UnityEngine;

public class RayCollider : MonoBehaviour
{
    HandPresence RightHandScript;
    HandPresence LeftHandScript;
    RectangleTool RectangleToolScript;

    public GameObject RightHand;
    public GameObject LeftHand;
    public GameObject selectedObj;
    public GameObject RectangleTool;
    //public GameObject RightGripRay;
    //public GameObject LeftGripRay;

    public Vector3 initialLeftHandPosition;
    public Vector3 initialLeftHandRotation;
    public Vector3 initialRightHandPosition;
    public Vector3 initialRightHandRotation;
    public bool leftGripWasPressed = false;
    public bool rightGripWasPressed = false;

    public Vector3 leftStart;
    public Vector3 leftDirection;
    public RaycastHit leftHit;
    public Vector3 rightStart;
    public Vector3 rightDirection;
    public RaycastHit rightHit;

    void Awake()
    {
        RightHandScript = RightHand.GetComponent<HandPresence>();
        LeftHandScript = LeftHand.GetComponent<HandPresence>();
        RectangleToolScript = RectangleTool.GetComponent<RectangleTool>();
    }

    /*
    public void onRayGripEnter(GameObject collidingObj)
    {
        Debug.Log("Selected " + collidingObj.name);
        selectedObj = collidingObj;
    }
    */

    void OnLeftGripHold() //GameObject collidingObj
    {
        if (selectedObj != null)
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
            selectedObj.transform.eulerAngles += LeftHand.transform.eulerAngles;
        }
    }

    void OnRightGripHold() //GameObject collidingObj
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
            selectedObj.transform.eulerAngles = RightHand.transform.eulerAngles;
        }
    }

    void Update()
    {
        if (RectangleToolScript.getMode() == 1)
        {
            if (RightHandScript.getPressedGrip())
            {
                if (!rightGripWasPressed)
                {
                    rightGripWasPressed = true;

                    rightStart = RightHand.transform.position;
                    rightDirection = -RightHand.transform.forward;

                    //Debug.DrawLine(rightStart, rightDirection + rightStart, Color.blue);

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
                }
                else
                {
                    OnRightGripHold();
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
                }
                else
                {
                    OnLeftGripHold();
                }
            }
            else
            {
                leftGripWasPressed = false;
            }
        }
    }
}
