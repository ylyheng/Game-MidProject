using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// STARTER — Score and win condition for the "Orb Collector" lab goal.
/// Create an empty GameObject named "GameManager" and attach this script.
///
/// ALREADY WORKS: a public Collect() method the Player calls when it grabs
/// an orb, plus an OnGUI() label showing the live score in the top-left corner.
///
/// YOUR LAB TASKS:
///   TODO 6: in Awake(), find all existing orbs with GameObject.FindGameObjectsWithTag
///           and set totalOrbs from the result.
///   TODO 7: in OnGUI(), when collected == totalOrbs show a big centered
///           "YOU WIN" message instead of the score line.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("State")]
    [SerializeField] private int totalOrbs = 5;

    private int collected;

    void Awake()
    {
        // Simple singleton so the player can call GameManager.Instance.Collect().
        Instance = this;

        // TODO 6: replace the serialized totalOrbs with a live count:
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("Orb");
        totalOrbs = orbs.Length;
    }

    public void Collect()
    {
        collected++;
        Debug.Log($"[GameManager] Collected {collected}/{totalOrbs}");
    }

    void OnGUI()
    {
        // Immediate-mode UI: runs several times per frame, so keep it cheap.
        GUI.Label(new Rect(12, 10, 400, 30), $"Orbs collected: {collected} / {totalOrbs}", new GUIStyle(GUI.skin.label) { fontSize = 20 });

        // if (collected >= totalOrbs)
        //     {
        //         // Build a big, centered style
        //         GUIStyle winStyle = new GUIStyle(GUI.skin.label);
        //         winStyle.fontSize = 80;
        //         winStyle.alignment = TextAnchor.MiddleCenter;
        //         winStyle.fontStyle = FontStyle.Bold;
        //         winStyle.normal.textColor = Color.yellow;

        //         // Optional outline so it reads on any background
        //         winStyle.normal.textColor = Color.yellow;

        //         // Center a box on screen
        //         float w = Screen.width;
        //         float h = Screen.height;
        //         Rect winRect = new Rect(0, h * 0.35f, w, 120);   // full width, 35% down, 120 tall

        //         GUI.Label(winRect, "YOU WIN!", winStyle);
        //     }
        // TODO 7: when collected >= totalOrbs, draw a large centered "YOU WIN!" label.
    }
}
