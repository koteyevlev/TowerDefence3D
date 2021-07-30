using System;
using UnityEngine;

public class Enemy: MonoBehaviour
{
    public EnemyFactory OriginFactory { get; set; }

    public void SpawnOn(GameTile spawnPoint)
    {
        transform.localPosition = spawnPoint.transform.localPosition;
    }

    public bool GameUpdate()
    {
        transform.localPosition += Vector3.forward * Time.deltaTime;
        return true;
    }
}
