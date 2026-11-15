using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("State")]
    [SerializeField] private int totalPockets = 3;
    
    public int insertedPockets = 0;
    public event Action OnGameCompleted;

    private bool isCompleted = false;

    // 1. Define an enum for the pocket levels
    public enum PocketLevel
    {
        LevelOne = 1,
        LevelTwo = 2,
        LevelThree = 3
    }

    // 2. Single event that passes WHICH level was inserted
    public event Action<PocketLevel> OnPocketInserted;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 3. Update the method to accept the level
    public void PocketInserted(PocketLevel level)
    {
        if (isCompleted) return;

        insertedPockets++;

        Debug.Log($"Pocket inserted: {level} ({insertedPockets}/{totalPockets})");

        OnPocketInserted?.Invoke(level);

        if (insertedPockets >= totalPockets)
        {
            isCompleted = true;
            OnGameCompleted?.Invoke();
        }
    }

}