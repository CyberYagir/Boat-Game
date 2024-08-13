using Content.Scripts.ItemsSystem;

namespace Content.Scripts.BoatGame
{
    public interface ICharacter
    {
        public void Select(bool state);
        void ActivatePotion(ItemObject storageItem);

        bool IsHaveEffect(PlayerCharacter.CharacterParameters.EffectBonusValueType type);
    }
}