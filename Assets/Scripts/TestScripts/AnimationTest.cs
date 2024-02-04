using System;
using System.Threading.Tasks;
using UnityEngine;

namespace TestScripts
{
    public class AnimationTest : MonoBehaviour
    {
        public ParticleSystem particle;

        private async void Awake()
        {
            await Task.Delay(2000);
            particle.Play();
        }

        private void Update()
        {
            if (particle.IsAlive(false))
            {
                print("bittiiğğğğ");
            }
        }
    }
}
