using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using OmniSARTechnologies.LiteFPSCounter;
using Packs.YagirConsole.ShellScripts.Base.Shell;
using UnityEngine;

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
            AddCommand("/showfps", new List<Argument>()
                {
                    new Argument("state", ArgumentType.Bool)
                },
                delegate(ArgumentsShell shell)
                {
                    var counter = Object.FindObjectOfType<LiteFPSCounter>(true);
                    if (counter == null)
                    {
                        Debug.LogWarning("FPSCounter not finded");
                        return;
                    }

                    counter.gameObject.SetActive(shell.GetBool("state"));
                });
        }

        public void SpawnDamager()
        {
            AddCommand("/summon",
                new List<Argument>()
                {
                    new Argument("damager_id", ArgumentType.Number)
                },
                delegate(ArgumentsShell shell)
                {
                    var damager = Object.FindObjectOfType<RaftDamagerService>();

                    if (damager == null)
                    {
                        Debug.LogWarning("RaftDamagerService not finded");
                        return;
                    }


                    var item = damager.CreateSituationByID((int) shell.GetInteger("damager_id"));

                    if (item)
                    {
                        Debug.Log("Summon " + item.transform.name);
                    }
                    else
                    {
                        Debug.Log("Cant find raft to summon");
                    }
                }
            );
        }
    }
}
