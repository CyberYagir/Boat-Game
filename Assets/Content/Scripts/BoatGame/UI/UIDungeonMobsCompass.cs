using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.DungeonGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.UI
{
    public class UIDungeonMobsCompass : MonoBehaviour
    {
        [SerializeField] private RectTransform arrowPrefab;
        [SerializeField] private float angle;
        [SerializeField] private AnimationCurve scaleCurve;
        private List<RectTransform> arrows = new List<RectTransform>();
        private DungeonEnemiesService enemiesService;
        private ICharacterService characterService;
        

        public void Init(DungeonEnemiesService enemiesService, ICharacterService characterService)
        {
            this.characterService = characterService;
            this.enemiesService = enemiesService;
            arrowPrefab.gameObject.SetActive(true);
            for (int i = 0; i < enemiesService.MobsCount; i++)
            {
                Instantiate(arrowPrefab, arrowPrefab.transform.parent)
                    .With(x=>arrows.Add(x));
            }
            arrowPrefab.gameObject.SetActive(false);
        }


        private void Update()
        {
            if (enemiesService.MobsCount <= 0)
            {
                enabled = false;
                gameObject.SetActive(false);
                return;
                
            }
            for (int i = 0; i < arrows.Count; i++)
            {
                if (i < enemiesService.MobsCount)
                {
                    arrows[i].gameObject.SetActive(true);
                    if (characterService.GetSpawnedCharacters().Count > 0)
                    {
                        var character = characterService.GetSpawnedCharacters()[0];

                        var pos = (character.transform.position - enemiesService.GetMobByID(i).position);

                        var ang = new Vector2(pos.z, pos.x).ToAngle() + angle;

                        var distance = pos.sqrMagnitude;

                        var scale = scaleCurve.Evaluate(Mathf.Clamp(distance, 0, 300f));


                        arrows[i].transform.localScale = Vector3.one * scale;


                        arrows[i].transform.SetZLocalEulerAngles(ang);
                    }
                }
                else
                {
                    arrows[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
