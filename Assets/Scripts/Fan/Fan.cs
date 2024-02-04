using System;
using System.Threading.Tasks;
using DG.Tweening;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Fan
{
    public class Fan : MonoBehaviour
    { 
        [SerializeField] private MeshRenderer _meshRenderer;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            // _meshRenderer.materials[^1].color = GameManager.FanColors.Choice();
            foreach (Material mat in _meshRenderer.materials)
            {
                if (mat.name.Contains("Clothes"))
                {
                     mat.color = GameManager.FanColors.Choice();
                }
            }
        }

        void Start() => Animate();

        private void Animate()
        {
            var duration = Random.Range(0.25f, 0.5f);
            transform.DOMoveY(transform.position.y + Random.Range(0.1f, 0.3f), duration)
                // .SetRelative()
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        
    }
}
