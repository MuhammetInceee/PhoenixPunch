using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    public enum GameStates
    {
        Start,
        Game,
        Fatality,
        Wait,
        End
    }

    public enum HapticType
    {
        Medium,
        Light
    }
    
    public class GameManager : Singleton<GameManager>
    {
        private static readonly int Dance = Animator.StringToHash("Dance");
        public static readonly List<Color> FanColors = new(){Color.red, Color.blue, Color.green, Color.magenta, Color.cyan};

        private HapticType _hapticType;
        private GameStates _gameState;
        [SerializeField] private Animator player;
        [SerializeField] private Animator aI;


        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        public GameStates GetGameState()
        {
            return _gameState;
        }

        public void SetGameState(GameStates state)
        {
            _gameState = state;
        }

        public async void DanceBegin()
        {
            aI.SetBool(Dance, true);
            player.SetBool(Dance, true);

            await Task.Delay(2000);
            
            aI.SetBool(Dance, false);
            player.SetBool(Dance, false);
        }

        internal void Haptic(HapticType type)
        {
            if(PlayerPrefs.GetInt("Haptic", 1) == 0) return;

            if (type == HapticType.Medium) Taptic.Medium();
            else Taptic.Light();
        }
    }
}
