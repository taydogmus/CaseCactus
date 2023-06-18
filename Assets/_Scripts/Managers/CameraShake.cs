using Cinemachine;
using UnityEngine;

namespace Tuna
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
        public static CameraShake Instance { get; private set; }
        private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
        private float shakeTimer;
        private float shakeTimerTotal;
        private float startingIntensity;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            cinemachineBasicMultiChannelPerlin = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void Update()
        {
            //Shakira shakira
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;
                if (shakeTimer <= 0)
                {
                    //Time is up!
                    cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
                }
            }
        }
        public void ShakeCamera(float intensity, float time)
        {
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
            startingIntensity = time;
            shakeTimerTotal = time;
            shakeTimer = time;
        }

    }
}