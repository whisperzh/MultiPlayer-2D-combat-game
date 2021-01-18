using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileVanquisher : MonoBehaviour
{
    public float flyingspeed=1;

    private void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up*flyingspeed;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, transform.up, 0.8f);

        Debug.DrawRay(transform.position, transform.up);
        if (raycastHit2D)
        {
            TileMapMorpher tmc = raycastHit2D.collider.gameObject.GetComponent<TileMapMorpher>();
            if (tmc != null)
            {
                //tmc.des(raycastHit2D.point);
                Destroy(this.gameObject);
            }
        }
    }
}
