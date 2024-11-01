using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    class UICraftsBuildItem : UICraftsItem
    {
        [SerializeField] private RectTransform heart;
        [SerializeField] private TMP_Text healthText;

        private float heartBeatCooldown;
        
        public override void Init(CraftObject item, IResourcesService resourcesService, UIService uiService, IRaftBuildService raftBuildService, SaveDataObject saveDataObject)
        {
            base.Init(item, resourcesService, uiService, raftBuildService, saveDataObject);

            healthText.text = raftBuildService.GetRaftPrefabByCraft(item).MaxHealth.ToString("F0");

            heartBeatCooldown = Random.Range(0.5f, 1f);
            
            DOVirtual.DelayedCall(Random.Range(0.5f, 1f), HeartBeat);
            
        }

        private void HeartBeat()
        {
            heart.DOPunchScale(Vector3.one / 5f, 0.25f).SetDelay(heartBeatCooldown).onComplete += HeartBeat;
        }
    }
}