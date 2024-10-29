using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.Global
{
    public partial class SaveDataObject
    {
        [System.Serializable]
        public class PlayerQuestsData
        {
            [System.Serializable]
            public class QuestSaveData
            {
                [SerializeField] private string uid;
                [SerializeField] private int value;

                public QuestSaveData(string uid, int value)
                {
                    this.uid = uid;
                    this.value = value;
                }

                public int Value => value;

                public string Uid => uid;

                public void SetValue(int i)
                {
                    value = i;
                }
            }

            [SerializeField] private List<QuestSaveData> appliedQuests = new List<QuestSaveData>();
            [SerializeField] private List<string> completedQuests = new List<string>();


            public void ApplyQuest(string uid)
            {
                appliedQuests.Add(new QuestSaveData(uid, 0));
            }

            public void UpdateQuest(string uid, int value)
            {
                var quest = appliedQuests.Find(x => x.Uid == uid);
                if (quest != null)
                {
                    quest.SetValue(value);
                }
            }

            public void CompleteQuest(string uid)
            {
                completedQuests.Add(uid);
                appliedQuests.RemoveAll(x => x.Uid == uid);
            }

            public bool IsQuestCompleted(string uid)
            {
                return completedQuests.Contains(uid);
            }

            public List<QuestSaveData> GetAllAppliedQuests()
            {
                return appliedQuests;
            }

            public bool IsQuestApplied(string questUid)
            {
                return appliedQuests.Find(x => x.Uid == questUid) != null;
            }
        }
    }
}