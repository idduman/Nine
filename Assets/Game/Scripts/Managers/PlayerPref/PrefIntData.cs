using UnityEngine;
using System;

[Serializable]
public class PrefIntData 
{
    public string PrefName;
    public int DefaultValue;    

    private int _val;
    private int val
    {
        get
        {
            return _val;
        }
        set
        {
            _val = value;
            PlayerPrefs.SetInt(PrefName, val);
        }
    }

    public void SetInt(int _val)
    {
        val = _val;
    }


    public int GetIntValue()
    {
        return val;
    }

}
