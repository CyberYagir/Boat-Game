namespace Content.Scripts.QuestsSystem
{
    public class QuestUpgradeLevel : QuestBase
    {
        public override void InitQuest(QuestsEventBus eventBus, QuestDataObject questDataObject, int value)
        {
            base.InitQuest(eventBus, questDataObject, value);
            eventBus.OnSkillUpgraded += AddValue;
        }

        public override void ClaimAndDispose()
        {
            base.ClaimAndDispose();
            eventBus.OnSkillUpgraded -= AddValue;
        }
    }
}