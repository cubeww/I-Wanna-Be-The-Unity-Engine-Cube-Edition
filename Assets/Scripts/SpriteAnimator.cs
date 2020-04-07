using UnityEngine;
using System;

public class SpriteAnimator : MonoBehaviour
{
    public SpriteAnimation[] animations = new SpriteAnimation[1] { new SpriteAnimation() { name = "Default" } };

    public string startAnimation = "Default";

    string _currentAnimation;

    public string currentAnimation
    {
        set
        {
            _currentAnimation = value;
            foreach (var i in animations)
            {
                if (i.name == _currentAnimation)
                    animation = i;
            }
        }
        get
        {
            return _currentAnimation;
        }
    }

    public SpriteAnimation animation { get; private set; }

    public float imageSpeed;
    public float imageIndex;

    public delegate void OnAnimationEnd();
    public OnAnimationEnd onAnimationEnd;

    SpriteRenderer spriteRenderer;

    private void Start()
    {
        currentAnimation = startAnimation;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        imageIndex += imageSpeed;

        spriteRenderer.sprite = animation.sprites[(int)imageIndex % animation.sprites.Length];

        if (imageIndex > animation.sprites.Length)
        {
            imageIndex -= (int)imageIndex;
            if (onAnimationEnd != null)
                onAnimationEnd();
        }
    }
}

[Serializable]
public class SpriteAnimation
{
    public string name;
    public Sprite[] sprites;
}


