﻿using System.Collections;
using UnityEngine;
public class EnemyView : MonoBehaviour
{
    protected Animator _animator;
    protected Enemy _enemy;
    private const float _waitingBeforeDestroy = 3f;

    protected const string DIED_KEY = "Died";

    public virtual void Init(Enemy enemy)
    {
        _animator = GetComponent<Animator>();
        _enemy = enemy;
    }

    public virtual void Die()
    {
        _animator.SetBool(DIED_KEY, true);
        StartCoroutine(DestroyRoutine());
    }

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(_waitingBeforeDestroy);
        _enemy.Recycle();
    }
}
