using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Bottom-center bar showing what the player is carrying.
/// Builds its own slots at runtime, so no prefab is needed.
/// Put this on an empty UI object under your Canvas (e.g. "InventoryBar").
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Inventory inventory;   // drag the Player here (auto-finds if empty)

    [Header("Layout")]
    [SerializeField] private Vector2 slotSize = new Vector2(100f, 70f);
    [SerializeField] private float spacing = 10f;
    [SerializeField] private float bottomMargin = 30f;

    [Header("Look")]
    [SerializeField] private Color slotColor = new Color(0f, 0f, 0f, 0.55f);
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private float nameFontSize = 20f;
    [SerializeField] private float countFontSize = 28f;

    void Awake()
    {
        if (inventory == null) inventory = FindFirstObjectByType<Inventory>();
        SetupContainer();
    }

    void OnEnable()
    {
        if (inventory != null) inventory.OnChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        if (inventory != null) inventory.OnChanged -= Refresh;
    }

    // Anchor to bottom center and lay slots out in a row
    private void SetupContainer()
    {
        RectTransform rt = (RectTransform)transform;
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(0f, bottomMargin);

        HorizontalLayoutGroup row = GetComponent<HorizontalLayoutGroup>();
        if (row == null) row = gameObject.AddComponent<HorizontalLayoutGroup>();
        row.spacing = spacing;
        row.childAlignment = TextAnchor.LowerCenter;
        row.childControlWidth = false;
        row.childControlHeight = false;
        row.childForceExpandWidth = false;
        row.childForceExpandHeight = false;

        ContentSizeFitter fitter = GetComponent<ContentSizeFitter>();
        if (fitter == null) fitter = gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void Refresh()
    {
        // Clear old slots
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        if (inventory == null) return;

        foreach (var item in inventory.Items)
            CreateSlot(item.Key, item.Value);
    }

    private void CreateSlot(string itemName, int count)
    {
        // Background
        GameObject slot = new GameObject("Slot_" + itemName, typeof(RectTransform), typeof(Image));
        slot.transform.SetParent(transform, false);
        ((RectTransform)slot.transform).sizeDelta = slotSize;
        slot.GetComponent<Image>().color = slotColor;

        // Text: item name on top, count underneath
        GameObject textObj = new GameObject("Label", typeof(RectTransform));
        textObj.transform.SetParent(slot.transform, false);

        RectTransform textRt = (RectTransform)textObj.transform;
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = new Vector2(4f, 4f);
        textRt.offsetMax = new Vector2(-4f, -4f);

        TextMeshProUGUI label = textObj.AddComponent<TextMeshProUGUI>();
        label.alignment = TextAlignmentOptions.Center;
        label.color = textColor;
        label.enableWordWrapping = false;
        label.overflowMode = TextOverflowModes.Ellipsis;
        label.fontSize = nameFontSize;
        label.text = itemName + "\n<b><size=" + countFontSize + ">x" + count + "</size></b>";
    }
}