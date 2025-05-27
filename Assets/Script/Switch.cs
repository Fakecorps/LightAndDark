using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;

public class Switch : MonoBehaviour
{
    [SerializeField] private GameObject lightPlayer;
    [SerializeField] private GameObject darkPlayer;

    private PlayerInput _input;
    private Vector3 _lastPosition;

    private void Awake()
    {
        _input = new PlayerInput();
        _input.Player.Switch.started += OnSwitch;
        lightPlayer.gameObject.SetActive(true);
        darkPlayer.gameObject.SetActive(false);
    }

    private void Start()
    {
        _lastPosition = transform.position;
    }

    private void OnSwitch(InputAction.CallbackContext context)
    {
        if (PlayerManager.Instance.player.IsAttacking()) return;

        _lastPosition = PlayerManager.Instance.player.transform.position;

        GameObject newPlayer = lightPlayer.activeSelf ? darkPlayer : lightPlayer;
        SetActivePlayer(newPlayer);
    }

    private void SetActivePlayer(GameObject target)
    {
        lightPlayer.SetActive(target == lightPlayer);
        darkPlayer.SetActive(target == darkPlayer);

        PlayerManager.Instance.SwitchPlayer(target.GetComponent<Player>());
        target.transform.position = _lastPosition;
    }

    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();
}
