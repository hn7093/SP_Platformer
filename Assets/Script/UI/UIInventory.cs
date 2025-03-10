using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class UIInventory : MonoBehaviour
{
    [HideInInspector] public ItemSlot[] itemSlots;
    public GameObject inventoryWindow;
    public Transform slotPanel;
    private Transform dropPosition;
    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedStatName;
    public TextMeshProUGUI selectedStatValue;
    public Button useButton;
    public Button dropButton;
    public Button equipButton;
    public Button unequipButton;

    ItemSO selectedItem;
    int selectedIndex;
    int curEquipIndex;
    // components
    private PlayerMovement movement;
    private PlayerStat condition;
    // Start is called before the first frame update
    void Start()
    {
        movement = CharacterManager.Instance.Player.movement;
        condition = CharacterManager.Instance.Player.stat;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        // 이벤트 등록
        movement.indentory += Toggle;
        CharacterManager.Instance.Player.addItem += AddItem;

        // 버튼 이벤트 등록
        useButton.onClick.AddListener(OnUseButton);
        dropButton.onClick.AddListener(OnDropButton);
        equipButton.onClick.AddListener(OnEquipButton);
        unequipButton.onClick.AddListener(OnUnequipButton);


        // 인벤토리 초기화
        inventoryWindow.SetActive(false);
        itemSlots = new ItemSlot[slotPanel.childCount];
        // 초기화
        for (int i = 0; i < slotPanel.childCount; i++)
        {
            itemSlots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            itemSlots[i].index = i;
            itemSlots[i].uIInventory = this;
        }

        // 정보 리셋
        ClearSelectedWindow();
    }
    // 정보 리셋
    void ClearSelectedWindow()
    {
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        useButton.gameObject.SetActive(false);
        dropButton.gameObject.SetActive(false);
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(false);
    }

    // 인벤토리 열기, 닫기
    public void Toggle()
    {
        inventoryWindow.SetActive(!IsOpen());
    }
    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }
    public void AddItem()
    {
        ItemSO data = CharacterManager.Instance.Player.item;

        // 중복 가능 체크
        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.item = null;
                return;
            }
        }

        // 중복 불가능이거나 중복 가능이지만 꽉찬 칸만 있는 경우
        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.itemData = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.item = null;
            return;
        }

        // 인벤토리가 꽉 찬 경우
        ThrowItem(data);
        CharacterManager.Instance.Player.item = null;
    }

    public void UpdateUI()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].itemData != null)
            {
                itemSlots[i].Set();
            }
            else
            {
                itemSlots[i].Clear();
            }
        }
    }

    // 같은 아이템이고, 최대 스택 수보다 적은 칸을 반환
    ItemSlot GetItemStack(ItemSO data)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].itemData == data && itemSlots[i].quantity < data.maxStack)
            {
                return itemSlots[i];
            }
        }
        return null;
    }

    // 가장 앞의 빈 칸을 반환
    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].itemData == null)
            {
                return itemSlots[i];
            }
        }
        return null;
    }
    // 드롭 아이템 생성
    public void ThrowItem(ItemSO data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    // 아이템 슬롯 클릭 시
    public void SelectItem(int index)
    {
        if (itemSlots[index].itemData == null)
        {
            return;
        }

        selectedItem = itemSlots[index].itemData;
        selectedIndex = index;


        // ui 표기
        selectedItemName.text = selectedItem.displayName;
        selectedItemDescription.text = selectedItem.description;

        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        for (int i = 0; i < selectedItem.eatableData.Length; i++)
        {
            selectedStatName.text += selectedItem.eatableData[i].type.ToString() + "\n";
            selectedStatValue.text += selectedItem.eatableData[i].value.ToString() + "\n";
        }

        useButton.gameObject.SetActive(selectedItem.type == ItemType.Eatable);
        dropButton.gameObject.SetActive(true);
        //equipButton.gameObject.SetActive(selectedItem.type == ItemType.Equipable && !itemSlots[index].isEquipped);
        //unequipButton.gameObject.SetActive(selectedItem.type == ItemType.Equipable && itemSlots[index].isEquipped);
    }

    public void OnUseButton()
    {
        if (selectedItem.type == ItemType.Eatable)
        {
            for (int i = 0; i < selectedItem.eatableData.Length; i++)
            {
                switch (selectedItem.eatableData[i].type)
                {
                    case EatableType.Health:
                        condition.Heal(selectedItem.eatableData[i].value);
                        break;
                    case EatableType.Stemina:
                        condition.Eat(selectedItem.eatableData[i].value);
                        break;
                    case EatableType.Speed:
                        movement.BuffSpeed(selectedItem.eatableData[i].value);
                        break;
                    case EatableType.Jump:
                        movement.BuffJump(selectedItem.eatableData[i].value);
                        break;
                }
            }
            RemoveSelectItem();
        }
    }
    public void OnDropButton()
    {
        // 드롭 아이템 생성
        ThrowItem(selectedItem);
        // 인벤토리에서 제거
        RemoveSelectItem();
    }

    // 인벤토리에서 1개 제거
    void RemoveSelectItem()
    {
        itemSlots[selectedIndex].quantity--;
        if (itemSlots[selectedIndex].quantity <= 0)
        {
            itemSlots[selectedIndex].itemData = null;
            selectedItem = null;
            selectedIndex = -1;
            ClearSelectedWindow();
        }

        UpdateUI();
    }

    public void OnEquipButton()
    {
        // 이미 장착된 아이템이 있는 경우 장착해제
        if (itemSlots[curEquipIndex].isEquipped)
        {
            UnEquip(curEquipIndex);
        }

        itemSlots[selectedIndex].isEquipped = true;
        curEquipIndex = selectedIndex;
        //CharacterManager.Instance.Player.equipment.EquipNew(selectedItem);
        UpdateUI();
        SelectItem(selectedIndex);
    }
    void UnEquip(int index)
    {
        itemSlots[index].isEquipped = false;
        //CharacterManager.Instance.Player.equipment.UnEquip();
        UpdateUI();
        if (selectedIndex == index)
        {
            SelectItem(selectedIndex);
        }
    }
    public void OnUnequipButton()
    {
        UnEquip(selectedIndex);
    }
}
