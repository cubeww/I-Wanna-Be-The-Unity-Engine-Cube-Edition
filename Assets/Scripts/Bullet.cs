using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    int timer = 40;
    float hspeed = 16;
    float xscale;
    PixelPerfectCollider collider;

    private void Start()
    {
        var player = GameObject.FindObjectOfType<Player>();
        foreach (var i in player.GetComponentsInChildren<Transform>())
        {
            if (i.gameObject.name == "Sprite")
                xscale = i.localScale.x;
        }
        collider = GetComponent<PixelPerfectCollider>();
    }

    private void Update()
    {
        if (timer-- < 0)
        {
            GameObject.Destroy(gameObject);
        }

        transform.position += new Vector3(hspeed * xscale, 0);
        if (collider.PlaceMeeting(transform.position.x, transform.position.y, "Block"))
        {
            GameObject.Destroy(gameObject);
        }
    }
}
