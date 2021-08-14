using UnityEngine;
public class Tower : GameTileContent
{
    [SerializeField, Range(1.5f, 10.5f)]
    private float _targetingRange = 1.5f;
    private TargetPoint _target;
    private const int ENEMY_LAYER_MASK = 1 << 9;

    public override void GameUpdate()
    {
        if (IsAcquireTarget())
        {
            Debug.Log("target found");
        }
    }

    private bool IsAcquireTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.localPosition, _targetingRange, ENEMY_LAYER_MASK);
        if (targets.Length > 0)
        {
            _target = targets[0].GetComponent<TargetPoint>();
            return true;
        }
        _target = null;
        return false;
    }

    private bool IsTargetTracked()
    {
        if (_target is null)
        {
            return false;
        }
        Vector3 myPosision = transform.localPosition;
        Vector3 targetPosition = _target.Position;
        if (Vector3.Distance(myPosision, targetPosition) > 
            _targetingRange + _target.ColliderSize * _target.Enemy.Scale)
        {
            _target = null;
            return false;
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, _targetingRange);
        if (_target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, _target.Position);
        }
    }
}

