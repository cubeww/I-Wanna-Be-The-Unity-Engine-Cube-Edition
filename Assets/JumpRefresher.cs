using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpRefresher : MonoBehaviour
{
    public int refreshTime = 100;

    int timer = -1;

    private void Update()
    {
        if (GetComponent<SpriteRenderer>().enabled)
        {
            var player = GetComponent<PixelPerfectCollider>().InstancePlace(transform.position.x, transform.position.y, "Player");
            if (player != null)
            {
                player.GetComponent<Player>().djump = true;
                GetComponent<SpriteRenderer>().enabled = false;
                timer = refreshTime;
            }
        }
        if (timer != -1)
        {
            if (timer-- == 0)
            {
                timer = -1;
                GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
}
