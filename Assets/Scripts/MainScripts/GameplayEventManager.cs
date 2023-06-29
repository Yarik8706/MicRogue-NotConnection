using UnityEngine.Events;

namespace MainScripts
{
    public static class GameplayEventManager
    {
        public static readonly UnityEvent OnNextRoom = new();
        public static readonly UnityEvent OnNextMove = new();
    }
}
