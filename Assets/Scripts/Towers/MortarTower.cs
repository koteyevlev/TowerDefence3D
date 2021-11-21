using System;
using TowerDefence3d.Scripts.MapObject;
using TowerDefence3d.Scripts.Setup;
using UnityEngine;

namespace TowerDefence3d.Scripts.Towers
{
    public class MortarTower : Tower
    {
        [SerializeField, Range(0.5f, 2f)]
        private float _shootsPerSecond = 1f;
        [SerializeField]
        private Transform _mortar;
        [SerializeField, Range(0.5f, 3f)]
        private float _shellBlastRadius = 1f;
        [SerializeField, Range(1f, 100f)]
        private float _damage;
        [SerializeField]
        private AudioSource _shootSound;
        [SerializeField]
        private Material[] _levelMaterials;
        public override TowerType TowerType => TowerType.Mortar;

        public override string TitleName => "Mortar " + CurrentLevel.ToString();
        public override string Description => "Super heavy mortar";

        public override string CurrentDPS => CalculateDamage().ToString();

        public override string UpgradeDescription => "+10 DPS, +1 radius";

        private float _launchSpeed;
        private float _launchProgress;

        private void Awake()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            float x = _targetingRange + 0.251f;
            float y = -_mortar.position.y;
            _launchSpeed = Mathf.Sqrt(9.81f * (y + Mathf.Sqrt(x * x + y * y)));
        }

        public override void GameUpdate()
        {
            _launchProgress += Time.deltaTime * _shootsPerSecond;
            while (_launchProgress >= 1f)
            {
                if (IsAcquireTarget(out TargetPoint target))
                {
                    Launch(target);
                    _launchProgress -= 1f;
                }
                else
                {
                    _launchProgress = 0.999f;
                }
            }
        }
        private void Launch(TargetPoint target)
        {
            Vector3 launchPoint = _mortar.position;
            Vector3 targetPoint = target.Position;
            targetPoint.y = 0f;

            Vector2 dir;
            dir.x = targetPoint.x - launchPoint.x;
            dir.y = targetPoint.z - launchPoint.z;

            float x = dir.magnitude;
            float y = -launchPoint.y;
            dir /= x;
            const float g = 9.81f;
            float s = _launchSpeed;
            float s2 = s * s;

            float r = s2 * s2 - g * (g * x * x + 2f * y * s2);
            float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
            float cosTheta = Mathf.Cos(Mathf.Atan(tanTheta));
            float sinTheta = cosTheta * tanTheta;
            Vector3 prev = launchPoint;

            _mortar.localRotation = Quaternion.LookRotation(new Vector3(dir.x, tanTheta, dir.y));
            _shootSound.Play();

            Game.SpawnShell().Initialize(launchPoint, targetPoint,
                new Vector3(s * cosTheta * dir.x, s * sinTheta, s * cosTheta * dir.y), _shellBlastRadius, CalculateDamage());
        }

        private float CalculateDamage() => 
            (_damage + ((CurrentLevel - 1) * 10)) * (1 / _gameDifficult);

        protected override void UpgradeView()
        {
            ChangeMaterial(_levelMaterials[CurrentLevel - 1]);
            Debug.Log("Upgrade View");
        }
    }
}
