using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.ItemsSystem;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Scripts.BoatGame
{
    public partial class PlayerCharacter
    {

        [System.Serializable]
        public class AnimationsManager
        {
            private static readonly int WeaponType = Animator.StringToHash("WeaponType");
            private static readonly int IsLegAttack = Animator.StringToHash("IsLegAttack");
            private static readonly int IsMove = Animator.StringToHash("IsMove");        
            private static readonly int Hood = Animator.StringToHash("ApplyHood");
            private static readonly int TG_Fishing = Animator.StringToHash("TG_Fishing");
            private static readonly int TG_Idle = Animator.StringToHash("TG_Idle");
            private static readonly int TG_Hook = Animator.StringToHash("TG_Hook");
            private static readonly int TG_Eat = Animator.StringToHash("TG_Eat");
            private static readonly int TG_Drink = Animator.StringToHash("TG_Drink");
            private static readonly int TG_Building = Animator.StringToHash("TG_Building");
            private static readonly int TG_Crafting = Animator.StringToHash("TG_Crafting");
            private static readonly int TG_Attack = Animator.StringToHash("TG_Attack");
            private static readonly int TG_ChopTree = Animator.StringToHash("TG_TreeChop");
            private static readonly int TG_Pickup = Animator.StringToHash("TG_Pickup");
            private static readonly int TG_Mine = Animator.StringToHash("TG_Mining");
            private static readonly int TG_GetDamage = Animator.StringToHash("TG_Damaged");
            private static readonly int HugFish = Animator.StringToHash("HugFish");
            private static readonly int DamageType = Animator.StringToHash("DamageType");
            private static readonly int RandomModify = Animator.StringToHash("RandomModify");

            
            [SerializeField] private Animator animator;
            [SerializeField] private Animation hoodAnimations;
            [SerializeField] private ParticleSystem bloodParticles;
            [SerializeField] private CharAnimationEvents animationEvents;
            
            [SerializeField] private Transform rightHand;
            [SerializeField] private Transform fishPoint;
            
            private AppearanceManager appearanceManager;
            public Transform RightHand => rightHand;

            public Transform FishPoint => fishPoint;

            public CharAnimationEvents AnimationEvents => animationEvents;

            public void Init(WeatherService weatherService, AppearanceManager appearanceManager)
            {
                this.appearanceManager = appearanceManager;
                animator.SetFloat(RandomModify, Random.Range(0.9f, 1.1f));
                if (weatherService != null)
                {
                    weatherService.OnChangeWeather += ApplyHood;
                    ApplyHood(weatherService.CurrentWeather);
                }
            }

            private void ApplyHood(WeatherService.EWeatherType type)
            {
                var inHood = appearanceManager.InHood;
                if (inHood)
                {
                    if (type != WeatherService.EWeatherType.Windy && type != WeatherService.EWeatherType.Сalm)
                    {
                        return;
                    }
                }
                else
                {
                    if (type != WeatherService.EWeatherType.Rain && type != WeatherService.EWeatherType.Storm)
                    {
                        return;
                    }
                }
                
                
                animator.ResetTrigger(Hood);
                animator.SetTrigger(Hood);
                hoodAnimations.Play(inHood ? "HoodAnimationDisable" : "HoodAnimationApply");

                inHood = !inHood;
                appearanceManager.SetHatState(inHood);
                appearanceManager.SetInHood(inHood);
            }

            public void Update(INavAgentProvider navAgentProvider)
            {
                animator.SetBool(IsMove, navAgentProvider.Velocity.magnitude != 0);
            }

            public void TriggerFishingAnimation(bool isReset = false)
            {
                if (!isReset)
                {                    
                    animator.ResetTrigger(TG_Fishing);
                    animator.SetTrigger(TG_Fishing);
                }
                else
                {
                    animator.ResetTrigger(TG_Fishing);
                }
            }


            public void ResetAllTriggers()
            {
                animator.ResetTrigger(TG_Fishing);
                animator.ResetTrigger(TG_Idle);
                animator.ResetTrigger(TG_Hook);
                animator.ResetTrigger(TG_Drink);
                animator.ResetTrigger(TG_Eat);
                animator.ResetTrigger(TG_Building);
                animator.ResetTrigger(TG_Crafting);
                animator.ResetTrigger(TG_Attack);
                animator.ResetTrigger(TG_ChopTree);
                animator.ResetTrigger(TG_Pickup);
                animator.ResetTrigger(TG_Mine);
                
                
                isAttackTriggerActived = false;
            }
            public void TriggerHoldFishAnimation(bool state)
            {
                animator.SetBool(HugFish, state);
            }

            public void TriggerIdle()
            {
                ResetAllTriggers();
                animator.SetTrigger(TG_Idle);
            }
            
            public void TriggerHook()
            {
                ResetAllTriggers();
                animator.SetTrigger(TG_Hook);
            }

            public void TriggerEatAnimation()
            {
                ResetAllTriggers();
                animator.SetTrigger(TG_Eat);
            }

            public void TriggerDrinkAnimation()
            {
                ResetAllTriggers();
                animator.SetTrigger(TG_Drink);
            }

            public void TriggerBuilding()
            {
                ResetAllTriggers();
                animator.SetTrigger(TG_Building);
            }
            public void TriggerCrafting()
            {
                ResetAllTriggers();
                animator.SetTrigger(TG_Crafting);
            } 
            
            public void TriggerChopTree()
            {
                ResetAllTriggers();
                animator.SetTrigger(TG_ChopTree);
            }
            public void TriggerPickUpAnim()
            {
                ResetAllTriggers();
                animator.SetTrigger(TG_Pickup);
            }
            public void TriggerMineAnim()
            {
                ResetAllTriggers();
                animator.SetTrigger(TG_Mine);
            }

            private bool isAttackTriggerActived = false;
            public void SetAttackTarget(EWeaponAnimationType animationType, bool isLegPunch)
            {
                if (!isAttackTriggerActived)
                {
                    animator.SetTrigger(TG_Attack);
                    isAttackTriggerActived = true;
                }

                animator.SetInteger(WeaponType, (int) animationType);
                animator.SetBool(IsLegAttack, isLegPunch);
            }

            public void TriggerGetDamage()
            {
                animator.ResetTrigger(TG_GetDamage);
                animator.SetInteger(DamageType, Random.Range(0, 3));
                animator.SetTrigger(TG_GetDamage);
                bloodParticles.Play();
            }

            public Animator GetAnimator()
            {
                return animator;
            }

            private const int TORCH_LAYER = 4;

            public void ShowTorch()
            {
                animator.SetLayerWeight(TORCH_LAYER, 1f);
            }
        }
    }
}