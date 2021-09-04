using UnityEngine;

namespace TowerDefence3d.Scripts.MapObject
{
    public static class DirectionExtension
    {
        private static Quaternion[] _rotations =
        {
        Quaternion.identity,
        Quaternion.Euler(0f, 90f, 0f),
        Quaternion.Euler(0f, 180f, 0f),
        Quaternion.Euler(0f, 270f, 0f)
    };

        private static Vector3[] _halfVector =
        {
        Vector3.forward * 0.5f,
        Vector3.right * 0.5f,
        Vector3.back * 0.5f,
        Vector3.left * 0.5f
    };

        public static Quaternion GetRotation(this Direction direction)
        {
            return _rotations[(int)direction];
        }

        public static DirectionChange GetDirectionChangeTo(this Direction current, Direction next)
        {
            if (current == next)
            {
                return DirectionChange.None;
            }
            if (current + 1 == next || current - 3 == next)
            {
                return DirectionChange.TurnRight;
            }
            if (current - 1 == next || current + 3 == next)
            {
                return DirectionChange.TurnLeft;
            }
            return DirectionChange.TurnAround;
        }

        public static float GetAngle(this Direction direction)
        {
            return (float)direction * 90f;
        }

        public static Vector3 GetHalfVector(this Direction direction)
        {
            return _halfVector[(int)direction];
        }
    }
}
