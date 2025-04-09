using UnityEngine;
using Garawell.Managers;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "EditorGameSettings", menuName = "Scriptlable Objects/EditorGameSettings")]
public class EditorGameSettings : ScriptableObject
{
    [TabGroup("Input Settings")]
    public ControllerType controllerType;
    [TabGroup("Editor Settings")]
    public bool startFromTSPreloadScene;
    [TabGroup("Editor Settings")]
    public bool ActivateDebugger;
    [TabGroup("PanelSettings")]
    public float failPanelAppearTime;
    [TabGroup("PanelSettings")]
    public float successPanelAppearTime;
}