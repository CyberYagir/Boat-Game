using System;
using System.Collections;
using Content.Scripts.Boot;
using Content.Scripts.Global;
using Content.Scripts.Loading;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Loading
{
    public class LoadingBar : MonoBehaviour
    {
        [SerializeField] private RectTransform bar;
        [SerializeField] private Fader fader;
        private SaveDataObject saveData;
        private ScenesService scenesService;


        [Inject]
        public void Construct(SaveDataObject saveData, ScenesService scenesService)
        {
            this.scenesService = scenesService;
            this.saveData = saveData;
            bar.DORotate(new Vector3(0, 0, 360f), 0.5f, RotateMode.WorldAxisAdd).SetLoops(-1, LoopType.Restart).SetLink(bar.gameObject);
            StartCoroutine(LoadingLoop());
        }

        IEnumerator LoadingLoop()
        {
            yield return new WaitForSeconds(1f);
            if (saveData.Characters.Count == 0)
            {
                scenesService.FadeScene(ESceneName.ManCreator);
            }
            else
            {
                scenesService.FadeScene(ESceneName.BoatGame);
            }
        }

        public void Start()
        {
            Application.targetFrameRate = 60;
        }
    }
}
