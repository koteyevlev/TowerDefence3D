using System;
using UnityEngine;

public class Enemy: MonoBehaviour
{
    public EnemyFactory OriginFactory { get; set; }

    internal void SpawnOn(GameTile spawnPoint)
    {
        transform.localPosition = spawnPoint.transform.localPosition;
    }
}
