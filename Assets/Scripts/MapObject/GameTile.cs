using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefence3d.Scripts.MapObject
{
    public class GameTile : MonoBehaviour, IDropHandler
    {
        [SerializeField]
        private Transform _arrow;

        private GameTile _north, _east, _south, _west, _nextOnPath;

        private int _distance;

        private Quaternion _northRotation = Quaternion.Euler(90f, 0f, 0f);
        private Quaternion _eastRotation = Quaternion.Euler(90f, 90f, 0f);
        private Quaternion _southRotation = Quaternion.Euler(90f, 180f, 0f);
        private Quaternion _westRotation = Quaternion.Euler(90f, 270f, 0f);

        public bool IsAlternative { get; set; }

        private GameTileContent _content;

        public GameTile NextOnPath => _nextOnPath;

        public Vector3 ExitPoint { get; private set; }

        public Direction PathDirection { get; private set; }

        public GameTileContent Content
        {
            get => _content;
            set
            {
                if (_content != null)
                {
                    _content.Recycle();
                }
                _content = value;
                _content.transform.localPosition = transform.localPosition;
            }
        }
        public bool HasPath => _distance != int.MaxValue;

        public static void MakeEastWestNeighbors(GameTile east, GameTile west)
        {
            west._east = east;
            east._west = west;
        }

        public static void MakeNorthSouthNeigbors(GameTile north, GameTile south)
        {
            north._south = south;
            south._north = north;
        }

        public void ClearPath()
        {
            _distance = int.MaxValue;
            _nextOnPath = null;
        }

        public void BecomeDestination()
        {
            _distance = 0;
            _nextOnPath = null;
            ExitPoint = transform.localPosition;
        }

        private GameTile GrowPathTo(GameTile neighbor, Direction direction, bool isBuilding)
        {
            if (!HasPath || neighbor == null || neighbor.HasPath)
            {
                return null;
            }

            neighbor._distance = _distance + 1;
            neighbor._nextOnPath = this;
            neighbor.ExitPoint = neighbor.transform.localPosition + direction.GetHalfVector();

            if (isBuilding)
            {
                return neighbor.Content.IsTheoreticBlockingPath ? null : neighbor;
            }
            neighbor.PathDirection = direction;

            return neighbor.Content.IsBlockingPath ? null : neighbor;
        }

        public GameTile GrowPathNorth(bool isBuilding) => GrowPathTo(_north, Direction.South, isBuilding);
        public GameTile GrowPathSouth(bool isBuilding) => GrowPathTo(_south, Direction.North, isBuilding);
        public GameTile GrowPathWest(bool isBuilding) => GrowPathTo(_west, Direction.East, isBuilding);
        public GameTile GrowPathEast(bool isBuilding) => GrowPathTo(_east, Direction.West, isBuilding);


        public void HidePath()
        {
            _arrow.gameObject.SetActive(false);
        }

         public void ShowPath()
        {
            if (_distance == 0)
            {
                _arrow.gameObject.SetActive(false);
                return;
            }
            _arrow.gameObject.SetActive(true);
            _arrow.localRotation =
                _nextOnPath == _north ? _northRotation :
                _nextOnPath == _east ? _eastRotation :
                _nextOnPath == _south ? _southRotation :
                _westRotation;

        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("OnDrop");
        }
    }
}
