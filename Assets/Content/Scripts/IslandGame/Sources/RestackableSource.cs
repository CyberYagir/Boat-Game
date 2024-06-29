using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using Content.Scripts.Mobs;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame.Sources
{
    public class RestackableSource : MonoBehaviour, ISourceObject
    {
        [SerializeField] private int maxStack;
        [SerializeField] private int restackTicks;
        [SerializeField] private DropTableObject dropTable;
        
        [SerializeField, ReadOnly] private int stack;
        [SerializeField, ReadOnly] private int ticksTimer = 0;


        public int TicksTimer => ticksTimer;

        public int Stack => stack;

        public bool IsValidSource => stack > 0;
        
        [Inject]
        private void Construct(TickService tickService)
        {
            stack = maxStack;
            tickService.OnTick += OnTick;
        }

        private void OnTick(float obj)
        {
            if (stack < maxStack)
            {
                ticksTimer++;
                if (ticksTimer >= restackTicks)
                {
                    stack++;
                    ticksTimer = 0;
                }
            }
        }

        public ItemObject GetFromItem()
        {
            if (IsValidSource)
            {
                stack--;
                return dropTable.GetItem();
            }

            return null;
        }

        public void LoadStorage(SaveDataObject.RaftsData.RaftWaterSource find)
        {
            if (find == null) return;;
            ticksTimer = find.CurrentTicks;
            stack = find.Stack;
        }
    }
}
