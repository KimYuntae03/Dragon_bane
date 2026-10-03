using UnityEngine;
using UnityEngine.UI;

public class StoreCategoryManager : MonoBehaviour
{
    [Header("Contents")]
    [SerializeField] private GameObject gauntletContent;
    [SerializeField] private GameObject robeContent;
    [SerializeField] private GameObject potionContent;

    [Header("Scroll View")]
    [SerializeField] private ScrollRect scrollRect;

    private void Start()
    {
        ShowGauntlet();
    }

    public void ShowGauntlet()
    {
        SetContent(gauntletContent);
    }

    public void ShowRobe()
    {
        SetContent(robeContent);
    }

    public void ShowPotion()
    {
        SetContent(potionContent);
    }

    private void SetContent(GameObject activeContent)
    {
        gauntletContent.SetActive(activeContent == gauntletContent);
        robeContent.SetActive(activeContent == robeContent);
        potionContent.SetActive(activeContent == potionContent);

        // ScrollRect가 현재 활성화된 Content를 사용하도록 변경
        if (scrollRect != null)
        {
            scrollRect.content =
                activeContent.GetComponent<RectTransform>();

            // 카테고리 변경 시 스크롤을 맨 왼쪽으로 초기화
            scrollRect.horizontalNormalizedPosition = 0f;
        }
    }
}