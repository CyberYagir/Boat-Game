using Content.Scripts.Boot;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame
{
    public class Moored : MonoBehaviour
    {
        [SerializeField] private GameObject isMoored;
        [SerializeField] private GameObject isRaft;
        [Inject]
        private void Construct(ScenesService scenesService)
        {
            if (scenesService.GetActiveScene() == ESceneName.IslandGame)
            {
                isMoored.gameObject.SetActive(true);
                isRaft.gameObject.SetActive(false);
            }

            if (scenesService.GetActiveScene() == ESceneName.BoatGame)
            {
                isMoored.gameObject.SetActive(false);
                isRaft.gameObject.SetActive(true);
            }
        }
    }
}
