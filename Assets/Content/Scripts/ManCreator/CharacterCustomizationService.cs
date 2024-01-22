using System;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.SkillsSystem;
using UnityEngine;
using Zenject;

namespace Content.Scripts.ManCreator
{
    public class CharacterCustomizationService : MonoBehaviour
    {
        
        [SerializeField] private Renderer characterRenderer;
        [SerializeField] private Transform upperHeadPoint;
        [SerializeField] private HatsHolder hatsHolder;
        [SerializeField] private Camera camera;
        [SerializeField] private int scores = 5;
        [SerializeField] private Character character;
        private GameDataObject gameData;
        private SaveDataObject saveData;        
        
        public Character GetCharacter() => character;



        public event Action OnNameChanged;
        public event Action<int> OnChangeSkillsCount;

        [Inject]
        private void Construct(GameDataObject gameData, SaveDataObject saveData)
        {
            character = new Character();
            this.saveData = saveData;
            this.gameData = gameData;
            ChangeName();
            ChangeSkin(0);
        }

        public Vector3 GetNamePoint()
        {
            return camera.WorldToScreenPoint(upperHeadPoint.transform.position);
        }

        public void ChangeName()
        {
            character.SetName(gameData.NamesList.GetRandomItem());
            OnNameChanged?.Invoke();
        }

        public string GetCharacterName()
        {
            return character.Name;
        }

        public int NextSkinColor()
        {
            if (character.SkinColorID + 1 >= gameData.SkinColors.Count)
            {
                ChangeSkin(0);
                return 0;
            }
            else
            {
                ChangeSkin(character.SkinColorID + 1);
                return character.SkinColorID;
            }
        }
        
        public int NextHat()
        {
            if (character.HatID + 1 >= hatsHolder.HatsCount)
            {
                ChangeHat(0);
                return 0;
            }
            else
            {
                ChangeHat(character.HatID + 1);
                return character.HatID;
            }
        }

        public int PrevHat()
        {
            if (character.HatID - 1 < 0)
            {
                ChangeHat(0);
                return hatsHolder.HatsCount - 1;
            }
            else
            {
                ChangeHat(character.HatID - 1);
                return character.HatID;
            }
        }

        private void ChangeSkin(int id)
        {
            character.SetSkinID(id);
            characterRenderer.material = gameData.SkinColors[id];
        }

        private void ChangeHat(int id)
        {
            character.SetHatID(id);
            hatsHolder.ShowHat(character.HatID);
        }


        public void AddSkill(SkillObject skill, int value)
        {
            if (scores - value >= 0)
            {
                if (character.AddSkillValue(skill.SkillID, value))
                {
                    scores -= value;
                    OnChangeSkillsCount?.Invoke(scores);
                }
            }
        }

        public void ApplyCharacter()
        {
            saveData.Characters.AddCharacter(character);
            saveData.SaveFile();
        }


    }
}
