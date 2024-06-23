using System;

namespace Content.Scripts.BoatGame.UI
{
    public interface ITabManager
    {
        event Action<int> OnTabChanged;
    }
}