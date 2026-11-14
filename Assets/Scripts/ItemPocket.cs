using UnityEngine;
using UnityEngine.Events;

public class ItemPocket : Interactable
{
    [Header("Item")]
    [SerializeField] private string itemName = "Orb";
    [SerializeField] private int amount = 1;

    [Header("Artifact")]
    [Tooltip("Prefab to spawn when the item is inserted. Best: a visual-only copy of the item (no PickupItem).")]
    [SerializeField] private GameObject artifactPrefab;

    [Tooltip("Optional: keep the spawned artifact in the same parent hierarchy.")]
    [SerializeField] private bool keepSameParent = false;

    [Header("Audio")]
    [Tooltip("Sound played when the item is successfully inserted.")]
    [SerializeField] private AudioClip insertSound;
    [Range(0f, 1f)]
    [SerializeField] private float insertVolume = 1f;

    [Header("Events")]
    [SerializeField] private UnityEvent OnInsertItem;

    public string ItemName => itemName;
    public int Amount => amount;

    public override string Prompt => "Required " + itemName;

    public override void Interact(PlayerInteractor interactor)
    {
        Debug.Log("Insert: " + itemName + " x" + amount);

        Inventory inventory = interactor.GetComponent<Inventory>();
        if (inventory == null) inventory = FindFirstObjectByType<Inventory>();
        if (inventory != null && !inventory.Remove(itemName, amount))
        {
            // TODO: show UI "you don't have that item"
            return;
        }

        // ---- Spawn the artifact at this transform's exact spot ----
        if (artifactPrefab != null)
        {
            Transform parent = keepSameParent ? transform.parent : null;

            GameObject artifact = Instantiate(
                artifactPrefab,
                transform.position,
                transform.rotation,
                parent
            );

            if (parent != null)
            {
                artifact.transform.position = transform.position;
                artifact.transform.rotation = transform.rotation;
            }

            MakeDecorative(artifact);

            Debug.Log(itemName + "is Inserted");

            GameManager manager = GameManager.Instance;
            if(manager.insertedPockets == 0)
            {
                GameManager.Instance.PocketInserted(GameManager.PocketLevel.LevelOne);
            }
            else if(manager.insertedPockets == 1)
            {
                GameManager.Instance.PocketInserted(GameManager.PocketLevel.LevelTwo);
            }
            else if(manager.insertedPockets >= 2)
            {
                GameManager.Instance.PocketInserted(GameManager.PocketLevel.LevelThree);
            }
            
        }
        else
        {
            Debug.LogWarning("[ItemPocket] No artifactPrefab assigned!");
        }

        // Only play the sound after the item was actually accepted & placed
        if (insertSound != null)
            AudioSource.PlayClipAtPoint(insertSound, transform.position, insertVolume);

        OnInsertItem?.Invoke();
        Destroy(gameObject);
    }

    // The placed artifact is decoration: it must not be pickable or collectable again
    private static void MakeDecorative(GameObject artifact)
    {
        foreach (Interactable i in artifact.GetComponentsInChildren<Interactable>(true))
            Destroy(i);

        foreach (Orb orb in artifact.GetComponentsInChildren<Orb>(true))
            Destroy(orb);

        // Stops PlayerController's OnTriggerEnter (CompareTag("Orb")) from collecting it
        foreach (Transform t in artifact.GetComponentsInChildren<Transform>(true))
            t.gameObject.tag = "Untagged";
    }
}