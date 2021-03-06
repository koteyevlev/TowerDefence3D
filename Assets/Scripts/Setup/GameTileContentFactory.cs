using TowerDefence3d.Scripts.MapObject;
using TowerDefence3d.Scripts.Towers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefence3d.Scripts.Setup
{
    [CreateAssetMenu]
    public class GameTileContentFactory : GameObjectFactory
    {
        [SerializeField]
        private GameTileContent _destinationPrefab;
        [SerializeField]
        private GameTileContent _emptyPrefab;
        [SerializeField]
        private GameTileContent _wallPrefab;
        [SerializeField]
        private GameTileContent _spawnPrefab;
        [SerializeField]
        private Tower[] _towerPrefabs;
        public void Reclaim(GameTileContent content)
        {
            Destroy(content.gameObject);
        }

        public GameTileContent Get(GameTileContentType type)
        {
            switch (type)
            {
                case GameTileContentType.Empty:
                    return Get(_emptyPrefab);
                case GameTileContentType.Destination:
                    return Get(_destinationPrefab);
                case GameTileContentType.Wall:
                    return Get(_wallPrefab);
                case GameTileContentType.SpawnPoint:
                    return Get(_spawnPrefab);
            }
            return null;
        }

        public Tower Get(TowerType type)
        {
            Tower tower = _towerPrefabs[(int)type];
            return Get(tower);
        }
        private T Get<T>(T prefab) where T : GameTileContent
        {
            T instance = CreateGameObjectInstance(prefab);
            instance.OriginFactory = this;
            return instance;
        }

        private Scene _contentScene;
    }
}
