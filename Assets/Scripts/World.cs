using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour
{
    string roomCaption = "I Wanna Be The Unity Engine Cube Edition";
    WindowCaption windowCaption = new WindowCaption();
    public static Dictionary<Texture2D, MaskData> maskDataManager = new Dictionary<Texture2D, MaskData>();
    public static Dictionary<string, List<PixelPerfectCollider>> colliders = new Dictionary<string, List<PixelPerfectCollider>>();

    void Start()
    {
        if (GameObject.FindObjectsOfType<World>().Length > 1)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Initialize game
        windowCaption.SetWindowCaption(roomCaption);
    }

    void Update()
    {
        // Restart game
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Update title
        windowCaption.SetWindowCaption(roomCaption);
    }

    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int key);

    // Using "Input.GetKey", unity cannot keep the key state when switching scenes
    // This is not comfortable for the player to press the R key and then the arrow keys
    // I can only think of this method, if you have a better one please tell me
    public static bool GetKeyPersistently(int key)
    {
        return (GetAsyncKeyState(key) & 0x8000) != 0;
    }
}

class WindowCaption
{
    delegate bool EnumWindowsCallBack(IntPtr hwnd, IntPtr lParam);

    [DllImport("user32", CharSet = CharSet.Unicode)]
    static extern bool SetWindowTextW(IntPtr hwnd, string title);

    [DllImport("user32")]
    static extern int EnumWindows(EnumWindowsCallBack lpEnumFunc, IntPtr lParam);

    [DllImport("user32")]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref IntPtr lpdwProcessId);

    IntPtr windowHandle;

    public WindowCaption()
    {
        IntPtr handle = (IntPtr)System.Diagnostics.Process.GetCurrentProcess().Id;
        EnumWindows(new EnumWindowsCallBack(EnumWindCallback), handle);
    }

    public void SetWindowCaption(string caption)
    {
        SetWindowTextW(windowHandle, caption);
    }

    bool EnumWindCallback(IntPtr hwnd, IntPtr lParam)
    {
        IntPtr pid = IntPtr.Zero;
        GetWindowThreadProcessId(hwnd, ref pid);
        if (pid == lParam)
        {
            windowHandle = hwnd;
            return true;
        }
        return false;
    }
}

public class MaskData
{
    public int left;
    public int right;
    public int top;
    public int bottom;

    public int width;
    public int height;

    public bool[] boolData;
}