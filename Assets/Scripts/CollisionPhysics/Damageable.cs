using Characters.CharacterBase;
using Interfaces;
using UnityEngine;

namespace CollisionPhysics
{
    public class Damageable : MonoBehaviour, IDamageable
    {
        private Character _targetCharacter;

        private void Awake()
        {
            GetReference();
        }

        private void GetReference()
        {
            _targetCharacter = GetComponentInParent<Character>();
        }
        
        public void Execute(float damage)
        {
            _targetCharacter.GetDamage(damage);
        }
    }
}
