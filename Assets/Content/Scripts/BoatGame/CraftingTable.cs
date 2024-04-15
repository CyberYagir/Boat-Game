using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.CraftsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class CraftingTable : MonoBehaviour
    {
        [SerializeField] private UIBarClockwise bar;
        
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
            
            bar.gameObject.SetActive(true);
            bar.UpdateBar(0);
        }

        public void UpdateCraftingTable(float timer)
        {
            time = timer;
            bar.UpdateBar(time/maxTime);
        }


        public void ClearCraft()
        {
            bar.gameObject.SetActive(false);
            currentCraft = null;
            time = 0;
        }
    }
}
