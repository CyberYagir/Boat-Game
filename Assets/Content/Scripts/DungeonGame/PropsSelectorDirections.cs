using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.DungeonGame
{
    public class PropsSelectorDirections : PropRandomBaseDirs
    {
        public override void Init(Random random)
        {
            if (dirs.Count == 4)
            {
                base.Init(random);

                int index = notAllocatedWeights.ChooseRandomIndexFromWeights(random);

                for (var i = 0; i < prefabs.Count; i++)
                {
                    if (prefabs[i].Prefab != null)
                    {
                        prefabs[i].Prefab.gameObject.SetActive(i == index);
                    }
                }
            }
            else
            {
                for (var i = 0; i < prefabs.Count; i++)
                {
                    if (prefabs[i].Prefab != null)
                    {
                        prefabs[i].Prefab.gameObject.SetActive(false);
                    }
                }
            }
        }

        [Button]
        public void GetAndSetWeights()
        {
            foreach (Transform ch in transform)
            {
                prefabs.Add(new WeightedProp(ch.gameObject));
            }
        }
    }
}