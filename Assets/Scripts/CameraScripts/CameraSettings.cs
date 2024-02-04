using UnityEngine;

namespace CameraScripts
{
    [CreateAssetMenu(menuName = "BlobBoxer/Camera/CameraSettings", fileName = "CameraSettings")]
    public class CameraSettings : ScriptableObject
    {
        [SerializeField] private float shakeIntensity;
        public float ShakeIntensity => shakeIntensity;

        [SerializeField] private float shakeDuration;
        public float ShakeDuration => shakeDuration;
    }
}
