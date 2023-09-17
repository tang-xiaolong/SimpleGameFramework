public interface IAssetLoad
{
    T LoadAsset<T>(string path) where T : UnityEngine.Object;
}