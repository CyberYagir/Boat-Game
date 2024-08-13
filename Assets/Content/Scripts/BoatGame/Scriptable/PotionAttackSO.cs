using Content.Scripts.ItemsSystem;
using UnityEngine;
using static Content.Scripts.BoatGame.PlayerCharacter.CharacterParameters;

namespace Content.Scripts.BoatGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create PotionAttackSO", fileName = "PotionAttackSO", order = 0)]
    class PotionAttackSO : PotionLogicBaseSO
    {
        public override void StartEffect(PlayerCharacter playerCharacter, ItemObject item)
        {
            base.StartEffect(playerCharacter, item);
            playerCharacter.AppearanceDataManager.PlayAttackParticles(true);
        }

        public override void StopEffect()
        {
            base.StopEffect();
            playerCharacter.AppearanceDataManager.PlayAttackParticles(false);
        }

        public override void AddEffectBonus()
        {
            playerCharacter.ParametersCalculator.AddEffectMult(GetPotionBonusValue(), Multiply);
        }

        public override EffectBonusValueType GetPotionBonusValue() => EffectBonusValueType.Attack;
    }
} 