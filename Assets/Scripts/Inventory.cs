using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple inventory: item name -> count. Attach to the Player.
/// Other scripts (and later a UI) listen to OnChanged to refresh.
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("0 = unlimited. Otherwise the max number of different item types.")]
    [SerializeField] private int maxSlots = 0;

    [Header("Debug")]
    [SerializeField] private bool logChanges = true;

    public readonly Dictionary<string, int> items = new Dictionary<string, int>();

    /// <summary>Fired whenever anything is added or removed.</summary>
    public event Action OnChanged;

    /// <summary>Fired after an item is added (name, amount added).</summary>
    public event Action<string, int> OnItemAdded;

    /// <summary>Fired after an item is removed (name, amount removed).</summary>
    public event Action<string, int> OnItemRemoved;

    public IReadOnlyDictionary<string, int> Items => items;

    public bool Add(string itemName, int amount = 1)
    {
        if (string.IsNullOrEmpty(itemName) || amount <= 0) return false;

        bool isNew = !items.ContainsKey(itemName);
        if (isNew && maxSlots > 0 && items.Count >= maxSlots)
        {
            if (logChanges) Debug.Log("[Inventory] Full, can't pick up " + itemName);
            return false;
        }

        items[itemName] = GetCount(itemName) + amount;
        Changed();
        OnItemAdded?.Invoke(itemName, amount);
        return true;
    }

    public bool Remove(string itemName, int amount = 1)
    {
        if (!Has(itemName, amount)) return false;

        items[itemName] -= amount;
        if (items[itemName] <= 0) items.Remove(itemName);
        Changed();
        OnItemRemoved?.Invoke(itemName, amount);
        return true;
    }

   

    public bool Has(string itemName, int amount = 1)
    {
        return GetCount(itemName) >= amount;
    }


    public int GetCount(string itemName)
    {
        return items.TryGetValue(itemName, out int count) ? count : 0;
    }

    private void Changed()
    {
        if (logChanges) Debug.Log("[Inventory] " + Summary());
        OnChanged?.Invoke();
    }

    private string Summary()
    {
        if (items.Count == 0) return "(empty)";
        var parts = new List<string>();
        foreach (var kv in items) parts.Add(kv.Key + " x" + kv.Value);
        return string.Join(", ", parts);
    }
}