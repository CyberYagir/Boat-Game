using Content.Scripts.BoatGame.Services;
using Zenject;

namespace Content.Scripts.QuestsSystem
{
    public class QuestBuildStorage : QuestBuildType
    {
        [Inject]
        private void Construct(IRaftBuildService raftBuildService)
        {
            if (raftBuildService.Storages.Count >= 2)
            {
                AddValue();
            }
        }
        
        public override void InitQuest(QuestsEventBus eventBus, QuestDataObject questDataObject, int value)
        {
            base.InitQuest(eventBus, questDataObject, value);
            raftType = RaftBuildService.RaftItem.ERaftType.Storage;
        }
    }
}