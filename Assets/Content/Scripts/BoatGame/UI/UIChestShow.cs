using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    [System.Serializable]
    public class UIChestShow : MonoBehaviour
    {

        [SerializeField] private Transform holder;
        [SerializeField] private GameObject emptyText, dropText;
        [SerializeField] private List<UIChestItem> items;
        
        
        private GameDataObject gameData;
        private RaftStorage selectedStorage;
        private SelectionService selectionService;

        public void Init(GameDataObject gameData, SelectionService selectionService)
        {
            this.selectionService = selectionService;
            this.gameData = gameData;
            holder.gameObject.SetActive(false);
            selectionService.OnChangeSelectObject += OnChangeSelectObject;
            selectionService.OnChangeSelectCharacter += OnChangeSelectCharacter;
        }

        private void OnChangeSelectCharacter(PlayerCharacter obj)
        {
            Close();
        }

        private void OnChangeSelectObject(ISelectable obj)
        {
            if (obj == null) return;
            if (obj.PlayerActions.Count != 0)
            {
                Close();
                return;
            }

            var storage = obj.Transform.GetComponent<RaftStorage>();

            if (storage == null)
            {
                Close();
                return;
            }

            selectionService.ClearSelectedObject();

            selectedStorage = storage;

            selectedStorage.OnStorageChange -= UpdateSelectedStorage;
            selectedStorage.OnStorageChange += UpdateSelectedStorage;
            var raft = selectedStorage.GetComponent<RaftBase>();
            raft.OnDeath -= OnStorageDestroy;
            raft.OnDeath += OnStorageDestroy;
            Redraw();
        }

        private void OnStorageDestroy(DamageObject obj)
        {
            Close();
        }

        private void UpdateSelectedStorage()
        {
            if (selectedStorage != null)
            {
                Redraw();
            }
            else
            {
                Close();
            }
        }

        public void Redraw()
        {
            if (!holder.gameObject.active)
            {
                holder.DOKill();
                holder.gameObject.SetActive(true);
                holder.transform.localScale = Vector3.zero;
                holder.DOScale(Vector3.one, 0.2f);
            }

            for (int i = 0; i < items.Count; i++)
            {
                items[i].gameObject.SetActive(false);
            }
            
            int id = 0;
            for (int i = 0; i < selectedStorage.Items.Count; i++)
            {
                items[id].gameObject.SetActive(true);
                items[id].Init(selectedStorage, selectedStorage.Items[i].Item, selectedStorage.Items[i].Count);
                id++;
            }

            emptyText.gameObject.SetActive(id == 0);
            dropText.gameObject.SetActive(id <= 7 && id != 0);
        }

        public void Update()
        {
            if (selectedStorage != null && holder.gameObject.active)
            {
                holder.transform.position = selectionService.Camera.WorldToScreenPoint(selectedStorage.transform.position);
            }
        }

        public void Close()
        {
            holder.DOScale(Vector3.zero, 0.2f).onComplete += delegate { holder.gameObject.SetActive(false); };
            if (selectedStorage != null)
            {
                var raft = selectedStorage.GetComponent<RaftBase>();
                selectedStorage.OnStorageChange -= UpdateSelectedStorage;
                raft.OnDeath -= OnStorageDestroy;
                selectedStorage = null;
            }
        }
    }
}