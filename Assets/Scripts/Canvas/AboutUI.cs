using Enemies;
using UnityEngine;

namespace Canvas
{
    public class AboutUI : MonoBehaviour
    {
        [SerializeField] private string googlePlayUrl;
        [SerializeField] private Cause telegramUrl;

        public void OpenGooglePlayUrl()
        {
            Application.OpenURL(googlePlayUrl);
        }

        public void OpenTelegramUrl()
        {
            Application.OpenURL(telegramUrl.GetCause());
        }
    }
}