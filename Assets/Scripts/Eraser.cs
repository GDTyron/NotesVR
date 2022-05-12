using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : MonoBehaviour
{
    public GameObject BruhshGameObject;

    bruhsh bruhshScript;

    void Start()
    {
        bruhshScript = BruhshGameObject.GetComponent<bruhsh>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(bruhshScript.getGripIsActive())
        {
            Destroy(this);
            UnityEngine.Debug.Log("Should be destroying right about now");
        }
    }
}
