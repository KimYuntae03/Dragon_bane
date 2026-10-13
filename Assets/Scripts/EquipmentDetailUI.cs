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


    public void ShowItem(StoreItemData data)
    {   

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

                if (statText != null)
                {
                    statText.text =
                        "Damage : " + data.Damage;
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

                    upgradeText.gameObject.SetActive(true);
                }

                break;


            case StoreItemType.Robe:

                if (statText != null)
                {
                    statText.text =
                        "Defense : " + data.Defense;
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
}