using UnityEngine;
using UnityEngine.UI;

namespace TowerDefence3d.Scripts.UIObjects
{
    [RequireComponent(typeof(Camera))]
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
        private float _maxX, _maxY, _maxZ, _minX, _minY, _minZ;
        
        private float _sensitivity = 1;
        private float _speedZoom = 300;
        private bool _wasZoomingLastFrame; // Touch mode only
        private Vector2[] _lastZoomPositions; // Touch mode only
        public bool Drag { get; set; }
        void Start()
        {
            _camera = Camera.main;
            _cameraGoal = transform.position;
            _maxX = transform.position.x + _maxCameraGoal;
            _maxY = transform.position.y + _maxCameraGoal;
            _maxZ = transform.position.z + _maxCameraGoal;
            _minX = transform.position.x - _maxCameraGoal;
            _minY = transform.position.y - _maxCameraGoal;
            _minZ = transform.position.z - _maxCameraGoal;

        }
        private void Update()
        {
            // print(Input.mouseScrollDelta.y);
            //print(_cameraGoal);
            if (IsZoom())
            {
                CalculateZoom();
            }
            else
            {
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
        }

        private void CalculateZoom()
        {
            #if UNITY_EDITOR
            var zoomLevel = Input.mouseScrollDelta.y * _sensitivity;
            #else
            var zoomLevel = CalcAndroidDelta() * _sensitivity;
            #endif

            var zoomPosition = Mathf.MoveTowards(0, zoomLevel, _speedZoom * Time.deltaTime);
            // print(zoomPosition);
            var position = transform.position + (transform.forward * zoomPosition);
            position.x = Mathf.Clamp(position.x, _minX, _maxX);
            position.y = Mathf.Clamp(position.y, _minY, _maxY);
            position.z = Mathf.Clamp(position.z, _minZ, _maxZ);
            _cameraGoal = position;
        }

        private float CalcAndroidDelta()
        {
            Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };
            if (!_wasZoomingLastFrame)
            {
                _lastZoomPositions = newPositions;
                _wasZoomingLastFrame = true;
            }
            else
            {
                // Zoom based on the distance between the new positions compared to the 
                // distance between the previous positions.
                float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                float oldDistance = Vector2.Distance(_lastZoomPositions[0], _lastZoomPositions[1]);
                float offset = newDistance - oldDistance;

                _lastZoomPositions = newPositions;
                if (offset > 0)
                    return 1f;
                else if (offset < 0)
                    return -1f;
            }

            return 0f;
        }

        private bool IsZoom()
        {
            #if UNITY_EDITOR
            return Input.mouseScrollDelta.y != 0;
            #else
            return Input.touchCount >= 2;
            #endif
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
