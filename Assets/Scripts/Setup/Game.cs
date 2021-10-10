using System;
using System.Collections;
using TowerDefence3d.Scripts.Enemies;
using TowerDefence3d.Scripts.MapObject;
using TowerDefence3d.Scripts.Towers;
using TowerDefence3d.Scripts.UIObjects;
using UnityEngine;

namespace TowerDefence3d.Scripts.Setup
{
    public class Game : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int _boardSize;

        [SerializeField]
        private GameBoard _board;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private CameraMove _cameraMove;

        [SerializeField]
        private WaveVizual _waveVizual;

        [SerializeField]
        private GameTileContentFactory _contentFactory;

        [SerializeField]
        private WarFactory _warFactory;

        [SerializeField]
        private GameScenario _scenario;

        [SerializeField, Range(10, 100)]
        private int _startingPlayerHealth;

        [SerializeField, Range(1f, 2f)]
        private float _prepareTime;
        [SerializeField, Range(1f, 50f)]
        private float _firstCurrency = 10f;

        public static int CurrentHealth => _instance._currentPlayerhealth;
        public Currency Currency => _currency;

        private bool _scenarioInProcess;
        private int _currentPlayerhealth;
        private bool _isPaused;
        private Currency _currency = new Currency();
        private GameScenario.State _activeScenario;

        private TowerType _currentTowerType;

        private GameBehaviourCollection _enemies = new GameBehaviourCollection();
        private GameBehaviourCollection _nonEnemies = new GameBehaviourCollection();

        private Ray TouchRay => _camera.ScreenPointToRay(Input.mousePosition);

        public static Game _instance;

        public event EventHandler EnemyReachedBase;
        public event EventHandler LevelComplete;
        public event EventHandler LevelDefeat;
        public event EventHandler NewGame;

        private void OnEnemyReachedBase(EventArgs e)
        {
            EventHandler handler = EnemyReachedBase;
            handler?.Invoke(this, e);
        }
        private void OnLevelComplete(EventArgs e)
        {
            EventHandler handler = LevelComplete;
            handler?.Invoke(this, e);
        }
        private void OnLevelDefeat(EventArgs e)
        {
            EventHandler handler = LevelDefeat;
            handler?.Invoke(this, e);
        }
        private void OnNewGame(EventArgs e)
        {
            EventHandler handler = NewGame;
            handler?.Invoke(this, e);
        }

        private void OnEnable()
        {
            _instance = this;
            _currentPlayerhealth = _startingPlayerHealth;
        }

        private void Start()
        {
            _board.Initialize(_boardSize, _contentFactory);
            _currency.SetCurrency(_firstCurrency);
            StopGame();
        }
        private void Update()
        {
            if (IsPointerDown())
            {
                HandleTouch();
            }

            if (_scenarioInProcess)
            {
                if (_currentPlayerhealth <= 0)
                {
                    Debug.Log("defeated");
                    OnLevelDefeat(EventArgs.Empty);
                    StopGame();
                }
                if (!_activeScenario.Progress() && _enemies.IsEmpty)
                {
                    Debug.Log("Win");
                    OnLevelComplete(EventArgs.Empty);
                    StopGame();
                    _activeScenario.Progress();
                }
            }

            _waveVizual.GameUpdate(_activeScenario);
            _enemies.GameUpdate();
            Physics.SyncTransforms();
            _board.GameUpdate();
            _nonEnemies.GameUpdate();
        }

        private void HandleTouch()
        {
            GameTile tile = _board.GetTile(TouchRay);
            if (tile != null && tile.Content.Type == GameTileContentType.Tower)
            {
                tile.Content.OnClick();
            }
            else if(tile != null)
            {
                // debug
                _board.ToggleWall(tile);
            }
            else
            {
                _cameraMove.Drag = true;
            }
        }

        private bool IsPointerDown()
        {
            #if UNITY_EDITOR
            return Input.GetMouseButtonDown(0);
            #else
            return Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Began;
            #endif
        }

        public static void SpawnEnemy(EnemyFactory factory, EnemyType type)
        {
            GameTile spawnPoint = _instance._board.GetSpawnPoint(UnityEngine.Random.Range(0, _instance._board.SpownPointsCount));
            Enemy enemy = factory.Get(type);
            enemy.SpawnOn(spawnPoint);
            _instance._enemies.Add(enemy);
        }


        public static Shell SpawnShell()
        {
            Shell shell = _instance._warFactory.Shell;
            _instance._nonEnemies.Add(shell);
            return shell;
        }

        public static Explosion SpawnExplosion()
        {
            Explosion explosion = _instance._warFactory.Explosion;
            _instance._nonEnemies.Add(explosion);
            return explosion;
        }

        public void StopGame()
        {
            _scenarioInProcess = false;
            if (_prepareRoutine != null)
            {
                StopCoroutine(_prepareRoutine);
            }
            _enemies.Clear();
            _nonEnemies.Clear();
            _board.Clear();
            _currentPlayerhealth = _startingPlayerHealth;
            _currency.SetCurrency(_firstCurrency);
            OnNewGame(EventArgs.Empty);
        }

        public void OnStartWaveButtonClicked()
        {
            _prepareRoutine = StartCoroutine(PrepareRoutine());
        }

        public void OnPausedClicked()
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;
        }

        public static void EnemyReachedDestination()
        {
            _instance._currentPlayerhealth--;
            _instance.OnEnemyReachedBase(EventArgs.Empty);
        }

        private Coroutine _prepareRoutine;

        private IEnumerator PrepareRoutine()
        {
            yield return new WaitForSeconds(_prepareTime);
            _activeScenario = _scenario.Begin();
            _scenarioInProcess = true;
        }
    }
}
