using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public bool bounce = false;

    public float hspeed = 0;
    public float vspeed { get; private set; } = 0;

    public float yspeed = 0;

    PixelPerfectCollider collider;

    private void Start()
    {
        collider = GetComponent<PixelPerfectCollider>();
    }

    private void Update()
    {
        if (hspeed != 0 || vspeed != 0 || yspeed != 0)
        {
            if (bounce)
            {
                if (collider.PlaceMeeting(transform.position.x + hspeed, transform.position.y, "Block"))
                {
                    hspeed = -hspeed;
                }

                if (collider.PlaceMeeting(transform.position.x, transform.position.y + vspeed + yspeed, "Block"))
                {
                    if (vspeed != 0)
                    {
                        yspeed = -vspeed;
                        vspeed = 0;
                    }
                    else
                    {
                        vspeed = -yspeed;
                        yspeed = 0;
                    }
                }
            }

            var col = collider.InstancePlace(transform.position.x, transform.position.y + 2, "Player");
            if (col != null)
            {
                var p = col.GetComponent<Player>();

                p.y += vspeed + yspeed;
                if (!p.GetComponent<PixelPerfectCollider>().PlaceMeeting(p.transform.position.x + hspeed, p.transform.position.y, "Block"))
                {
                    p.x += hspeed;
                }
            }

            transform.position += new Vector3(0, yspeed);

            if (vspeed > 0)
            {
                yspeed = vspeed;
                vspeed = 0;
            }
            if (yspeed < 0)
            {
                vspeed = yspeed;
                yspeed = 0;
            }
        }

        transform.position += new Vector3(hspeed, vspeed);
    }
}
