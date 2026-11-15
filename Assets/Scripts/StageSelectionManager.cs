using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class StageSelectionManager
{
    private const string Stage1ClearKey = "Stage1Cleared";
    private const string LockedStageSpritePath = "UI/LockedStage";
    private const int DefaultStage = 2;
    private const string MainMapSceneName = "Mainmap";
    private const string BattleSceneName = "BattleScene";
    private const string DragonResourceFolder = "Dragons/Stage";

    public static int SelectedStage { get; private set; } = DefaultStage;
    public static int LastGoldReward { get; private set; }
    public static int LastUpgradeStoneReward { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState()
    {
        SelectedStage = DefaultStage;
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;

        HandleSceneLoaded(
            SceneManager.GetActiveScene(),
            LoadSceneMode.Single
        );
    }

    private static void HandleSceneLoaded(
        Scene scene,
        LoadSceneMode loadMode)
    {
        if (scene.name == MainMapSceneName)
        {
            BindStageButtons();
            BindQuitButton();
            return;
        }

        if (scene.name == BattleSceneName)
            ApplySelectedStageDragon();
    }

    private static void BindStageButtons()
    {
        bool stage2Unlocked =
            PlayerPrefs.GetInt(Stage1ClearKey, 0) == 1;

        bool stage3Unlocked =
            PlayerPrefs.GetInt("Stage2Cleared", 0) == 1;

        Sprite lockedStageSprite =
            Resources.Load<Sprite>(LockedStageSpritePath);

        Button[] buttons = Object.FindObjectsByType<Button>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Button button in buttons)
        {
            if (!TryGetStageNumber(
                    button.gameObject.name,
                    out int stageNumber))
            {
                continue;
            }

            int selectedStage = stageNumber;

            bool isLocked =
                (selectedStage == 2 && !stage2Unlocked) ||
                (selectedStage == 3 && !stage3Unlocked);

            if (isLocked)
            {
                button.interactable = false;

                UnityEngine.UI.Image buttonImage =
                    button.GetComponent<UnityEngine.UI.Image>();

                if (buttonImage != null && lockedStageSprite != null)
                    buttonImage.sprite = lockedStageSprite;

                continue;
            }

            button.interactable = true;
            button.onClick.AddListener(
                () => EnterStage(selectedStage)
            );
        }
    }

    private static void BindQuitButton()
    {
        Button[] buttons = Object.FindObjectsByType<Button>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Button button in buttons)
        {
            string buttonName = button.gameObject.name
                .Replace(" ", string.Empty)
                .ToLowerInvariant();

            TMPro.TMP_Text tmpLabel =
                button.GetComponentInChildren<TMPro.TMP_Text>(true);

            UnityEngine.UI.Text legacyLabel =
                button.GetComponentInChildren<UnityEngine.UI.Text>(true);

            string labelText = tmpLabel != null
                ? tmpLabel.text
                : legacyLabel != null
                    ? legacyLabel.text
                    : string.Empty;

            string normalizedLabel = labelText
                .Replace(" ", string.Empty)
                .ToLowerInvariant();

            if (buttonName != "quitgame" &&
                normalizedLabel != "quitgame")
                continue;

            button.onClick.AddListener(QuitGame);
            break;
        }
    }

    private static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static void CompleteCurrentStage()
    {
        GetStageRewards(
            SelectedStage,
            out int goldReward,
            out int stoneReward
        );

        string clearKey = "Stage" + SelectedStage + "Cleared";
        bool isFirstClear = PlayerPrefs.GetInt(clearKey, 0) == 0;

        if (!isFirstClear)
        {
            goldReward /= 2;
            stoneReward /= 2;
        }

        LastGoldReward = goldReward;
        LastUpgradeStoneReward = stoneReward;

        if (PlayerInventoryManager.Instance != null)
        {
            PlayerInventoryManager.Instance.AddGold(goldReward);
            PlayerInventoryManager.Instance.AddUpgradeStone(stoneReward);
        }

        PlayerPrefs.SetInt(clearKey, 1);
        PlayerPrefs.Save();
    }

    private static void GetStageRewards(
        int stageNumber,
        out int goldReward,
        out int stoneReward)
    {
        switch (stageNumber)
        {
            case 1:
                goldReward = 500;
                stoneReward = 15;
                break;
            case 3:
                goldReward = 2000;
                stoneReward = 30;
                break;
            default:
                goldReward = 1200;
                stoneReward = 20;
                break;
        }
    }

    private static bool TryGetStageNumber(
        string objectName,
        out int stageNumber)
    {
        stageNumber = 0;

        if (string.IsNullOrEmpty(objectName) ||
            !objectName.StartsWith("Stage"))
        {
            return false;
        }

        return int.TryParse(
            objectName.Substring("Stage".Length),
            out stageNumber
        ) && stageNumber > 0;
    }

    public static void EnterStage(int stageNumber)
    {
        SelectedStage = Mathf.Max(1, stageNumber);
        SceneManager.LoadScene(BattleSceneName);
    }

    private static void ApplySelectedStageDragon()
    {
        DragonHealth dragonHealth =
            Object.FindFirstObjectByType<DragonHealth>();

        if (dragonHealth == null)
            return;

        dragonHealth.SetDragonElement(
            GetStageElement(SelectedStage)
        );

        GetStageBattleStats(
            SelectedStage,
            out float maxHealth,
            out float attackDamage
        );

        dragonHealth.SetMaxHealth(maxHealth);

        GameObject stageDragonPrefab =
            Resources.Load<GameObject>(
                DragonResourceFolder + SelectedStage
            );

        // Stage 2와 아직 프리팹이 없는 스테이지는
        // BattleScene의 기본 Red Dragon을 그대로 사용한다.
        if (stageDragonPrefab == null)
        {
            ApplyAttackDamage(dragonHealth.gameObject, attackDamage);
            return;
        }

        ApplyDragonVisual(
            dragonHealth.gameObject,
            stageDragonPrefab
        );

        ApplyAttackDamage(dragonHealth.gameObject, attackDamage);
    }

    private static void GetStageBattleStats(
        int stageNumber,
        out float maxHealth,
        out float attackDamage)
    {
        switch (stageNumber)
        {
            case 1:
                maxHealth = 225f;
                attackDamage = 20f;
                break;
            case 3:
                maxHealth = 1000f;
                attackDamage = 50f;
                break;
            default:
                maxHealth = 400f;
                attackDamage = 30f;
                break;
        }
    }

    private static void ApplyAttackDamage(
        GameObject dragonRoot,
        float attackDamage)
    {
        DragonAttackHitbox[] hitboxes =
            dragonRoot.GetComponentsInChildren<DragonAttackHitbox>(true);

        foreach (DragonAttackHitbox hitbox in hitboxes)
            hitbox.SetDamage(attackDamage);
    }

    private static ElementType GetStageElement(int stageNumber)
    {
        switch (stageNumber)
        {
            case 1:
                return ElementType.Wind;

            case 3:
                return ElementType.Lightning;

            default:
                return ElementType.Fire;
        }
    }

    private static void ApplyDragonVisual(
        GameObject dragonRoot,
        GameObject stageDragonPrefab)
    {
        Animator currentAnimator =
            dragonRoot.GetComponentInChildren<Animator>(true);

        Animator prefabAnimator =
            stageDragonPrefab.GetComponentInChildren<Animator>(true);

        // 같은 리그를 공유하는 색상 변형은 모델을 새로 만들지 않고
        // 머티리얼만 교체해 기존 공격 판정과 Animation Event를 유지한다.
        if (currentAnimator != null &&
            prefabAnimator != null &&
            currentAnimator.avatar == prefabAnimator.avatar)
        {
            ApplyCompatibleRendererMaterials(
                dragonRoot,
                stageDragonPrefab
            );
            dragonRoot.name = stageDragonPrefab.name;
            return;
        }

        // 리그가 다른 드래곤은 기존 전투 루트는 유지하고
        // 외형과 Animator만 프리팹 것으로 교체한다.
        SkinnedMeshRenderer[] currentRenderers =
            dragonRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        foreach (SkinnedMeshRenderer renderer in currentRenderers)
            renderer.enabled = false;

        if (currentAnimator != null)
            currentAnimator.enabled = false;

        GameObject newVisual = Object.Instantiate(
            stageDragonPrefab,
            dragonRoot.transform
        );

        newVisual.name = stageDragonPrefab.name + " Visual";
        newVisual.transform.localPosition = Vector3.zero;
        newVisual.transform.localRotation = Quaternion.identity;
        newVisual.transform.localScale = Vector3.one;

        Animator newAnimator =
            newVisual.GetComponentInChildren<Animator>(true);

        DragonController dragonController =
            dragonRoot.GetComponent<DragonController>();

        NightmareDragonCombatSetup nightmareSetup =
            newVisual.GetComponent<NightmareDragonCombatSetup>();

        if (dragonController != null && newAnimator != null)
        {
            if (nightmareSetup != null)
            {
                dragonController.ConfigureAttackProfile(
                    newAnimator,
                    "HornAttack",
                    "ClawAttack",
                    nightmareSetup.HornHitbox,
                    nightmareSetup.ClawHitbox
                );
            }
            else
            {
                dragonController.SetAnimator(newAnimator);
            }

            DragonAnimationEventRelay relay =
                newAnimator.gameObject.AddComponent<DragonAnimationEventRelay>();

            relay.Initialize(dragonController);
        }

        dragonRoot.name = stageDragonPrefab.name;
    }

    private static void ApplyCompatibleRendererMaterials(
        GameObject dragonRoot,
        GameObject stageDragonPrefab)
    {
        SkinnedMeshRenderer[] sourceRenderers =
            stageDragonPrefab.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        Dictionary<string, Material[]> materialsByRendererName =
            new Dictionary<string, Material[]>();

        foreach (SkinnedMeshRenderer sourceRenderer in sourceRenderers)
        {
            materialsByRendererName[sourceRenderer.gameObject.name] =
                sourceRenderer.sharedMaterials;
        }

        SkinnedMeshRenderer[] targetRenderers =
            dragonRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        foreach (SkinnedMeshRenderer targetRenderer in targetRenderers)
        {
            if (materialsByRendererName.TryGetValue(
                    targetRenderer.gameObject.name,
                    out Material[] materials))
            {
                targetRenderer.sharedMaterials = materials;
            }
        }
    }
}

public class DragonAnimationEventRelay : MonoBehaviour
{
    private DragonController dragonController;

    public void Initialize(DragonController controller)
    {
        dragonController = controller;
    }

    public void StartFlame()
    {
        if (dragonController != null)
            dragonController.StartFlame();
    }

    public void StopFlame()
    {
        if (dragonController != null)
            dragonController.StopFlame();
    }

    public void StartChargeHitbox()
    {
        if (dragonController != null)
            dragonController.StartChargeHitbox();
    }

    public void StopChargeHitbox()
    {
        if (dragonController != null)
            dragonController.StopChargeHitbox();
    }

    public void StartHornHitbox()
    {
        if (dragonController != null)
            dragonController.StartChargeHitbox();
    }

    public void StopHornHitbox()
    {
        if (dragonController != null)
            dragonController.StopChargeHitbox();
    }

    public void StartClawHitbox()
    {
        if (dragonController != null)
            dragonController.StartSecondaryMeleeHitbox();
    }

    public void StopClawHitbox()
    {
        if (dragonController != null)
            dragonController.StopSecondaryMeleeHitbox();
    }

    public void EndAttack()
    {
        if (dragonController != null)
            dragonController.EndAttack();
    }
}
