using TowerDefence3d.Scripts.MapObject;
using UnityEngine;

namespace TowerDefence3d.Scripts.Towers
{
    public abstract class Tower : GameTileContent
    {
        [SerializeField, Range(1.5f, 10.5f)]
        protected float _targetingRange = 1.5f;

        [SerializeField, Range(1.5f, 10.5f)]
        public float _purchaseCost = 1.5f;

        [SerializeField] 
        private Transform _radiusVisualizer;

        public float PurchaseCost => _purchaseCost;
        private const int ENEMY_LAYER_MASK = 1 << 9;

        public abstract TowerType TowerType { get; }

        private void Start()
        {
            _radiusVisualizer.localScale = new Vector3(
                _targetingRange * 2, 
                _targetingRange * 2, 
                _targetingRange * 2);
        }

        protected bool IsAcquireTarget(out TargetPoint target)
        {
            Collider[] targets = Physics.OverlapSphere(transform.localPosition, _targetingRange, ENEMY_LAYER_MASK);
            if (targets.Length > 0)
            {
                target = targets[0].GetComponent<TargetPoint>();
                return true;
            }
            target = null;
            return false;
        }

        protected bool IsTargetTracked(ref TargetPoint target)
        {
            if (target == null)
            {
                return false;
            }
            Vector3 myPosision = transform.localPosition;
            Vector3 targetPosition = target.Position;
            if (Vector3.Distance(myPosision, targetPosition) >
                _targetingRange + target.ColliderSize * target.Enemy.Scale ||
                target.IsEnabled == false)
            {
                target = null;
                return false;
            }
            return true;
        }
    }
}
