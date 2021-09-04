using UnityEngine;

namespace TowerDefence3d.Scripts.Enemies
{
    public class ChomperView : EnemyView
    {
        public void OnDieAnimationFinished()
        {
            _enemy.Recycle();
        }
    }
}

