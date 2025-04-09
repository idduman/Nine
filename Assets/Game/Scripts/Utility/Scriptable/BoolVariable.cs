using Garawell.Managers;
using UnityEngine;

[CreateAssetMenu(fileName = "Boolean", menuName = "Garawell Framework/Create/Variables/Boolean")]
public class BoolVariable : ScriptableVariable
{
    public bool value;

    public override void GetFromRemote()
    {
    //    value = MainManager.Instance.RemoteConfigManager.GetBoolConfig(name, value);
    }
}
