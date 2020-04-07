using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public bool refreshDoubleJump = false;
    PixelPerfectCollider collider;

    private void Start()
    {
        collider = GetComponent<PixelPerfectCollider>();
    }

    private void Update()
    {
        var col = collider.InstancePlace(transform.position.x, transform.position.y, "Player");
        if (col != null)
        {
            var player = col.GetComponent<Player>();
            if (refreshDoubleJump)
                player.djump = true;

            if (player.vspeed < -2)
                player.vspeed = -2;
        }
    }
}
