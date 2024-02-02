using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ManCreator;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Content.Scripts.BoatGame
{
    public partial class PlayerCharacter : MonoBehaviour, ICharacter
    {
        [SerializeField] private List<PlayerAction> playerActions = new List<PlayerAction>();
        [SerializeField, ReadOnly] private Character character;
        [SerializeField] private AppearanceManager appearanceManager;
        [SerializeField] private AnimationsManager animationsManager;
        [SerializeField] private AIManager aiManager;
        [SerializeField] private NeedsManager needsManager;
        [SerializeField] private RagdollController ragdollController;
        
        [SerializeField] private StateMachine<PlayerCharacter, EStateType> stateMachine;
        [SerializeField] private ActionsHolder actionsHolder;

        [SerializeField] private bool onlyVisuals;
        
        private SelectionService selectionService;


        public EStateType CurrentState => stateMachine.CurrentStateType;
        public AIManager NavigationManager => aiManager;
        public AnimationsManager AnimationManager => animationsManager;
        public SelectionService SelectionService => selectionService;

        public Character Character => character;

        public NeedsManager NeedManager => needsManager;


        public Action OnChangeState;
        private GameDataObject gameData;
        
        private TickService tickService;
        private RaftBuildService raftBuildService;

        public void Init(
            Character character, 
            GameDataObject gameData, 
            RaftBuildService raftBuildService,
            WeatherService weatherService,
            TickService tickService,
            SelectionService selectionService)
        {
            this.raftBuildService = raftBuildService;
            this.tickService = tickService;
            this.gameData = gameData;
            this.character = character;
            this.selectionService = selectionService;
            
            appearanceManager.Init(character, gameData);
            
            if (onlyVisuals) return;
            
            aiManager.Init(raftBuildService);
            animationsManager.Init(weatherService, appearanceManager);
            needsManager.Init(character, weatherService, gameData, this.selectionService);
            actionsHolder.Construct(selectionService);

            raftBuildService.OnChangeRaft += CheckGround;
            needsManager.OnDeath += Death;

            transform.position =  aiManager.GenerateRandomPos();
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, Mathf.Infinity, ~0))
            {
                transform.position = hit.position;
            }

            transform.SetYEulerAngles(Random.value * 360);
            
            
            stateMachine.Init(this);
            stateMachine.OnChangeState += OnStateMachineStateChanged;
            
            tickService.OnTick += OnTick;
            
            Select(false);
        }

        private void CheckGround()
        {
            if (!aiManager.IsOnGround())
            {
                needsManager.Death();
            }
        }

        private void Death(Character target)
        {
            ragdollController.ActiveRagdoll();
            stateMachine.enabled = false;
            aiManager.NavMeshAgent.enabled = false;
            GetComponent<Collider>().enabled = false;
            appearanceManager.ChangeSelection(false);
            gameObject.ChangeLayerWithChilds(LayerMask.NameToLayer("PlayerDead"));
            
            
            StartCoroutine(UnderWaterDeath());


            IEnumerator UnderWaterDeath()
            {
                bool isUnderWater = false;
                float time = 0;
                while (true)
                {
                    yield return new WaitForSeconds(0.1f);
                    if (isUnderWater == false && appearanceManager.CanChangeLayerOnDeath())
                    {
                        gameObject.ChangeLayerWithChilds(LayerMask.NameToLayer("Default"));
                        ragdollController.SetUnderWaterRagdoll();
                        isUnderWater = true;
                    }

                    if (isUnderWater)
                    {
                        if (appearanceManager.GetBone(AppearanceManager.EBones.Spine1).position.y <= -10f)
                        {
                            Destroy(gameObject);
                        }
                    }
                    else
                    {
                        time += 0.1f;
                        if (time >= 5)
                        {
                            Destroy(gameObject);
                        }
                    }
                }
            }
        }

        private void OnTick(float delta)
        {
            if (onlyVisuals) return;
            needsManager.OnTick(delta);

        }

        private void OnStateMachineStateChanged()
        {
            OnChangeState?.Invoke();
        }

        private void Update()
        {
            if (onlyVisuals) return;
            animationsManager.Update();
            aiManager.ExtraRotation();
            needsManager.Update();
        }

        public void Select(bool state)
        {
            appearanceManager.ChangeSelection(state);
            transform.localScale = Vector3.one;
            if (state)
            {
                transform.DOPunchScale(Vector3.one * 0.2f, 0.1f);
            }
        }

        public void ActiveAction(EStateType state)
        {
            stateMachine.StartAction(state);
        }

        public T GetCharacterAction<T>() where T : StateAction<PlayerCharacter>
        {
            return stateMachine.GetStateByType<T>();
        }

        public void StopAction()
        {
            ActiveAction(EStateType.Idle);
        }

        public void ChangeCharacter(Character ch)
        {
            character = ch;
            appearanceManager.Init(ch, gameData);
        }

        public void AddExp(int exp)
        {
            var xp = character.SkillData.Xp;
            var maxXp = gameData.GetLevelXP(character.SkillData.Level);

            if (xp + exp >= maxXp)
            {
                character.SkillData.AddLevel();
                if ((xp + exp) - maxXp > 0)
                {
                    AddExp((xp + exp) - maxXp);
                }
            }
            else
            {
                character.SkillData.AddXp(exp);
            }

           
        }


        private void OnDestroy()
        {
            tickService.OnTick -= OnTick;
            raftBuildService.OnChangeRaft -= CheckGround;
        }
    }
}
