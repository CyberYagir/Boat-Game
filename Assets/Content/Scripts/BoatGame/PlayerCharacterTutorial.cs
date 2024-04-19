using System;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class PlayerCharacterTutorial : MonoBehaviour
    {
        private CharStateMachine stateMachine;
        private int actionsCount;
        private GameDataObject gameData;
        private SaveDataObject saveData;

        public void Init(GameDataObject gameData, SaveDataObject saveData)
        {
            this.saveData = saveData;
            this.gameData = gameData;
            stateMachine = GetComponent<CharStateMachine>();

            if (!saveData.Tutorials.ClickTutorial)
            {
                stateMachine.OnChangeState += OnChangeState;
            }
        }

        private void OnChangeState()
        {
            if (stateMachine.CurrentStateType != EStateType.Idle)
            {
                actionsCount++;

                if (actionsCount >= gameData.ConfigData.ActionsCountToTutorial)
                {
                    saveData.Tutorials.ClickTutorialSet();
                }
            }
        }
    }
}
