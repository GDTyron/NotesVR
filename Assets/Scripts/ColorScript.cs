using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorScript : MonoBehaviour
{
    public float trans;

    void Start()
    {
        trans = 0.5f;

        this.GetComponent<MeshRenderer>().material.color = new Color(this.GetComponent<MeshRenderer>().material.color.r, this.GetComponent<MeshRenderer>().material.color.g, this.GetComponent<MeshRenderer>().material.color.b, trans);

        UnityEngine.Debug.Log("col is now " + this.GetComponent<MeshRenderer>().material.color);
    }
}
