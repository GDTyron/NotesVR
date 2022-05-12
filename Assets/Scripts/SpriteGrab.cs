using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteGrab : MonoBehaviour
{
    bool isColliding = false;
    GameObject otherCollider;

    void OnCollisionStay(Collision other)
    {
        //UnityEngine.Debug.Log("Object is within collision");
        isColliding = true;
        otherCollider = other.gameObject;
    }

    void OnTriggerEnter(Collider other)
    {
        //UnityEngine.Debug.Log("Object is within trigger");
        isColliding = true;
        otherCollider = other.gameObject;
    }

    public GameObject getCollidingObject(GameObject calledGameObject)
    {
        //UnityEngine.Debug.Log("getCollidingObject is being called");
        if (isColliding && calledGameObject != otherCollider)
        {
            //return this.gameObject;
            return otherCollider.gameObject;
        }
        else
        {
            return null;
        }
    }
}
