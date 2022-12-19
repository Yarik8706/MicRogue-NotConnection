using Enemies;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace RoomObjects
{
    public class Lamp : MonoBehaviour, IColdAttack
    {
        private Animator _animator;
        private Light2D _light2D;

        private void Start()
        {
            _light2D = GetComponentInChildren<Light2D>();
            _animator = GetComponent<Animator>();
        }

        public void ColdAttack()
        {
            _light2D.enabled = false;
            _animator.SetTrigger("Freezing");
            _animator.SetBool("isFreezing", true);
        }
    }
}
