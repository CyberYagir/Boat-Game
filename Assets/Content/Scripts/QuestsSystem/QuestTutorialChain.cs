using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.QuestsSystem
{
    public class QuestTutorialChain : MonoBehaviour
    {
        [SerializeField] private QuestDataObject startQuest;
        [Inject]
        private void Construct(QuestService questService, SaveDataObject saveDataObject)
        {
            if (!saveDataObject.QuestsData.IsQuestApplied(startQuest.Uid))
            {
                questService.ApplyQuest(startQuest);
            }
        }
    }
}
