using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create HealingPotion", fileName = "HealingPotion", order = 0)]
    class PotionHealingSO : PotionLogicBaseSO
    {
        public override void StartEffect(PlayerCharacter playerCharacter, ItemObject item)
        {
            base.StartEffect(playerCharacter, item);
            playerCharacter.NeedManager.AddParameters(sender.ParametersData);
            playerCharacter.AppearanceDataManager.PlayHealParticles();
        }
    }
}