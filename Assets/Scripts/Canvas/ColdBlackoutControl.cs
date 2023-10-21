using System.Collections;
using UnityEngine;

namespace Canvas
{
    public class ColdBlackoutControl : MonoBehaviour
    {
        [SerializeField] private GameObject coldBlackout;
    
        public static ColdBlackoutControl instance;

        private void Start()
        {
            instance = this;
        }
    
        public void ActivateColdBlackout()
        {
            StartCoroutine(ActivateColdBlackoutCoroutine());
        }

        private IEnumerator ActivateColdBlackoutCoroutine()
        {
            coldBlackout.SetActive(true);
            yield return new WaitForSeconds(0.7f);
            coldBlackout.SetActive(false);
        }
    }
}