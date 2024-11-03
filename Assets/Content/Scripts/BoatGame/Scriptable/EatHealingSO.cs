using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create EatPotion", fileName = "EatPotion", order = 0)]
    class EatHealingSO : PotionLogicBaseSO
    {
        public override void StartEffect(PlayerCharacter playerCharacter, ItemObject item)
        {
            base.StartEffect(playerCharacter, item);
            playerCharacter.NeedManager.AddParameters(sender.ParametersData);
        }
    }
}