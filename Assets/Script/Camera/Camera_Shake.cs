using Cinemachine;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }

    [Header("Shake Settings")]
    public float shakeDuration = 0.2f;
    public float shakeIntensity = 0.5f;
    public float frequencyGain = 1.0f;

    private CinemachineVirtualCamera activeCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            // 平滑过渡抖动强度
            noise.m_AmplitudeGain = Mathf.Lerp(0, shakeIntensity, shakeTimer / shakeDuration);

            if (shakeTimer <= 0f)
            {
                // 抖动结束，重置参数
                noise.m_AmplitudeGain = 0f;
                noise.m_FrequencyGain = 0f;
            }
        }
    }

    public void Initialize(CinemachineVirtualCamera currentCamera)
    {
        activeCamera = currentCamera;
        noise = activeCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise == null)
        {
            noise = activeCamera.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    public void TriggerShake()
    {
        if (activeCamera == null || noise == null)
        {
            Debug.LogWarning("CinemachineShake not properly initialized!");
            return;
        }

        shakeTimer = shakeDuration;
        noise.m_AmplitudeGain = shakeIntensity;
        noise.m_FrequencyGain = frequencyGain;
    }
}