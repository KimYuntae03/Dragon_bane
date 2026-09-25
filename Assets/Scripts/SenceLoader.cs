using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    //UI의 각 버튼을 누르면 정해진 씬으로 이동하도록 함.
    public void LoadEquipmentScene()
    {
        SceneManager.LoadScene("EquipmentScene");
    }

    public void LoadStoreScene()
    {
        SceneManager.LoadScene("StoreScene");
    }

    public void LoadBattleScene()
    {
        SceneManager.LoadScene("BattleScene");
    }
}