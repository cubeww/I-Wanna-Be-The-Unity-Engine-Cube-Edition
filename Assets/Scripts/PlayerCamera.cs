using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerCamera : MonoBehaviour
{
    PixelPerfectCamera camera;
    float xStart;
    float yStart;

    private void Start()
    {
        camera = GetComponent<PixelPerfectCamera>();

        xStart = transform.position.x;
        yStart = transform.position.y;
    }
    private void Update()
    {
        var player = GameObject.FindObjectOfType<Player>();
        if (player != null)
        {
            var xFollow = player.x - xStart + camera.refResolutionX / 2;
            var yFollow = player.y - yStart + camera.refResolutionY / 2;

            var width = camera.refResolutionX;
            var height = camera.refResolutionY;

            transform.position = new Vector3(Mathf.Floor(xFollow / width) * width + xStart,
                Mathf.Floor(yFollow / height) * height + yStart, transform.position.z);
        }
    }
}
