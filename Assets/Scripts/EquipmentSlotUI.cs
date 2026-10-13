using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentSlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image slotImage;
    [SerializeField] private TMP_Text itemNameText;

    private Button button;
    private EquipmentDetailUI detailUI;

    private StoreItemData itemData;


    private void Awake()
    {
        // 자기 오브젝트에 붙어있는 Button 자동으로 가져오기
        button = GetComponent<Button>();

        // 장비창의 상세정보 UI 자동으로 찾기
        detailUI = FindFirstObjectByType<EquipmentDetailUI>();

        // 버튼 클릭 이벤트 자동 연결
        if (button != null)
            button.onClick.AddListener(OnClickSlot);
    }


    public void SetItem(StoreItemData data)
    {
        itemData = data;

        if (slotImage != null)
        {
            slotImage.sprite = data.ItemIcon;

            Color color = slotImage.color;
            color.a = 1f;
            slotImage.color = color;
        }

        if (itemNameText != null)
            itemNameText.text = data.ItemName;

        if (button != null)
            button.interactable = true;
    }


    public void ClearSlot()
    {
        itemData = null;

        if (slotImage != null)
        {
            slotImage.sprite = null;

            Color color = slotImage.color;
            color.a = 0f;
            slotImage.color = color;
        }

        if (itemNameText != null)
            itemNameText.text = "";

        if (button != null)
            button.interactable = false;
    }


    private void OnClickSlot()
    {
        if (itemData == null || detailUI == null)
            return;

        detailUI.ShowItem(itemData);
    }
}