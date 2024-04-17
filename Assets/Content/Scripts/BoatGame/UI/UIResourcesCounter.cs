using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIResourcesCounter : MonoBehaviour
    {
        [SerializeField] private UIResourcesCounterGroup counterGroupPrefab;
        [SerializeField] private RectTransform holder, scroll;
        [SerializeField] private float maxY = 775;
        
        private Dictionary<EResourceTypes, UIResourcesCounterGroup> headers = new Dictionary<EResourceTypes, UIResourcesCounterGroup>(5);
        private IEnumerator waiterCoroutine;
        private ResourcesService resourcesService;
        private RaftBuildService raftBuildService;

        public void Init(RaftBuildService raftBuildService, GameDataObject gameData, ResourcesService resourcesService, TickService tickService)
        {
            this.raftBuildService = raftBuildService;
            this.resourcesService = resourcesService;
            
            raftBuildService.OnChangeRaft += UpdateRaftEvents;
            
            
            counterGroupPrefab.gameObject.SetActive(true);

            var enums = Enum.GetNames(typeof(EResourceTypes));
            for (int i = 0; i < enums.Length; i++)
            {
                var en = (EResourceTypes)Enum.Parse(typeof(EResourceTypes), enums[i]);
                
                Instantiate(counterGroupPrefab, counterGroupPrefab.transform.parent)
                    .With(x=>x.Init(this, gameData, en))
                    .With(x=>headers.Add(en, x));
            }
            counterGroupPrefab.gameObject.SetActive(false);

            UpdateCounter();

            UpdateRaftEvents();
            
            tickService.OnTick += UpdateCounterSize;
        }

        private void UpdateRaftEvents()
        {
            for (int i = 0; i < raftBuildService.Storages.Count; i++)
            {
                raftBuildService.Storages[i].OnStorageChange -= OnStorageChange;
                raftBuildService.Storages[i].OnStorageChange += OnStorageChange;
            }
        }
        private void UpdateCounterSize(float obj)
        {
            scroll.sizeDelta = new Vector2(scroll.sizeDelta.x, Mathf.Clamp(holder.sizeDelta.y, 0, maxY));
        }

        private void OnStorageChange()
        {
            if (waiterCoroutine == null)
            {
                waiterCoroutine = WaitForUpdateCounter();

                StartCoroutine(WaitForUpdateCounter());
            }
        }

        IEnumerator WaitForUpdateCounter()
        {
            yield return null;

            UpdateCounter();
            
            waiterCoroutine = null;
        }

        public void UpdateCounter()
        {
            resourcesService.PlayerItemsList();
            foreach (var header in headers)
            {
                header.Value.DrawList(resourcesService.AllItemsList.FindAll(x => x.Item.Type == header.Key));
            }
        }

        public void RemoveItem(ItemObject itemObject)
        {
            resourcesService.RemoveItemFromAnyRaft(itemObject);
        }
    }
}
