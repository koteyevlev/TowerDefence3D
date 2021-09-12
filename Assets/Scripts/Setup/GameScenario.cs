using UnityEngine;

namespace TowerDefence3d.Scripts.Setup
{

    [CreateAssetMenu]
    public class GameScenario : ScriptableObject
    {
        [SerializeField]
        private EnemyWave[] _waves;

        public int? WavesCount => _waves?.Length ?? 0;
        public int CurrentWave { get; private set; }
        public float WaveProgress { get; private set; }
        public State Begin() => new State(this);

        public struct State
        {
            private GameScenario _scenario;
            private int _index;
            private EnemyWave.State _wave;

            public State(GameScenario scenario)
            {
                _scenario = scenario;
                _index = 0;
                _wave = scenario._waves[0].Begin();
                scenario.CurrentWave = _index + 1;
                scenario.WaveProgress = 0;
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
                    _scenario.CurrentWave = _index + 1;
                    _wave = _scenario._waves[_index].Begin();
                    deltaTime = _wave.Progress(deltaTime, out progressWave);
                }
                _scenario.WaveProgress = progressWave;
                Debug.Log($"progress wave {_scenario.WaveProgress}, curr wave - {_scenario.CurrentWave}");
                return true;
            }
        }
    }
}
