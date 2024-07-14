using Content.Scripts.Global;
using Content.Scripts.IslandGame.Natives;
using static Content.Scripts.Global.SaveDataObject.MapData.IslandData.VillageData;

namespace Content.Scripts.BoatGame.Services
{
    
    [System.Serializable]
    public class SlaveCreatedCharacterInfo : SlaveCharacterInfo
    {
        private SlaveData slaveData;
        private int cost;
        
        public int Cost => cost;

        public SlaveData SlaveData => slaveData;
        public bool IsDead => slaveData.IsDead;


        public SlaveCreatedCharacterInfo(Character character, ENativeType type, int seed, int islandLevel,string skinID, SlaveData slaveData, int cost) : base(character, type, seed, islandLevel, skinID)
        {
            this.slaveData = slaveData;
            this.cost = cost;
        }

        public void SetSlaveData(SlaveData createdSlave)
        {
            slaveData = createdSlave;
        }
    }
}