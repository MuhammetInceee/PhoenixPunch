using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class ParticleManager : MonoBehaviour
    {
        public static Action<Vector3, Vector3> PunchParticleAction;
        public static Action<Vector3, Vector3> TextParticleAction;
        public static Action ConfettiParticleAction;

        private List<ParticleSystem> _particlePool;
        [SerializeField] private List<ParticleSystem> _textParticlePool;

        [SerializeField] private ParticleSystem confettiParticle;
        [SerializeField] private ParticleSystem punchParticlePrefab;
        [SerializeField] private ParticleSystem[] textParticlesPrefabs;
        [SerializeField] private int poolSize;

        private void OnEnable()
        {
            PunchParticleAction += GetPunchParticle;
            TextParticleAction += GetRandomTextPunchParticle;
            ConfettiParticleAction += ConfettiParticle;
        }

        private void OnDisable()
        {
            PunchParticleAction -= GetPunchParticle;
            TextParticleAction -= GetRandomTextPunchParticle;
            ConfettiParticleAction -= ConfettiParticle;
        }

        private void Awake()
        {
            _particlePool = new List<ParticleSystem>();
            for (var i = 0; i < poolSize; i++)
            {
                var particleSystemObject = Instantiate(punchParticlePrefab, Vector3.zero, Quaternion.identity, transform.GetChild(0).transform);
                particleSystemObject.gameObject.SetActive(false);
                _particlePool.Add(particleSystemObject);
            }

            for (var i = 0; i < poolSize/3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    var textParticle = Instantiate(textParticlesPrefabs[j], Vector3.zero, Quaternion.identity, transform.GetChild(1).transform);
                    textParticle.gameObject.SetActive(false);
                    _textParticlePool.Add(textParticle);
                }
            }
        }

        private ParticleSystem GetParticleSystem()
        {
            ParticleSystem particleSystemObject = null;

            foreach (var t in _particlePool)
            {
                if (!t.gameObject.activeInHierarchy)
                {
                    particleSystemObject = t;
                    break;
                }
            }

            if (particleSystemObject == null)
            {
                particleSystemObject = Instantiate(punchParticlePrefab, Vector3.zero, Quaternion.identity, transform.GetChild(0).transform);
                _particlePool.Add(particleSystemObject);
            }

            return particleSystemObject;
        }


        private ParticleSystem GetRandomTextParticle()
        {
            var particleSystemObject = _textParticlePool[Random.Range(0, _textParticlePool.Count)];
            
            if(particleSystemObject.gameObject.activeInHierarchy)  particleSystemObject = _textParticlePool[Random.Range(0, _textParticlePool.Count)];

            if (particleSystemObject == null)
            {
                particleSystemObject = Instantiate(textParticlesPrefabs[Random.Range(0, textParticlesPrefabs.Length)], Vector3.zero, Quaternion.identity, transform.GetChild(1).transform);
                _particlePool.Add(particleSystemObject);
            }

            return particleSystemObject;
        }

        private void GetPunchParticle(Vector3 position, Vector3 rotation)
        {
            ParticleSystem particle = GetParticleSystem();

            var transform1 = particle.transform;
            transform1.position = position;
            transform1.localEulerAngles = rotation;
            particle.gameObject.SetActive(true);
            particle.Play();

            ReturnParticleSystem(particle);
        }

        private void GetRandomTextPunchParticle(Vector3 position, Vector3 rotation)
        {
            ParticleSystem particle = GetRandomTextParticle();

            var transform1 = particle.transform;
            transform1.position = position;
            transform1.localEulerAngles = rotation;
            particle.gameObject.SetActive(true);
            particle.Play();

            ReturnTextParticle(particle);
        }

        private async void ReturnParticleSystem(ParticleSystem particle)
        {
            await Task.Delay((int)particle.main.duration * 1000);
            if (particle == null) return;
            
            particle.gameObject.SetActive(false);
            particle.transform.position = Vector3.zero;
        }

        private async void ReturnTextParticle(ParticleSystem particle)
        {
            await Task.Delay((int)particle.main.duration * 1000);
            if (particle == null) return;

            particle.gameObject.SetActive(false);
            particle.transform.position = Vector3.zero;
        }

        private void ConfettiParticle()
        {
            confettiParticle.Play();
        }
    }
}
