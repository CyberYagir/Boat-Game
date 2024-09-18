using UnityEngine;
using Zenject;

namespace Content.Scripts.Boot
{
    public class BootLoader : MonoBehaviour
    {
        [SerializeField] private ESceneName loadingScene = ESceneName.Loading;

        [Inject]
        private void Construct(ScenesService scenesService)
        {
            scenesService.ChangeScene(loadingScene);
        }
    }
}