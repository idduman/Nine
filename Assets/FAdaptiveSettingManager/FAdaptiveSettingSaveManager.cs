using UnityEngine;

public class FAdaptiveSettingSaveManager : MonoBehaviour
{
    private const string UrpChoiceKey = "UrpChoice";
        
    public static int AdaptiveUrpAssetChoice 
    {
        get => PlayerPrefs.GetInt(UrpChoiceKey,-1);

        set => PlayerPrefs.SetInt(UrpChoiceKey, value);
    }
}