using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TubeSpawn : MonoSingleton<TubeSpawn>
{
    public GameObject tube;
    private bool isBegin = false;
    private WaitForSeconds _waitForSeconds;
    private float spawnPositionX;

    
    void ListenMessage()
    {
        MessageManager.Instance.Register(MessageDefine.BEGIN_GAME, BeginSpawnTube);
        MessageManager.Instance.Register(MessageDefine.BIRD_DIE, StopSpawnTube);
    }
    
    public void BeginSpawnTube()
    {
        isBegin = true;
        StartCoroutine(SpawnTube());
    }

    public void StopSpawnTube()
    {
        isBegin = false;
        StopAllCoroutines();
    }

    IEnumerator SpawnTube()
    {
        while (true)
        {
            yield return _waitForSeconds;
            // instantiate tube and set position
            GameObject obj = Instantiate(tube);
            obj.transform.position = new Vector3(spawnPositionX, Random.Range(-0.6f, 0.8f), 0);

        }
    }
    private void Awake()
    {
        ListenMessage();
        _waitForSeconds = new WaitForSeconds(GlobalConfig.SPAWN_TUBE_TIME);
        spawnPositionX = Camera.main.orthographicSize * GlobalConfig.SCREEN_ASPECT + 1;
        // BeginSpawnTube();
    }

    private void Update()
    {
        
    }
}
