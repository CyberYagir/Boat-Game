using System;
using System.Collections;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using Content.Scripts.Mobs.MobSnake;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.Scripts.BoatGame
{
    public partial class PlayerCharacter : MonoBehaviour, ICharacter, IDamagable
    {
        [SerializeField, ReadOnly] private Character character;
        [SerializeField] private AppearanceManager appearanceManager;
        [SerializeField] private AnimationsManager animationsManager;
        [SerializeField] private AIManager aiManager;
        [SerializeField] private NeedsManager needsManager;
        [SerializeField] private RagdollController ragdollController;
        
        [SerializeField] private StateMachine<PlayerCharacter, EStateType> stateMachine;
        [SerializeField] private ActionsHolder actionsHolder;

        [SerializeField] private bool onlyVisuals;

        private CharacterGrounder characterGrounder = new CharacterGrounder();
        private SelectionService selectionService;
        private TickService tickService;
        private RaftBuildService raftBuildService;
        private PrefabSpawnerFabric prefabSpawnerFabric;
        private GameDataObject gameData;


        public EStateType CurrentState => stateMachine.CurrentStateType;
        public AIManager AIMoveManager => aiManager;
        public AnimationsManager AnimationManager => animationsManager;
        public SelectionService SelectionService => selectionService;

        public Character Character => character;

        public NeedsManager NeedManager => needsManager;

        public AppearanceManager AppearanceDataManager => appearanceManager;

        public PrefabSpawnerFabric SpawnerFabric => prefabSpawnerFabric;

        public RaftBuildService BuildService => raftBuildService;

        public Action OnChangeState;
        private SaveDataObject saveDataObject;


        public void Init(
            Character character,
            GameDataObject gameData,
            RaftBuildService raftBuildService,
            WeatherService weatherService,
            TickService tickService,
            SelectionService selectionService,
            PrefabSpawnerFabric prefabSpawnerFabric,
            INavMeshProvider navMeshProvider, 
            SaveDataObject saveDataObject
        )
        {
            this.saveDataObject = saveDataObject;
            this.gameData = gameData;
            this.prefabSpawnerFabric = prefabSpawnerFabric;
            this.raftBuildService = raftBuildService;
            this.tickService = tickService;
            this.character = character;
            this.selectionService = selectionService;

            appearanceManager.Init(character, gameData);

            if (onlyVisuals) return;

            characterGrounder.Init(transform);
            aiManager.Init(raftBuildService, navMeshProvider, character);
            
            animationsManager.Init(weatherService, appearanceManager);
            needsManager.Init(character, weatherService, this.selectionService, gameData);
            actionsHolder.Construct(selectionService, gameData);

            raftBuildService.OnChangeRaft += CheckGround;
            needsManager.OnDeath += Death;

            SetCharacterRaftPosition();

            stateMachine.Init(this);
            stateMachine.OnChangeState += OnStateMachineStateChanged;

            tickService.OnTick += OnTick;

            Select(false);
        }

        public void SetCharacterRaftPosition()
        {
            transform.position = aiManager.GenerateRandomPos();
            transform.SetYEulerAngles(Random.value * 360);
            aiManager.NavMeshAgent.SetDestination(transform.position);
            aiManager.NavMeshAgent.SetTargetPoint(transform.position);
        }

        private void CheckGround()
        {
            if (!aiManager.IsOnGround() && !needsManager.IsDead)
            {
                needsManager.Death();
            }
        }

        private void Death(Character target)
        {
            ragdollController.ActiveRagdoll();
            stateMachine.enabled = false;
            aiManager.NavMeshAgent.Disable();
            GetComponent<Collider>().enabled = false;
            appearanceManager.ChangeSelection(false);
            gameObject.ChangeLayerWithChilds(LayerMask.NameToLayer("PlayerDead"));
            this.enabled = false;
            
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

            NeedsManagerTick(delta);
        }

        private void NeedsManagerTick(float delta)
        {
            if (saveDataObject.Global.TotalSecondsInGame + TimeService.PlayedTime >= gameData.ConfigData.StartNeedsActiveTime)
            {
                needsManager.OnTick(delta);
            }
            else
            {
                needsManager.ClampValues();
            }
        }

        private void Update()
        {
            if (onlyVisuals) return;
            animationsManager.Update(aiManager.NavMeshAgent);
            aiManager.ExtraRotation();
            needsManager.Update();

            characterGrounder.PlaceUnitOnGround();
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
            if (tickService)
            {
                tickService.OnTick -= OnTick;
            }

            if (BuildService)
            {
                BuildService.OnChangeRaft -= CheckGround;
            }
        }

        public ItemObject GetEquipmentWeapon()
        {
            return gameData.GetItem(character.Equipment.WeaponID);
        }

        public void Damage(float dmg, GameObject sender)
        {
            animationsManager.TriggerGetDamage();
            needsManager.Damage(dmg);

            if (stateMachine.CurrentStateType == EStateType.Idle)
            {
                var attackTarget = sender.GetComponent<DamageObject>();
                if (attackTarget)
                {
                    stateMachine.GetStateByType<CharActionAttack>().SetAutoAttackMob(attackTarget);
                    ActiveAction(EStateType.Attack);
                }
            }
        }
    }
}
