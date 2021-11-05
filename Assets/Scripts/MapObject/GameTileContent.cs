using System.Collections;
using System.Collections.Generic;
using TowerDefence3d.Scripts.Setup;
using UnityEngine;

namespace TowerDefence3d.Scripts.MapObject
{
    [SelectionBase]
    public class GameTileContent : MonoBehaviour
    {
        [SerializeField] private GameTileContentType _type;

        public GameTileContentType Type => _type;

        public bool IsChangableByRayCast = false;

        public GameTileContentFactory OriginFactory { get; set; }
        public bool IsBlockingPath 
        {
            get
            {
                return 
                    Type == GameTileContentType.Wall 
                    || Type == GameTileContentType.Tower
                    || IsChangableByRayCast;
            }
        }

        public bool IsTheoreticBlockingPath
        {
            get
            {
                return
                    Type == GameTileContentType.Wall
                    || Type == GameTileContentType.Tower
                    || IsChangableByRayCast;
            }
        }

        public void Recycle()
        {
            OriginFactory.Reclaim(this);
        }

        public virtual void GameUpdate()
        {

        }

        public virtual void OnClick()
        {

        }
    }

    public enum GameTileContentType
    {
        Empty,
        Destination,
        Wall,
        SpawnPoint,
        Tower
    }
}
