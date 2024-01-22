using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ManCreator;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public partial class PlayerCharacter
    {
        [System.Serializable]
        public class AppearanceManager
        {
            [SerializeField] private Renderer renderer;
            [SerializeField] private GameObject selectedCircle;
            [SerializeField] private HatsHolder hatsHolder;
            public void Init(Character character, GameDataObject gameData)
            {
                renderer.material = gameData.SkinColors[character.SkinColorID];
                hatsHolder.ShowHat(character.HatID);
            }

            public void SetHatState(bool state)
            {
                if (state)
                {
                    DOVirtual.DelayedCall(1f, delegate
                    {
                        hatsHolder.gameObject.SetActive(true);
                    });
                }
                else
                {
                    hatsHolder.gameObject.SetActive(false);
                }
            }


            public void ChangeSelection(bool state)
            {
                selectedCircle.SetActive(state);
            }
        }
    }
}