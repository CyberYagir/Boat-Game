using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters
{
    
    public enum EStateType
    {
        Idle,
        Fishing,
        Hooking,
        Eating,
        Drinkings,
        Building,
        ViewInfo,
        Crafting,
        Attack,
        MoveTo,
        TreeChop,
        StoneChop,
        GetGrass,
        PickupItem,
        BuildingFromItem,
        Furnace,
        GetFromSourceWater,
        VillageViewInfo,
        HitAWoman,
        LoreView
    }

    
    public abstract class StateMachine<T, K> : MonoBehaviour
    {

        [System.Serializable]
        public class StateHolder
        {
            [SerializeField] private K stateType;
            [SerializeField] private StateAction<T> stateAction;

            public StateAction<T> StateAction => stateAction;

            public K StateType => stateType;
        }

        [SerializeField] private List<StateHolder> statesList = new List<StateHolder>();
        [SerializeField] private K startAction;

        [SerializeField, ReadOnly] private StateAction<T> currentAction;

        private K lastStateKey;


        public K CurrentStateType => lastStateKey;
        public event Action OnChangeState;

        public void Init(T target)
        {
            foreach (var state in statesList)
            {
                state.StateAction.Init(target);
            }
            
            StartAction(startAction);
        }

        public void StartAction(K type)
        {
            if (currentAction != null)
            {
                currentAction.EndState();
            }

            var action = statesList.Find(x => x.StateType.Equals(type));
            action.StateAction.ResetState();
            action.StateAction.StartState();
            
            lastStateKey = type;
            
            if (currentAction != action.StateAction)
            {
                OnChangeState?.Invoke();
            }

            currentAction = action.StateAction;

            
        }

        public void Update()
        {
            if (currentAction != null)
            {
                if (currentAction.IsEnded)
                {
                    StartAction(startAction);
                }
                else
                {
                    currentAction.ProcessState();   
                }
            }
        }

        public T GetStateByType<T>() where T : StateAction<PlayerCharacter>
        {
            foreach (var st in statesList)
            {
                if (st.StateAction as T != null)
                {
                    return st.StateAction as T;
                }
            }

            return null;
        }

        public StateAction<T> GetStateByType(EStateType state)
        {
            return statesList.Find(x => x.StateType.Equals(state)).StateAction;
        }
    }
}
