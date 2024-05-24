using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Content.Scripts.IslandGame.Natives
{
    [CreateAssetMenu(menuName = "Create NativesListSO", fileName = "NativesListSO", order = 0)]
    public class NativesListSO : ScriptableObject
    {
        [SerializeField] private List<NativeController> nativesList;

        private Dictionary<ENativeType, List<NativeController>> nativesMap = new Dictionary<ENativeType, List<NativeController>>();

        public void Init()
        {
            nativesMap.Clear();
            for (int i = 0; i < nativesList.Count; i++)
            {
                if (!nativesMap.ContainsKey(nativesList[i].NativeType))
                {
                    nativesMap.Add(nativesList[i].NativeType, new List<NativeController>());
                }

                nativesMap[nativesList[i].NativeType].Add(nativesList[i]);
            }
        }


        public NativeController GetByType(ENativeType type, System.Random rnd)
        {
            return nativesMap[type].GetRandomItem(rnd);
        }
    } 
}
