using CameraScripts;
using Characters.AI;
using CollisionPhysics;
using Interfaces;
using Managers;
using UnityEngine;

public class FootCollision : MonoBehaviour
{
    private PunchControllerData _punchControllerData;
    private CameraSettings _cameraSettings;
    
    private const float CollisionDelay = 0.5f;
    private float _lastCollisionTime = -Mathf.Infinity;
    
    private bool CanHit => Time.time - _lastCollisionTime > CollisionDelay;
    private void Start()
    {
        _cameraSettings = Resources.Load<CameraSettings>("Game/Camera/CameraSettings");
        _punchControllerData = Resources.Load<PunchControllerData>("Game/Physics/PunchControllerData");
    }

    private void OnCollisionEnter(Collision other)
    {
        if(!CanHit) return;
        
        var otherCharacter = other.gameObject.GetComponentInParent<AI>();
        var damageable = other.gameObject.GetComponentInParent<IDamageable>();

        if (otherCharacter == null) return;
        
        _lastCollisionTime = Time.time;
        CameraManager.CameraShakeAction?.Invoke(_cameraSettings.ShakeIntensity, _cameraSettings.ShakeDuration);
        damageable.Execute(_punchControllerData.PunchDamage);
        Taptic.Medium();
        SoundManager.PunchSoundAction?.Invoke();
            
        var transform1 = transform;
        var position = transform1.position;
            
        //ParticleManager.PunchParticleAction?.Invoke(position, transform1.forward);
        ParticleManager.TextParticleAction?.Invoke(position + Vector3.up, Vector3.zero);

    }
}
