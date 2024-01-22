using Content.Scripts.BoatGame.Services;
using Content.Scripts.Game;
using Zenject;

namespace Content.Scripts.ManCreator
{
    public class ManCreatorInstaller : MonoBinder
    {
        public override void InstallBindings()
        {
            BindService<CharacterCustomizationService>();
            BindService<ManCreatorUIService>();
        }
    }
}