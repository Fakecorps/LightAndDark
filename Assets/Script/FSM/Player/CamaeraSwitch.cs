using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaeraSwitch : MonoBehaviour
{
    [SerializeField]private CinemachineVirtualCamera lightCamera;
    [SerializeField] private CinemachineVirtualCamera darkCamera;
    private bool isLightCameraActive = true;
    void Start()
    {
        SetCameraPriorities();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isLightCameraActive = !isLightCameraActive;
            SetCameraPriorities();
        }
    }

    private void SetCameraPriorities()
    {
        lightCamera.Priority = isLightCameraActive ? 10 : 0;
        darkCamera.Priority = isLightCameraActive ? 0 : 10;
    }
}
