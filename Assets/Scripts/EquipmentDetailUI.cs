using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentDetailUI : MonoBehaviour
{
    [Header("Detail UI")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text statText;
    [SerializeField] private TMP_Text upgradeText;

    [Header("Panel")]
    [SerializeField] private GameObject detailPanel;

    [Header("Action Buttons")]
    [SerializeField] private GameObject equipButton;
    [SerializeField] private GameObject upgradeButton;

    [Header("Upgrade")]//강화버튼 클릭시 팝업 참조
    [SerializeField] private UpgradePopupManager upgradePopupManager;

    private StoreItemData selectedItem;

    public void ShowItem(StoreItemData data)
    {   

        selectedItem = data;

        if (data == null)
            return;

        if (detailPanel != null)
            detailPanel.SetActive(true);

        // 아이템 이미지
        if (itemImage != null)
        {
            itemImage.sprite = data.ItemIcon;
            itemImage.enabled = true;
        }


        // 아이템 이름
        if (itemNameText != null)
            itemNameText.text = data.ItemName;


        // 아이템 설명
        if (descriptionText != null)
            descriptionText.text = data.ItemDescription;


        // 아이템 종류에 따라 상세 정보 변경
        switch (data.ItemType)
        {
            case StoreItemType.Gauntlet:

                if (equipButton != null) //장착 버튼 활성화(건틀릿, 로브에만 적용)
                    equipButton.SetActive(true);
                
                if (upgradeButton != null)//강화버튼 활성화
                    upgradeButton.SetActive(true);

                if (statText != null)
                {
                    int finalDamage =
                        PlayerInventoryManager.Instance
                            .GetGauntletDamage(data);

                    statText.text =
                        "Damage : " + finalDamage;
                }

                if (upgradeText != null)
                {
                    int level =
                        PlayerInventoryManager.Instance
                            .GetGauntletUpgradeLevel(
                                data.ItemId
                            );

                    upgradeText.text =
                        "Upgrade : +" + level;
                    
                    if (upgradeButton != null)
                    {
                        upgradeButton.SetActive(
                            level < 17
                        );
                    }

                    upgradeText.gameObject.SetActive(true);
                }

                break;


            case StoreItemType.Robe:
                
                if (equipButton != null) //장착 버튼 활성화(건틀릿, 로브에만 적용)
                    equipButton.SetActive(true);

                if (upgradeButton != null)//강화버튼 활성화
                    upgradeButton.SetActive(true);

                if (statText != null)
                {
                    int damage =
                        PlayerInventoryManager.Instance
                            .GetGauntletDamage(data);

                    statText.text =
                        "Damage : " + damage;
                }

                if (upgradeText != null)
                {
                    int level =
                        PlayerInventoryManager.Instance
                            .GetRobeUpgradeLevel(
                                data.ItemId
                            );

                    upgradeText.text =
                        "Upgrade : +" + level;

                    upgradeText.gameObject.SetActive(true);
                }

                break;


            case StoreItemType.Potion:
                
                //선택된 아이템이 포션이면 강화나 장착 버튼이 나타나지 않도록 설정
                if (equipButton != null)
                    equipButton.SetActive(false);

                if (upgradeButton != null)
                    upgradeButton.SetActive(false);

                // 회복 포션인 경우
                if (statText != null)
                {
                    if (data.HealAmount > 0)
                    {
                        statText.text =
                            "Heal : +" + data.HealAmount;
                    }
                    else
                    {
                        // 공격력 강화 포션 등은 설명만 사용
                        statText.text = "";
                    }
                }

                // 아이템은 강화 레벨 없음
                if (upgradeText != null)
                {
                    upgradeText.text = "";
                    upgradeText.gameObject.SetActive(false);
                }

                break;


            case StoreItemType.Pet:

                if (equipButton != null)
                    equipButton.SetActive(true);

                if (upgradeButton != null)
                    upgradeButton.SetActive(false);

                // 펫 능력치는 나중에 추가
                if (statText != null)
                    statText.text = "";

                if (upgradeText != null)
                {
                    upgradeText.text = "";
                    upgradeText.gameObject.SetActive(false);
                }

                break;
        }
    }


    //장비창에서 Equip버튼을 누르면  이 함수가 실행되어서 
    // 선택된게 건틀릿인지,로브인지 확인한다음 장착처리하는 구조
    public void EquipSelectedItem()
    {
        if (selectedItem == null ||
            PlayerInventoryManager.Instance == null)
            return;

        // 건틀릿 장착
        if (selectedItem.ItemType == StoreItemType.Gauntlet)
        {
            PlayerInventoryManager.Instance.EquipGauntlet(
                selectedItem.ItemId,
                selectedItem.ElementType
            );
        }

        // 로브 장착
        else if (selectedItem.ItemType == StoreItemType.Robe)
        {
            PlayerInventoryManager.Instance.EquipRobe(
                selectedItem.ItemId
            );
        }
        else if (selectedItem.ItemType == StoreItemType.Pet)
        {
            PlayerInventoryManager.Instance.EquipPet(
                selectedItem.ItemId
            );
        }
    }

    //강화버튼 누르면 호출하는 함수: 강화 팝업을 여는 기능임
    public void OpenSelectedItemUpgradePopup()
    {
        if (selectedItem == null ||
            upgradePopupManager == null)
            return;

        upgradePopupManager.OpenUpgradePopup(
            selectedItem
        );
    }

    public void RefreshSelectedItem()
    {
        if (selectedItem == null)
            return;

        ShowItem(selectedItem);
    }
}