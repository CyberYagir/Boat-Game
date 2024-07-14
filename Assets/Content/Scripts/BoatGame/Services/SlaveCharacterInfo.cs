using Content.Scripts.IslandGame.Natives;
using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    [System.Serializable]
    public class SlaveCharacterInfo
    {
        private Character character;
        [SerializeField] private string skinID;
        [SerializeField] private ENativeType type;
        [SerializeField] private int seed;
        [SerializeField] private int islandLevel;

        public SlaveCharacterInfo(Character character, ENativeType type, int seed, int islandLevel, string skinID)
        {
            this.skinID = skinID;
            this.character = character;
            this.type = type;
            this.seed = seed;
            this.islandLevel = islandLevel;
        }
            
        public int IslandLevel => islandLevel;

        public int Seed => seed;

        public ENativeType Type => type;

        public Character Character => character;

        public string SkinID => skinID;
    }
}