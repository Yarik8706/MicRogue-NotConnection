using System;
using MainScripts;
using UnityEngine;

namespace Other
{
    public class OtherGameobjectController : MonoBehaviour
    {
        public GameObject coldBlackount;
        public static OtherGameobjectController instance;

        private void Start()
        {
            instance = this;
        }
    }
}