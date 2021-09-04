using UnityEngine;

namespace TowerDefence3d.Scripts.Enemies
{
    public class GolemView : EnemyView
    {
        public void OnDieAnimationFinished()
        {
            _enemy.Recycle();
        }
    }
}