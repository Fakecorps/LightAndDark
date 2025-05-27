using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class PlayerPosTrace : MonoBehaviour
{
    Player player;
    void Start()
    {
        player = PlayerManager.instance.player;
    }
    void Update()
    {
        player = PlayerManager.instance.player;
        transform.position = player.transform.position;
        Debug.Log(transform.position);
    }
}
