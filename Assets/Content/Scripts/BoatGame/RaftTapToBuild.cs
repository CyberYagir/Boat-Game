using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class RaftTapToBuild : MonoBehaviour
    {
        private Vector3Int coord;

        public Vector3Int Coords => coord;

        public void SetCoords(Vector3Int coord)
        {
            this.coord = coord;
        }
    }
}
