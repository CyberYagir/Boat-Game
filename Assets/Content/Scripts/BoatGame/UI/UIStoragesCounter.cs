using System.Collections;
using System.Linq;
using Content.Scripts.BoatGame.Services;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIStoragesCounter : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private TMP_Text text;
        private RaftBuildService raftBuildService;
        private float startScale;

        public void Init(RaftBuildService raftBuildService)
        {
            this.raftBuildService = raftBuildService;
            raftBuildService.OnChangeRaft += UpdateRaftsEvent;
            startScale = transform.localScale.x;
            UpdateRaftsEvent();
        }

        private void UpdateRaftsEvent()
        {
            for (int i = 0; i < raftBuildService.Storages.Count; i++)
            {
                raftBuildService.Storages[i].OnStorageChange -= UpdateStorage;
                raftBuildService.Storages[i].OnStorageChange += UpdateStorage;
            }

            UpdateStorage();
        }

        private void UpdateStorage()
        {
            if (!isStoragesUpdateInQueue)
                StartCoroutine(UpdateStorageStack());
        }

        private void UpdateVisuals()
        {
            var max = raftBuildService.Storages.Sum(x => x.MaxItemsCount);
            var inside = raftBuildService.Storages.Sum(x => x.MaxItemsCount - x.GetEmptySlots());
            
            

            canvas.enabled = max != 0 && inside != 0;
            text.text = inside + "<b>/</b>" + max;


            transform.DOKill();
            transform.localScale = Vector3.one * startScale;
            transform.DOPunchScale(Vector3.one * 0.1f, 0.25f).SetUpdate(true);
        }

        private bool isStoragesUpdateInQueue;

        IEnumerator UpdateStorageStack()
        {
            isStoragesUpdateInQueue = true;
            yield return null;
            UpdateVisuals();
            isStoragesUpdateInQueue = false;
        }
    }
}
