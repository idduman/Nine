using UnityEditor;

namespace Garawell.Editor
{
    public class DefineSymbolAdder
    {
        public ScriptablePackageImporter packageImprter;
        public static void AddDefineSymbolForAllPlatforms(string defineSymbol)
        {
            string newDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            newDefineSymbols += " " + defineSymbol;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, newDefineSymbols);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, newDefineSymbols);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newDefineSymbols);
        }
    }
}

