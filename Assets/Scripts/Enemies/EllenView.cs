namespace TowerDefence3d.Scripts.Enemies
{
    public class EllenView : EnemyView
    {
        public void OnDieAnimationFinished()
        {
            _enemy.Recycle();
        }
    }
}
