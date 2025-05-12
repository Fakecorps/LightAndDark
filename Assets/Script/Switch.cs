using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;

public class Switch : MonoBehaviour
{

    private PlayerInput _inputControl;
    public GameObject Light;
    public GameObject Dark;
    private GameObject _currentPlayer;
    public GameObject Camera_Point;

    private void Awake()
    {
        _inputControl = new PlayerInput();
        _inputControl.Player.Switch.started += SwitchController;
    }
    private void OnEnable()
    {
        _inputControl.Enable();
    }

    private void OnDisable()
    {
        _inputControl.Disable();
    }
    private void SwitchController(InputAction.CallbackContext context)
    {
        Player currentPlayerComponent = _currentPlayer.GetComponent<Player>();
        if (currentPlayerComponent.IsAttacking())
        {
            Debug.Log("Cannot switch while attacking");
            return;
        }
        Vector3 currentPosition = _currentPlayer.transform.position;
        Debug.Log("Switched");
        if (Light.activeInHierarchy)
        {
            Light.SetActive(false);
            Dark.SetActive(true);
            _currentPlayer = Dark;
        }
        else if (Dark.activeInHierarchy)
        {
            Light.SetActive(true);
            Dark.SetActive(false);
            _currentPlayer = Light;
        }
        _currentPlayer.transform.position = currentPosition;

    }

    void Start()
    {
        Light.SetActive(true);
        Dark.SetActive(false);
        _currentPlayer = Light;
    }

    void Update()
    {
        Camera_Point.transform.position = _currentPlayer.transform.position;
    }
}
