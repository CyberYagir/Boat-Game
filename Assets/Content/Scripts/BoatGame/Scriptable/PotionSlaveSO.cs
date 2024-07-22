using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create SlavePotion", fileName = "SlavePotion", order = 0)] 
    class PotionSlaveSO : PotionLogicBaseSO
    {
        [SerializeField] private int xpAdd = 10;
        public override void StartEffect(PlayerCharacter playerCharacter, ItemObject item)
        {
            base.StartEffect(playerCharacter, item);
            playerCharacter.NeedManager.AddParameters(sender.ParametersData);
            playerCharacter.AppearanceDataManager.PlayHealSlaveParticles();
            playerCharacter.AddExp(xpAdd);
        }
    }
}