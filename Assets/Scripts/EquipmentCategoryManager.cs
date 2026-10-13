using UnityEngine;

public class EquipmentCategoryManager : MonoBehaviour
{
    [Header("Contents")]
    [SerializeField] private GameObject gauntletContent;
    [SerializeField] private GameObject robeContent;
    [SerializeField] private GameObject itemContent;
    [SerializeField] private GameObject petContent;

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

    public void ShowItem()
    {
        SetContent(itemContent);
    }

    public void ShowPet()
    {
        SetContent(petContent);
    }

    private void SetContent(GameObject activeContent)
    {
        gauntletContent.SetActive(activeContent == gauntletContent);
        robeContent.SetActive(activeContent == robeContent);
        itemContent.SetActive(activeContent == itemContent);
        petContent.SetActive(activeContent == petContent);
    }
}