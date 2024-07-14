using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Natives;
using UnityEngine;
using static Content.Scripts.Global.SaveDataObject.MapData.IslandData.VillageData;

namespace Content.Scripts.BoatGame.UI
{
    [System.Serializable]
    public class DisplayCharacter
    {
        [SerializeField] private UIVillageSlavesHolder display;
        [SerializeField] private GameObject prefab;
        [SerializeField] private string description;
        

        public DisplayCharacter(GameObject prefab, string description)
        {
            this.prefab = prefab;
            this.description = description;
        }
        public UIVillageSlavesHolder Display => display;

        public string Description => description;

        public void SetDisplay(UIVillageSlavesHolder uiVillageSlavesHolder, SlaveData slaveData)
        {
            uiVillageSlavesHolder.Init(prefab, slaveData);
            display = uiVillageSlavesHolder;
        }
    }
}