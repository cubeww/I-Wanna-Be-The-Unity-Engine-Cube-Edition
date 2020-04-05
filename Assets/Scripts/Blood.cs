using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public class Blood : MonoBehaviour
{
    float x, y, hspeed, vspeed, gravity, speed, direction = 0;
    public PixelPerfectCollider collider;

    void Start()
    {
        x = transform.position.x;
        y = transform.position.y;
        direction = (int)Random.Range(0, 35) * 10;
        speed = Random.Range(0, 6);
        gravity = -(0.1f + Random.Range(0, 0.2f));
        hspeed = speed * Cos(direction * Deg2Rad);
        vspeed = speed * Sin(direction * Deg2Rad);
    }

    void Update()
    {
        vspeed += gravity;

        x += hspeed;
        y += vspeed;

        if (collider.PlaceMeeting(x, y, "Block"))
        {
            hspeed = 0;
            vspeed = 0;
            gravity = 0;
            return;
        }

        transform.position = new Vector3(x, y);
    }
}
