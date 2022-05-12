using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueScript : MonoBehaviour
{
    public int IntValue;
    public float FloatValue;
    public string StringValue;
    public double DoubleValue;
    public bool BoolValue;

    public float[,,,] FourDFloatArrayValue;

    public void setIntValue(int value)
    {
        IntValue = value;
        return;
    }

    public int getIntValue()
    {
        return IntValue;
    }

    public void setFloatValue(float value)
    {
        FloatValue = value;
        return;
    }

    public float getFloatValue()
    {
        return FloatValue;
    }

    public void setStringValue(string value)
    {
        StringValue = value;
        return;
    }

    public string getStringValue()
    {
        return StringValue;
    }

    public void setDoubleValue(double value)
    {
        DoubleValue = value;
        return;
    }

    public double getDoubleValue()
    {
        return DoubleValue;
    }

    public void setBoolValue(bool value)
    {
        BoolValue = value;
        return;
    }

    public bool getBoolValue()
    {
        return BoolValue;
    }

    public void setFourDFloatArrayValue(float[,,,] value)
    {
        FourDFloatArrayValue = value;
        return;
    }

    public float[,,,] getFourDFloatArrayValue()
    {
        return FourDFloatArrayValue;
    }
}
