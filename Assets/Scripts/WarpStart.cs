using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarpStart : MonoBehaviour
{
    public Difficulty difficulty;
    public PixelPerfectCollider collider;

    public enum Difficulty
    {
        Medium = 0,
        Hard = 1,
        VeryHard = 2,
        Impossible = 3,
        LoadGame = 4,
    }

    void Start()
    {
        collider = GetComponent<PixelPerfectCollider>();    
    }

    void Update()
    {
        if (collider.PlaceMeeting(transform.position.x, transform.position.y, "Player"))
        {
            if (difficulty == Difficulty.LoadGame)
            {
                if (File.Exists($"Data/save{World.savenum}"))
                {
                    // Load exists game
                    World.LoadGame(true);
                }
                else
                {
                    // Restart scene
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
            else
            {
                // Start new game
                World.gameStarted = true;
                World.autosave = true;

                World.difficulty = (World.Difficulty)difficulty;

                if (File.Exists($"Data/save{World.savenum}"))
                    File.Delete($"Data/save{World.savenum}");

                SceneManager.LoadScene(World.startScene);
            }
        }
    }
}
