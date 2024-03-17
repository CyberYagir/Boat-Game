using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures
{
    [CreateAssetMenu(menuName = "Create RoadSO", fileName = "RoadSO", order = 0)]
    public class RoadSO : ScriptableObject
    {
        public enum ERoadsType
        {
            Start,
            End,
            OneRoad,
            TwoRoad,
            ThreeRoad
        }
        [System.Serializable]
        public class RoadsHolder
        {
            [SerializeField] private ERoadsType type;
            [SerializeField] private RoadBuilder roadPrefab;

            public RoadBuilder RoadPrefab => roadPrefab;

            public ERoadsType Type => type;
        }

        [SerializeField] private List<RoadsHolder> roads;

        private Dictionary<ERoadsType, RoadBuilder> roadsMap = null;

        public RoadBuilder GetRoadByEnum(ERoadsType type)
        {
            if (roadsMap == null)
            {
                roadsMap = roads.ToDictionary(x => x.Type, x=>x.RoadPrefab);
            }
            
            return roadsMap[type];
        }
    }
}
