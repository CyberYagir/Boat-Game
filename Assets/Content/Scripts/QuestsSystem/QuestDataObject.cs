using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.ItemsSystem;
using Content.Scripts.Mobs;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Content.Scripts.QuestsSystem
{
    [CreateAssetMenu(menuName = "Create QuestDataObject", fileName = "QuestDataObject", order = 0)]
    public class QuestDataObject : ScriptableObject
    {
        [System.Serializable]
        public class AdditionalQuestItem
        {
            [SerializeField] private string key;
            [SerializeField] private Object data;

            public Object Data => data;

            public string Key => key;
        }
        
        [SerializeField] 
        private string uid;
        
        [SerializeField, FoldoutGroup("Quest Data")]
        private string questName;
        
        [SerializeField, PreviewField, FoldoutGroup("Quest Data")] 
        private Sprite icon;
        
        [SerializeField, FoldoutGroup("Quest Data")] 
        private bool isOneTimeQuest;

        [SerializeField, TextArea, FoldoutGroup("Quest Data")]
        private string description;

        [SerializeField, FoldoutGroup("Quest Data")]
        private List<AdditionalQuestItem> additionalQuestItems;

        [SerializeField] private int maxValue;
        [SerializeField] private QuestDataObject nextQuest;
        [SerializeField] private DropTableObject reward;
        
        [SerializeField, ValueDropdown("@Content.Scripts.QuestsSystem.QuestBuilder.GetQuestTypesEditor()")] private string questClass;

        public string QuestClass => questClass;

        public string Description => description;

        public int MaxValue => maxValue;

        public Sprite Icon => icon;

        public string QuestName => questName;

        public string Uid => uid;

        public bool IsOneTimeQuest => isOneTimeQuest;

        public QuestDataObject NextQuest => nextQuest;

        [Button]
        public void GenerateID()
        {
            uid = Guid.NewGuid().ToString();
        }

        public List<RaftStorage.StorageItem> GetReward()
        {
            return reward.GetItemsIterated(new System.Random().Next(1, 3));
        }

        public Object GetDataByKey(string key)
        {
            return additionalQuestItems.Find(x => x.Key == key).Data;
        }
    }
}
