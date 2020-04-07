using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerCamera : MonoBehaviour
{
    PixelPerfectCamera camera;
    private void Start()
    {
        camera = GetComponent<PixelPerfectCamera>();
    }
    private void Update()
    {
        var player = GameObject.FindObjectOfType<Player>();
        if (player != null)
        {
            var xFollow = player.x;
            var yFollow = player.y;

            var width = camera.refResolutionX;
            var height = camera.refResolutionY;

            transform.position = new Vector3(Mathf.Floor(xFollow / width) * width + width / 2, 
                Mathf.Floor(yFollow / height) * height + height / 2, transform.position.z);
        }
    }
}
