using UnityEngine;

namespace Rooms
{
    public class RoomSelectController : MonoBehaviour
    {
        private UnityEngine.Camera _camera;
        private MoveCameraController _mover;

        public float moveTime = 0.5f;
    
        // Start is called before the first frame update
        void Start()
        {
            _camera = GetComponent<Camera>();
            _mover = GetComponent<MoveCameraController>();
        }

        // Update is called once per frame
        void Update()
        {
            var mask = LayerMask.GetMask("Movement");
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit, 100, mask))
                return;
            
            var obj = hit.transform;
            var targetLocation = obj.GetComponent<RoomSelectTargetLocation>().targetLocation.position;
        
            if (Input.GetMouseButtonDown(0))
            {
                _mover.StartMove(targetLocation, moveTime);
            }
        }
    }
}
