using System;
using TowerDefence3d.Scripts.MapObject;
using TowerDefence3d.Scripts.Setup;
using TowerDefence3d.Scripts.Towers;
using UnityEngine;

namespace TowerDefence3d.Scripts.Enemies
{
    public class Enemy : GameBehaviour
    {
        [SerializeField]
        private Transform _model;
        [SerializeField]
        private EnemyView _enemyView;
        [SerializeField]
        private float _cost = 1.0f;

        public EnemyFactory OriginFactory { get; set; }
        public float Scale { get; private set; }
        public float Health { get; private set; }

        private GameTile _tileFrom, _tileTo;
        private Vector3 _positionFrom, _positionTo;
        private float _progress, _progressFactor;
        private float _firstHealth;
        Currency _currency;

        private Direction _direction;
        private DirectionChange _directionChange;
        private float _directionAngleFrom, _directionAngleTo;
        private float _pathOffset, _speed;

        public event EventHandler<GetDamageEventArgs> GetDamage;

        public void OnEnemyGetDamage(GetDamageEventArgs e)
        {
            EventHandler<GetDamageEventArgs> handler = GetDamage;
            handler?.Invoke(this, e);
        }



        public void Initialize(float scale, float pathOffset, float speed, float health)
        {
            _model.localScale = new Vector3(scale, scale, scale);
            _pathOffset = pathOffset;
            _speed = speed;
            Scale = scale;
            Health = health;
            _firstHealth = health;
            _enemyView.Init(this);
            _currency = Game._instance.Currency;
        }
        public void SpawnOn(GameTile spawnPoint)
        {
            transform.localPosition = spawnPoint.transform.localPosition;
            _progress = 0f;
            _tileFrom = spawnPoint;
            _tileTo = spawnPoint.NextOnPath;
            PrepareIntro();
        }

        public void TakeDamage(float damage)
        {
            Health -= damage;
            OnEnemyGetDamage(new GetDamageEventArgs 
            { 
                Damage = damage,
                HealthRemainedInProcent = Health / _firstHealth,
            });
        }

        public class GetDamageEventArgs : EventArgs
        {
            public float Damage { get; set; }
            public float HealthRemainedInProcent { get; set; }
        }


        private void PrepareIntro()
        {
            _positionFrom = _tileFrom.transform.localPosition;
            _positionTo = _tileFrom.ExitPoint;
            _direction = _tileFrom.PathDirection;
            _directionChange = DirectionChange.None;
            _directionAngleFrom = _directionAngleTo = _direction.GetAngle();
            _model.localPosition = new Vector3(_pathOffset, 0f);
            transform.localRotation = _direction.GetRotation();
            _progressFactor = 2f * _speed;
        }

        private void PrepareOutro()
        {
            _positionTo = _tileFrom.transform.localPosition;
            _directionChange = DirectionChange.None;
            _directionAngleTo = _direction.GetAngle();
            _model.localPosition = new Vector3(_pathOffset, 0f);
            transform.localRotation = _direction.GetRotation();
            _progressFactor = 2f * _speed;
        }
        public override bool GameUpdate()
        {
            if (_enemyView.IsInited == false)
            {
                return true;
            }
            if (Health < 0)
            {
                DisableView();
                _enemyView.Die();
                _currency.IncrementCurrency(_cost);
                return false;
            }
            _progress += Time.deltaTime * _progressFactor;
            while (_progress > 1)
            {
                if (_tileTo == null)
                {
                    Game.EnemyReachedDestination();
                    Recycle();
                    return false;
                }
                _progress = (_progress - 1f) / _progressFactor;
                PrepareNextState();
                _progress *= _progressFactor;
            }
            if (_directionChange == DirectionChange.None)
            {
                transform.localPosition = Vector3.LerpUnclamped(_positionFrom, _positionTo, _progress);
            }
            else
            {
                float angle = Mathf.LerpUnclamped(_directionAngleFrom, _directionAngleTo, _progress);
                transform.localRotation = Quaternion.Euler(0f, angle, 0f);
            }
            return true;
        }

        private void PrepareNextState()
        {
            _tileFrom = _tileTo;
            _tileTo = _tileFrom.NextOnPath;
            _positionFrom = _positionTo;
            if (_tileTo == null)
            {
                PrepareOutro();
            }
            _positionTo = _tileFrom.ExitPoint;
            _directionChange = _direction.GetDirectionChangeTo(_tileFrom.PathDirection);
            _direction = _tileFrom.PathDirection;
            _directionAngleFrom = _directionAngleTo;

            switch (_directionChange)
            {
                case DirectionChange.None: PrepareForward(); break;
                case DirectionChange.TurnRight: PrepareTurnRight(); break;
                case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
                default: PrepareTurnAround(); break;

            }
        }

        private void PrepareForward()
        {
            transform.localRotation = _direction.GetRotation();
            _directionAngleTo = _direction.GetAngle();
            _model.localPosition = new Vector3(_pathOffset, 0f);
            _progressFactor = _speed;
        }

        private void PrepareTurnRight()
        {
            _directionAngleTo = _directionAngleFrom + 90f;
            _model.localPosition = new Vector3(_pathOffset - 0.5f, 0f);
            transform.localPosition = _positionFrom + _direction.GetHalfVector();
            _progressFactor = _speed / (Mathf.PI * 0.5f * (0.5f - _pathOffset));
        }

        private void PrepareTurnLeft()
        {
            _directionAngleTo = _directionAngleFrom - 90f;
            _model.localPosition = new Vector3(_pathOffset + 0.5f, 0f);
            transform.localPosition = _positionFrom + _direction.GetHalfVector();
            _progressFactor = _speed / (Mathf.PI * 0.5f * (0.5f - _pathOffset));
        }

        private void PrepareTurnAround()
        {
            _directionAngleTo = _directionAngleFrom + (_pathOffset < 0 ? 180f : -180f);
            _model.localPosition = new Vector3(_pathOffset, 0f);
            transform.localPosition = _positionFrom;
            _progressFactor = _speed / (Mathf.PI * Mathf.Max(Mathf.Abs(_pathOffset), 0.2f));
        }

        public override void Recycle()
        {
            OriginFactory.Reclaim(this);
        }

        private void DisableView()
        {
            _enemyView.GetComponent<Collider>().enabled = false;
            _enemyView.GetComponent<TargetPoint>().IsEnabled = false;
        }
    }
}
