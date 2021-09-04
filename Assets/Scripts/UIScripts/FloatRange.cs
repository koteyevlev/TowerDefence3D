using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace TowerDefence3d.Scripts.UIScripts
{
    [Serializable]
    public struct FloatRange
    {
        [SerializeField]
        private float _min, _max;
        public float Min => _min;
        public float Max => _max;

        public float RandomValueInRange
        {
            get => Random.Range(_min, _max);
        }

        public FloatRange(float value)
        {
            _min = _max = value;
        }

        public FloatRange(float min, float max)
        {
            _min = min;
            _max = max;
        }
    }
}
