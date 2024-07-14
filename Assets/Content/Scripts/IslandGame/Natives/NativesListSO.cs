using System.Collections.Generic;
using System.Linq;
using Content.Scripts.IslandGame.Scriptable;
using DG.DemiLib;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.IslandGame.Natives
{
    [CreateAssetMenu(menuName = "Create NativesListSO", fileName = "NativesListSO", order = 0)]
    public class NativesListSO : ScriptableObject
    {
        [SerializeField] private List<NativeController> nativesList;
        [SerializeField] private List<SlaveActivitiesObject> slavesActivities;
        [SerializeField] private int baseUnitCost = 400;
        [SerializeField] private Range slavesOnIslandCount;

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

        public ENativeType GetRandomSlaveSkin(Random rnd)
        {
            var slavesSkins = nativesList.FindAll(x => x.NativeType != ENativeType.Shaman);

            return slavesSkins.GetRandomItem(rnd).NativeType;
        }
    } 
}
