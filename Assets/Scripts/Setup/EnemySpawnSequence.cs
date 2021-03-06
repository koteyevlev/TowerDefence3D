using UnityEngine;
using System;
using TowerDefence3d.Scripts.Enemies;

namespace TowerDefence3d.Scripts.Setup
{
    [Serializable]
    public class EnemySpawnSequence
    {
        [SerializeField]
        private EnemyFactory _factory;
        [SerializeField]
        private EnemyType _type;
        [SerializeField, Range(1, 100)]
        internal int _amount = 10;
        [SerializeField, Range(0.1f, 10f)]
        private float _cooldown = 1f;

        [Serializable]
        public struct State
        {
            private EnemySpawnSequence _sequence;
            private int _count;
            private float _cooldown;
            public State(EnemySpawnSequence sequence)
            {
                _sequence = sequence;
                _count = 0;
                _cooldown = sequence._cooldown;
            }

            public float Progress(float deltaTime, out int spawned)
            {
                _cooldown += deltaTime;
                spawned = 0;
                while (_cooldown >= _sequence?._cooldown)
                {
                    _cooldown -= _sequence._cooldown;
                    if (_count >= _sequence._amount)
                    {
                        return _cooldown;
                    }

                    _count++;
                    Game.SpawnEnemy(_sequence._factory, _sequence._type);
                    spawned += 1;

                }

                return -1f;
            }
        }

        public State Begin() => new State(this);
    }
}
