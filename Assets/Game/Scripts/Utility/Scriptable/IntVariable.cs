using Garawell.Managers;
using UnityEngine;

[CreateAssetMenu(fileName = "Integer", menuName = "Garawell Framework/Create/Variables/Integer")]
public class IntVariable : ScriptableVariable
{
    public string name;
    public int value;

    public override void GetFromRemote()
    {
       // value = MainManager.Instance.RemoteConfigManager.GetIntConfig(name, value);
    }
}