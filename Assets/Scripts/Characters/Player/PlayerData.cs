using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Player
{
    [CreateAssetMenu(menuName = "BlobBoxer/Player/PlayerSettings", fileName = "PlayerSettings")]
    public class PlayerData : ScriptableObject
    {
        [FoldoutGroup("Inputs")]
        [SerializeField] private int minSwipeDistance;
        public int MinSwipeDistance => minSwipeDistance;
        
    }
}
