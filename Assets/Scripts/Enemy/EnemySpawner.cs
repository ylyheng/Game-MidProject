using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject dollPrefab;

    [Header("Level 2")]
    [SerializeField] private Transform Point1;
    [SerializeField] private Transform Point2;

    [Header("Level 3")]
    [SerializeField] private Transform Point3;
    [SerializeField] private Transform Point4;
    [SerializeField] private Transform Point5;

    private readonly List<GameObject> spawnedDolls = new List<GameObject>();

    /// <summary>Read-only access to the currently spawned dolls.</summary>
    public IReadOnlyList<GameObject> SpawnedDolls => spawnedDolls;

    /// <summary>
    /// Clears any previously spawned dolls and spawns the ones for the given level.
    /// Safe to call from other scripts. Unknown levels just clear and warn.
    /// </summary>
    public void SpawnForLevel(int level)
    {
        ClearSpawned();

        switch (level)
        {
            case 2:
                SpawnAt(Point1);
                SpawnAt(Point2);
                break;

            case 3:
                SpawnAt(Point3);
                SpawnAt(Point4);
                SpawnAt(Point5);
                break;

            default:
                Debug.LogWarning($"[EnemySpawner] No spawn points configured for level {level}.");
                break;
        }
    }

    /// <summary>Destroys every doll this spawner has created.</summary>
    public void ClearSpawned()
    {
        foreach (GameObject doll in spawnedDolls)
        {
            if (doll != null) Destroy(doll);
        }
        spawnedDolls.Clear();
    }

    private void SpawnAt(Transform point)
    {
        if (dollPrefab == null)
        {
            Debug.LogWarning("[EnemySpawner] dollPrefab is not assigned!");
            return;
        }
        if (point == null) return;

        GameObject doll = Instantiate(dollPrefab, point.position, point.rotation);
        spawnedDolls.Add(doll);
    }
}