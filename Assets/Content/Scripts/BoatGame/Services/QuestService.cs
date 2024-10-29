using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Scriptable;
using Content.Scripts.Global;
using Content.Scripts.QuestsSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using static Content.Scripts.Global.SaveDataObject.PlayerQuestsData;

namespace Content.Scripts.BoatGame.Services
{
    public class QuestService : MonoBehaviour
    {
        
        [SerializeField] private QuestsEventBus questsEventBus;
        [SerializeField] private QuestsListSO questsList;
        
        
        private List<QuestBase> appliedQuests = new List<QuestBase>();
        private SaveDataObject saveDataObject;
        private QuestBuilder questBuilder;


        public event Action OnQuestsChanged;

        public List<QuestBase> AppliedQuests => appliedQuests;

        [Inject]
        private void Construct(SaveDataObject saveDataObject, QuestBuilder questBuilder)
        {
            this.questBuilder = questBuilder;
            this.saveDataObject = saveDataObject;
            var quests = saveDataObject.QuestsData.GetAllAppliedQuests();
            foreach (var questSaveData in quests)
            {
                var quest = questsList.GetQuestDataByID(questSaveData.Uid);
                BuildQuest(quest, questSaveData);
            }
            OnQuestsChanged?.Invoke();
        }

        [Button]
        public void DebugAddValue(int value = 1)
        {
            for (int i = 0; i < appliedQuests.Count; i++)
            {
                appliedQuests[i].AddValue(value);
            }
        }

        private QuestBase BuildQuest(QuestDataObject questData, QuestSaveData save)
        {
            var buildedQuest = questBuilder.BuildQuest(questsEventBus, questData, save);
            if (buildedQuest != null)
            {
                AppliedQuests.Add(buildedQuest);
                buildedQuest.OnQuestChanged += OnQuestChanged;
                buildedQuest.OnQuestComplete += OnQuestComplete;
            }
            return buildedQuest;
        }

        private void OnQuestComplete(QuestBase obj)
        {
            saveDataObject.PlayerInventory.AddItemsToPlayerStorage(obj.QuestData.GetReward());
            appliedQuests.RemoveAll(x => x.QuestData.Uid == obj.QuestData.Uid);
            saveDataObject.QuestsData.CompleteQuest(obj.QuestData.Uid);
            OnQuestsChanged?.Invoke();

            if (obj.QuestData.NextQuest)
            {
                ApplyQuest(obj.QuestData.NextQuest);
            }
        }

        private void OnQuestChanged(QuestBase obj)
        {
            saveDataObject.QuestsData.UpdateQuest(obj.QuestData.Uid, obj.Value);
        }

        [Button]
        public void ApplyQuest(QuestDataObject quest)
        {
            if (quest.IsOneTimeQuest)
            {
                if (saveDataObject.QuestsData.IsQuestCompleted(quest.Uid) && !saveDataObject.QuestsData.IsQuestApplied(quest.Uid))
                {
                    return;
                }
            }

            var buildedQuest = BuildQuest(quest, null);
            if (buildedQuest != null)
            {
                saveDataObject.QuestsData.ApplyQuest(buildedQuest.QuestData.Uid);
                OnQuestsChanged?.Invoke();
            }
        }
    }
}
