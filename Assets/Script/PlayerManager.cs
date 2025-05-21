using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    [SerializeField]private Player player;
    public bool isPlayerLight;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        { 
            Destroy(instance);
        }

        player = FindActivePlayer();
        isPlayerLight = true;
    }

    private Player FindActivePlayer()
    {
        foreach (Player p in FindObjectsOfType<Player>())
        {
            if (p.gameObject.activeSelf)
            {
                return p;
            }
        }
        return null;
    }

    public void SetActivePlayer(Player activePlayer)
    {
        player = activePlayer;
    }
}
