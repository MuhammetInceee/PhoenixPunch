using System;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource punchSoundEffect;
    [SerializeField] private AudioSource blockSoundEffect;
    [SerializeField] private AudioSource stadiumEffect;
    [SerializeField] private AudioSource[] bellEffects;
    [SerializeField] private AudioSource[] manEffects;
    [SerializeField] private AudioSource finishHim;

    public static Action PunchSoundAction;
    public static Action BlockSoundAction;

    private void Start()
    {
        StadiumSound();
    }

    private void OnEnable()
    {
        AddListener();
    }

    private void OnDisable()
    {
        RemoveListener();
    }

    private void AddListener()
    {
        PunchSoundAction += PunchPlay;
        BlockSoundAction += BlockPlay;
    }

    private void RemoveListener()
    {
        PunchSoundAction -= PunchPlay;
        BlockSoundAction -= BlockPlay;
    }

    private void PunchPlay()
    {
        if(PlayerPrefs.GetInt("SFX", 1) == 0) return; 
        punchSoundEffect.Play();
    }

    private void BlockPlay()
    {
        if(PlayerPrefs.GetInt("SFX", 1) == 0) return;
        blockSoundEffect.Play();
    }

    public void StartBell()
    {
        bellEffects[Random.Range(0, bellEffects.Length)].Play();
    }

    public async void PlayAnnouncerSound(Action onComplete)
    {
        var source = manEffects[Random.Range(0, manEffects.Length)];
        var delay = (int)(source.clip.length * 1000);
        
        source.Play();
        await Task.Delay(delay);
        onComplete?.Invoke();
    }

    internal void StadiumSound()
    {
        if(PlayerPrefs.GetInt("Music", 1) == 1) stadiumEffect.Play();
        else stadiumEffect.Stop();
    }
}
