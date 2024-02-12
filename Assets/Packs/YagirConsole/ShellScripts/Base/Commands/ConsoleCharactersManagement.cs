using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Packs.YagirConsole.ShellScripts.Base.Shell;
using UnityEngine;

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
            AddCommand("/ch_heal", new List<Argument>(), delegate(ArgumentsShell shell)
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
          
        }
    }
}
