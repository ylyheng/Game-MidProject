using System;
using UnityEngine;

/// <summary>
/// Shows the item you just picked up in your right hand.
/// Attach to an empty object that is a CHILD of the Main Camera
/// (so it moves with the view). The held object spawns at this
/// transform's position and rotation.
/// </summary>
public class HoldObjectRightHand : MonoBehaviour
{
    
    [Serializable]
    public class HeldItem
    {
        [Tooltip("Must match the Item Name on PickupItem, e.g. Orb, Stone, Cube.")]
        public string itemName;
        public GameObject prefab;
        [Tooltip("Fine-tune placement relative to this hand object.")]
        public Vector3 positionOffset;
        public Vector3 rotationOffset;
        public Vector3 scale = Vector3.one;
    }

    [Header("References")]
    [SerializeField] private Inventory inventory;   // drag the Player here (auto-finds if empty)
    [SerializeField] AutoReplayAudio soundManager;

    [Header("Items you can hold")]
    [SerializeField] private HeldItem[] heldItems;

    private GameObject currentObject;
    private string currentItemName;

    void Awake()
    {
        if (inventory == null) inventory = FindFirstObjectByType<Inventory>();
    }

    void OnEnable()
    {
        if (inventory == null) return;
        inventory.OnItemAdded += HandleItemAdded;
        inventory.OnItemRemoved += HandleItemRemoved;
    }

    void OnDisable()
    {
        if (inventory == null) return;
        inventory.OnItemAdded -= HandleItemAdded;
        inventory.OnItemRemoved -= HandleItemRemoved;
    }

    // Picked something up: hold it
    private void HandleItemAdded(string itemName, int amount)
    {
        Hold(itemName);
    }

    // Used something: if we ran out of what's in hand, clear it (or show another item)
    private void HandleItemRemoved(string itemName, int amount)
    {
        if (itemName != currentItemName) return;
        if (inventory.Has(itemName)) return;   // still have more, keep holding

        Release();

        // Fall back to any other item we carry that has a hand model
        foreach (var kv in inventory.Items)
        {
            if (FindEntry(kv.Key) != null)
            {
                Hold(kv.Key);
                break;
            }
        }
    }

    public void Hold(string itemName)
    {
        HeldItem entry = FindEntry(itemName);
        if (entry == null || entry.prefab == null) return;   // no hand model for this item

        Release();

        // check the current level and whether player hold atifact than enable chassing
        GameManager manager = GameManager.Instance;
        if (manager != null && manager.insertedPockets >= 1)
        {
            if (itemName == "Orb" || itemName == "Cube" || itemName == "Stone")
            {
                DollChaser.SetAllChasing(true);

                if (soundManager)soundManager.Play("chassing");
                
            }
        }

        currentObject = Instantiate(entry.prefab, transform);
        currentObject.transform.localScale = entry.scale;
        currentObject.transform.localPosition = entry.positionOffset;
        currentObject.transform.localRotation = Quaternion.Euler(entry.rotationOffset);
        currentItemName = itemName;

        MakeDecorative(currentObject);
    }

    public void Release()
    {
        if (currentObject != null) Destroy(currentObject);
        currentObject = null;
        currentItemName = null;

        DollChaser.SetAllChasing(false);
        if (soundManager)soundManager.Play("default");
    }

    private HeldItem FindEntry(string itemName)
    {
        if (heldItems == null) return null;
        foreach (HeldItem h in heldItems)
            if (h != null && h.itemName == itemName) return h;
        return null;
    }

    // The held copy is only visual: no collision, physics, pickup or tag
    private static void MakeDecorative(GameObject obj)
    {
        foreach (Interactable i in obj.GetComponentsInChildren<Interactable>(true)) Destroy(i);
        foreach (Orb o in obj.GetComponentsInChildren<Orb>(true)) Destroy(o);
        foreach (Collider c in obj.GetComponentsInChildren<Collider>(true)) Destroy(c);
        foreach (Rigidbody rb in obj.GetComponentsInChildren<Rigidbody>(true)) Destroy(rb);
        foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
            t.gameObject.tag = "Untagged";
    }
}