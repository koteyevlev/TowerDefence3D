using System.Collections;
using UnityEngine;
public class EnemyView : MonoBehaviour
{
    protected Animator _animator;
    protected Enemy _enemy;

    public bool IsInited { get; protected set; } = false;

    protected const string DIED_KEY = "Died";

    public virtual void Init(Enemy enemy)
    {
        _animator = GetComponent<Animator>();
        _enemy = enemy;
    }

    public virtual void Die()
    {
        _animator.SetBool(DIED_KEY, true);
    }

    public void OnSpawnAnimationFinished()
    {
        IsInited = true;
        GetComponent<TargetPoint>().IsEnabled = true;
    }
}

