using UnityEngine;
using UnityEngine.UI;

namespace TowerDefence3d.Scripts.UIObjects
{
    public class CameraMove : MonoBehaviour
    {
        private Vector3 _cameraGoal, _oldMousePos, _newPos;
        private Camera _camera;
        [SerializeField, Range(0.01f, 0.1f)]
        private float _cameraSpeed = 0.1f;
        [SerializeField, Range(0.01f, 0.1f)]
        private float _norm = 1f;
        [SerializeField, Range(0.5f, 10f)]
        private float _maxCameraGoal = 1f;
        private float _maxX, _maxZ, _minX, _minZ;

        public bool Drag { get; set; }
        void Start()
        {
            _camera = Camera.main;
            _cameraGoal = transform.position;
            _maxX = transform.position.x + _maxCameraGoal;
            _maxZ = transform.position.z + _maxCameraGoal;
            _minX = transform.position.x - _maxCameraGoal;
            _minZ = transform.position.z - _maxCameraGoal;

        }
        private void Update()
        {
            // print(_cameraGoal);
            if (Input.GetMouseButtonDown(0))
            {
                _oldMousePos = _camera.ScreenToViewportPoint(Input.mousePosition);
            }
            if (Drag)
            {
                _newPos = _camera.ScreenToViewportPoint(Input.mousePosition);

                _cameraGoal = GetShift();
            }
            if (Input.GetMouseButtonUp(0))
            {
                Drag = false;
            }
        }

        private Vector3 GetShift()
        {
            var xShift = -Mathf.Clamp((_oldMousePos.x - _newPos.x) * _norm, 
                -0.1f, 0.1f);
            var zShift = -Mathf.Clamp((_oldMousePos.y - _newPos.y) * _norm, 
                -0.1f, 0.1f);
            var position = new Vector3(xShift, 0, zShift) + _cameraGoal;
            position.x = Mathf.Clamp(position.x, _minX, _maxX);
            position.z = Mathf.Clamp(position.z, _minZ, _maxZ);
            return position;
        }

        void FixedUpdate()
        {
            if (transform.position.x != _cameraGoal.x || transform.position.z != _cameraGoal.z)
            {
                transform.position = Vector3.Lerp(transform.position, _cameraGoal, _cameraSpeed);
            }
        }
    }
}
