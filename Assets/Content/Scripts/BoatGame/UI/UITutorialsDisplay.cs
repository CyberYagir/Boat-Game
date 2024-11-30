using System;
using System.Collections;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UITutorialsDisplay : MonoBehaviour
    {
        [System.Serializable]
        public class CharactersByType
        {
            [SerializeField] private TutorialDialogObject.ECharacter character;
            [SerializeField] private GameObject worldObject;

            public GameObject WorldObject => worldObject;

            public TutorialDialogObject.ECharacter Character => character;
        }
        
        
        [SerializeField] private GameObject rendererPart;
        [SerializeField] private GameObject canvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private CharactersByType[] characters;
        
        [SerializeField] private TMP_Text text;
        [SerializeField] private string highlightColor;

        
        private bool isTextDisplayed = false;
        private bool nextPhraseFlag = false;
        private TutorialDialogObject textObject;
        private TickService tickService;

        public bool IsTextDisplayed => isTextDisplayed;

        public event Action OnDialogueEnded; 
        
        public void Init(TickService tickService)
        {
            this.tickService = tickService;
        }
        
        
        public void DrawDialogue(TutorialDialogObject textObject)
        {
            this.textObject = textObject;
            rendererPart.gameObject.SetActive(true);
            canvas.gameObject.SetActive(true);

            tickService.NormalTime();// .ChangeTimeScale(1);
            
            text.text = "";
            canvasGroup.alpha = 0;

            foreach (var character in characters)
            {
                character.WorldObject.SetActive(character.Character == textObject.Character);
            }
            
            canvasGroup.DOFade(1f, 0.25f).onComplete += delegate
            {
                StartCoroutine(DrawPhrases());
            };
        }


        IEnumerator DrawPhrases()
        {
            isTextDisplayed = true;
            for (int i = 0; i < textObject.Phrases.Count; i++)
            {
                yield return StartCoroutine(DrawPhrase(i));

                while (!nextPhraseFlag)
                {
                    yield return null;
                }

                nextPhraseFlag = false;
            }
            
            OnDialogueEnded?.Invoke();
            canvasGroup.DOFade(0, 0.25f).onComplete += delegate
            {
                rendererPart.gameObject.SetActive(false);
                canvas.gameObject.SetActive(false);
                isTextDisplayed = false;
            };
            
        }

        IEnumerator DrawPhrase(int phraseID)
        {
            text.text = "";
            var str = textObject.Phrases[phraseID];
            str = str.Replace("$color", highlightColor);

            bool isEmoji = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '<')
                {
                    isEmoji = true;
                }

                text.text += str[i];


                if (!isEmoji)
                {
                    yield return new WaitForSeconds(1f / str.Length);
                }

                if (str[i] == '>')
                {
                    isEmoji = false;
                }

                if (nextPhraseFlag)
                {
                    text.text = str;
                    nextPhraseFlag = false;
                    break;
                }
            }
        }


        public void Next()
        {
            nextPhraseFlag = true;
        }


    }
}
