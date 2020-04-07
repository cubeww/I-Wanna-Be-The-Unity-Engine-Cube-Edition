using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

// World singleton helps us manage the game
public class World : Singleton<World>
{
    string roomCaption = "I Wanna Be The Unity Engine Cube Edition";
    WindowCaption windowCaption = new WindowCaption();

    public int savenum = 1;
    public Difficulty difficulty = Difficulty.Medium;
    public enum Difficulty
    {
        Medium = 0,
        Hard = 1,
        VeryHard = 2,
        Impossible = 3,
    }
    public int death = 0;
    public int time = 0;
    public int grav = 1;

    public bool gameStarted = false;
    public bool autosave = false;
    public string startScene = "Stage01";

    public string saveScene;
    public float savePlayerX;
    public float savePlayerY;
    public int saveGrav;

    public Player playerPrefab;
    public AudioSource deathSound;

    // May move these to separate class
    public Dictionary<Texture2D, MaskData> maskDataManager = new Dictionary<Texture2D, MaskData>();
    public Dictionary<string, List<PixelPerfectCollider>> colliders = new Dictionary<string, List<PixelPerfectCollider>>();

    void Start()
    {
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

    public void LoadGame(bool loadFile)
    {
        if (loadFile)
        {
            var saveJson = File.ReadAllText($"Data/save{savenum}");
            var saveFile = JsonUtility.FromJson<SaveFile>(saveJson);

            instance.death = saveFile.death;
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

        var player = GameObject.Instantiate<Player>(playerPrefab);
        player.gameObject.transform.position = new Vector3(savePlayerX, savePlayerY);

        SceneManager.LoadScene(saveScene);
    }

    public void SaveGame(bool savePosition)
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
    public void KillPlayer()
    {
        deathSound.Play();
        death++;
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