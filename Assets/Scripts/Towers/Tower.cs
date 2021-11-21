using System;
using System.Collections.Generic;
using TowerDefence3d.Scripts.MapObject;
using TowerDefence3d.Scripts.UIObjects;
using TowerDefense.Game;
using UnityEngine;

namespace TowerDefence3d.Scripts.Towers
{
    public abstract class Tower : GameTileContent
    {
        [SerializeField, Range(1.5f, 10.5f)]
        protected float _targetingRange = 1.5f;

        [SerializeField, Range(1.5f, 10.5f)]
        private float _purchaseCost = 1.5f;

        [SerializeField, Range(1, 5)]
        private int _maxLevel = 3;

        [SerializeField] 
        private Transform _radiusVisualizer;

        [SerializeField]
        private TowerUI _towerUIController;

        //костыль
        public Vector3 Position => _radiusVisualizer.position;
        public float PurchaseCost => _purchaseCost;
        private const int ENEMY_LAYER_MASK = 1 << 9;

        private static List<string> _exceptRenders = new List<string>
        {
            "RadiusVisualiser",
            "LaserBeam",
        };

        public abstract TowerType TowerType { get; }

        public int CurrentLevel { get; set; } = 1;

        private bool _isRadiusEnabled;

        private List<MeshRenderer> _towerRenderes;

        private float _extraCost = 0;

        private float _totalCost => _purchaseCost + _extraCost;

        protected float _gameDifficult = 1f;

        public int GetSellLevel() => (int)(_totalCost / 2);

        // TODO временный костыль
        public int GetCostForNextLevel() => (int)(_purchaseCost / 2);

        public bool IsAtMaxLevel => CurrentLevel >= _maxLevel;

        public abstract string TitleName { get; }
        public abstract string Description { get; }
        public abstract string UpgradeDescription { get; }

        public abstract string CurrentDPS { get; }

        private void Start()
        {
            _radiusVisualizer.localScale = new Vector3(
                _targetingRange * 2, 
                _targetingRange * 2, 
                _targetingRange * 2);

            _gameDifficult = GameManager.instance.LevelDifficults;
            _towerRenderes = new List<MeshRenderer>();
            AddRendersOfChild(this.transform);
        }

        public void ChangeMaterial(Material material)
        {
            _towerRenderes.ForEach(p => p.material = material);
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

        internal void DisableRadius()
        {
            _radiusVisualizer.gameObject.SetActive(false);
            _isRadiusEnabled = false;
        }

        public override void OnClick()
        {
            ChangeRadiusEnabled();
            _towerUIController.OnUISelectionChanged(this);
            _radiusVisualizer.gameObject.SetActive(_isRadiusEnabled);
        }

        public void OnCanvasClick()
        {
            if (!_towerUIController.IsClickOnCanvas())
            {
                OnClick();
            }
        }

        private void ChangeRadiusEnabled()
        {
            _isRadiusEnabled = !_isRadiusEnabled;
        }

        internal void UpgradeLevel()
        {
            _extraCost += GetCostForNextLevel();
            DisableRadius();
            CurrentLevel += 1;
            Debug.Log("Level Up. Current level - " + CurrentLevel.ToString());
            _targetingRange += CurrentLevel;
            UpgradeView();
        }

        protected abstract void UpgradeView();

        private void AddRendersOfChild(Transform itemTransform)
        {
            int numOfChildren = itemTransform.childCount;

            for (int i = 0; i < numOfChildren; i++)
            {
                GameObject child = itemTransform.GetChild(i).gameObject;
                if (_exceptRenders.Contains(child.gameObject.name))
                {
                    continue;
                }

                var render = child.GetComponent<MeshRenderer>();
                if (render != null)
                {
                    _towerRenderes.Add(render);
                }

                if (child.transform.childCount > 0)
                {
                    AddRendersOfChild(child.transform);
                }
            }
        }
    }
}
