using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public ItemSO itemData;
    [HideInInspector] public UIInventory uIInventory;
    public Image icon;
    private Button button;
    public TextMeshProUGUI quatityText;
    private Outline outline;
    public int index;
    public bool isEquipped = false;
    public int quantity; // 누적 수

    void Awake()
    {
        button = GetComponentInChildren<Button>();
        quatityText = GetComponentInChildren<TextMeshProUGUI>();
        outline = GetComponent<Outline>();
        button.onClick.AddListener(OnClickSlot);
    }
    private void OnEnable()
    {
        outline.enabled = isEquipped;
    }
    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = itemData.icon;
        quatityText.text = quantity > 1 ? quantity.ToString() : string.Empty;

        if (outline != null)
        {
            outline.enabled = isEquipped;
        }
    }

    public void Clear()
    {
        itemData = null;
        icon.gameObject.SetActive(false);
        quatityText.text = string.Empty;
        outline.enabled = false;
    }

    public void OnClickSlot()
    {
        uIInventory.SelectItem(index);
    }
}
