using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

// Pixel Perfect Collider
// Use texture as collision mask, check each pixel data
// In I Wanna Fangame, this collision method is usually adopted

public class PixelPerfectCollider : MonoBehaviour
{
    MaskData maskData;
    SpriteRenderer maskRenderer;

    int left { get => maskData.left; }
    int right { get => maskData.right; }
    int top { get => maskData.top; }
    int bottom { get => maskData.bottom; }

    bool[] boolData { get => maskData.boolData; }

    int width { get => maskData.width; }
    int height { get => maskData.height; }

    float xPos { get => gameObject.transform.position.x; }
    float yPos { get => gameObject.transform.position.y; }

    int xPivot { get => (int)(maskRenderer.sprite.pivot.x); }
    int yPivot { get => (int)(maskRenderer.sprite.pivot.y); }

    float xScale { get => gameObject.transform.localScale.x; }
    float yScale { get => gameObject.transform.localScale.y; }

    float rotation { get => gameObject.transform.rotation.eulerAngles.z; }

    void Start()
    {
        maskRenderer = GetComponent<SpriteRenderer>();
        var texture = maskRenderer.sprite.texture;

        // Get mask data
        if (!World.maskDataManager.ContainsKey(texture))
        {
            maskData = new MaskData();

            // Get bool data
            var boolData = new bool[texture.width * texture.height];
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    boolData[x + y * texture.width] = ((Color32)texture.GetPixel(x, y)).a != 0;
                }
            }
            maskData.boolData = boolData;

            // Get relative texture bounding box

            // Get bbox bottom
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    if (((Color32)texture.GetPixel(x, y)).a != 0)
                    {
                        maskData.bottom = y - yPivot;
                        goto OutBottom;
                    }
                }
            }
        OutBottom:

            // Get bbox top
            for (var y = texture.height - 1; y >= 0; y--)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    if (((Color32)texture.GetPixel(x, y)).a != 0)
                    {
                        maskData.top = y - yPivot;
                        goto OutTop;
                    }
                }
            }
        OutTop:

            // Get bbox left
            for (var x = 0; x < texture.width; x++)
            {
                for (var y = 0; y < texture.height; y++)
                {
                    if (((Color32)texture.GetPixel(x, y)).a != 0)
                    {
                        maskData.left = x - xPivot;
                        goto OutLeft;
                    }
                }
            }
        OutLeft:

            // Get bbox right
            for (var x = texture.width - 1; x >= 0; x--)
            {
                for (var y = 0; y < texture.height; y++)
                {
                    if (((Color32)texture.GetPixel(x, y)).a != 0)
                    {
                        maskData.right = x - xPivot;
                        goto OutRight;
                    }
                }
            }
        OutRight:

            // Other stuff
            maskData.width = texture.width;
            maskData.height = texture.height;

            // Add to mask data manager to ensure we don't load repeatedly
            World.maskDataManager[texture] = maskData;
        }
        else
        {
            // Load data directly from the mask data manager
            maskData = World.maskDataManager[texture];
        }

        // Add to colliders
        if (gameObject.tag == null)
            Debug.LogWarning($"Pixel perfect collidable game object \"{gameObject.name}\" is using a empty string tag !");

        if (!World.colliders.ContainsKey(gameObject.tag))
        {
            World.colliders[gameObject.tag] = new List<PixelPerfectCollider>();
        }
        World.colliders[gameObject.tag].Add(this);
    }

    public bool PlaceMeeting(float x, float y, string tag)
    {
        if (!World.colliders.ContainsKey(tag))
            return false;

        var cders = World.colliders[tag];
        if (cders.Count == 0)
            return false;

        var x1 = x;
        var y1 = y;

        // Get self bounding box with transform
        GetBoundingBox(left, right, top, bottom, out var left1, out var right1, out var top1, out var bottom1,
            x1, y1, xScale, yScale, rotation);

        foreach (var i in cders)
        {
            if (i == this)
                continue;

            var x2 = i.xPos;
            var y2 = i.yPos;

            // Get other bounding box with transform
            GetBoundingBox(i.left, i.right, i.top, i.bottom, out var left2, out var right2, out var top2, out var bottom2,
                x2, y2, i.xScale, i.yScale, i.rotation);

            // Get intersection
            int iLeft = Max(left1, left2);
            int iRight = Min(right1, right2);
            int iBottom = Max(bottom1, bottom2);
            int iTop = Min(top1, top2);

            // Check each pixel
            var sina1 = Sin(-rotation * Deg2Rad);
            var cosa1 = Cos(-rotation * Deg2Rad);

            var sina2 = Sin(-i.rotation * Deg2Rad);
            var cosa2 = Cos(-i.rotation * Deg2Rad);
            for (int xx = iLeft; xx <= iRight; xx++)
            {
                for (int yy = iBottom; yy <= iTop; yy++)
                {
                    var lx1 = xx - x1;
                    var ly1 = yy - y1;
                    RotateAround(lx1, ly1, 0, 0, sina1, cosa1, out var lx1a, out var ly1a);
                    var px1 = (int)(lx1a / xScale + xPivot);
                    var py1 = (int)(ly1a / yScale + yPivot);
                    var p1 = px1 >= 0 && py1 >= 0 && px1 < width && py1 < height && boolData[px1 + py1 * width];

                    var lx2 = xx - x2;
                    var ly2 = yy - y2;
                    RotateAround(lx2, ly2, 0, 0, sina2, cosa2, out var lx2a, out var ly2a);
                    var px2 = (int)(lx2a / i.xScale + i.xPivot);
                    var py2 = (int)(ly2a / i.yScale + i.yPivot);
                    var p2 = px2 >= 0 && py2 >= 0 && px2 < i.width && py2 < i.height && i.boolData[px2 + py2 * i.width];

                    if (p1 && p2)
                        return true;
                }
            }
        }
        return false;
    }

    void OnDestroy()
    {
        World.colliders[gameObject.tag].Remove(this);
    }

    static void GetBoundingBox(int left, int right, int top, int bottom, out int left1, out int right1, out int top1, out int bottom1,
        float x, float y, float xscale, float yscale, float angle)
    {
        angle *= Deg2Rad;
        var sina = Sin(angle);
        var cosa = Cos(angle);

        RotateAround(x + left * xscale, y + bottom * yscale, x, y, sina, cosa, out var xlb, out var ylb);
        RotateAround(x + (right + 1) * xscale - 1, y + bottom * yscale, x, y, sina, cosa, out var xrb, out var yrb);
        RotateAround(x + left * xscale, y + (top + 1) * yscale - 1, x, y, sina, cosa, out var xlt, out var ylt);
        RotateAround(x + (right + 1) * xscale - 1, y + (top + 1) * yscale - 1, x, y, sina, cosa, out var xrt, out var yrt);

        left1 = (int)(Min(xlb, xrb, xlt, xrt) + 0.5f);
        right1 = (int)(Max(xlb, xrb, xlt, xrt) + 0.5f);
        bottom1 = (int)(Min(ylb, yrb, ylt, yrt) + 0.5f);
        top1 = (int)(Max(ylb, yrb, ylt, yrt) + 0.5f);
    }

    static void RotateAround(float xs, float ys, float xo, float yo, float sina, float cosa, out float ox, out float oy)
    {
        ox = (xs - xo) * cosa - (ys - yo) * sina + xo;
        oy = (xs - xo) * sina + (ys - yo) * cosa + yo;
    }
}

