using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.DungeonGame
{
    public class PropsSelector : PropRandomBase
    {
        public override void Init()
        {
            base.Init();

            int index = notAllocatedWeights.ChooseRandomIndexFromWeights();

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
