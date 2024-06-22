using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.UI
{
    public class UICharacterPreview : MonoBehaviour
    {
        [System.Serializable]
        private class AnimationForEquipment
        {
            [SerializeField] private List<ItemObject> items;
            [SerializeField] private AnimationClip animationName;

            public string AnimationName => animationName.name;

            public List<ItemObject> Items => items;
        }
        
        [SerializeField] private PlayerCharacter character;
        [SerializeField] private AnimationClip defaultAnimation;
        [SerializeField] private List<AnimationForEquipment> animations;


        private Character targetCharacter;
        private GameDataObject gameDataObject;

        private Animator animator;
        private static readonly int Blend = Animator.StringToHash("Blend");

        [Inject]
        private void Construct(GameDataObject gameDataObject, INavMeshProvider navMeshService)
        {
            this.gameDataObject = gameDataObject;
            character.Init(new Character(), gameDataObject, null, null, null, null, null, navMeshService, null, null);
            animator = character.AnimationManager.GetAnimator();

        }
        private void OnEquipmentChange()
        {
            character.gameObject.ChangeLayerWithChilds(LayerMask.NameToLayer("Overlay"));
            
            
            var item = gameDataObject.GetItem(character.Character.Equipment.ArmorID);
            if (item != null)
            {
                print(item.name);
                for (int i = 0; i < animations.Count; i++)
                {
                    if (animations[i].Items.Contains(item))
                    {
                        animator.Play(animations[i].AnimationName);
                        return;
                    }
                }
            }
            animator.Play(defaultAnimation.name);

        }

        public void UpdateCharacterVisuals(Character ch)
        {
            if (targetCharacter != null){
                targetCharacter.Equipment.OnEquipmentChange -= OnEquipmentChange;
            }
            targetCharacter = ch;
            character.ChangeCharacter(ch);
            targetCharacter.Equipment.OnEquipmentChange += OnEquipmentChange;
            
            
            OnEquipmentChange();
        }

        private void OnDestroy()
        {
            if (targetCharacter != null)
            {
                targetCharacter.Equipment.OnEquipmentChange -= OnEquipmentChange;
            }
        }
    }
}
