using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A pickable object. Put on the Orb (needs a Collider, trigger or not).
/// For now it just prints what was picked up and removes the object.
/// Later: replace the Debug.Log with inventory.Add(itemName).
/// </summary>
public class PickupItem : Interactable
{
    [Header("Item")]
    [SerializeField] private string itemName = "Orb";
    [SerializeField] private int amount = 1;

    [Header("Audio")]
    [Tooltip("Sound played when the item is successfully picked up.")]
    [SerializeField] private AudioClip pickupSound;
    [Range(0f, 1f)]
    [SerializeField] private float pickupVolume = 1f;

    [Header("Events")]
    [SerializeField] private UnityEvent onPickedUp;   // e.g. hook GameManager.Collect here

    public string ItemName => itemName;
    public int Amount => amount;

    public override string Prompt => "Pick up " + itemName;

    public override void Interact(PlayerInteractor interactor)
    {
        Debug.Log("Picked up: " + itemName + " x" + amount);

        Inventory inventory = interactor.GetComponent<Inventory>();
        if (inventory == null) inventory = FindFirstObjectByType<Inventory>();
        if (inventory != null && !inventory.Add(itemName, amount))
            return;

        // Only play the sound if the pickup actually succeeded
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupVolume);

        onPickedUp?.Invoke();
        Destroy(gameObject);
    }
}