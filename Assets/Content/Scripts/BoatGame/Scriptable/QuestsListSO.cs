using System.Collections.Generic;
using Content.Scripts.QuestsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create QuestsListSO", fileName = "QuestsListSO", order = 0)]
    public class QuestsListSO : ScriptableObject
    {
        [SerializeField] private List<QuestDataObject> questsLists;
        [SerializeField] private List<QuestDataObject> tutorialQuests;


        public QuestDataObject GetQuestDataByID(string id)
        {
            return questsLists.Find(x => x.Uid == id);
        }
    }
}
