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

    float x { get => transform.position.x; set => transform.position = new Vector3(value, y, transform.position.z); }
    float y { get => transform.position.y; set => transform.position = new Vector3(x, value, transform.position.z); }

    float xprevious;
    float yprevious;

    bool onPlatform = false;

    public GameObject sprite;
    SpriteAnimator animator;

    public World world;
    PixelPerfectCollider collider;
    public BloodEmitter bloodEmitter;
    public Bullet bullet;

    public AudioSource jumpSound;
    public AudioSource djumpSound;
    public AudioSource shootSound;
    public AudioSource walljumpSound;

    void Start()
    {
#if UNITY_EDITOR
        // Do something to help us debug the level
        if (GameObject.FindObjectsOfType<World>().Length < 1)
        {
            GameObject.Instantiate(world);
            World.gameStarted = true;
            World.saveScene = SceneManager.GetActiveScene().name;
        }
#endif
        DontDestroyOnLoad(gameObject);

        collider = GetComponent<PixelPerfectCollider>();
        animator = sprite.GetComponent<SpriteAnimator>();

        if (World.autosave)
        {
            World.SaveGame(true);
            World.autosave = false;
        }
    }

    void Update()
    {
        xprevious = x;
        yprevious = y;

        var L = Input.GetKey(KeyCode.LeftArrow);
        var R = Input.GetKey(KeyCode.RightArrow);

        var h = 0;

        if (R)
            h = 1;
        else if (L)
            h = -1;

        var notOnBlock = !collider.PlaceMeeting(x, y - 1, "Block");
        var onVineL = collider.PlaceMeeting(x - 1, y, "WalljumpL") && notOnBlock;
        var onVineR = collider.PlaceMeeting(x + 1, y, "WalljumpR") && notOnBlock;

        if (h != 0)
        {
            if (h == -1)
                sprite.transform.localScale = new Vector3(-1, 1);
            else if (h == 1)
                sprite.transform.localScale = new Vector3(1, 1);

            animator.currentAnimation = "Running";
            animator.imageSpeed = 0.5f;
        }
        else
        {
            animator.currentAnimation = "Idle";
            animator.imageSpeed = 0.2f;
        }
        hspeed = maxSpeed * h;

        if (!onPlatform)
        {
            if (vspeed > 0.05)
            {
                animator.currentAnimation = "Jump";
            }
            else if (vspeed < -0.05)
            {
                animator.currentAnimation = "Fall";
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

        if (Input.GetKeyDown(KeyCode.Z))
            Shoot();

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            Jump();

        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            VJump();

        // Walljumps
        if (onVineL || onVineR)
        {
            if (onVineR)
                sprite.transform.localScale = new Vector3(-1, 1);
            else
                sprite.transform.localScale = new Vector3(1, 1);

            vspeed = -2;
            animator.currentAnimation = "Sliding";
            animator.imageSpeed = 0.5f;

            if ((onVineL && Input.GetKeyDown(KeyCode.RightArrow)) || (onVineR && Input.GetKeyDown(KeyCode.LeftArrow)))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    if (onVineR)
                        hspeed = -15;
                    else
                        hspeed = 15;

                    vspeed = 9;

                    walljumpSound.Play();
                    animator.currentAnimation= "Jump";
                }
                else
                {
                    if (onVineR)
                        hspeed = -3;
                    else
                        hspeed = 3;

                    animator.currentAnimation = "Fall";
                }
            }
        }

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

        // Update position
        transform.position = new Vector3(x, y);
    }
    void Jump()
    {
        if (collider.PlaceMeeting(x, y - 1, "Block") || collider.PlaceMeeting(x, y - 1, "Platform") || onPlatform)
        {
            vspeed = -jump;
            djump = true;
            jumpSound.Play();
        }
        else if (djump)
        {
            animator.currentAnimation = "Jump";
            vspeed = -jump2;
            djump = false;
            djumpSound.Play();
        }
    }
    void VJump()
    {
        if (vspeed > 0)
            vspeed *= 0.45f;
    }
    void Shoot()
    {
        if (GameObject.FindObjectsOfType<Bullet>().Length < 4)
        {
            var inst = GameObject.Instantiate(bullet);
            inst.transform.position = new Vector3(x, y);
            shootSound.Play();
        }
    }
}
