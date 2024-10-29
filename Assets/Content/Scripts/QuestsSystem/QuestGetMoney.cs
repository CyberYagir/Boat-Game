using Content.Scripts.BoatGame.Services;
using Content.Scripts.DungeonGame.Services;
using Content.Scripts.ItemsSystem;
using Zenject;

namespace Content.Scripts.QuestsSystem
{
    public class QuestGetMoney : QuestBase
    {
        private IResourcesService resourcesService;
        private ItemObject goldBag;


        [Inject]
        private void Construct(IResourcesService resourcesService)
        {
            this.resourcesService = resourcesService;
            resourcesService.OnAddStorageItem += ResourcesServiceOnOnAddStorageItem;
        }

        public override void InitQuest(QuestsEventBus eventBus, QuestDataObject questDataObject, int value)
        {
            base.InitQuest(eventBus, questDataObject, value);

            goldBag = QuestData.GetDataByKey("Item") as ItemObject;
        }

        private void ResourcesServiceOnOnAddStorageItem(ItemObject arg1, int arg2)
        {
            if (goldBag == arg1)
            {
                AddValue(arg2);
            }
        }

        public override void ClaimAndDispose()
        {
            base.ClaimAndDispose();
           
            resourcesService.OnAddStorageItem -= ResourcesServiceOnOnAddStorageItem;
        }
    }
    
    
    
    public class QuestGetScroll : QuestBase
    {
        private ItemObject magicScroll;
        private IResourcesService resourcesService;

        [Inject]
        private void Construct(IResourcesService resourcesService)
        {
            this.resourcesService = resourcesService;
            
            resourcesService.OnAddStorageItem += ResourcesServiceOnOnAddStorageItem;
        }

        public override void InitQuest(QuestsEventBus eventBus, QuestDataObject questDataObject, int value)
        {
            base.InitQuest(eventBus, questDataObject, value);

            magicScroll = QuestData.GetDataByKey("Item") as ItemObject;
        }

        private void ResourcesServiceOnOnAddStorageItem(ItemObject arg1, int arg2)
        {
            if (magicScroll == arg1)
            {
                AddValue(arg2);
            }
        }

        public override void ClaimAndDispose()
        {
            base.ClaimAndDispose();
            resourcesService.OnAddStorageItem -= ResourcesServiceOnOnAddStorageItem;
        }
    }
}