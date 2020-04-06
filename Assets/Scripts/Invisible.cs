using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisible : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false; 
    }
}
