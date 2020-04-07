using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour
{
    string roomCaption = "I Wanna Be The Unity Engine Cube Edition";
    WindowCaption windowCaption = new WindowCaption();

    public static int savenum = 1;
    public static Difficulty difficulty = Difficulty.Medium;
    public enum Difficulty
    {
        Medium = 0,
        Hard = 1,
        VeryHard = 2,
        Impossible = 3,
    }
    public static int death = 0;
    public static int time = 0;
    public static int grav = 1;

    public static bool gameStarted = false;
    public static bool autosave = false;
    public static string startScene = "Stage01";

    public static string saveScene;
    public static float savePlayerX;
    public static float savePlayerY;
    public static int saveGrav;

    public Player playerPrefab;
    static Player _playerPrefab;

    // May move these to separate class
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

        _playerPrefab = playerPrefab;

        // Initialize game
        windowCaption.SetWindowCaption(roomCaption);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 50;
    }

    void Update()
    {
        if (gameStarted)
        {
            // Restart game
            if (Input.GetKeyDown(KeyCode.R))
            {
                SaveGame(false);
                LoadGame(false);
            }

            // Update title
            windowCaption.SetWindowCaption(roomCaption);
        }
    }

    public static void LoadGame(bool loadFile)
    {
        if (loadFile)
        {
            var saveJson = File.ReadAllText($"Data/save{savenum}");
            var saveFile = JsonUtility.FromJson<SaveFile>(saveJson);

            death = saveFile.death;
            time = saveFile.time;

            difficulty = saveFile.difficulty;
            saveScene = saveFile.scene;

            savePlayerX = saveFile.playerX;
            savePlayerY = saveFile.playerY;
            saveGrav = saveFile.playerGrav;
        }
        gameStarted = true;
        autosave = false;
        grav = saveGrav;

        foreach (var p in GameObject.FindObjectsOfType<Player>())
            GameObject.Destroy(p.gameObject);

        var player = GameObject.Instantiate<Player>(_playerPrefab);
        player.gameObject.transform.position = new Vector3(savePlayerX, savePlayerY);

        SceneManager.LoadScene(saveScene);
    }

    public static void SaveGame(bool savePosition)
    {
        if (savePosition)
        {
            var player = GameObject.FindObjectOfType<Player>();
            if (player != null)
            {
                saveScene = SceneManager.GetActiveScene().name;
                savePlayerX = player.transform.position.x;
                savePlayerY = player.transform.position.y;
                saveGrav = grav;
            }
        }

        var saveFile = new SaveFile()
        {
            death = death,
            time = time,
            difficulty = difficulty,
            scene = saveScene,
            playerX = savePlayerX,
            playerY = savePlayerY,
            playerGrav = saveGrav,
        };

        var saveJson = JsonUtility.ToJson(saveFile);
        if (!Directory.Exists("Data"))
            Directory.CreateDirectory("Data");

        File.WriteAllText($"Data/save{savenum}", saveJson);
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