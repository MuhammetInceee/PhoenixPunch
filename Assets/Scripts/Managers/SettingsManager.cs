using System;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    private const int ToggleOnPos = 44;
    private const int ToggleOffPos = -44;
    
    [SerializeField] private SettingsButton hapticButton;
    [SerializeField] private SettingsButton musicButton;
    [SerializeField] private SettingsButton sfxButton;
    [SerializeField] private Button backButton;


    private void Awake()
    {
        InitToggle(hapticButton, "Haptic");
        InitToggle(musicButton, "Music");
        InitToggle(sfxButton, "SFX");
    }

    private void InitToggle(SettingsButton button, string prefs) => button.toggle.DOAnchorPosX(PlayerPrefs.GetInt(prefs, 1) == 1 ? ToggleOnPos : ToggleOffPos, 0);

    private void OnEnable()
    {
        hapticButton.button.onClick.AddListener(Haptic);
        musicButton.button.onClick.AddListener(Music);
        sfxButton.button.onClick.AddListener(SFX);
        backButton.onClick.AddListener(Back);
    }

    private void Haptic()
    {
        PlayerPrefs.SetInt("Haptic", PlayerPrefs.GetInt("Haptic",1) == 0 ? 1 : 0);
        ToggleMove(hapticButton);
    }

    private void Music()
    {
        PlayerPrefs.SetInt("Music", PlayerPrefs.GetInt("Music",1) == 0 ? 1 : 0);
        SoundManager.Instance.StadiumSound();
        ToggleMove(musicButton);
    }

    private void SFX()
    {
        PlayerPrefs.SetInt("SFX", PlayerPrefs.GetInt("SFX",1) == 0 ? 1 : 0);
        ToggleMove(sfxButton);
    }


    private void Back()
    {
        gameObject.GetComponent<RectTransform>().DOAnchorPosY(1920, 0.3f)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        GameManager.Instance.SetGameState(GameStates.Game);
    }

    private void ToggleMove(SettingsButton button)
    {
        var xValue = button.toggle.anchoredPosition.x;
        if (Mathf.Approximately(xValue, 44)) button.toggle.DOAnchorPosX(-44, 0.3f);
        else if (Mathf.Approximately(xValue, -44)) button.toggle.DOAnchorPosX(44, 0.3f);
    }
}

[Serializable]
public struct SettingsButton
{
    public Button button;
    public RectTransform toggle;
}
