using UnityEngine;
using System;

[Serializable]
public class PrefStringData 
{
    public string PrefName;
    public string DefaultValue;    

    private string _val;
    private string val
    {
        get
        {
            return _val;
        }
        set
        {
            _val = value;
            PlayerPrefs.SetString(PrefName, val);
        }
    }



    public void SetString(string _val)
    {
        val = _val;
    }


    public string GetStringValue()
    {
        return val;
    }







}
