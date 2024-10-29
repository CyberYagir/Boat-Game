namespace Content.Scripts.QuestsSystem
{
    public class QuestOnIsland : QuestBase
    {
        public override void InitQuest(QuestsEventBus eventBus, QuestDataObject questDataObject, int value)
        {
            base.InitQuest(eventBus, questDataObject, value);
            eventBus.OnLandIsland += AddValue;
        }

        public override void ClaimAndDispose()
        {
            base.ClaimAndDispose();
            eventBus.OnLandIsland -= AddValue;
        }
        
    }
}