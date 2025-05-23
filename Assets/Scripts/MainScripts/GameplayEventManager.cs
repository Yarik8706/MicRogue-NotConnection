using Enemies;
using UnityEngine.Events;

namespace MainScripts
{
    public static class GameplayEventManager
    {
        public static readonly UnityEvent OnNextRoom = new();
        public static readonly UnityEvent OnNextMove = new();
        public static readonly UnityEvent OnGetAllEnemies = new();
        public static readonly UnityEvent OnGetAllTraps = new();
        public static readonly UnityEvent OnPlayerDied = new();

        public static TheEnemy[] GetAllEnemies()
        {
            GameController.instance.allEnemies.Clear();
            OnGetAllEnemies.Invoke();
            return GameController.instance.allEnemies.ToArray();
        }
    }
}
