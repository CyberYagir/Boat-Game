using System.Collections.Generic;
using System.Linq;
using Content.Scripts.IslandGame.Scriptable;
using Content.Scripts.Mobs;
using DG.DemiLib;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.IslandGame.Natives
{
    [CreateAssetMenu(menuName = "Create NativesListSO", fileName = "NativesListSO", order = 0)]
    public class NativesListSO : ScriptableObject
    {
        [System.Serializable]
        public class PillagersCalculator
        {
            [System.Serializable]
            public class PillagerByLevel
            {
                [SerializeField] private int level;
                [SerializeField] private List<MobObject> pillagers;

                public List<MobObject> Pillagers => pillagers;

                public int Level => level;
            }

            [SerializeField] private List<PillagerByLevel> pillagerByLevels;
            [SerializeField] private AnimationCurve modify;
            [SerializeField] private Range count;

            public int GetCount(Random rnd, int level)
            {
                return Mathf.RoundToInt(rnd.Next((int) count.min, (int) count.max) * modify.Evaluate(level));
            }

            public List<MobObject> GetRandomPillagersByLevel(Random rnd, int level, int count)
            {
                List<MobObject> list = new List<MobObject>();
                for (int i = 0; i < count; i++)
                {
                    var available = pillagerByLevels.FindAll(x => x.Level <= level);
                    list.Add(available.GetRandomItem(rnd).Pillagers.GetRandomItem(rnd));
                }

                return list;
            }
        }

        [SerializeField] private List<NativeController> nativesList;
        [SerializeField] private List<SlaveActivitiesObject> slavesActivities;
        [SerializeField] private int baseUnitCost = 400;
        [SerializeField] private Range slavesOnIslandCount;
        [SerializeField] private PillagersCalculator pillagersCalculator;
        
        
        private Dictionary<ENativeType, List<NativeController>> nativesMap = new Dictionary<ENativeType, List<NativeController>>();
        public List<NativeController> NativesList => nativesList;
        public int BaseUnitCost => baseUnitCost;
        public List<SlaveActivitiesObject> SlavesActivities => slavesActivities;
        public Range SlavesOnIslandCount => slavesOnIslandCount;

        public void Init()
        {
            nativesMap.Clear();
            for (int i = 0; i < NativesList.Count; i++)
            {
                if (!nativesMap.ContainsKey(NativesList[i].NativeType))
                {
                    nativesMap.Add(NativesList[i].NativeType, new List<NativeController>());
                }

                nativesMap[NativesList[i].NativeType].Add(NativesList[i]);
            }
        }


        public NativeController GetByType(ENativeType type, System.Random rnd)
        {
            return nativesMap[type].GetRandomItem(rnd);
        }

        public NativeController GetRandomSlaveSkin(Random rnd)
        {
            var slavesSkins = nativesList.FindAll(x => x.NativeType != ENativeType.Shaman);

            return slavesSkins.GetRandomItem(rnd);
        }

        public NativeController GetSkinByID(string slaveSkinID)
        {
            var find = nativesList.Find(x => x.SkinUid == slaveSkinID);
            
            return find != null ? find : nativesList[0];
        }

        public List<MobObject> GetPillagersCount(System.Random rnd, int level)
        {
            return pillagersCalculator.GetRandomPillagersByLevel(rnd, level, pillagersCalculator.GetCount(rnd, level));
        }
    } 
}
