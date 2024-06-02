using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class Structure : MonoBehaviour
    {
        
        [SerializeField] private List<VillageGenerator.SubStructures> structures = new List<VillageGenerator.SubStructures>();
        [SerializeField] private List<GameObject> randomVisuals;

        [SerializeField] private UnityEvent<TerrainBiomeSO> OnAfterInitBiome;
        private float spawnRandomItemChance = 0.75f;

        public void Init(Random rnd, TerrainBiomeSO biome)
        {

            foreach (var s in structures)
            {
                for (int i = 0; i < s.Structures.Count; i++)
                {
                    s.Structures[i].Structure.gameObject.SetActive(false);
                }
            }
            
            var holder = structures.Find(x => x.Biome == biome);
            int randomItem = 0;
            if (structures.Count != 0 && holder != null)
            {
                randomItem = rnd.Next(0, structures.Count - 1);
                for (int i = 0; i < holder.Structures.Count; i++)
                {
                    holder.Structures[i].Structure.gameObject.SetActive(i == randomItem);
                }
            }

            for (int i = 0; i < randomVisuals.Count; i++)
            {
                randomVisuals[i].gameObject.SetActive(false);
            }

            if (randomVisuals.Count != 0)
            {
                for (int i = 0; i < randomVisuals.Count; i++)
                {
                    if (rnd.NextDouble() <= spawnRandomItemChance)
                    {
                        // randomItem = randomVisuals.GetRandomIndex(rnd);
                        randomVisuals[i].gameObject.SetActive(true);
                        // randomVisuals.RemoveAt(randomItem);
                    }
                }
            }

            OnAfterInitBiome?.Invoke(biome);
        }
    }
}