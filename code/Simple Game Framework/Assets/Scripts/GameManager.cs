using System;
using PoolModule;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoSingleton<GameManager>
{
    public IAssetLoad AssetLoad = new ResourceLoader();

    private void Awake()
    {
        UnityObjectPoolFactory.Instance.LoadFuncDelegate = AssetLoad.LoadAsset<UnityEngine.Object>;
    }
}