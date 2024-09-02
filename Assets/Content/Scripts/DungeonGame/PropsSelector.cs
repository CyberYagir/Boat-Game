using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.DungeonGame
{
    public class PropsSelector : PropRandomBase
    {
        public override void Init(Random random)
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
    }
}
