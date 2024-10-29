namespace Content.Scripts.QuestsSystem
{
    public class QuestHookItems : QuestBase
    {
        public override void InitQuest(QuestsEventBus eventBus, QuestDataObject questDataObject, int value)
        {
            base.InitQuest(eventBus, questDataObject, value);
            eventBus.OnHookedItem += AddValue;
        }

        public override void ClaimAndDispose()
        {
            base.ClaimAndDispose();
            eventBus.OnHookedItem -= AddValue;
        }
    }
}