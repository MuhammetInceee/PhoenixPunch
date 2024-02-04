using CameraScripts;
using Characters;
using Characters.CharacterBase;
using Characters.Player;
using Interfaces;
using Managers;
using UnityEngine;

namespace CollisionPhysics
{
    public class PunchController : MonoBehaviour
    {
        public bool CanTouch = false;
        private PunchControllerData _punchControllerData;
        private CameraSettings _cameraSettings;
        private GameManager _gameManager;
        private const float CollisionDelay = 0.5f;
        private float _lastCollisionTime = -Mathf.Infinity;
        
        private Character _myCharacter;
        private bool CanHit => Time.time - _lastCollisionTime > CollisionDelay && CanTouch;

        private void Start()
        {
            ReadDataResource();
            GetReferences(); 
        }

        private void GetReferences()
        {
            _myCharacter = transform.GetComponentInParent<Character>();
            _gameManager = GameManager.Instance;
        }

        private void ReadDataResource()
        {
            _cameraSettings = Resources.Load<CameraSettings>("Game/Camera/CameraSettings");
            _punchControllerData = Resources.Load<PunchControllerData>("Game/Physics/PunchControllerData");
        }

        private void OnCollisionEnter(Collision other)
        {
            if(!CanHit) return;
            if(other.gameObject.GetComponent<PunchController>()) return;

            var otherCharacter = other.gameObject.GetComponentInParent<Character>();
            var otherIDamageable = other.gameObject.GetComponentInParent<IDamageable>();

            if (otherCharacter == null || otherCharacter == _myCharacter) return;

            if (otherIDamageable != null && !otherCharacter.HasBlock && _myCharacter.HasAttack)
            {
                SuccessHit(otherIDamageable);
            }
            else if (otherCharacter.HasBlock && _myCharacter.HasAttack)
            {
                BlockedHit();
            }
        }

        private void SuccessHit(IDamageable damageable)
        {
            _lastCollisionTime = Time.time;
            CameraManager.CameraShakeAction?.Invoke(_cameraSettings.ShakeIntensity, _cameraSettings.ShakeDuration);
            damageable.Execute(_punchControllerData.PunchDamage);
            _gameManager.Haptic(HapticType.Medium);
            SoundManager.PunchSoundAction?.Invoke();
            
            var transform1 = transform;
            var position = transform1.position;
            
            //ParticleManager.PunchParticleAction?.Invoke(position, transform1.forward);
            ParticleManager.TextParticleAction?.Invoke(position + Vector3.up, Vector3.zero);
        }

        private void BlockedHit()
        {
            _lastCollisionTime = Time.time;
            
            //CameraManager.CameraShakeAction?.Invoke(_cameraSettings.ShakeIntensity, _cameraSettings.ShakeDuration);
            _gameManager.Haptic(HapticType.Light);
            SoundManager.BlockSoundAction?.Invoke();
        }
    }
}
