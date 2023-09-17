using UnityEngine;

public class ResourceLoader : IAssetLoad
{
    public T LoadAsset<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }
}