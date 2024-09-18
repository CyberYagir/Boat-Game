using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using Content.Scripts.Global;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIDeathWindow : AnimatedWindow
    {
        [SerializeField] private UITooltip soulsTooltip;
        [SerializeField] private TooltipDataObject soulsTooltipText;
        [SerializeField] private TMP_Text soulsCounter;
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private TextAsset text;
        [SerializeField] private List<string> hints;
        private SaveDataObject saveDataObject;
        private ScenesService scenesService;
        private ICharacterService characterService;

        public void Init(ICharacterService characterService, SaveDataObject saveDataObject, ScenesService scenesService)
        {
            this.characterService = characterService;
            this.scenesService = scenesService;
            this.saveDataObject = saveDataObject;
            
            soulsTooltip.Init(soulsTooltipText);
            soulsCounter.text = 000.ToString("000");
            
            if (characterService.GetSpawnedCharacters().Count == 0)
            {
                ShowWindow();
            }

            characterService.OnCharactersChange += ShowWindow;
        }


        public override void ShowWindow()
        {
            if (characterService.GetSpawnedCharacters().Count == 0)
            {
                DOVirtual.DelayedCall(2f, delegate
                {
                    base.ShowWindow();
                    hints = text.LinesToList();
                    hintText.text = hints.GetRandomItem();
                    hintText.SetAlpha(0);
                    hintText.DOFade(0.5f, 2f).SetDelay(1);
                    DOVirtual.Float(0, 1f, 1f, delegate(float value) { soulsCounter.text = Mathf.RoundToInt(saveDataObject.CrossGame.SoulsCount * value).ToString("000"); }).SetDelay(0.5f);
                });


                saveDataObject.DeleteFile(true);
                saveDataObject.LoadFile();
            }
        }


        public void NextButton()
        {
            scenesService.FadeScene(ESceneName.Boot);
        }
    }
}
