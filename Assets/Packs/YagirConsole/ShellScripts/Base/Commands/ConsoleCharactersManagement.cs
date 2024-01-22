using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using YagirConsole.Scripts.Base.Shell;

namespace Packs.YagirConsole.ShellScripts.Base.Commands
{
    public class ConsoleCharactersManagement : ConsoleManagementBase
    {
        public ConsoleCharactersManagement()
        {
            HealAllCharacters();
        }


        public void HealAllCharacters()
        {
            var command = new ConsoleCommandData("/ch_heal", new List<Argument>());
            
            command.Action.AddListener(delegate(ArgumentsShell arg0)
            {
                var charactersService = Object.FindObjectOfType<CharacterService>();

                if (charactersService == null)
                {
                    Debug.LogWarning("CharacterService not finded");
                    return;
                }
                
                foreach (var ch in charactersService.SpawnedCharacters)
                {
                    ch.NeedManager.AddParameters(new Character.ParametersData(100, 100, 100));
                }
                
                Debug.Log("All characters have been restored.");
            });
            
            commands.Add(command);
        }
    }
}
