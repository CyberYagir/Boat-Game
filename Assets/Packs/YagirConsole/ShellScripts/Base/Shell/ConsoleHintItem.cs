using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YagirConsole.Scripts.Base.Shell
{
    public class ConsoleHintItem : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image background;

        [SerializeField] private bool selected;

        public bool Selected => selected;

        public void Select()
        {
            text.DOColor(Color.gray, 0.2f);
            background.DOColor(Color.white, 0.2f);
            CursorManager.SetCursor(CursorManager.CursorState.Hand);
            selected = true;
        }

        public void Deselect()
        {
            text.DOColor(Color.white, 0.2f);
            background.DOColor(Color.clear, 0.2f);
            CursorManager.SetCursor(CursorManager.CursorState.Pointer);
            selected = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Select();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Deselect();
        }

        public void SetText(string text)
        {
            this.text.text = text;
        }

        public string GetText()
        {
            return text.text;
        }
    }
}
