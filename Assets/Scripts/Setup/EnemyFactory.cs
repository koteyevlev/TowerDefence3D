using UnityEngine;
using System;
using TowerDefence3d.Scripts.UIScripts;
using TowerDefence3d.Scripts.Enemies;

namespace TowerDefence3d.Scripts.Setup
{
    [CreateAssetMenu]
    public class EnemyFactory : GameObjectFactory
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
        private EnemyConfig _chomper, _ellen, _golem;

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
                case EnemyType.Chomper:
                    return _chomper;
                case EnemyType.Ellen:
                    return _ellen;
                case EnemyType.Golem:
                    return _golem;
            }
            Debug.LogError($"Cant find { type }");
            return _ellen;
        }
    }
}
