using System.Collections;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using Content.Scripts.Global;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Loading
{
    public class LoadingBar : MonoBehaviour
    {
        [SerializeField] private RectTransform bar;
        [SerializeField] private TMP_Text version; 
        [SerializeField] private TMP_Text state; 
        private SaveDataObject saveData;
        private ScenesService scenesService;


        [Inject]
        public void Construct(SaveDataObject saveData, ScenesService scenesService)
        {
            this.scenesService = scenesService;
            this.saveData = saveData;
            version.text = Application.version;
            bar.DORotate(new Vector3(0, 0, 360f), 0.5f, RotateMode.WorldAxisAdd).SetLoops(-1, LoopType.Restart).SetLink(bar.gameObject);
            StartCoroutine(LoadingLoop());
        }

        IEnumerator LoadingLoop()
        {
            state.text = "Connecting to server...";
            while (DateService.WebTimeState == DateService.WebTimeType.None)
            {
                yield return null;
            }

            if (DateService.WebTimeState == DateService.WebTimeType.Local)
            {
                state.text = "No connection";
            }
            else
            {
                state.text = "Connected";
            }
            
            
            yield return new WaitForSeconds(1f);
            if (saveData.Characters.Count == 0)
            {
                scenesService.FadeScene(ESceneName.ManCreator);
            }
            else if (saveData.Global.isInDungeon)
            {
                scenesService.FadeScene(ESceneName.DungeonGame);
            }
            else if (saveData.Global.isOnIsland)
            {
                scenesService.FadeScene(ESceneName.IslandGame);
            }
            else
            {
                scenesService.FadeScene(ESceneName.BoatGame);
            }
        }

        public void Start()
        {
#if UNITY_EDITOR
            Application.targetFrameRate = 60;
#else
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
#endif
        }
    }
}
