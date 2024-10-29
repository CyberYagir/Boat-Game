using System;
using UnityEngine;

namespace Content.Scripts.QuestsSystem
{
    public class QuestBase
    {
        private QuestDataObject questDataObject;
        private int value = 0;
        private bool isRewardClaimed;
       
        protected QuestsEventBus eventBus;

        public bool IsCompleted => Value >= QuestData.MaxValue;

        public bool IsRewardClaimed => isRewardClaimed;

        public QuestDataObject QuestData => questDataObject;

        public int Value => value;

        public event Action<QuestBase> OnQuestChanged;
        public event Action<QuestBase> OnQuestComplete;

        
        public virtual void InitQuest(QuestsEventBus eventBus, QuestDataObject questDataObject, int value)
        {
            this.eventBus = eventBus;
            this.questDataObject = questDataObject;
            this.value = value;
        }

        public virtual void AddValue()
        {
            if (this.value  > QuestData.MaxValue) return;
            value++;
            OnQuestChanged?.Invoke(this);
        }
        
        public virtual void AddValue(int value)
        {
            if (this.value + value  > QuestData.MaxValue) return;
            this.value += value;
            OnQuestChanged?.Invoke(this);
        }

        public virtual void ClaimAndDispose()
        {
            OnQuestComplete?.Invoke(this);
            isRewardClaimed = true;
        }
    }
}
