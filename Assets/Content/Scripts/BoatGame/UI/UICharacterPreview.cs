using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

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
        [SerializeField] private Camera camera;
        [SerializeField] private List<AnimationForEquipment> animations;
        [SerializeField] private bool generateNewRenderTexture;

        public RenderTexture RenderTexture => camera.targetTexture;

        private Character targetCharacter;
        private GameDataObject gameDataObject;

        private Animator animator;
        private static readonly int RandomAnim = Animator.StringToHash("RandomAnim");
        private static readonly int ToStart = Animator.StringToHash("ToStart");

        private int targetIdleAnimationID;
        
        private static int animationID;
        private const int MAX_ANIMATIONS = 9;

        [Inject]
        private void Construct(GameDataObject gameDataObject)
        {
            this.gameDataObject = gameDataObject;
            if (generateNewRenderTexture)
            {
                var targetTexture = camera.targetTexture;
                targetTexture = new RenderTexture(targetTexture.width, targetTexture.height, targetTexture.depth, targetTexture.graphicsFormat);
                camera.targetTexture = targetTexture;
            }
            character.InitDummy(new Character(), gameDataObject);
            animator = character.AnimationManager.GetAnimator();

            if (generateNewRenderTexture)
            {

                targetIdleAnimationID = animationID;
                animationID++;

                if (animationID >= MAX_ANIMATIONS)
                {
                    animationID = 0;
                }
            }
            else
            {
                targetIdleAnimationID = new System.Random(character.Character.Uid.GetHashCode()).Next(0, MAX_ANIMATIONS);
            }
        }

        private void OnEquipmentChange()
        {
            character.gameObject.ChangeLayerWithChilds(LayerMask.NameToLayer("Overlay"));
            
            
            var item = gameDataObject.GetItem(character.Character.Equipment.ArmorID);
            if (item != null)
            {
                for (int i = 0; i < animations.Count; i++)
                {
                    if (animations[i].Items.Contains(item))
                    {
                        animator.Play(animations[i].AnimationName);
                        return;
                    }
                }
            }
            SetRandomAnimation();
        }

        private void SetRandomAnimation()
        {
            animator.SetInteger(RandomAnim, targetIdleAnimationID);
            animator.ResetTrigger(ToStart);
            animator.SetTrigger(ToStart);
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
