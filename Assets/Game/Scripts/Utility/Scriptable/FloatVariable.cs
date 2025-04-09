using Garawell.Managers;
using UnityEngine;

[CreateAssetMenu(fileName = "Float", menuName = "Garawell Framework/Create/Variables/Float")]
public class FloatVariable : ScriptableVariable
{
    public float value;

    public override void GetFromRemote()
    {
       // value = MainManager.Instance.RemoteConfigManager.GetFloatConfig(name, value);
    }
}
