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

    [Header("Events")]
    [SerializeField] private UnityEvent onPickedUp;   // e.g. hook GameManager.Collect here

    public string ItemName => itemName;
    public int Amount => amount;

    public override string Prompt => "Pick up " + itemName;

    public override void Interact(PlayerInteractor interactor)
    {
        Debug.Log("Picked up: " + itemName + " x" + amount);

        // FUTURE: interactor.GetComponent<Inventory>()?.Add(itemName, amount);

        // Prefabs can't hold references to scene objects, so find the GameManager here
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null) gm.Collect();

        onPickedUp?.Invoke();
        Destroy(gameObject);
    }
}