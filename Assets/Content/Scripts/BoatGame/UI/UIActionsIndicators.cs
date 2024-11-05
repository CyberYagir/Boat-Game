using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIActionsIndicators : MonoBehaviour
    {
        private CharacterService characterService;
        [SerializeField] private UICharacterIndicator characterArrow;
        [SerializeField] private UIActionIndicator actionsArrow;
        [SerializeField] private List<Color> colors = new List<Color>();

        private List<UICharacterIndicator> characterIndicators = new List<UICharacterIndicator>();
        private List<UIActionIndicator> actionsIndicators = new List<UIActionIndicator>();

        public void Init(CharacterService characterService, SelectionService selectionService)
        {
            this.characterService = characterService;

            int color = 0;
            foreach (var ch in characterService.SpawnedCharacters)
            {
                var targetColorID = color;
                Instantiate(characterArrow, characterArrow.transform.parent)
                    .With(x => x.Init(ch, selectionService.Camera, colors[targetColorID]))
                    .With(x => characterIndicators.Add(x));
                
                Instantiate(actionsArrow, actionsArrow.transform.parent)
                    .With(x => x.Init(ch, selectionService.Camera, colors[targetColorID]))
                    .With(x => actionsIndicators.Add(x));

                color++;
            }
            
            
        }


        private void Update()
        {
            for (int i = 0; i < characterIndicators.Count; i++)
            {
                characterIndicators[i].UpdateItem(); 
            }
            
            for (int i = 0; i < actionsIndicators.Count; i++)
            {
                actionsIndicators[i].UpdateItem(); 
            }
        }
    }
}
