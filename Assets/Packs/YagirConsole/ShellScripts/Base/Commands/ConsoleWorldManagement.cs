using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using OmniSARTechnologies.LiteFPSCounter;
using UnityEngine;
using YagirConsole.Scripts.Base.Shell;

namespace Packs.YagirConsole.ShellScripts.Base.Commands
{
    public class ConsoleWorldManagement : ConsoleManagementBase
    {
        public ConsoleWorldManagement()
        {
            SpawnDamager();
            ShowFPS();
        }

        public void ShowFPS()
        {
            var command = new ConsoleCommandData("/showfps", new List<Argument>()
            {
                new Argument("state", ArgumentType.Bool)
            });



            command.Action.AddListener(delegate(ArgumentsShell arg0)
            {
                var counter = Object.FindObjectOfType<LiteFPSCounter>(true);

                if (counter == null)
                {
                    Debug.LogWarning("FPSCounter not finded");
                    return;
                }

                counter.gameObject.SetActive(arg0.GetBool("state"));
            });

            commands.Add(command);
        }

        public void SpawnDamager()
        {
            var command = new ConsoleCommandData("/summon", new List<Argument>()
            {
                new Argument("damager_id", ArgumentType.Number)
            });
            
            command.Action.AddListener(delegate(ArgumentsShell arg0)
            {
                var damager = Object.FindObjectOfType<RaftDamagerService>();

                if (damager == null)
                {
                    Debug.LogWarning("RaftDamagerService not finded");
                    return;
                }


                var item = damager.CreateSituationByID((int) arg0.GetInteger("damager_id"));

                if (item)
                {
                    Debug.Log("Summon " + item.transform.name);
                }
                else
                {
                    Debug.Log("Cant find raft to summon");
                }
            });
            
            commands.Add(command);
        }
    }
}
