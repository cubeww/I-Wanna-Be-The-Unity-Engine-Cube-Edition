using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;
using UnityEditor;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    float jump = -8.5f;
    float jump2 = -7;
    float maxSpeed = 3;
    float maxVspeed = 9;

    bool djump = true;

    float hspeed = 0;
    float vspeed = 0;
    float gravity = -0.4f;

    float x;
    float y;
    float xprevious;
    float yprevious;

    bool onPlatform = false;

    public GameObject sprite;

    PixelPerfectCollider collider;
    public BloodEmitter bloodEmitter;

    string currentAnimation;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 50;

        collider = GetComponent<PixelPerfectCollider>();

        x = transform.position.x;
        y = transform.position.y;
    }

    void Update()
    {
        xprevious = x;
        yprevious = y;

        var L = World.GetKeyPersistently(37);
        var R = World.GetKeyPersistently(39);

        var h = 0;

        if (R)
            h = 1;
        else if (L)
            h = -1;

        if (h != 0)
        {
            if (h == -1)
                sprite.transform.localScale = new Vector3(-1, 1);
            else if (h == 1)
                sprite.transform.localScale = new Vector3(1, 1);

            currentAnimation = "Running";
        }
        else
        {
            currentAnimation = "Idle";
        }
        hspeed = maxSpeed * h;

        if (!onPlatform)
        {
            if (vspeed > 0.05)
            {
                currentAnimation = "Jump";
            }
            else if (vspeed < -0.05)
            {
                currentAnimation = "Fall";
            }
        }
        else
        {
            if (!collider.PlaceMeeting(x, y - 4, "Platform"))
            {
                onPlatform = false;
            }
        }

        if (Abs(vspeed) > maxVspeed)
        {
            vspeed = Sign(vspeed) * maxVspeed;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            Jump();

        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            VJump();

        // Move
        vspeed += gravity;
        x += hspeed;
        y += vspeed;

        // Collision

        // Check block
        if (collider.PlaceMeeting(x, y, "Block"))
        {
            x = xprevious;
            y = yprevious;

            if (collider.PlaceMeeting(x + hspeed, y, "Block"))
            {
                if (hspeed <= 0) while (!collider.PlaceMeeting(x - 1, y, "Block")) x--;
                if (hspeed > 0) while (!collider.PlaceMeeting(x + 1, y, "Block")) x++;
                hspeed = 0;
            }

            if (collider.PlaceMeeting(x, y + vspeed, "Block"))
            {
                if (vspeed >= 0) while (!collider.PlaceMeeting(x, y + 1, "Block")) y++;
                if (vspeed < 0)
                {
                    while (!collider.PlaceMeeting(x, y - 1, "Block")) y--;
                    djump = true;
                }
                vspeed = 0;
            }

            if (collider.PlaceMeeting(x + hspeed, y + vspeed, "Block"))
            {
                hspeed = 0;
            }

            x += hspeed;
            y += vspeed;
            if (collider.PlaceMeeting(x, y, "Block"))
            {
                x = xprevious;
                y = yprevious;
            }
        }

        // Check platform
        var platform = collider.InstancePlace(x, y, "Platform");
        if (platform != null)
        {
            if (y - vspeed / 2 >= platform.transform.position.y)
            {
                y = platform.transform.position.y + 9;
                onPlatform = true;
                djump = true;
                vspeed = 0;
            }
        }

        // Check killer
        if (collider.PlaceMeeting(x, y, "Killer"))
        {
            var inst = GameObject.Instantiate(bloodEmitter);
            inst.transform.position = transform.position;
            GameObject.Destroy(gameObject);
        }

        // Update animation
        if (!sprite.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(currentAnimation))
            sprite.GetComponent<Animator>().Play(currentAnimation);

        // Update position
        transform.position = new Vector3(x, y);
    }
    void Jump()
    {
        if (collider.PlaceMeeting(x, y - 1, "Block") || collider.PlaceMeeting(x, y - 1, "Platform") || onPlatform)
        {
            vspeed = -jump;
            djump = true;
        }
        else if (djump)
        {
            currentAnimation = "Jump";
            vspeed = -jump2;
            djump = false;
        }
    }
    void VJump()
    {
        if (vspeed > 0)
            vspeed *= 0.45f;
    }
}
