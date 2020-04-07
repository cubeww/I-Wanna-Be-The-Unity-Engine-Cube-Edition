using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour
{
    bool canSave = true;
    PixelPerfectCollider collider;
    SpriteAnimator animtor;
    int timer = -1;

    private void Start()
    {
        if ((int)World.instance.difficulty > (int)World.Difficulty.Hard)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        collider = GetComponent<PixelPerfectCollider>();
        animtor = GetComponent<SpriteAnimator>();

        animtor.onAnimationEnd = OnAnimationEnd;
    }

    private void Update()
    {
        if (collider.PlaceMeeting(transform.position.x, transform.position.y, "Player"))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                OnSave();
            }
        }
        if (collider.PlaceMeeting(transform.position.x, transform.position.y, "Bullet"))
        {
            OnSave();
        }
        if (timer != -1)
        {
            if (timer-- == 0)
            {
                timer = -1;
                canSave = true;
            }
        }
    }

    void OnSave()
    {
        if (canSave)
        {
            var player = GameObject.FindObjectOfType<Player>();
            if (player != null)
            {
                canSave = false;
                animtor.imageIndex = 1;
                animtor.imageSpeed = 0.017f;
                timer = 30;
                World.instance.SaveGame(true);
            }
        }
    }

    void OnAnimationEnd()
    {
        animtor.imageSpeed = animtor.imageIndex = 0;
    }
}
