using UnityEngine;

namespace CollisionPhysics
{
    [CreateAssetMenu(menuName = "BlobBoxer/Physics/PunchController", fileName = "PunchControllerSettings")]
    public class PunchControllerData : ScriptableObject
    {
        [SerializeField] private float punchForce;
        public float PunchForce => punchForce;

        [SerializeField] private float punchDamage;
        public float PunchDamage => punchDamage;
    }
}
