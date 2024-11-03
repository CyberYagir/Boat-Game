using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.BoatGame.PlayerActions
{
    
    public class PlayerAction : MonoBehaviour
    {
        [SerializeField] private float priority;
        [SerializeField] private EStateType state;
        
        
        private Sprite icon;
        private SelectionService selectionService;




        public Sprite Icon => icon;
        
        public EStateType State => state;
        
        public SelectionService SelectionService => selectionService;

        public float Priority => priority + PriorityAdd();

        public void Init(SelectionService selectionService, GameDataObject gameDataObject)
        {
            icon = gameDataObject.ActionsData.GetActionIcon(state);
            this.selectionService = selectionService;
        }

        public virtual int PriorityAdd()
        {
            return 0;
        }
        
        public virtual bool IsCanShow()
        {
            if (SelectionService.SelectedCharacter == null)
            {
                return false;
            }

            return SelectionService.SelectedCharacter.GetCurrentAction().IsCanCancel();
        }

        public virtual bool IsCanCancel()
        {
            if (IsSelectedCharacterOnThisAction())
            {
                return SelectionService.SelectedCharacter.GetCharacterAction(state).IsCanCancel();
            }

            return false;
        }

        public bool IsSelectedCharacterOnThisAction()
        {
            return SelectionService.SelectedCharacter.CurrentState == State;
        }
        
        public virtual void Action()
        {
            SelectionService.SelectedCharacter.ActiveAction(state);
        }

        public virtual void BreakAction()
        {
            SelectionService.SelectedCharacter.StopAction();
        }

        public void SetState(EStateType action)
        {
            state = action;
        }
    }
}