using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UnityEngine;

namespace HelperScripts
{
    public static class AnimationHelper
    {
        public static void PlayAnimation(this Animator animator, string animationName, ref bool changeBool, float duration, Action onComplete)
        {
            changeBool = true;
            animator.SetBool(animationName, true);
            
            DisableBoolAfterAnimation(() =>
            {
                if(animator == null) return;
                animator.SetBool(animationName, false);
                onComplete?.Invoke();
            }, SecondsToMilliseconds(duration));
        }
        
        private static async void DisableBoolAfterAnimation(Action onComplete, int animationLength)
        {
            await Task.Delay(animationLength);
            onComplete?.Invoke();
        }
        
        private static int SecondsToMilliseconds(float seconds) {
            return Mathf.RoundToInt(seconds * 1000f);
        }

    }
}