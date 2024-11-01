using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using Content.Scripts.QuestsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIQuestsOverlay : MonoBehaviour
    {
        [System.Serializable]
        public class QuestDrawer
        {
            [SerializeField] private UIQuestItem item;
            [SerializeField] private RectTransform holder;

            private List<UIQuestItem> spawnedItems = new List<UIQuestItem>();
            public Transform Holder => holder;

            public void Redraw(List<QuestBase> questServiceAppliedQuests)
            {
                item.gameObject.SetActive(false);
                
                for (int i = 0; i < spawnedItems.Count; i++)
                {
                    if (spawnedItems[i].Animated == false)
                    {
                        spawnedItems[i].gameObject.SetActive(false);
                    }
                }
                for (var i = 0; i < questServiceAppliedQuests.Count; i++)
                {
                    var availableItem = spawnedItems.Find(x=>x.gameObject.activeInHierarchy == false && x.Animated == false);
                    
                    if (availableItem == null)
                    {
                        availableItem = Instantiate(item, item.transform.parent)
                            .With(x=>spawnedItems.Add(x));
                    }

                    availableItem.Dispose();
                    availableItem.Init(questServiceAppliedQuests[i], this);
                    availableItem.gameObject.SetActive(true);
                }
            }
        }

        // [SerializeField] private QuestDrawer windowDrawer;
        [SerializeField] private QuestDrawer shortDrawer;
        [SerializeField] private Canvas canvas;
        private QuestService questService;
        private ScenesService scenesService;


        public void Init(QuestService questService, ScenesService scenesService)
        {
            this.scenesService = scenesService;
            this.questService = questService;
            questService.OnQuestsChanged += Redraw;
            Redraw();
            
            scenesService.OnChangeActiveScene += ScenesServiceOnOnChangeActiveScene;
        }

        private void ScenesServiceOnOnChangeActiveScene(ESceneName obj)
        {
            canvas.enabled = obj != ESceneName.Map;
        }

        private void OnDestroy()
        {
            scenesService.OnChangeActiveScene -= ScenesServiceOnOnChangeActiveScene;
        }

        private void Redraw()
        {
            // windowDrawer.Redraw(questService.AppliedQuests);
            shortDrawer.Redraw(questService.AppliedQuests);
        }
    }
}
