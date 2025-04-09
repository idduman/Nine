using Sirenix.OdinInspector;
using UnityEditor.VersionControl;
using UnityEngine;

[CreateAssetMenu(fileName = "New Package", menuName = "Garawell Framework/Create/Package")]
public class Package : ScriptableObject
{
    public string packageName;
    [FilePath]
    public string path;
    public string[] importedPath;
    public string dependencies;
    public bool imported = false;

    public Package(string name, string path, string[] importedPath, string dependencies)
    {
        this.packageName = name;
        this.path = path;
        this.importedPath = importedPath;
        this.dependencies = dependencies;
    }
}