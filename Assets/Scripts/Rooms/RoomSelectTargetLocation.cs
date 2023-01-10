using UnityEngine;

namespace Rooms
{
    public class RoomSelectTargetLocation : MonoBehaviour
    {
        public Transform targetLocation;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(targetLocation.position, 0.5f);
        }
    }
}
