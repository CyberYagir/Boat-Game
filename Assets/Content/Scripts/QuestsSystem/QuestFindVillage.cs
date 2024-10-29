namespace Content.Scripts.QuestsSystem
{
    public class QuestFindVillage : QuestBase
    {
        public override void InitQuest(QuestsEventBus eventBus, QuestDataObject questDataObject, int value)
        {
            base.InitQuest(eventBus, questDataObject, value);
            eventBus.OnFindVillage += AddValue;
        }

        public override void ClaimAndDispose()
        {
            base.ClaimAndDispose();
            eventBus.OnFindVillage -= AddValue;
        }
    }
}