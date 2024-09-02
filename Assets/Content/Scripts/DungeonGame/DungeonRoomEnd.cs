using System;
using System.Collections;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using Content.Scripts.DungeonGame.Services;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Content.Scripts.DungeonGame
{
    public class DungeonRoomEnd : MonoBehaviour
    {
        [SerializeField] private GameObject glow, gray;
        private DungeonService dungeonService;
        private ICharacterService characterService;

        private IEnumerator coroutine;

        public event Action<DungeonRoomEnd> OnEnter;

        [SerializeField] private bool isEnter = false;

        [Inject]
        private void Construct(DungeonEnemiesService enemiesService, DungeonService dungeonService, DungeonCharactersService characterService, RoomsPlacerService roomsPlacerService, ScenesService scenesService)
        {
            this.scenesService = scenesService;
            this.roomsPlacerService = roomsPlacerService;
            this.characterService = characterService;
            this.dungeonService = dungeonService;
            enemiesService.OnChangeEnemies += UpdateCounter;
            
            
            UpdateCounter();
        }

        private void UpdateCounter()
        {
            var isDungeonCompleted = (dungeonService.GetDeadMobsCount() / (float) dungeonService.GetMaxMobsCount()) >= 1f;
            glow.gameObject.SetActive(isDungeonCompleted);
            gray.gameObject.SetActive(!isDungeonCompleted);

            if (coroutine == null && isDungeonCompleted)
            {
                StartCoroutine(WaitForPlayer());
            }
        }

        [SerializeField] private bool isNothingInside;
        private RoomsPlacerService roomsPlacerService;
        private ScenesService scenesService;

        IEnumerator WaitForPlayer()
        {
            while (true)
            {
                yield return null;

                var c = characterService.GetSpawnedCharacters();

                isNothingInside = true;
                for (int i = 0; i < c.Count; i++)
                {
                    if (Vector3.Distance(c[i].transform.position, transform.position) < 7f)
                    {
                        isNothingInside = false;    
                        if (!isEnter)
                        {
                            OnEnter?.Invoke(this);
                        }

                        isEnter = true;

                        break;
                    }
                }

                if (isNothingInside)
                {
                    isEnter = false;
                }
            }
        }

        public void EnterBoss()
        {
            StopAllCoroutines();

            scenesService.Fade(delegate
            {
                foreach (var spawnedCharacter in characterService.GetSpawnedCharacters())
                {
                    spawnedCharacter.transform.position = roomsPlacerService.BossPoint.transform.position + new Vector3(Random.Range(-1.5f, 1.5f), 0, Random.Range(-1.5f, 1.5f));
                }
            });
        }
    }
}
