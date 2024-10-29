using Content.Scripts.BoatGame.Services;

namespace Content.Scripts.QuestsSystem
{
    public class QuestBuildType : QuestBase
    {
        protected RaftBuildService.RaftItem.ERaftType raftType;

        public override void InitQuest(QuestsEventBus eventBus, QuestDataObject questDataObject, int value)
        {
            base.InitQuest(eventBus, questDataObject, value);
            eventBus.OnRaftBuild += CheckRaft;
        }

        public override void ClaimAndDispose()
        {
            base.ClaimAndDispose();
            eventBus.OnRaftBuild -= CheckRaft;
        }

        public void CheckRaft(RaftBuildService.RaftItem.ERaftType type)
        {
            if (type == raftType)
            {
                AddValue();
            }
        }
    }
}