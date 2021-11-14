using TowerDefence3d.Scripts.MapObject;
using UnityEngine;

namespace TowerDefence3d.Scripts.Towers
{
    public class LaserTower : Tower
    {
        [SerializeField]
        private Transform _turret;
        [SerializeField]
        private Transform _laserBeam;
        [SerializeField, Range(1f, 100f)]
        private float _damagePerSecond = 30f;
        [SerializeField]
        private AudioSource _shootSound;
        [SerializeField]
        private Material[] _levelMaterials;
        private Vector3 _laserBeamScale;
        private Vector3 _laserStartPosition;

        public override TowerType TowerType => TowerType.Laser;

        public override string TitleName => "Laser " + CurrentLevel.ToString();
        public override string Description => "Incredible laser";

        public override string CurrentDPS => CalculateDamage().ToString();
        public override string UpgradeDescription => "+10 DPS, +1 radius";

        private TargetPoint _target;

        private void Awake()
        {
            _laserBeamScale = _laserBeam.localScale;
            _laserStartPosition = _laserBeam.localPosition;
        }
        public override void GameUpdate()
        {
            if (IsTargetTracked(ref _target) || IsAcquireTarget(out _target))
            {
                Shoot();
            }
            else
            {
                _laserBeam.localScale = Vector3.zero;
            }
        }

        private void Shoot()
        {
            var point = _target.Position;
            _turret.LookAt(point);
            _laserBeam.localRotation = _turret.localRotation;

            var distance = Vector3.Distance(_turret.position, point);
            _laserBeamScale.z = distance;
            _laserBeam.localScale = _laserBeamScale;
            _laserBeam.localPosition = _laserStartPosition + 0.5f * distance * _laserBeam.forward;
            if (!_shootSound.isPlaying)
                _shootSound.Play();
            _target.Enemy.TakeDamage(CalculateDamage() * Time.deltaTime);
        }


        private float CalculateDamage() => _damagePerSecond + ((CurrentLevel - 1) * 10);

        protected override void UpgradeView()
        {
            ChangeMaterial(_levelMaterials[CurrentLevel - 1]);
            Debug.Log("Upgrade View");
        }
    }
}
