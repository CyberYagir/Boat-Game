using System.Collections.Generic;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageManageSubWindow : MonoBehaviour
    {
        [SerializeField] private UIVillageManageCharItem charactersListItem;
        [SerializeField] private GameObject infoWindow;
        
        
        private List<UIVillageManageCharItem> charactersListItems = new List<UIVillageManageCharItem>();
        private UIVillageSlavesVisualsGenerator generator;
        private SaveDataObject.MapData.IslandData.VillageData villageData;
        private DisplayCharacter selectedCharacter;
        
        
        public void Init(UIVillageSlavesVisualsGenerator generator, SaveDataObject.MapData.IslandData.VillageData villageData)
        {
            this.villageData = villageData;
            this.generator = generator;

            Redraw();
        }

        public void SelectCharacter(DisplayCharacter character)
        {
            selectedCharacter = character;
            Redraw();
        }


        public void Redraw()
        {
            RedrawSlavesList();
            UpdateSlavesList();

            infoWindow.SetActive(selectedCharacter != null);
        }

        private void RedrawSlavesList()
        {
            charactersListItem.gameObject.SetActive(true);
            for (int i = 0; i < villageData.SlavesCount() - charactersListItems.Count; i++)
            {
                Instantiate(charactersListItem, charactersListItem.transform.parent)
                    .With(x => charactersListItems.Add(x));
            }

            charactersListItem.gameObject.SetActive(false);
        }

        private void UpdateSlavesList()
        {
            for (int i = 0; i < charactersListItems.Count; i++)
            {
                charactersListItems[i].gameObject.SetActive(i < villageData.SlavesCount());
                if (i < villageData.SlavesCount())
                {
                    charactersListItems[i].Init(generator.Characters[i], this, selectedCharacter == generator.Characters[i]);
                }
            }
        }
    }
}
