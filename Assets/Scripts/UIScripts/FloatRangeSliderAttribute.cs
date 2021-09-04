using UnityEngine;

namespace TowerDefence3d.Scripts.UIScripts
{
    public class FloatRangeSliderAttribute : PropertyAttribute
    {
        public float Min { get; private set; }
        public float Max { get; private set; }

        public FloatRangeSliderAttribute(float min, float max)
        {
            Min = min;
            Max = min > max ? min : max;
        }
    }
}
