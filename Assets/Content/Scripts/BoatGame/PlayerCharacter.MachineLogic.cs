using Content.Scripts.BoatGame.Characters;

namespace Content.Scripts.BoatGame
{
    public partial class PlayerCharacter
    {
        private void OnStateMachineStateChanged()
        {
            OnChangeState?.Invoke();
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
    }
}