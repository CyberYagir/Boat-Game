using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.ItemsSystem;
using UnityEngine;
using Zenject;
using static Content.Scripts.BoatGame.Services.RaftBuildService.RaftItem;

namespace Content.Scripts.QuestsSystem
{
    public class QuestsEventBus : MonoBehaviour
    {
        private static QuestsEventBus Instance;
        public event Action OnHookedItem;
        public event Action<ERaftType> OnRaftBuild;
        public event Action OnCatchFish;
        public event Action OnSkillUpgraded;
        public event Action<ItemObject, int> OnCraftingEnd;
        public event Action OnLandIsland;
        public event Action OnFindVillage;

        [Inject]
        private void Construct()
        {
            Instance = this;
        }

        public static void CallHookedItem()
        {
            Instance.OnHookedItem?.Invoke();
        }

        public static void CallRaftBuild(ERaftType type)
        {
            if (Instance == null) return;
            Instance.OnRaftBuild?.Invoke(type);
        }
        
        public static void CallCatchFish()
        {
            if (Instance == null) return;
            Instance.OnCatchFish?.Invoke();
        }
        
        public static void CallUpgradeSkill()
        {
            if (Instance == null) return;
            Instance.OnSkillUpgraded?.Invoke();
        }
        
        public static void CallOnCraftEnd(ItemObject item, int count)
        {
            if (Instance == null) return;
            Instance.OnCraftingEnd?.Invoke(item, count);
        }
        
        public static void CallOnLandIsland()
        {
            if (Instance == null) return;
            Instance.OnLandIsland?.Invoke();
        }

        public static void CallFindVillage()
        {
            if (Instance == null) return;
            Instance.OnFindVillage?.Invoke();
        }
    }
}
