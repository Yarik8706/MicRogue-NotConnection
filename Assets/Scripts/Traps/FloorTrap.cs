using System;
using System.Collections;
using UnityEngine;

namespace Traps
{
    public interface IThrustAttack
    {
        public void ThrustAttack();
    }

    public class FloorTrap : TheTrap
    {
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        protected override IEnumerator Attack()
        {
            _audioSource.Play();
            animator.SetTrigger(attack);
            if (attackObjects.Count != 0)
            {
                if (attackObjects[0].GetComponent<IThrustAttack>() is { } component)
                {
                    component.ThrustAttack();
                }
                else if (attackObjects[0].GetComponent<TheEssence>() is { } essence)
                {
                    essence.Died(this);
                }
            }

            yield return base.Attack();
        }
    }
}
