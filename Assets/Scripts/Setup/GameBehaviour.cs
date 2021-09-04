using UnityEngine;

namespace TowerDefence3d.Scripts.Setup
{
    public abstract class GameBehaviour : MonoBehaviour
    {
        public virtual bool GameUpdate() => true;

        public abstract void Recycle();
    }
}
