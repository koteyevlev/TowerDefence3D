using UnityEngine;
using System;

[CreateAssetMenu]
public class EnemyFactory: GameObjectFactory
{
    [Serializable]
    class EnemyConfig
    {
        public Enemy Prefab;
        [FloatRangeSlider(0.5f, 2.0f)]
        public FloatRange Scale = new FloatRange(1f);

        [FloatRangeSlider(-0.4f, 0.4f)]
        public FloatRange PathOffset = new FloatRange(0f);

        [FloatRangeSlider(0.2f, 5.0f)]
        public FloatRange Speed = new FloatRange(1f);

        [FloatRangeSlider(10f, 1000f)]
        public FloatRange Health = new FloatRange(100f);
    }
    [SerializeField]
    private EnemyConfig _small, _medium, _large;

    public Enemy Get(EnemyType type)
    {
        var config = GetConfig(type);
        Enemy instance = CreateGameObjectInstance(config.Prefab);
        instance.OriginFactory = this;
        instance.Initialize(config.Scale.RandomValueInRange, 
            config.PathOffset.RandomValueInRange, 
            config.Speed.RandomValueInRange,
            config.Health.RandomValueInRange);
        return instance;
    }

    public void Reclaim(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    private EnemyConfig GetConfig(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Small:
                return _small;
            case EnemyType.Medium:
                return _medium;
            case EnemyType.Large:
                return _large;
        }
        Debug.LogError($"Cant find { type }");
        return _medium;
    }
}
