using System;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.Mobs;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Content.Scripts
{
    public class WeightsWindow : EditorWindow
    {
        [MenuItem("Tools/Drop Table")]
        public static void ShowWindow()
        {
            var window = GetWindow<WeightsWindow>();
            window.titleContent = new GUIContent("Drop Table");
        }

        private Object dropTable;
        private ITableObject tableObject;

        private List<Color> colors = new List<Color>()
        {
            new Color32(60, 60, 60, 255),
            new Color32(75, 75, 75, 255),
            new Color32(100, 100, 100, 255),
            new Color32(125, 125, 125, 255),
            new Color32(150, 150, 150, 255),
        };

        private List<float> targetWeights = new List<float>();
        private List<string> targetNames = new List<string>();
        private List<float> targetPercents = new List<float>();

        private void OnGUI()
        {
            dropTable = EditorGUILayout.ObjectField("Table:", dropTable, typeof(ITableObject));

            if (dropTable is ITableObject k)
            {
                tableObject = k;
            }

            if (tableObject != null)
            {
                GUILayout.Label("Weights:");


                GUILayout.BeginHorizontal();
                {
                    var weights = tableObject.GetWeights();
                    var sum = weights.Sum(x => x.Value);

                    var back = GUI.backgroundColor;
                    int i = 0;
                    targetWeights.Clear();
                    targetNames.Clear();
                    targetPercents.Clear();
                    foreach (var weight in weights)
                    {
                        targetWeights.Add(weight.Value);
                        targetNames.Add(weight.Key);
                        var percent = weight.Value / sum;
                        var len = (position.width * percent) * 0.97f;
                        targetPercents.Add(percent);

                        GUILayout.BeginVertical();
                        {
                            GUI.backgroundColor = colors[i] * 2f;
                            GUILayout.Button((percent * 100f).ToString("F1") + "%", GUILayout.Width(len));
                            i++;

                            if (i >= colors.Count)
                            {
                                i = 0;
                            }

                            if (len > 50)
                            {
                                GUILayout.Label(weight.Key, new GUIStyle(GUI.skin.label) {wordWrap = true}, GUILayout.MaxWidth(len));
                            }
                        }
                        GUILayout.EndVertical();
                    }

                    GUI.backgroundColor = back;
                }
                GUILayout.EndHorizontal();

                EditorGUILayout.Space(20);

                EditorGUI.BeginChangeCheck();
                {
                    for (int i = 0; i < targetWeights.Count; i++)
                    {
                        GUILayout.BeginHorizontal("Box");
                        {
                            GUILayout.BeginVertical(GUILayout.Width(50));
                            {
                                GUILayout.Label((targetPercents[i]*100f).ToString("F1") + "%", GUILayout.Width(50));
                            }
                            GUILayout.EndVertical();
                            
                            
                            GUILayout.BeginVertical();
                            {
                                targetWeights[i] = EditorGUILayout.FloatField(targetNames[i] + ":", targetWeights[i]);
                            }
                            GUILayout.EndVertical();
                            
                            
             
                        }
                        GUILayout.EndHorizontal();
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(dropTable, "Change Weights");
                        tableObject.ChangeWeights(targetWeights);
                    }
                }
            }
        }
    }
}