using System;
using System.Collections.Generic;
using TowerDefence3d.Scripts.MapObject;
using TowerDefence3d.Scripts.Towers;
using TowerDefense.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefence3d.Scripts.Setup
{

    public class GameBoard : MonoBehaviour
    {
        [SerializeField]
        private Transform _ground;

        [SerializeField]
        private GameTile _tilePrefab;

        private Vector2Int _size;

        private List<GameTile> _tiles;
        private Queue<GameTile> _searchFrontier = new Queue<GameTile>();
        private GameTileContentFactory _contentFactory;
        private List<GameTile> _spawnPoints = new List<GameTile>();
        private List<GameTileContent> _contetToUpdate = new List<GameTileContent>();

        public int SpownPointsCount => _spawnPoints.Count;
        public void Initialize(GameTileContentFactory contentFactory)
        {
            _size = GameManager.instance.GetLevelForCurrentScene().BoardSize;
            _ground.localScale = new Vector3(_size.x, _size.y, 1f);
            Vector2 offset = new Vector2((_size.x - 1) * 0.5f, (_size.y - 1) * 0.5f);
            _tiles = new List<GameTile>(_size.x * _size.y);
            _contentFactory = contentFactory;
            for (int i = 0, y = 0; y < _size.y; y++)
            {
                for (int x = 0; x < _size.x; x++, i++)
                {
                    GameTile tile = Instantiate(_tilePrefab);
                    _tiles.Add(tile);
                    tile.transform.SetParent(transform, false);
                    tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);

                    if (x > 0)
                    {
                        GameTile.MakeEastWestNeighbors(tile, _tiles[i - 1]);
                    }
                    if (y > 0)
                    {
                        GameTile.MakeNorthSouthNeigbors(tile, _tiles[i - _size.x]);
                    }
                    tile.IsAlternative = (x & 1) == 0;
                    if ((y & 1) == 0)
                    {
                        tile.IsAlternative = !tile.IsAlternative;
                    }
                    tile.Content = _contentFactory.Get(GameTileContentType.Empty);
                }
            }
            Clear();
        }

        public bool FindPaths(bool isBuilding = false)
        {
            foreach (GameTile tile in _tiles)
            {
                if (tile.Content.Type == GameTileContentType.Destination)
                {
                    tile.BecomeDestination();
                    _searchFrontier.Enqueue(tile);
                }
                else
                {
                    tile.ClearPath();
                }
            }

            if (_searchFrontier.Count == 0)
            {
                return false;
            }

            while (_searchFrontier.Count > 0)
            {
                GameTile tile = _searchFrontier.Dequeue();
                if (tile != null)
                {
                    if (tile.IsAlternative)
                    {
                        _searchFrontier.Enqueue(tile.GrowPathNorth(isBuilding));
                        _searchFrontier.Enqueue(tile.GrowPathSouth(isBuilding));
                        _searchFrontier.Enqueue(tile.GrowPathEast(isBuilding));
                        _searchFrontier.Enqueue(tile.GrowPathWest(isBuilding));
                    }
                    else
                    {
                        _searchFrontier.Enqueue(tile.GrowPathWest(isBuilding));
                        _searchFrontier.Enqueue(tile.GrowPathEast(isBuilding));
                        _searchFrontier.Enqueue(tile.GrowPathSouth(isBuilding));
                        _searchFrontier.Enqueue(tile.GrowPathNorth(isBuilding));
                    }

                }
            }
            foreach (GameTile t in _tiles)
            {
                if (!t.HasPath && t.Content.Type == GameTileContentType.SpawnPoint)
                {
                    return false;
                }
                else if (!t.HasPath && t.Content.Type != GameTileContentType.SpawnPoint)
                {
                    t.HidePath();
                }
                t.ShowPath();
            }
            return true;
        }

        public bool FindPaths(GameTile tile)
        {
            tile.Content.IsChangableByRayCast = true;
            var result = FindPaths(true);
            tile.Content.IsChangableByRayCast = false;
            FindPaths();
            return result;
        }

        public void ToggleDestination(GameTile tile)
        {
            if (tile.Content.Type == GameTileContentType.Destination)
            {
                tile.Content = _contentFactory.Get(GameTileContentType.Empty);
                if (!FindPaths())
                {
                    tile.Content = _contentFactory.Get(GameTileContentType.Destination);
                    FindPaths();
                }
            }
            else if (tile.Content.Type == GameTileContentType.Empty)
            {
                tile.Content = _contentFactory.Get(GameTileContentType.Destination);
                FindPaths();
            }
        }

        public void ToggleSpawnPoint(GameTile tile)
        {
            if (tile.Content.Type == GameTileContentType.SpawnPoint)
            {
                if (_spawnPoints.Count > 1)
                {
                    _spawnPoints.Remove(tile);
                    tile.Content = _contentFactory.Get(GameTileContentType.Empty);
                }
            }
            else if (tile.Content.Type == GameTileContentType.Empty)
            {
                tile.Content = _contentFactory.Get(GameTileContentType.SpawnPoint);
                _spawnPoints.Add(tile);
            }
        }

        public void ToggleWall(GameTile tile)
        {
            //if (tile.Content.Type == GameTileContentType.Wall)
            //{
            //    tile.Content = _contentFactory.Get(GameTileContentType.Empty);
            //    FindPaths();
            //}
            if (tile.Content.Type == GameTileContentType.Empty)
            {
                tile.Content = _contentFactory.Get(GameTileContentType.Wall);
                //if (!FindPaths())
                //{
                //    tile.Content = _contentFactory.Get(GameTileContentType.Empty);
                //    FindPaths();
                //}
            }
        }

        public Tower SetTower(GameTile tile, TowerType type)
        {
            return _toggleTowerInternal(tile, type);
        }

        public void SellTower(GameTile tile, Tower tower)
        {
            if (tile.Content.Type == GameTileContentType.Tower)
            {
                _contetToUpdate.Remove(tile.Content);
                tile.Content = _contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }

            Debug.Log("Sell complete");
        }

        [Obsolete]
        public void ToggleTower(GameTile tile, TowerType type)
        {
            if (tile.Content.Type == GameTileContentType.Tower)
            {
                _contetToUpdate.Remove(tile.Content);
                tile.Content = _contentFactory.Get(GameTileContentType.Empty);
                FindPaths();

            }
            else if (tile.Content.Type == GameTileContentType.Empty)
            {
                tile.Content = _contentFactory.Get(type);
                if (FindPaths())
                {
                    _contetToUpdate.Add(tile.Content);
                }
                else
                {
                    tile.Content = _contentFactory.Get(GameTileContentType.Empty);
                    FindPaths();
                }
            }
            else if (tile.Content.Type == GameTileContentType.Wall)
            {
                tile.Content = _contentFactory.Get(type);
                _contetToUpdate.Add(tile.Content);
            }
        }

        public GameTile GetTile(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue, 1))
            {
                int x = (int)(hit.point.x + _size.x * 0.5f);
                int y = (int)(hit.point.z + _size.y * 0.5f);
                if (x >= 0 && x < _size.x && y > 0 && y < _size.y)
                {
                    return _tiles[x + y * _size.x];
                }
            }
            return null;
        }

        public GameTile GetSpawnPoint(int index)
        {
            return _spawnPoints[index];
        }

        public void GameUpdate()
        {
            for (int i = 0; i < _contetToUpdate.Count; i++)
            {
                _contetToUpdate[i].GameUpdate();
            }
        }

        public void Clear()
        {
            foreach (var tile in _tiles)
            {
                tile.Content = _contentFactory.Get(GameTileContentType.Empty);
            }
            _spawnPoints.Clear();
            _contetToUpdate.Clear();
            try
            {
                SpawnByLevel();
                // Debug.Log("Spawn Successful");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                ToggleDestination(_tiles[0]);
                ToggleSpawnPoint(_tiles[_tiles.Count - 1]);
            }
        }

        public void ReInitialize()
        {
            foreach (var tile in _tiles)
            {
                tile.HidePath();
                _contentFactory.Reclaim(tile.Content);
                Destroy(tile);
            }
            Initialize(_contentFactory);
        }

        private void SpawnByLevel()
        {
            var level = GameManager.instance.GetLevelForCurrentScene();
            foreach (var rock in level.RocksPoints)
            {
                //Debug.Log("rock");
                // Debug.Log(rock);
                ToggleWall(_tiles[rock.y + rock.x * level.BoardSize.x]);
            }

            foreach (var spawn in level.StartPoints)
            {
                ToggleSpawnPoint(_tiles[spawn.y + spawn.x * level.BoardSize.x]);
            }

            foreach (var end in level.EndPoints)
            {
                ToggleDestination(_tiles[end.y + end.x * level.BoardSize.x]);
            }
        }

        private Tower _toggleTowerInternal(GameTile tile, TowerType type)
        {
            //if (tile.Content.Type == GameTileContentType.Tower)
            //{
            //    _contetToUpdate.Remove(tile.Content);
            //    tile.Content = _contentFactory.Get(GameTileContentType.Empty);
            //    FindPaths();
            //}
            var tower = _contentFactory.Get(type);
            if (tile.Content.Type == GameTileContentType.Empty)
            {
                tile.Content = tower;
                if (FindPaths())
                {
                    _contetToUpdate.Add(tile.Content);
                    return tower;
                }
                else
                {
                    tile.Content = _contentFactory.Get(GameTileContentType.Empty);
                    FindPaths();
                }
            }
            else if (tile.Content.Type == GameTileContentType.Wall)
            {
                tile.Content = tower;
                _contetToUpdate.Add(tile.Content);
                return tower;
            }

            return null;
        }
    }
}
