using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Vector2Int _boardSize;

    [SerializeField]
    private GameBoard _board;

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private GameTileContentFactory _contentFactory;

    [SerializeField]
    private WarFactory _warFactory;

    [SerializeField]
    private GameScenario _scenario;

    [SerializeField, Range(10, 100)]
    private int _startingPlayerHealth;

    [SerializeField, Range(10f, 50f)]
    private float _prepareTime;

    private bool _scenarioInProcess;
    private int _currentPlayerhealth;
    private bool _isPaused;
    private GameScenario.State _activeScenario;

    private TowerType _currentTowerType;

    private GameBehaviourCollection _enemies = new GameBehaviourCollection();
    private GameBehaviourCollection _nonEnemies = new GameBehaviourCollection();

    private Ray TouchRay => _camera.ScreenPointToRay(Input.mousePosition);

    public static Game _instance;

    private void OnEnable()
    {
        _instance = this;
    }

    private void Start()
    {
        _board.Initialize(_boardSize, _contentFactory);
        BeginNewGame();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            BeginNewGame();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _currentTowerType = TowerType.Laser;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _currentTowerType = TowerType.Mortar;
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            HandleAlternativeTouch();
        }

        if (_scenarioInProcess)
        {
            if (_currentPlayerhealth <= 0)
            {
                Debug.Log("defeated");
                BeginNewGame();
            }
            if (!_activeScenario.Progress() && _enemies.IsEmpty)
            {
                Debug.Log("Win");
                BeginNewGame();
                _activeScenario.Progress();
            }
        }

        _enemies.GameUpdate();
        Physics.SyncTransforms();
        _board.GameUpdate();
        _nonEnemies.GameUpdate();
    }

    public static void SpawnEnemy(EnemyFactory factory, EnemyType type)
    {
        GameTile spawnPoint = _instance._board.GetSpawnPoint(Random.Range(0, _instance._board.SpownPointsCount));
        Enemy enemy = factory.Get(type);
        enemy.SpawnOn(spawnPoint);
        _instance._enemies.Add(enemy);
    }

    private void HandleTouch()
    {
        GameTile tile = _board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _board.ToggleTower(tile, _currentTowerType);
            }
            else
            {
                _board.ToggleWall(tile);
            }
        }
    }

    private void HandleAlternativeTouch()
    {
        GameTile tile = _board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _board.ToggleDestination(tile);
            }
            else
            {
                _board.ToggleSpawnPoint(tile);
            }
        }
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

    public void BeginNewGame()
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
        _prepareRoutine = StartCoroutine(PrepareRoutine());
    }

    public static void EnemyReachedDestination()
    {
        _instance._currentPlayerhealth--;
    }

    private Coroutine _prepareRoutine;

    private IEnumerator PrepareRoutine()
    {
        yield return new WaitForSeconds(_prepareTime);
        _activeScenario = _scenario.Begin();
        _scenarioInProcess = true;
    }
}
