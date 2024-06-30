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
        
        public bool IsHavePoints(List<Vector3> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (IsHavePoint(points[i]))
                {
                    return true;
                }
            }
           
            return false;
        }

        public void AddPoints(List<Vector3> getPoints)
        {
            for (int i = 0; i < getPoints.Count; i++)
            {
                gridPoints.Add(getPoints[i]);
            }
        }
    }
}
