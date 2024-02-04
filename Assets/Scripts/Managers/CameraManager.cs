using System;
using Cinemachine;
using UnityEngine;

namespace Managers
{
    public class CameraManager : Singleton<CameraManager>
    {
        private float _shakeTimer;
        private float _shakeTimerTotal;
        private float _startingIntensity;

        [SerializeField] private CinemachineBrain brain;

        [Header("CM Virtual Cameras")]
        [SerializeField] private CinemachineVirtualCamera gameCMCamera;
        [SerializeField] private CinemachineVirtualCamera startCMCamera;
        [SerializeField] private CinemachineVirtualCamera fatalityCamera;
        [SerializeField] private CinemachineVirtualCamera happyCamera;

        public static Action<float, float> CameraShakeAction;

        private void Awake()
        {
            ShakeCamera(0, 0);
            SetCameraDuration(2);
        }

        private void OnEnable()
        {
            CameraShakeAction += ShakeCamera;
        }

        private void OnDisable()
        {
            CameraShakeAction -= ShakeCamera;
        }

        private void ShakeCamera(float intensity, float time) 
        {
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
                gameCMCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

            _startingIntensity = intensity;
            _shakeTimerTotal = time;
            _shakeTimer = time;
        }

        private void Update()
        {
            if (!(_shakeTimer > 0)) return;
            _shakeTimer -= Time.deltaTime;
            var cinemachineBasicMultiChannelPerlin =
                gameCMCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 
                Mathf.Lerp(_startingIntensity, 0f, 1 - _shakeTimer / _shakeTimerTotal);
        }

        public void StartCameraChange()
        {
            startCMCamera.gameObject.SetActive(false);
            gameCMCamera.gameObject.SetActive(true);
        }

        public void SetFatalityCamera()
        {
            gameCMCamera.gameObject.SetActive(false);
            fatalityCamera.gameObject.SetActive(true);
        }

        public void SetHappyCamera()
        {
            fatalityCamera.gameObject.SetActive(false);
            happyCamera.gameObject.SetActive(true);
        }

        public void SetCameraDuration(float duration)
        {
            brain.m_DefaultBlend.m_Time = duration;
        }
    }
}
