using System;
using UnityEngine;

namespace TowerDefence3d.Scripts.Setup
{
    [CreateAssetMenu]
    public class EnemyWave : ScriptableObject
    {
        [SerializeField]
        private EnemySpawnSequence[] _spawnSequences;
        public State Begin() => new State(this);

        [Serializable]
        public struct State
        {
            private EnemyWave _wave;
            private int _index;
            private EnemySpawnSequence.State _sequence;
            private readonly int _totalAmount;
            private float _spawned;

            public State(EnemyWave wave)
            {
                _wave = wave;
                _index = 0;
                _sequence = _wave._spawnSequences[0].Begin();
                _totalAmount = 0;
                _spawned = 0;
                foreach (var seq in _wave._spawnSequences)
                {
                    _totalAmount += seq._amount;
                }
            }

            public float Progress(float deltaTime, out float progressWave)
            {
                deltaTime = _sequence.Progress(deltaTime, out int spawned);
                _spawned += spawned;
                progressWave = _spawned / _totalAmount;
                while (deltaTime >= 0)
                {
                    if (++_index >= _wave._spawnSequences.Length)
                    {
                        return deltaTime;
                    }

                    _sequence = _wave._spawnSequences[_index].Begin();
                    deltaTime = _sequence.Progress(deltaTime, out spawned);
                    _spawned += spawned;
                }

                return -1f;
            }
        }

    }
}
