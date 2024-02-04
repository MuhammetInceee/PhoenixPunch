using UnityEngine;

namespace Characters.CharacterBase
{
    [CreateAssetMenu(menuName = "BlobBoxer/CharacterBase/CharacterBaseData", fileName = "CharacterBaseData")]
    public class CharacterBaseData : ScriptableObject
    {
        [SerializeField] private float attackRange;
        public float AttackRange => attackRange;

        [SerializeField] private float startHealth;
        public float StartHealth => startHealth;
    }
}
