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
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private TextAsset text;
        [SerializeField] private List<string> hints;
        private SaveDataObject saveDataObject;
        private ScenesService scenesService;

        public void Init(CharacterService characterService, SaveDataObject saveDataObject, ScenesService scenesService)
        {
            this.scenesService = scenesService;
            this.saveDataObject = saveDataObject;
            if (characterService.SpawnedCharacters.Count == 0)
            {
                ShowWindow();
            }

            characterService.OnCharactersChange += ShowWindow;
        }
        
        
        public override void ShowWindow()
        {
            DOVirtual.DelayedCall(2f, delegate
            {
                base.ShowWindow();
                hints = text.text.Split("\n").ToList();
                hintText.text = hints.GetRandomItem();
                hintText.SetAlpha(0);
                hintText.DOFade(0.5f, 2f).SetDelay(1);
            });

            
            saveDataObject.DeleteFile();
            saveDataObject.LoadFile();
        }


        public void NextButton()
        {
            scenesService.FadeScene(ESceneName.Boot);
        }
    }
}