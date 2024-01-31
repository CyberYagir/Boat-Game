using UnityEngine;
using Zenject;

namespace Content.Scripts.Boot
{
    public class BootInstaller : MonoInstaller
    {
        [SerializeField] private string loadingScene;
        public override void InstallBindings()
        {
            Container.Resolve<ScenesService>().ChangeScene(loadingScene);
        }
    }
}