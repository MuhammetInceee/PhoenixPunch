using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Characters;
using Characters.Player;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Managers
{
    public class UIManager : Singleton<UIManager>
    {
        public static Action<Slider, float> HealthUIUpdateAction;
        public static Action<CharacterTypes> LevelEndAction;

        private int counter = 3;
        [SerializeField] private TextMeshProUGUI counterText;
        public GameObject fatality;
        [SerializeField] private Image starFillable;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button settingsButton;

        [Header("Require Components")] 
        [SerializeField] private Player player;

        [FoldoutGroup("Screens")] 
        [SerializeField] private GameObject tapToPlayScreen;
        [FoldoutGroup("Screens")]
        [SerializeField] private GameObject gameScreen;
        [FoldoutGroup("Screens")]
        [SerializeField] private GameObject levelEndScreens;
        [FoldoutGroup("Screens")]
        [SerializeField] private GameObject[] levelSuccessScreens;
        [FoldoutGroup("Screens")]
        [SerializeField] private GameObject[] levelFailScreens;        
        [FoldoutGroup("Screens")]
        [SerializeField] private GameObject settingPopUp;

        [FoldoutGroup("MovablePanels")]
        [SerializeField] private RectTransform upToDownPanel;
        [FoldoutGroup("MovablePanels")]
        [SerializeField] private RectTransform downToUpPanel;

        [FoldoutGroup("GeneralPanel")] 
        [SerializeField] private Image levelEndBackground;

        [FoldoutGroup("Triggers")] 
        [SerializeField] private Button tapToPlayTrigger;
        [FoldoutGroup("Triggers")]
        [SerializeField] private Button fatalityTrigger;

        private Button Button => levelEndBackground.GetComponent<Button>();

        private void Awake()
        {
            gameScreen.SetActive(false);
            levelEndScreens.SetActive(false);
            levelSuccessScreens.ToList().ForEach(e => e.SetActive(false));
            levelFailScreens.ToList().ForEach(e => e.SetActive(false));
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnSubscribeEvent();
        }

        private void SubscribeEvents()
        {
            HealthUIUpdateAction += HealthUpdate;
            LevelEndAction += LevelEnd;
            Button.onClick.AddListener(ScreenClick);
            tapToPlayTrigger.onClick.AddListener(TapToPlayTrigger);
            fatalityTrigger.onClick.AddListener(Fatality);
            restartButton.onClick.AddListener(Restart);
            settingsButton.onClick.AddListener(SettingsButton);
        }

        private void UnSubscribeEvent()
        {
            HealthUIUpdateAction -= HealthUpdate;
            LevelEndAction -= LevelEnd;
        }

        private void HealthUpdate(Slider slider, float damage)
        {
            DOTween.To(() => slider.value, x => slider.value = x, slider.value - damage, 0.3f);
        }

        private void LevelEnd(CharacterTypes type)
        {
            ScreenToggle(levelEndScreens, gameScreen);

            var sequence = DOTween.Sequence();

            levelEndBackground.DOFade(0.9f, 0.3f)
                .SetEase(Ease.Linear);

            if(type == CharacterTypes.AI) levelSuccessScreens.ToList().ForEach(e => e.SetActive(true));
            else levelFailScreens.ToList().ForEach(e => e.SetActive(true));

            Tween upToDownTween = upToDownPanel.DOAnchorPosY(170, 0.3f)
                .SetEase(Ease.OutSine);

            Tween downToUpTween = downToUpPanel.DOAnchorPosY(-195, 0.3f)
                .SetEase(Ease.OutBack);

            sequence.Insert(0, upToDownTween);
            sequence.Insert(0, downToUpTween);

            sequence.Play().OnComplete(LevelEndStar);
        }

        private void LevelEndStar()
        {
            DOTween.To(() => starFillable.fillAmount, x => starFillable.fillAmount = x, player.GetHealthRatio(), 1.4f)
                .SetEase(Ease.Linear);
            // .OnUpdate(() =>
            // {
            //     switch (starFillable.fillAmount)
            //     {
            //         case > 0.34f and < 0.36f:
            //             print("1 Star");
            //             break;
            //         case > 0.665f and < 0.68f:
            //             print("2 Star");
            //             break;
            //         case 1:
            //             print("3 Star");
            //             break;
            //     }
            // });
        }

        private void ScreenClick()
        {
            Button.interactable = false;
            
            var sequence = DOTween.Sequence();

            Tween upToDownTween = upToDownPanel.DOAnchorPosY(1470, 0.3f)
                .SetEase(Ease.OutSine);

            Tween downToUpTween = downToUpPanel.DOAnchorPosY(-1470, 0.3f)
                .SetEase(Ease.OutBack);

            sequence.Insert(0, upToDownTween);
            sequence.Insert(0, downToUpTween);
            
            sequence.Play()
                .OnComplete(SwitchToRandomScene);
        }
        
        private void SwitchToRandomScene()
        {
            var sceneNames = new List<string>();

            var activeSceneName = SceneManager.GetActiveScene().name;

            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                if (sceneName != activeSceneName)
                {
                    sceneNames.Add(sceneName);
                }
            }

            var randomSceneName = sceneNames[Random.Range(0, sceneNames.Count)];

            SceneManager.LoadScene(randomSceneName);
        }

        public async void TapToPlay()
        {
            await Task.Delay(2000);
            StartCoroutine(Counter());
            SoundManager.Instance.StartBell();
            Dance();
        }

        private async void Dance()
        {
            GameManager.Instance.DanceBegin();
            await Task.Delay(1500);
            SoundManager.Instance.PlayAnnouncerSound(() =>
            {
                player.Movement();
            });
        }

        private void ScreenToggle(GameObject openScreen, GameObject closeScreen)
        {
            closeScreen.SetActive(false);
            openScreen.SetActive(true);
        }
        
        private IEnumerator Counter()
        {
            counterText.gameObject.SetActive(true);
            
            while (counter > 0)
            {
                counterText.text = counter.ToString();
                yield return new WaitForSeconds(1f);
                counter--;
            }

            counterText.text = "FIGHT!";
            ObjectCloser(counterText.gameObject);
        }

        private async void ObjectCloser(GameObject targetObj)
        {
            await Task.Delay(500);
            targetObj.SetActive(false);
        }

        public void Fatality()
        {
            fatality.GetComponentInChildren<Button>().interactable = false;
            player.FatalityMovement();
        }

        private void SettingsButton()
        {
            if(GameManager.Instance.GetGameState() != GameStates.Game) return;
            GameManager.Instance.SetGameState(GameStates.Wait);
            settingPopUp.SetActive(true);
            settingPopUp.GetComponent<RectTransform>().DOAnchorPosY(0, 0.3f)
                .SetEase(Ease.OutSine);
        }

        private void TapToPlayTrigger()
        {
            tapToPlayScreen.SetActive(false);
            gameScreen.SetActive(true);
            TapToPlay();
            CameraManager.Instance.StartCameraChange();
        }
        
        private void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
