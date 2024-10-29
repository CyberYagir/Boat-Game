using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;
using static Content.Scripts.Global.SaveDataObject.PlayerQuestsData;

namespace Content.Scripts.QuestsSystem
{
    public class QuestBuilder : IFactory
    {
        private DiContainer container;

        [Inject]
        private void Construct(DiContainer container)
        {
            this.container = container;
        }
        
        public static List<Type> GetQuestTypes()
        {
            List<Type> objects = new List<Type>();
            foreach (Type type in 
                Assembly.GetAssembly(typeof(QuestBase)).GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(QuestBase))))
            {
                objects.Add(type);
            }
            return objects; 
        }

        public static List<string> GetQuestTypesEditor()
        {
            var list = GetQuestTypes();
            var strs = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                strs.Add(list[i].FullName);
            }


            return strs;
        }

        public QuestBase BuildQuest(QuestsEventBus questsEventBus, QuestDataObject questData, QuestSaveData questSaveData = null)
        {
            if (questData == null) return null;
            var type = Type.GetType(questData.QuestClass);
            if (type != null)
            {
                var quest = (QuestBase) Activator.CreateInstance(type);

                if (quest != null)
                {
                    if (questSaveData != null)
                    {
                        quest.InitQuest(questsEventBus, questData, questSaveData.Value);
                    }
                    else
                    {
                        quest.InitQuest(questsEventBus, questData, 0);
                    }

                    container.Inject(quest);
                    return quest;
                }
            }

            return null;
        }
    }
}
