namespace Content.Scripts.QuestsSystem
{
    public class QuestCatchFish : QuestBase
    {
        public override void InitQuest(QuestsEventBus eventBus, QuestDataObject questDataObject, int value)
        {
            base.InitQuest(eventBus, questDataObject, value);
            eventBus.OnCatchFish += AddValue;
        }

        public override void ClaimAndDispose()
        {
            base.ClaimAndDispose();
            eventBus.OnCatchFish -= AddValue;
        }
    }
}