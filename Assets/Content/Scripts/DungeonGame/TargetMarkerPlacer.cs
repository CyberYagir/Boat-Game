using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.DungeonGame.Services;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame
{
    public class TargetMarkerPlacer : MonoBehaviour
    {
        [SerializeField] private GameObject mark;
        private DungeonCharactersService charactersService;

        [Inject]
        private void Construct(DungeonSelectionService selectionService, DungeonCharactersService charactersService)
        {
            this.charactersService = charactersService;
            selectionService.OnPointChange += OnSelectionChange;
            mark.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (!InputService.IsLMBPressed)
            {
                foreach (var c in charactersService.SpawnedCharacters)
                {
                    if (Vector3.Distance(c.transform.position.RemoveY(), mark.transform.position.RemoveY()) <= 1.5f)
                    {

                        mark.transform.DOKill();
                        mark.transform.DOScale(Vector3.zero, 0.2f).onComplete += delegate { mark.transform.gameObject.SetActive(false); };
                        return;
                    }
                }
            }
        }

        private void OnSelectionChange(Vector3 pos)
        {
            mark.gameObject.SetActive(true);
            mark.transform.position = pos;
            if (InputService.IsLMBDown)
            {
                mark.transform.DOKill();
                mark.transform.localScale = Vector3.zero;
                mark.transform.DOScale(Vector3.one, 0.2f);
            }
        }
    }
}
