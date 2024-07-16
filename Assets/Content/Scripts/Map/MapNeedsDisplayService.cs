using System.Collections;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Content.Scripts.Map
{
    public class MapNeedsDisplayService : MonoBehaviour
    {
        [SerializeField] private GameObject popup;
        [SerializeField] private GameObject damagerPopup;
        [SerializeField] private Image icon;
        private ScenesService scenesService;
        private CharacterService characterService;
        private RaftDamagerService raftDamagerService;

        [Inject]
        private void Construct(ScenesService scenesService)
        {
            this.characterService = CrossSceneContext.GetCharactersService();
            this.raftDamagerService = CrossSceneContext.GetDamagerService();
            this.scenesService = scenesService;
            scenesService.OnChangeActiveScene += OnChangeActiveScene;
            scenesService.OnLoadOtherScene += ClearEvents;
            scenesService.OnUnLoadOtherScene += ClearEvents;
        }



        private void ClearEvents(ESceneName obj)
        {
        
            scenesService.OnChangeActiveScene -= OnChangeActiveScene;
            scenesService.OnLoadOtherScene -= ClearEvents;
            scenesService.OnUnLoadOtherScene -= ClearEvents;
        }

        private void OnChangeActiveScene(ESceneName obj)
        {
            if (obj == ESceneName.Map)
            {
                StartCoroutine(CheckLoop());
            }
            else
            {
                StopAllCoroutines();
            }
        }

        IEnumerator CheckLoop()
        {
            while (true)
            {
                yield return null;

                popup.SetActive(false);
                foreach (var character in characterService.SpawnedCharacters)
                {
                    var sprite = character.NeedManager.GetCurrentIcons(out bool isActive);
                    if (isActive)
                    {
                        icon.sprite = sprite;
                        popup.SetActive(true);
                        break;
                    }
                }

                damagerPopup.gameObject.SetActive(raftDamagerService.IsHaveDamager);
            }
        }
    }
}
