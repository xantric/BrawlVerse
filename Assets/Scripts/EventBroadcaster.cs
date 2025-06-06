using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBroadcaster : MonoBehaviour
{
    public static event Action<GameObject> Attacked;

    public void HeAttacked() 
    {
        Debug.Log("sir attacked");
        Attacked?.Invoke(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) 
        {
            HeAttacked();
        }
    }
}
