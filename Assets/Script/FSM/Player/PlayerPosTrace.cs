using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class PlayerPosTrace : MonoBehaviour
{
    public GameObject light;
    public GameObject dark;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!dark.activeSelf)
        {
            transform.position = light.transform.position;
        }
        else
        {
            transform.position = dark.transform.position;
        }

    }
}
