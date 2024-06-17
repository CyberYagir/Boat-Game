using Content.Scripts.Global;

namespace Content.Scripts.ManCreator
{
    public class RenameIslandWindow : AnimatedWindowInput
    {
        private SaveDataObject saveDataObject;

        public void Init(SaveDataObject saveDataObject)
        {
            this.saveDataObject = saveDataObject;
        }

        public override void ShowWindow()
        {
            base.ShowWindow();

            var island = saveDataObject.GetTargetIsland();
            if (island != null)
            {
                inputField.text = island.IslandName;
            }
        }

        public override bool Apply()
        {
            if (base.Apply())
            {
                saveDataObject.GetTargetIsland().SetIslandName(inputField.text.Trim());
                saveDataObject.SaveFile();
            }

            return false;
        }
    }
}