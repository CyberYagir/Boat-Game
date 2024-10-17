using System.Collections.Generic;
using Content.Scripts.BoatGame.Scriptable;
using Content.Scripts.DungeonGame.Mobs;
using DG.DemiLib;
using Sirenix.OdinInspector;
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

        [SerializeField, FoldoutGroup("Visuals")] private DungeonTile mainTile;
        [SerializeField, FoldoutGroup("Visuals")] private NoneTile noneTile;
        [SerializeField, FoldoutGroup("Visuals")] private RoomGrid roomStart, roomEnd;
        [SerializeField, FoldoutGroup("Visuals")] private List<RoomGrid> rooms = new List<RoomGrid>();
        [SerializeField, FoldoutGroup("Visuals")] private LightningSO lightning;
        [SerializeField] private DungeonSize dungeonSize;
        [SerializeField] private List<DungeonMobObject> mobs = new List<DungeonMobObject>();
        [SerializeField] private List<DungeonMobObject> bosses = new List<DungeonMobObject>();
        [SerializeField] private float damageModify;
        [SerializeField] private float healthModify;
        
        public NoneTile NoneTile => noneTile;

        public DungeonTile MainTile => mainTile;

        public DungeonSize Size => dungeonSize;
        public List<RoomGrid> Rooms => rooms;

        public float DamageModify => damageModify;

        public float HealthModify => healthModify;

        public LightningSO Lightning => lightning;

        public RoomGrid RoomStart => roomStart;

        public RoomGrid RoomEnd => roomEnd;


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
