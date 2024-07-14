using Content.Scripts.Global;
using Content.Scripts.IslandGame.Natives;
using Sirenix.OdinInspector;
using UnityEngine;
using static Content.Scripts.Global.SaveDataObject.MapData.IslandData.VillageData;

namespace Content.Scripts.BoatGame.Services
{
    
    [System.Serializable]
    public class SlaveCreatedCharacterInfo : SlaveCharacterInfo
    {
        [SerializeField, ReadOnly] private string characterId; //debug only
        private SlaveData slaveData;
        private int cost;
        
        public int Cost => cost;

        
        public SlaveData SlaveData => slaveData;
        public bool IsDead => slaveData.IsDead;


        public SlaveCreatedCharacterInfo(Character character, ENativeType type, int seed, int islandLevel,string skinID, SlaveData slaveData, int cost) : base(character, type, seed, islandLevel, skinID)
        {
            characterId = character.Uid;
            this.slaveData = slaveData;
            this.cost = cost;
        }

        public void SetSlaveData(SlaveData createdSlave)
        {
            slaveData = createdSlave;
        }
    }
}