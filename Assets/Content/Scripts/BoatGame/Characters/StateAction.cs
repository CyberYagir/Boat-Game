using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters
{
    public abstract class StateAction<T> : MonoBehaviour
    {

        [SerializeField, ReadOnly, FoldoutGroup("State Data")] private bool isEnded;
        [SerializeField, ReadOnly, FoldoutGroup("State Data")] private T machine;

        public bool IsEnded => isEnded;

        public T Machine => machine;

        public void Init(T inputObject)
        {
            this.machine = inputObject;
        }


        public virtual void StartState()
        {
            isEnded = false;
        }

        public virtual void ProcessState(){}

        public virtual void EndState()
        {
            isEnded = true;
        }
        public virtual void ResetState(){}
    }
}