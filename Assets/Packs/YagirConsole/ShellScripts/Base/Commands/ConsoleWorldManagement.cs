using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using OmniSARTechnologies.LiteFPSCounter;
using Packs.YagirConsole.ShellScripts.Base.Shell;
using Unity.VisualScripting;
using UnityEngine;

namespace ConsoleShell
{
    public class ConsoleWorldManagement : ConsoleManagementBase
    {
        public ConsoleWorldManagement()
        {
            SpawnDamager();
            ShowFPS();
            Quit();
            GetAllResources();
            DebugMessage();
        }

        private void DebugMessage()
        {
            AddCommand("/log_debug_message", new List<Argument>(), delegate(ArgumentsShell shell)
            {
                var str = "";
                for (int i = 0; i < 20; i++)
                {
                    str += i + "\n";
                }
                
                Debug.LogError(str);
            });
        }

        private void Quit()
        {
            void ApplicationQuit(ArgumentsShell shell)
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }

            AddCommand("/quit", new List<Argument>(), ApplicationQuit);
            AddCommand("/q", new List<Argument>(), ApplicationQuit);
        }

        public void ShowFPS()
        {
            AddCommand("/showfps", new List<Argument>()
                {
                    new Argument("state", EArgumentType.Bool, false, 0f, "", true)
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
                    new Argument("damager_id", EArgumentType.Number)
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
                        ConsoleLogger.Log("Summon " + item.transform.name, ELogType.CmdSuccess);
                    }
                    else
                    {
                        Debug.Log("Cant find raft to summon");
                    }
                }
            );
        }

        private void GetAllResources()
        {
            AddCommand("/res_add", new List<Argument>()
                {
                    new Argument("item_name_with_underscores", EArgumentType.String, true),
                    new Argument("item_count", EArgumentType.Number, false, 1),
                    new Argument("check_storage_size", EArgumentType.Bool, false, 0, "", true)
                },
                delegate(ArgumentsShell shell)
                {

                    var resourceName = shell.GetString("item_name_with_underscores").Replace("_", " ");
                    var count = (int) shell.GetInteger("item_count");
                    var checkStorageSize = shell.GetBool("check_storage_size");
                        
                        
                    var gameData = Resources.Load<GameDataObject>("GameData");
                    var item = gameData.Items.ItemsList.Find(x => x.ItemName.ToLower().Trim() == resourceName);
                    
                    AddResourceToRaft(item, count, checkStorageSize);
                    
                });

            AddCommand("/res_list", new List<Argument>(), delegate(ArgumentsShell shell)
            {
                var gameData = Resources.Load<GameDataObject>("GameData");

                ConsoleLogger.Log("Items: ", ELogType.Log);
                foreach (var it in gameData.Items.ItemsList)
                {
                    ConsoleLogger.Log(it.ItemName.Trim().Replace(" ", "_").ToLower(), ELogType.Log);
                }
            });

            AddCommand("/res_creative", new List<Argument>(), delegate(ArgumentsShell shell)
            {
                var gameData = Resources.Load<GameDataObject>("GameData");

                foreach (var it in gameData.Items.ItemsList)
                {
                    if (it.Type != EResourceTypes.Other)
                    {
                        AddResourceToRaft(it, 100, false);
                    }
                }
            });
        }

        private static void AddResourceToRaft(ItemObject item, int count, bool checkStorageSize)
        {
            if (item)
            {
                var storages = Object.FindObjectOfType<RaftBuildService>().Storages;

                for (int i = 0; i < storages.Count; i++)
                {
                    if (storages[i].IsEmptyStorage(item, count) || !checkStorageSize)
                    {
                        storages[i].AddToStorage(item, count);
                        ConsoleLogger.Log($"  {item.ItemName} [{count}] item added to storage", ELogType.CmdSuccess);
                        return;
                    }
                }

                ConsoleLogger.Log("Cant Find Empty Storage", ELogType.CmdException);
            }
            else
            {
                ConsoleLogger.Log("Item not found", ELogType.CmdException);
            }
        }
    }
}
