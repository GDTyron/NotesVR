using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    RectangleTool RectTool;
    public GameObject DrawTool;
    public GameObject RightTeleportRay;
    public GameObject LeftTeleportRay;

    public InputDeviceCharacteristics controllerCharacteristics;
    public List<GameObject> controllerPrefabs;
    private InputDevice targetDevice;
    private GameObject spawnedController;
    public bool pressedPrimary;
    public Vector2 touchLocation;
    public bool pressedTouchpad;
    public int currentMode;
    public float pressedTriggerDegree;
    public bool pressedTrigger;
    public bool pressedGrip;
    public bool touchedTouchpad;

    void Start()
    {
        UnityEngine.Debug.Log("Initialized VR testerinos");
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        foreach (var item in devices) {
            //UnityEngine.Debug.Log(item.name + item.characteristics);
        }

        if (devices.Count > 0) {
            targetDevice = devices[0];
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);
            if (prefab)
            {
                spawnedController = Instantiate(prefab, transform);
            }
            else {
                UnityEngine.Debug.Log("Did not find corresponding controller model");
                spawnedController = Instantiate(controllerPrefabs[0], transform);
            }
        }
        RectTool = DrawTool.GetComponent<RectangleTool>();
    }

    void Update()
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue))
        {
            if (primaryButtonValue)
            {
                pressedPrimaryChange(true);
            }
            else { pressedPrimaryChange(false); }
            if (targetDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButtonValue) && triggerButtonValue)
            {
                pressedTriggerChange(true);
            }
            else
            {
                pressedTriggerChange(false);
            }
            if (targetDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gripButtonValue) && gripButtonValue)
            {
                //UnityEngine.Debug.Log("Pressing grip");
                pressedGripChange(true);
            }
            else
            {
                pressedGripChange(false);
            }
            if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.1f)
            {
                pressedTriggerDegreeChange(triggerValue);
            }
            else
            {
                pressedTriggerDegreeChange(0f);
            }
            if(targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool primary2DAxisClickValue) && primary2DAxisClickValue == true)
            {
                pressedTouchpadChange(true);
            }
            else
            {
                pressedTouchpadChange(false);
            }
            if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue))
            {
                primary2DAxisChange(primary2DAxisValue);
            }
            if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool primary2DAxisTouchValue))
            {
                touchedTouchpadChange(true);
            }
            else
            {
                touchedTouchpadChange(false);
            }

            // 0 -Up- Teleport  1 -Left- RectangleTool  2 -Down- unassigned  3 -Right- unassigned
            if (RectTool.getMode() == 0)
            {
                LeftTeleportRay.SetActive(true);
                RightTeleportRay.SetActive(true);
                currentMode = 0;
            }
            else if (RectTool.getMode() == 1)
            {
                LeftTeleportRay.SetActive(false);
                RightTeleportRay.SetActive(false);
                currentMode = 1;
            }
            else if (RectTool.getMode() == 2)
            {
                LeftTeleportRay.SetActive(false);
                RightTeleportRay.SetActive(false);
                currentMode = 2;
            }
            else if (RectTool.getMode() == 3)
            {
                LeftTeleportRay.SetActive(false);
                RightTeleportRay.SetActive(false);
                currentMode = 3;
            }
            //UnityEngine.Debug.Log(currentMode);

        }
    }

    public void pressedPrimaryChange(bool status)
    {
        pressedPrimary = status;
    }

    public void primary2DAxisChange(Vector2 touchLocationXY)
    {
        touchLocation = touchLocationXY;
    }

    public void pressedTouchpadChange(bool status)
    {
        pressedTouchpad = status;
    }

    public void pressedTriggerDegreeChange(float status)
    {
        pressedTriggerDegree = status;
    }

    public void pressedTriggerChange(bool status)
    {
        pressedTrigger = status;
    }

    public void pressedGripChange(bool status)
    {
        pressedGrip = status;
    }

    public void touchedTouchpadChange(bool status)
    {
        touchedTouchpad = status;
    }

    public bool getTouchedTouchpad()
    {
        return touchedTouchpad;
    }

    public bool getPressedPrimary()
    {
        return pressedPrimary;
    }

    public Vector2 getPrimary2DAxis()
    {
        return touchLocation;
    }

    public bool getPressedTouchpad()
    {
        return pressedTouchpad;
    }

    public float getPressedTriggerDegree()
    {
        return pressedTriggerDegree;
    }

    public bool getPressedTrigger()
    {
        return pressedTrigger;
    }

    public bool getPressedGrip()
    {
        //UnityEngine.Debug.Log("getPressedGrip got called in the script with grip being " + pressedGrip);
        return pressedGrip;
    }
}
