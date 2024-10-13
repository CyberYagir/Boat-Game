using System.Collections.Generic;
using Content.Scripts.DungeonGame.Mobs;
using DG.DemiLib;
using UnityEngine;

namespace Content.Scripts.DungeonGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create DungeonConfigObject", fileName = "DungeonConfigObject", order = 0)] 
    public class DungeonConfigObject : ScriptableObject
    {
        [System.Serializable]
        public class DungeonSize
        {
            [SerializeField] private float maxDistance;
            [SerializeField] private int roomsCount;

            public int RoomsCount => roomsCount;

            public float MaxDistance => maxDistance;
        }

        [SerializeField] private DungeonSize dungeonSize;
        [SerializeField] private DungeonTile mainTile;
        [SerializeField] private NoneTile noneTile;

        [SerializeField] private List<DungeonMobObject> mobs = new List<DungeonMobObject>();
        [SerializeField] private List<RoomGrid> rooms = new List<RoomGrid>();
        [SerializeField] private List<DungeonMobObject> bosses = new List<DungeonMobObject>();
        [SerializeField] private float damageModify;
        [SerializeField] private float healthModify;
        
        public NoneTile NoneTile => noneTile;

        public DungeonTile MainTile => mainTile;

        public DungeonSize Size => dungeonSize;
        public List<RoomGrid> Rooms => rooms;

        public float DamageModify => damageModify;

        public float HealthModify => healthModify;


        public DungeonMobObject GetMob(DungeonMobObject.EMobDifficult difficult, System.Random rnd)
        {
            if (difficult != DungeonMobObject.EMobDifficult.Boss)
            {
                return mobs.FindAll(x => x.Difficult == difficult).GetRandomItem(rnd);
            }
            else
            {
                return GetBoss(rnd);
            }
        }
        
        public DungeonMobObject GetBoss(System.Random rnd)
        {
            return bosses.GetRandomItem(rnd);
        }
    }
}
