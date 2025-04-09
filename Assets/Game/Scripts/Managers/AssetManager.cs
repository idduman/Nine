using System.Collections.Generic;
using UnityEngine;
using Garawell.Data;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "AssetManager", menuName = "Scriptlable Objects/Asset Manager")]

public class AssetManager : ScriptableObject
{
    [SerializeField] private DictionaryData<Sprite>[] spriteData;
    [SerializeField] private DictionaryData<GameObject>[] prefabData;
    [SerializeField] private DictionaryData<Material>[] materialData;
    [SerializeField] private DictionaryData<Texture2D>[] textureData;
    [SerializeField] private DictionaryData<Color>[] colorData;

    private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    private Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, Material> materials = new Dictionary<string, Material>();
    private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
    private Dictionary<string, Color> colors = new Dictionary<string, Color>();

    public void Initialize()
    {
        //Putting all data into dictionarires to be faster. We lose memory and earn speed
        for (int i = 0; i < spriteData.Length; i++)
        {
            sprites.Add(spriteData[i].key, spriteData[i].value);
        }
        for (int i = 0; i < prefabData.Length; i++)
        {
            gameObjects.Add(prefabData[i].key, prefabData[i].value);
        }
        for (int i = 0; i < materialData.Length; i++)
        {
            materials.Add(materialData[i].key, materialData[i].value);
        }
        for (int i = 0; i < textureData.Length; i++)
        {
            textures.Add(textureData[i].key, textureData[i].value);
        }
        for (int i = 0; i < colorData.Length; i++)
        {
            colors.Add(colorData[i].key, colorData[i].value);
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// Use it only in editor code
    /// </summary>
    public static Sprite GetSpriteEditor(string key)
    {
        AssetManager assetManager = GetMainAsset();
        for (int i = 0; i < assetManager.spriteData.Length; i++)
        {
            if (assetManager.spriteData[i].key == key)
                return assetManager.spriteData[i].value;
        }
        return null;
    }

    /// <summary>
    /// Use it only in editor code
    /// </summary>
    public static GameObject GetPrefabEditor(string key)
    {
        AssetManager assetManager = GetMainAsset();
        for (int i = 0; i < assetManager.prefabData.Length; i++)
        {
            if (assetManager.prefabData[i].key == key)
                return assetManager.prefabData[i].value;
        }
        return null;
    }

    /// <summary>
    /// Use it only in editor code
    /// </summary>
    public static Material GetMaterialEditor(string key)
    {
        AssetManager assetManager = GetMainAsset();
        for (int i = 0; i < assetManager.materialData.Length; i++)
        {
            if (assetManager.materialData[i].key == key)
                return assetManager.materialData[i].value;
        }
        return null;
    }

    /// <summary>
    /// Use it only in editor code
    /// </summary>
    public static Texture2D GetTexture2DEditor(string key)
    {
        AssetManager assetManager = GetMainAsset();
        for (int i = 0; i < assetManager.textureData.Length; i++)
        {
            if (assetManager.textureData[i].key == key)
                return assetManager.textureData[i].value;
        }
        return null;
    }
    

    public static AssetManager GetMainAsset()
    {
        return AssetDatabase.LoadAssetAtPath<AssetManager>("Assets/Game/ScriptableObjects/Managers/AssetManager.asset");
    }
#endif

    public Sprite GetSprite(string key)
    {
        if (sprites.ContainsKey(key))
        {
            return sprites[key];
        }
        return null;
    }

    public GameObject GetPrefab(string key)
    {
        if (gameObjects.ContainsKey(key))
        {
            return gameObjects[key];
        }
        return null;
    }

    public Material GetMaterial(string key)
    {
        if (materials.ContainsKey(key))
        {
            return materials[key];
        }
        return null;
    }

    public Texture2D GetTexture2D(string key)
    {
        if (textures.ContainsKey(key))
        {
            return textures[key];
        }
        return null;
    }

    public Color GetColor(string key)
    {
        if (colors.ContainsKey(key))
        {
            return colors[key];
        }
        return Color.gray;
    }
}
