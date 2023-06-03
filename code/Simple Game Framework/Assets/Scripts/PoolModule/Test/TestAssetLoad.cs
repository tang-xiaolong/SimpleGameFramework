using UnityEngine;

namespace PoolModule
{
    public static class TestAssetLoad
    {
        public static T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            return Resources.Load<T>(path);
        }
    }
}