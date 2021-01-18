using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Z_tester : MonoBehaviour
{
    private Transform upper, lower;
    public bool Dynamic;
    private int time = 3;
    float max = 1;
    float min =-1;
    float me = 0;
    // Start is called before the first frame update
    void Start()
    {
        upper = GameObject.Find("upper").transform;
        lower = GameObject.Find("lower").transform;
        GetZ();
    }

    // Update is called once per frame
    void Update()
    {
        if (Dynamic)
        {
            time--;
            if(time <0)
            {
                GetZ();
                time = 3;
            }
        }
    }
    private void GetZ() {
        me = (transform.position.y - lower.position.y) / (upper.position.y - lower.position.y);
        transform.position = new Vector3(transform.position.x, transform.position.y, me);
    }
}
