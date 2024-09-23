using Content.Scripts.BoatGame.Scriptable;
using Content.Scripts.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UISoulsShopItem: MonoBehaviour
    {
        [SerializeField] private RawImage rawImage;
        [SerializeField] private UICraftSubItem subItem;
        [SerializeField] private TMP_Text soulsCount;
        [SerializeField] private UICustomButton buyButton;
        
        private SoulsResourceContainerSO container;
        private SaveDataObject saveDataObject;
        private UISoulsShopWindow window;

        public void Init(SoulsResourceContainerSO it, SaveDataObject saveDataObject, UISoulsShopWindow window)
        {
            this.window = window;
            this.saveDataObject = saveDataObject;
            container = it;
            subItem.gameObject.SetActive(true);
            for (int i = 0; i < it.Resources.Length; i++)
            {
                var i1 = i;
                Instantiate(subItem, subItem.transform.parent)
                    .With(x => x.Init(0, it.Resources[i1].Item.ItemIcon))
                    .With(x => x.DrawValues(it.Resources[i1].Count));
            }
            subItem.gameObject.SetActive(false);
            soulsCount.text = it.SoulsCount.ToString();
            
            buyButton.Button.onClick.AddListener(Buy);
            
            UpdateItem();
        }

        public void UpdateItem()
        {
            buyButton.SetInteractable(container.SoulsCount <= saveDataObject.CrossGame.SoulsCount);
        }

        public void ApplyRenderTexture(RenderTexture renderTexture)
        {
            rawImage.texture = renderTexture;
        }

        public void Buy()
        {
            window.BuyContainer(container);
        }
    }
}