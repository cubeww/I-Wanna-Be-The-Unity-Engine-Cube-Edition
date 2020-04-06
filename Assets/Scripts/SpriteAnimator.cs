using UnityEngine;
using System;

public class SpriteAnimator : MonoBehaviour
{
    public SpriteAnimation[] animations;

    public string startAnimation;

    string _currentAnimation;

    string currentAnimation
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

    SpriteAnimation animation;

    public float imageSpeed;
    public float imageIndex;

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
    }
}

[Serializable]
public class SpriteAnimation
{
    public string name;
    public Sprite[] sprites;
}


