using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkOut : MonoBehaviour
{
    private SpriteRenderer sp;
    bool _isOut;
    // Start is called before the first frame update
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        _isOut = true;
    }

    private void Update()
    {
        if(_isOut)
        {
            sp.color = Color.Lerp(sp.color, new Color(1, 1, 1, 1), 0.05f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _isOut = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            sp.color = Color.Lerp(sp.color, new Color(1, 1, 1,0.32f), 0.05f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _isOut = true;
        }
    }
}
