namespace Content.Scripts.BoatGame.PlayerActions
{
    class ActionHolderTransferedMage : ActionHolderTransfered
    {
        private ActionsHolder holder;
        public override ActionsHolder GetActionHolder()
        {
            if (holder == null)
            {
                holder = GetComponentInParent<ActionsHolder>();

            }

            holder.RegisterCustomTransformPoint(transform);

            return holder;
        }
    }
}