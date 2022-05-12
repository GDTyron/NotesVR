using UnityEngine;

public class CollisionScript : MonoBehaviour
{
    HandPresence RightHandScript;
    HandPresence LeftHandScript;

    public GameObject RightHand;
    public GameObject LeftHand;

    public Vector3 initialHandPosition;
    public Vector3 initialHandRotation;
    public bool leftGripWasPressed = false;
    public bool rightGripWasPressed = false;

    void Awake()
    {
        RightHandScript = RightHand.GetComponent<HandPresence>();
        LeftHandScript = LeftHand.GetComponent<HandPresence>();
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        Debug.Log("We hit " + collisionInfo.collider.name);

        if (collisionInfo.collider.name == "NonXRGrab" && LeftHandScript.getPressedGrip())
        {
            if (!leftGripWasPressed)
            {
                leftGripWasPressed = true;
                initialHandPosition = collisionInfo.gameObject.transform.position;
                initialHandRotation = collisionInfo.gameObject.transform.eulerAngles;
            }

            leftGripWasPressed = true;

            transform.position += collisionInfo.gameObject.transform.position - initialHandPosition;
            transform.eulerAngles += collisionInfo.gameObject.transform.eulerAngles - initialHandRotation;
        }

        else if (!LeftHandScript.getPressedGrip())
        {
            leftGripWasPressed = false;
        }

        if (collisionInfo.collider.name == "NonXRGrab" && RightHandScript.getPressedGrip())
        {
            if (!rightGripWasPressed)
            {
                rightGripWasPressed = true;
                initialHandPosition = collisionInfo.gameObject.transform.position;
                initialHandRotation = collisionInfo.gameObject.transform.eulerAngles;
            }

            rightGripWasPressed = true;

            transform.position += collisionInfo.gameObject.transform.position - initialHandPosition;
            transform.eulerAngles += collisionInfo.gameObject.transform.eulerAngles - initialHandRotation;
        }

        else if (!RightHandScript.getPressedGrip())
        {
            rightGripWasPressed = false;
        }
    }
}
