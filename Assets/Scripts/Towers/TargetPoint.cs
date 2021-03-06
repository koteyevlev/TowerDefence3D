using UnityEngine;
using TowerDefence3d.Scripts.Enemies;

namespace TowerDefence3d.Scripts.Towers
{
    public class TargetPoint : MonoBehaviour
    {
        public Enemy Enemy { get; private set; }

        private bool _isEnabled = false;
        private SphereCollider _collider;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _collider.enabled = value;
                _isEnabled = value;
            }
        }
        public Vector3 Position => new Vector3(0, 0.1f, 0) + transform.position;
        public float ColliderSize { get; private set; }

        private const int ENEMY_LAYER_MASK = 1 << 9;
        private static Collider[] _buffer = new Collider[100];
        public static int BufferedCount { get; private set; }
        public void Awake()
        {
            Enemy = transform.root.GetComponent<Enemy>();
            _collider = GetComponent<SphereCollider>();
            _collider.enabled = false;
            ColliderSize = GetComponent<SphereCollider>().radius * transform.localScale.x;
        }

        public static bool FillBuffer(Vector3 position, float range)
        {
            Vector3 top = position;
            top.y += 3f;
            BufferedCount = Physics.OverlapCapsuleNonAlloc(position, top, range, _buffer, ENEMY_LAYER_MASK);
            return BufferedCount > 0;
        }

        public static TargetPoint GetBuffered(int index)
        {
            var target = _buffer[index].GetComponent<TargetPoint>();
            return target;
        }
    }
}
