using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public Player player;
    public bool isPlayerLight;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        InitializePlayer();
    }

    private void InitializePlayer()
    {

        isPlayerLight = player != null;
    }

    private Player FindActivePlayer()
    {
        foreach (Player p in FindObjectsByType<Player>(FindObjectsSortMode.None))
        {
            if (p.gameObject.activeInHierarchy)
                return p;
        }
        return null;
    }  

    public void SwitchPlayer(Player newPlayer)
    {
        player = newPlayer;
        isPlayerLight = !isPlayerLight;
    }
}
