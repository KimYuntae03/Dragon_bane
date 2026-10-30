using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    //UI의 각 버튼을 누르면 정해진 씬으로 이동하도록 함.
    public void LoadEquipmentScene()
    {
        if (UpgradePopupManager.IsUpgradeInProgress)
            return;

        SceneManager.LoadScene("EquipmentScene");
    }

    public void LoadStoreScene()
    {
        if (UpgradePopupManager.IsUpgradeInProgress)
            return;

        SceneManager.LoadScene("StoreScene");
    }

    public void LoadBattleScene()
    {
        if (UpgradePopupManager.IsUpgradeInProgress)
            return;

        SceneManager.LoadScene("BattleScene");
    }
    public void ReturnToMainMap()
    {
        if (UpgradePopupManager.IsUpgradeInProgress)
            return;

        SceneManager.LoadScene("Mainmap");
    }
}
