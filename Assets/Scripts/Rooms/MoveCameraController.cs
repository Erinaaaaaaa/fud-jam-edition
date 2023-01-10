using UnityEngine;

namespace Rooms
{
    public class MoveCameraController : MonoBehaviour
    {
        private Vector3 _startPosition;
        private float _startTime;
        private Vector3 _targetPosition;
        private float _duration;

        private bool _moving;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (!_moving) return;
        
            var t = transform;

            var progress = (Time.time - _startTime) / _duration;

            if (progress >= 1)
            {
                t.position = _targetPosition;
                _moving = false;
            }
            else
            {
                t.position = Vector3.Lerp(_startPosition, _targetPosition, progress);   
            }
        }

        public void StartMove(Vector3 position, float duration)
        {
            _startPosition = transform.position;
            _targetPosition = position;
            _startTime = Time.time;
            _duration = duration;

            _moving = true;
        }
    }
}
