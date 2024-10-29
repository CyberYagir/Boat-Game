using Content.Scripts.BoatGame.Services;
using Zenject;

namespace Content.Scripts.QuestsSystem
{
    public class QuestBuildAnchor : QuestBuildType
    {
        [Inject]
        private void Construct(IRaftBuildService raftBuildService)
        {
            if (raftBuildService.SpawnedRafts.Find(x=>x.RaftType == raftType) != null)
            {
                AddValue();
            }
        }
        public override void InitQuest(QuestsEventBus eventBus, QuestDataObject questDataObject, int value)
        {
            base.InitQuest(eventBus, questDataObject, value);
            raftType = RaftBuildService.RaftItem.ERaftType.Moored;
        }
    }
}