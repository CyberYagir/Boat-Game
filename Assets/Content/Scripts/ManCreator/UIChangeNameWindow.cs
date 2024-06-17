namespace Content.Scripts.ManCreator
{
    public class UIChangeNameWindow : AnimatedWindowInput
    {
        private ManCreatorUIService manCreatorUIService;

        public void Init(ManCreatorUIService manCreatorUIService)
        {
            this.manCreatorUIService = manCreatorUIService;
        }

        public override bool Apply()
        {
            if (base.Apply()){
                manCreatorUIService.ChangeName(inputField.text);
            }

            return false;
        }
    }
}
