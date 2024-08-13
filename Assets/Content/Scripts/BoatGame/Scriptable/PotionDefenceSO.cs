using Content.Scripts.ItemsSystem;
using UnityEngine;
using static Content.Scripts.BoatGame.PlayerCharacter.CharacterParameters;

namespace Content.Scripts.BoatGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create PotionDefenceSO", fileName = "PotionDefenceSO", order = 0)]
    class PotionDefenceSO : PotionLogicBaseSO
    {
        public override void StartEffect(PlayerCharacter playerCharacter, ItemObject item)
        {
            base.StartEffect(playerCharacter, item);
            playerCharacter.AppearanceDataManager.PlayDefenceParticles(true);
        }

        public override void StopEffect()
        {
            base.StopEffect();
            playerCharacter.AppearanceDataManager.PlayDefenceParticles(false);
        }

        public override void AddEffectBonus()
        {
            playerCharacter.ParametersCalculator.AddEffectMult(GetPotionBonusValue(), Multiply);
        }
        
        
        public override EffectBonusValueType GetPotionBonusValue() => EffectBonusValueType.Defence;
    }
}