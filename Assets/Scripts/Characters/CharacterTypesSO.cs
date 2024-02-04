using UnityEngine;

namespace Characters
{
    public enum CharacterTypes
    {
        Player,
        AI
    }
    
    [CreateAssetMenu(menuName = "BlobBoxer/Character/CharacterTypes", fileName = "CharacterTypes")]
    public class CharacterTypesSO : ScriptableObject
    {
        public CharacterTypes characterType;
    }
}
