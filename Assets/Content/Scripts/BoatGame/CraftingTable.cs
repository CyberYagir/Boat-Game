using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class CraftingTable : MonoBehaviour
    {
        private float time;
        private CraftObject currentCraft;

        private float maxTime;


        public CraftObject CurrentCraft => currentCraft;

        public float CraftingTimeTime => maxTime;


        public void StartCraft(CraftObject currentCraft, float skillValue)
        {
            this.currentCraft = currentCraft;
            maxTime = currentCraft.CraftTime * skillValue;
            time = 0;
        }


        public void ClearCraft()
        {
            currentCraft = null;
            time = 0;
        }
    }
}
