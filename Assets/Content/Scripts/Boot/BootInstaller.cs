using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Boot
{
    public class BootInstaller : MonoInstaller
    {
        [SerializeField] private string loadingScene;
        public override void InstallBindings()
        {
            SceneManager.LoadScene(loadingScene);
        }
    }
}