using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Content.Scripts.ItemsSystem;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Internal.UIToolkitIntegration;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Content.Scripts
{
    public class ItemsManagerWindow : EditorWindow
    {
        private enum Tabs
        {
            Create, Edit, Find
        }
        private Tabs tabIndex;

        private ItemObject dummyItem;
        
        [MenuItem("Tools/Items Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<ItemsManagerWindow>();
            window.titleContent = new GUIContent("Items Manager");
        }

        private ItemObject selectedItem;
        private Editor selectedItemEditor;
        private string findText = "";
        private List<ItemObject> items = new List<ItemObject>();
        private Vector2 scroll;
        private void OnGUI()
        {
            tabIndex = (Tabs)GUILayout.Toolbar((int)tabIndex, new[] {"Create Asset", "Edit", "Find"});

            scroll = GUILayout.BeginScrollView(scroll);
            switch (tabIndex)
            {
                case Tabs.Create:
                    DrawCreateTab();
                    break;
                case Tabs.Edit:
                    DrawEditTab();
                    break;
                case Tabs.Find:
                    FindTabDraw();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            GUILayout.EndScrollView();
        }

        private void FindTabDraw()
        {
            if (items.Count == 0)
            {
                items = Resources.LoadAll<ItemObject>("").ToList();
            }

            GUILayout.Label("Items count: " + items.Count);
            findText = EditorGUILayout.TextField(findText);

            var searched = items;

            if (!string.IsNullOrEmpty(findText.Trim()))
            {
                searched = items.FindAll(x => x.ItemName.Contains(findText.Trim()));
            }




            GUILayout.BeginVertical();
            {
                for (int i = 0; i < searched.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.BeginVertical("Box");
                        {
                            var state = EditorPrefs.GetBool(searched[i].ID + "-state", false);
                            var newState = EditorGUILayout.Foldout(state, searched[i].ItemName, true);
                            if (state != newState)
                            {
                                EditorPrefs.SetBool(searched[i].ID + "-state", newState);
                            }

                            if (newState)
                            {
                                GUILayout.Label(AssetDatabase.GetAssetPath(searched[i]), new GUIStyle(GUI.skin.label)
                                {
                                    wordWrap = true
                                });

                                if (GUILayout.Button("Select"))
                                {
                                    Selection.activeObject = searched[i];
                                }
                                if (GUILayout.Button("Edit"))
                                {
                                    selectedItem = searched[i];
                                    tabIndex = Tabs.Edit;
                                }
                            }
                        }
                        GUILayout.EndVertical();
                        
                        GUILayout.BeginVertical("Box");
                        {
                            GUILayout.Box(new GUIContent(AssetPreview.GetAssetPreview(searched[i].ItemIcon)), GUILayout.Width(50), GUILayout.Height(50));
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        private void DrawEditTab()
        {
            if (!selectedItem)
            {
                EditorGUILayout.HelpBox("Select a item", MessageType.Info);

                if (Selection.activeObject is ItemObject)
                {
                    if (GUILayout.Button("Selected Active"))
                    {
                        selectedItem = Selection.activeObject as ItemObject;
                    }
                }
            }
            else
            {
                if (selectedItemEditor == null || selectedItemEditor.target != selectedItem)
                {
                    selectedItemEditor = Editor.CreateEditor(selectedItem);
                }
                
                selectedItemEditor.OnInspectorGUI();
                
                if (GUILayout.Button("Deselect"))
                {
                    selectedItem = null;
                }
            }
        }

        private Editor dummyEditor;
        private void DrawCreateTab()
        {
            if (dummyItem == null)
            {
                dummyItem = ScriptableObject.CreateInstance<ItemObject>();
                dummyItem.GenerateID();
                dummyEditor = Editor.CreateEditor(dummyItem);
            }

            dummyEditor.OnInspectorGUI();


            if (GUILayout.Button("Save Object"))
            {
                if (!string.IsNullOrEmpty(dummyItem.ItemName.Trim().Replace(" ", "")))
                {
                    var path = EditorUtility.SaveFilePanel("Save Asset", "", "Item_" + dummyItem.ItemName.Trim().Replace(" ", ""), "asset");
                    var relativepath = "Assets" + path.Substring(Application.dataPath.Length);
                    AssetDatabase.CreateAsset(dummyItem, relativepath);
                    
                    
                    Selection.activeObject = dummyItem;
                    dummyItem = null;
                }
            }
        }
    }
}
