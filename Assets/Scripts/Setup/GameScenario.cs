using UnityEngine;

namespace TowerDefence3d.Scripts.Setup
{

    [CreateAssetMenu]
    public class GameScenario : ScriptableObject
    {
        [SerializeField]
        private EnemyWave[] _waves;
        public State Begin() => new State(this);

        public struct State
        {
            private GameScenario _scenario;
            private int _index;
            private EnemyWave.State _wave;


            public int? WavesCount => _scenario?._waves?.Length ?? 0;
            public int CurrentWave { get; private set; }
            public float WaveProgress { get; private set; }

            public State(GameScenario scenario)
            {
                _scenario = scenario;
                _index = 0;
                _wave = scenario._waves[0].Begin();
                CurrentWave = _index + 1;
                WaveProgress = 0;
            }

            public bool Progress()
            {
                float deltaTime = _wave.Progress(Time.deltaTime, out float progressWave);
                while (deltaTime >= 0f)
                {
                    if (++_index >= _scenario._waves.Length)
                    {
                        return false;
                    }
                    CurrentWave = _index + 1;
                    _wave = _scenario._waves[_index].Begin();
                    deltaTime = _wave.Progress(deltaTime, out progressWave);
                }
                WaveProgress = progressWave;
                // Debug.Log($"progress wave {WaveProgress}, curr wave - {CurrentWave}");
                return true;
            }
        }
    }
}
