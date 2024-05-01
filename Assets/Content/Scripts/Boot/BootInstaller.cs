using UnityEngine;
using Zenject;

namespace Content.Scripts.Boot
{
    public class BootInstaller : MonoInstaller
    {
        [SerializeField] private ESceneName loadingScene = ESceneName.Loading;

        public override void InstallBindings()
        {
        }

        public override void Start()
        {
            Container.Resolve<ScenesService>().ChangeScene(loadingScene);
        }
    }
}