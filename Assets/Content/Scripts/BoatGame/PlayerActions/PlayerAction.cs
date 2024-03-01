using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.BoatGame.PlayerActions
{
    
    public class PlayerAction : MonoBehaviour
    {
        [SerializeField] private EStateType state;
        
        
        private Sprite icon;
        private SelectionService selectionService;




        public Sprite Icon => icon;
        
        public EStateType State => state;
        
        public SelectionService SelectionService => selectionService;

        public void Init(SelectionService selectionService, GameDataObject gameDataObject)
        {
            icon = gameDataObject.ActionsData.GetActionIcon(state);
            this.selectionService = selectionService;
        }

        public virtual bool IsCanShow()
        {
            if (SelectionService.SelectedCharacter == null)
            {
                return false;
            }

            if (SelectionService.SelectedCharacter.CurrentState != EStateType.Idle)
            {
                return false;
            }
            
            return true;
        }

        public virtual bool IsCanCancel()
        {
            return true;
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