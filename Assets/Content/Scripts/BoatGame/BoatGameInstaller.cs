using Content.Scripts.BoatGame.Services;
using UnityEngine.Events;

namespace Content.Scripts.BoatGame
{
    public class BoatGameInstaller : GameInstaller
    {
        public override void InstallBindings()
        {
            InstallBoatGame();
        }
    }
}
