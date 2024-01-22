using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public class WorldGridService : MonoBehaviour
    {
        [SerializeField] private List<Vector3> gridPoints;


        public bool IsHavePoint(Vector3 point)
        {
            return gridPoints.Contains(point);
        }
        
        public bool AddPoint(Vector3 point)
        {
            if (!IsHavePoint(point))
            {
                gridPoints.Add(point);
                return true;
            }
            return false;
        }
        
        public bool RemovePoint(Vector3 point)
        {
            if (IsHavePoint(point))
            {
                gridPoints.Remove(point);
                return true;
            }
            return false;
        }
    }
}
