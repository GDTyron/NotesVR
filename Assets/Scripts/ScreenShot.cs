using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShot : MonoBehaviour
{
    public GameObject ScreenShotCam;
    public GameObject ScreenShotText;

    public RenderTexture ScreenShotTex;

    void Awake()
    {
        ScreenShotCam.GetComponent<Camera>().targetTexture = ScreenShotTex;
    }

    public Texture2D CaptureScreen()
    {
        //yield WaitForEndOfFrame();
        ScreenShotCam.transform.parent.gameObject.SetActive(true);
        var tex = new Texture2D(ScreenShotTex.width, ScreenShotTex.height, TextureFormat.RGB24, false);
        //Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, ScreenShotTex.width, ScreenShotTex.height), 0, 0);
        tex.Apply();

        ScreenShotCam.transform.parent.gameObject.SetActive(false);
        return tex;
    }

    public void changeText(string inputText)
    {
        ScreenShotText.GetComponent<Text>().text = inputText;
    }
}
