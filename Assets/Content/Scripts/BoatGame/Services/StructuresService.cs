using System.Collections.Generic;
using Content.Scripts.Global;
using Content.Scripts.IslandGame;
using Content.Scripts.IslandGame.WorldStructures;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class StructuresService : MonoBehaviour
    {
        [System.Serializable]
        public class VillageStructure
        {
            [SerializeField] private VillageGenerator villageGenerator;
            [SerializeField] private VillagePopulation population;
            [SerializeField] private VillageDataCollector dataCollector;

            public VillageStructure(VillageGenerator villageGenerator)
            {
                this.villageGenerator = villageGenerator;
                this.population = villageGenerator.Population;
                this.dataCollector = villageGenerator.DataCollector;
            }

            public VillageDataCollector DataCollector => dataCollector;

            public VillagePopulation Population => population;

            public VillageGenerator VillageGenerator => villageGenerator;
        }
        
        [System.Serializable]
        public class PlayerStructure
        {
            [SerializeField] private string uid;
            [SerializeField] private bool isBuilded;
            [SerializeField] private GameObject gameObject;

            public PlayerStructure(string uid, bool isBuilded, GameObject gameObject)
            {
                this.uid = uid;
                this.isBuilded = isBuilded;
                this.gameObject = gameObject;
            }

            public GameObject GameObject => gameObject;

            public bool IsBuilded => isBuilded;

            public string Uid => uid;
            public bool IsSpawned => GameObject != null;

            public void DestroyBuilding()
            {
                Destroy(gameObject);
            }
        }
        
        [SerializeField] private IslandGenerator islandGenerator;
        [SerializeField] private List<VillageStructure> villages = new List<VillageStructure>();
        [SerializeField] private List<Structure> structures = new List<Structure>();
        [SerializeField] private List<PlayerStructure> playerStructures = new List<PlayerStructure>();
        [SerializeField] private StructureBuildProcess structureBuildProcess;
        private PrefabSpawnerFabric fabric;

        public IslandGenerator Island => islandGenerator;
        
        private SaveDataObject saveDataObject;


        public VillageStructure GetVillageStructure(string villageID)
        {
            return villages.Find(x => x.VillageGenerator.Uid == villageID);
        }

        public void AddVillageStructure(VillageGenerator village)
        {
            villages.Add(new VillageStructure(village));
        }

        public void SetIsland(IslandGenerator island)
        {
            islandGenerator = island;
        }


        public void AddStructure(Structure structure)
        {
            if (structure != null)
            {
                structures.Add(structure);
            }
        }


        public void RemoveStructure(Structure structure)
        {
            structures.Remove(structure);
        }

        public PlayerStructure GetPlayerStructure(string uid)
        {
            return playerStructures.Find(x => x.Uid == uid);
        }

        public void DestroyPlayerBuilding(string id)
        {
            var build = playerStructures.Find(x => x.Uid == id);

            if (build.IsSpawned)
            {
                var structure = build.GameObject.GetComponent<Structure>();
                RemoveStructure(structure);
            }

            build.DestroyBuilding();


            playerStructures.Remove(build);
        }

        public void AddPlayerBuilding(PlayerStructure p)
        {
            playerStructures.Add(p);
        }
    }
}
