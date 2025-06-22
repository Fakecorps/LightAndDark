using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaeraSwitch : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera lightCamera;
    [SerializeField] private CinemachineVirtualCamera darkCamera;
    private bool isLightCameraActive = true;

    void Start()
    {
        SetCameraPriorities();

        // 初始化屏幕抖动系统
        InitializeShake();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isLightCameraActive = !isLightCameraActive;
            SetCameraPriorities();

            // 更新抖动系统的当前相机
            UpdateShakeSystem();
        }
    }

    private void SetCameraPriorities()
    {
        lightCamera.Priority = isLightCameraActive ? 10 : 0;
        darkCamera.Priority = isLightCameraActive ? 0 : 10;
    }

    private void InitializeShake()
    {
        // 确保CinemachineShake实例存在
        if (CinemachineShake.Instance == null)
        {
            GameObject shakeObj = new GameObject("CinemachineShakeManager");
            shakeObj.AddComponent<CinemachineShake>();
        }

        // 初始化抖动系统
        CinemachineShake.Instance.Initialize(isLightCameraActive ? lightCamera : darkCamera);
    }

    private void UpdateShakeSystem()
    {
        if (CinemachineShake.Instance != null)
        {
            Debug.Log("UpdateShakeSystem");
            CinemachineShake.Instance.Initialize(isLightCameraActive ? lightCamera : darkCamera);
        }
    }
}