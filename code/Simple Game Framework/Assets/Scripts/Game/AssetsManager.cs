#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

/// <summary>
/// 负责资源的加载和释放。
/// 对外提供统一的加载接口，方便后期维护：例如引用计数等等。
/// 根据类型对外提供资源路径获取方法。
/// 内部针对不同的加载方式，做不同的加载实现。
/// </summary>
public class AssetsManager : Singleton<AssetsManager>
{
    public enum AssetLoadType
    {
        Resources,
        AssetDatabase, //编辑器内使用
        AssetBundle
    }

    private AssetLoadType assetLoadType = AssetLoadType.AssetDatabase;

    public AssetLoadType AssetsLoadType
    {
        set => assetLoadType = value;
    }

    private string AssetDataBasePreviousPath = "Assets/Resources/";

    #region Paths

    private const string prefabPath = "prefabs/";
    private const string spritePath = "sprites/";

    public static string GetPrefabPath(string assetPath)
    {
        return prefabPath + assetPath;
    }

    public static string GetSpritePath(string assetPath)
    {
        return spritePath + assetPath;
    }

    #endregion

    /// <summary>
    /// assetName 为Resources目录下的完整相对路径，不带后缀名,后期如果加入了AB加载，同时还需要加入AssetDataBase的支持
    /// </summary>
    /// <param name="assetName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T LoadAsset<T>(string assetName) where T : UnityEngine.Object
    {
        string assetFullPath = GetAssetFullPath<T>(assetName);

        switch (assetLoadType)
        {
            case AssetLoadType.Resources:
            {
                return Resources.Load<T>(assetFullPath);
            }
#if UNITY_EDITOR
            case AssetLoadType.AssetDatabase:
            {
                return AssetDatabase.LoadAssetAtPath<T>(assetFullPath);
            }
#endif
            case AssetLoadType.AssetBundle:
            {
                return null;
            }
            default: break;
        }

        return null;
    }

    /// <summary>
    /// 根据资源名字和资源类型返回完整路径
    /// </summary>
    /// <param name="assetName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private string GetAssetFullPath<T>(string assetName) where T : UnityEngine.Object
    {
        switch (assetLoadType)
        {
            case AssetLoadType.Resources:
                return assetName;
#if UNITY_EDITOR
            case AssetLoadType.AssetDatabase:
                string extensionName = GetAssetExtensionNameByType<T>();
                return AssetDataBasePreviousPath + assetName + extensionName;
#endif
        }

        return assetName;
    }

    /// <summary>
    /// 根据资源类型返回对应的后缀名
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private string GetAssetExtensionNameByType<T>() where T : UnityEngine.Object
    {
        Type t = typeof(T);
        if (t == typeof(GameObject))
        {
            return ".prefab";
        }
        else if (t == typeof(Texture) || t == typeof(Sprite) || t == typeof(Texture2D))
        {
            return ".png";
        }
        else if (t == typeof(Material))
        {
            return ".mat";
        }
        else if (t == typeof(Shader))
        {
            return ".shader";
        }
        else if (t == typeof(TextAsset))
        {
            return ".txt";
        }
        else if (t == typeof(AudioClip))
        {
            return ".wav";
        }
        else
        {
            return ".asset";
        }
    }
}