using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    [System.Serializable]
    public class DisplayCharacter
    {
        [SerializeField] private Character character;
        [SerializeField] private UIVillageSlavesHolder display;
        [SerializeField] private SaveDataObject.MapData.IslandData.VillageData.SlaveData slaveData;
        [SerializeField] private GameObject prefab;
        [SerializeField] private string description;
        [SerializeField] private int cost;

        public bool isDead => slaveData.IsDead;
        

        public DisplayCharacter(Character character, GameObject prefab,SaveDataObject.MapData.IslandData.VillageData.SlaveData slaveData,  string description, int cost)
        {
            this.slaveData = slaveData;
            this.cost = cost;
            this.character = character;
            this.prefab = prefab;
            this.description = description;
        }

        public GameObject Prefab => prefab;

        public UIVillageSlavesHolder Display => display;

        public Character Character => character;

        public string Description => description;

        public int Cost => cost;

        public void SetDisplay(UIVillageSlavesHolder uiVillageSlavesHolder)
        {
            uiVillageSlavesHolder.Init(prefab, slaveData);
            display = uiVillageSlavesHolder;
        }
    }
}