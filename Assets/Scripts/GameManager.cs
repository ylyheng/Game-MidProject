using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("State")]
    [SerializeField] private int totalOrbs = 5;
    [SerializeField] private int totalPockets = 3;
    
    public int insertedPockets = 0;
    private int collected = 0;

    // 1. Define an enum for the pocket levels
    public enum PocketLevel
    {
        LevelOne = 1,
        LevelTwo = 2,
        LevelThree = 3
    }

    // 2. Single event that passes WHICH level was inserted
    public event Action<PocketLevel> OnPocketInserted;

    // (Optional) If you STILL want separate explicit events, you can add these:
    // public event Action OnPocketLevelOneInserted;
    // public event Action OnPocketLevelTwoInserted;
    // public event Action OnPocketLevelThreeInserted;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Live count of orbs
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("Orb");
        totalOrbs = orbs.Length;
    }

    public void Collect()
    {
        collected++;
        Debug.Log($"[GameManager] Collected {collected}/{totalOrbs}");
    }

    // 3. Update the method to accept the level
    public void PocketInserted(PocketLevel level)
    {
        insertedPockets++;
        Debug.Log($"[GameManager] Pocket inserted: {level} ({insertedPockets}/{totalPockets})");

        // Fire the event with the specific level data
        OnPocketInserted?.Invoke(level);

        /* 
        // If you chose the explicit separate events approach (Option 2), you would do this instead:
        switch (level)
        {
            case PocketLevel.LevelOne:
                OnPocketLevelOneInserted?.Invoke();
                break;
            case PocketLevel.LevelTwo:
                OnPocketLevelTwoInserted?.Invoke();
                break;
            case PocketLevel.LevelThree:
                OnPocketLevelThreeInserted?.Invoke();
                break;
        }
        */
    }

    void OnGUI()
    {
        // GUI.Label(new Rect(12, 10, 400, 30), $"Orbs collected: {collected} / {totalOrbs}", new GUIStyle(GUI.skin.label) { fontSize = 20 });

        if (insertedPockets > 2)
        {
            GUIStyle winStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 80,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.yellow }
            };

            float w = Screen.width;
            float h = Screen.height;
            Rect winRect = new Rect(0, h * 0.35f, w, 120);

            GUI.Label(winRect, "YOU WIN!", winStyle);
        }
    }
}