using TowerDefence3d.Scripts.Setup;
using UnityEngine;

namespace TowerDefence3d.Scripts.MapObject
{
    public abstract class WarEntity : GameBehaviour
    {
        public WarFactory OriginFactory { get; set; }

        public override void Recycle()
        {
            OriginFactory.Reclaim(this);
        }
    }
}

