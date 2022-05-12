using UnityEngine;

[System.Serializable]
public class Uwc_windowData
{
    public string TextField { get; set; }
    public float[] position;
    public float[] rotation;
    public float[] scale;
    public string partialWindowTitle;
    public string type;

    public Uwc_windowData(Uwc_window uwc_window)
    {
        TextField = uwc_window.TextField;

        Vector3 uwc_windowPos = uwc_window.transform.position;

        position = new float[]
        {
            uwc_windowPos.x, uwc_windowPos.y, uwc_windowPos.z
        };

        Quaternion uwc_windowRot = uwc_window.transform.rotation;

        rotation = new float[]
        {
            uwc_windowRot.x, uwc_windowRot.y, uwc_windowRot.z, uwc_windowRot.w
        };

        Vector3 uwc_windowScale = uwc_window.transform.localScale;

        scale = new float[]
        {
            uwc_windowScale.x, uwc_windowScale.y, uwc_windowScale.z
        };

        TextField = uwc_window.GetComponentInChildren<UnityEngine.UI.Text>().text;

        partialWindowTitle = uwc_window.GetComponentInChildren<uWindowCapture.UwcWindowTexture>().partialWindowTitle;

        if (uwc_window.GetComponentInChildren<uWindowCapture.UwcWindowTexture>().type == uWindowCapture.WindowTextureType.Window)
        {
            type = "Window";
        }
        else if (uwc_window.GetComponentInChildren<uWindowCapture.UwcWindowTexture>().type == uWindowCapture.WindowTextureType.Desktop)
        {
            type = "Desktop";
        }
        else
        {
            type = "Window";
            UnityEngine.Debug.Log("Didn't recognize UwcWindowTexture.type");
        }
    }
}