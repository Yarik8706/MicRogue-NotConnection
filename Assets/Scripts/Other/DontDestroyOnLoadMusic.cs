using System.Linq;
using UnityEngine;

namespace Other
{
    public class DontDestroyOnLoadMusic : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (FindObjectsOfType<AudioSource>()
                .Any(audioSource => audioSource.gameObject.name == gameObject.name && audioSource.gameObject != gameObject))
            {
                Destroy(gameObject);
            }
        }
    }
}
