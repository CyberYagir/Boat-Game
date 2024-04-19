using System.Collections;
using Content.Scripts.Global;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UITutorialsDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject rendererPart;
        [SerializeField] private GameObject canvas;

        [SerializeField] private TMP_Text text;
        [SerializeField] private string highlightColor;

        private bool isTextDisplayed = false;
        
        private bool nextPhraseFlag = false;
        
        private TutorialDialogObject textObject;

        public bool IsTextDisplayed => isTextDisplayed;

        public void DrawDialogue(TutorialDialogObject textObject)
        {
            this.textObject = textObject;
            rendererPart.gameObject.SetActive(true);
            canvas.gameObject.SetActive(true);

            StartCoroutine(DrawPhrases());
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
            
            rendererPart.gameObject.SetActive(false);
            canvas.gameObject.SetActive(false);
            isTextDisplayed = false;
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
