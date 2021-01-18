using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrenade_icon : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player_Controller p = collision.GetComponent<Player_Controller>();
        if (p != null)
        {
            if (p.CollectGrenade())
                gameObject.SetActive(false);
        }
    }
}
