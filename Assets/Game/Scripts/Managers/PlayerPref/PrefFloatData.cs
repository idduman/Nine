using UnityEngine;
using System;

[Serializable]
public class PrefFloatData 
{    
    public string PrefName;
    public float DefaultValue;
    

    private float _val;
    private float val
    {
        get
        {
            return _val;
        }
        set
        {
            _val = value;
            PlayerPrefs.SetFloat(PrefName, val);
        }
    }

    public void SetFloat(float _val)
    {
        val = _val;
    }

    public float GetFloatValue()
    {
        return val;
    }


}
