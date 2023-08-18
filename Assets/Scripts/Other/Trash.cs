using MainScripts;
using UnityEngine;

namespace Other
{
    public class Trash : MonoBehaviour
    {
        private void Start()
        {
            GameplayEventManager.OnNextRoom.AddListener(() => Destroy(gameObject));
        }
    }
}