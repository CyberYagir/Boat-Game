using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class CharacterGrounder
    {
        private Transform transform;

        public void Init(Transform transform)
        {
            this.transform = transform;
        }
        public void PlaceUnitOnGround()
        {
            var playerPos = transform.position;
            if (playerPos.y < 0)
            {
                playerPos.y = 0;
            }

            if (Physics.Raycast(playerPos + Vector3.up * 2f, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default", "Raft", "Terrain"), QueryTriggerInteraction.Ignore))
            {
                if (hit.point.y >= 0)
                {
                    var pos = transform.position;
                    pos.y = hit.point.y;
                    transform.position = pos;
                }
            }
        }
    }
}