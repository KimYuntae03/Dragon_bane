using UnityEngine;
using UnityEngine.UI;

public class ElementIconUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GauntletManager gauntletManager;//플레이어 건틀릿 속성 확인
    [SerializeField] private DragonHealth dragonHealth;//드래곤 속성 확인(DragonHealth.cs에 타입이 있음)

    [Header("UI")]
    [SerializeField] private Image playerElementIcon;
    [SerializeField] private Image dragonElementIcon;

    [Header("Element Icons")] //속성별 적용할 아이콘 이미지
    [SerializeField] private Sprite fireIcon;
    [SerializeField] private Sprite lightningIcon;
    [SerializeField] private Sprite windIcon;

    private void Start()
    {
        UpdateElementIcons(); //게임 시작시 UpdateElementIcons호출
    }

    public void UpdateElementIcons()
    {
        //플레이어 건틀릿 속성 아이콘 갱신
        if (gauntletManager != null && playerElementIcon != null)
        {
            playerElementIcon.sprite =
                GetElementIcon(gauntletManager.EquippedGauntlet);
        }

        // 드래곤 속성 아이콘 갱신
        if (dragonHealth != null && dragonElementIcon != null)
        {
            dragonElementIcon.sprite =
                GetElementIcon(dragonHealth.DragonElement);
        }
    }

    private Sprite GetElementIcon(ElementType element)
    {
        // 전달받은 속성에 맞는 아이콘 반환
        switch (element)
        {
            case ElementType.Fire:
                return fireIcon;

            case ElementType.Lightning:
                return lightningIcon;

            case ElementType.Wind:
                return windIcon;

            default:
                return null;
        }
    }
}