using UnityEngine;
using System;

[Serializable]
public class EnemySpawnSequence
{
    [SerializeField]
    private EnemyFactory _factory;
    [SerializeField]
    private EnemyType _type;
    [SerializeField, Range(1, 100)]
    private int _amoint = 10;
    [SerializeField, Range(0.1f, 10f)]
    private float _cooldown = 1f;
}

