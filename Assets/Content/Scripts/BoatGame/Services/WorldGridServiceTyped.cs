using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public class WorldGridServiceTyped : WorldGridService
    {
        public enum ECellType
        {
            None,
            Filled,
            Room
        }
        [SerializeField] private Dictionary<Vector3,ECellType> gridPoints = new Dictionary<Vector3, ECellType>();

        
        public bool IsHavePoint(Vector3 point, out ECellType type)
        {
            if (IsHavePoint(point))
            {
                type = gridPoints[point];
                return true;
            }

            type = ECellType.None;
            return false;
        }
        
        public bool AddPoint(Vector3 point, ECellType type)
        {
            if (AddPoint(point))
            {
                gridPoints.TryAdd(point, type);
                return true;
            }
            return false;
        }
        
        
        public void AddPoints(List<Vector3> getPoints, ECellType type)
        {
            for (int i = 0; i < getPoints.Count; i++)
            {
                AddPoint(getPoints[i], type);
            }
        }
    }
}