using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamagePopupSpawner : MonoBehaviour
{
    private const string CriticalIconResourcePath =
        "UI/거친 치명타 충격 번쩍임";

    private static DamagePopupSpawner instance;

    private RectTransform popupLayer;
    private Canvas rootCanvas;
    private Sprite criticalIcon;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        EnsureInstance();
    }

    public static void ShowDamage(
        int damage,
        Vector3 worldPosition,
        bool showCriticalIcon)
    {
        DamagePopupSpawner spawner = EnsureInstance();

        if (spawner == null)
            return;

        spawner.Spawn(damage, worldPosition, showCriticalIcon);
    }

    private static DamagePopupSpawner EnsureInstance()
    {
        if (instance != null)
            return instance;

        Canvas[] canvases = FindObjectsByType<Canvas>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Canvas canvas in canvases)
        {
            if (!canvas.isRootCanvas ||
                canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                continue;
            }

            GameObject layerObject = new GameObject(
                "DamagePopupLayer",
                typeof(RectTransform),
                typeof(DamagePopupSpawner)
            );

            RectTransform layerRect =
                layerObject.GetComponent<RectTransform>();

            layerRect.SetParent(canvas.transform, false);
            layerRect.anchorMin = Vector2.zero;
            layerRect.anchorMax = Vector2.one;
            layerRect.offsetMin = Vector2.zero;
            layerRect.offsetMax = Vector2.zero;
            layerRect.SetAsLastSibling();

            instance = layerObject.GetComponent<DamagePopupSpawner>();
            instance.Initialize(canvas, layerRect);
            return instance;
        }

        return null;
    }

    private void Initialize(Canvas canvas, RectTransform layerRect)
    {
        rootCanvas = canvas;
        popupLayer = layerRect;
        criticalIcon = Resources.Load<Sprite>(CriticalIconResourcePath);
    }

    private void Spawn(
        int damage,
        Vector3 worldPosition,
        bool showCriticalIcon)
    {
        if (popupLayer == null || Camera.main == null)
            return;

        GameObject popupObject = new GameObject(
            "DamagePopup",
            typeof(RectTransform),
            typeof(CanvasGroup),
            typeof(DamagePopupUI)
        );

        RectTransform popupRect =
            popupObject.GetComponent<RectTransform>();

        popupRect.SetParent(popupLayer, false);
        popupRect.sizeDelta = new Vector2(220f, 90f);

        CanvasGroup canvasGroup =
            popupObject.GetComponent<CanvasGroup>();

        TMP_Text damageText = CreateDamageText(
            popupRect,
            damage,
            showCriticalIcon
        );

        Image icon = null;

        if (showCriticalIcon && criticalIcon != null)
            icon = CreateCriticalIcon(popupRect);

        DamagePopupUI popupUI =
            popupObject.GetComponent<DamagePopupUI>();

        popupUI.Initialize(
            popupRect,
            canvasGroup,
            damageText,
            icon,
            popupLayer,
            rootCanvas,
            Camera.main,
            worldPosition
        );
    }

    private TMP_Text CreateDamageText(
        RectTransform parent,
        int damage,
        bool isHighDamageRoll)
    {
        GameObject textObject = new GameObject(
            "DamageText",
            typeof(RectTransform),
            typeof(TextMeshProUGUI)
        );

        RectTransform textRect =
            textObject.GetComponent<RectTransform>();

        textRect.SetParent(parent, false);
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(150f, 80f);
        textRect.anchoredPosition =
            new Vector2(isHighDamageRoll ? 30f : 0f, 0f);

        TextMeshProUGUI text =
            textObject.GetComponent<TextMeshProUGUI>();

        text.text = damage.ToString();
        text.fontSize = 54f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;
        text.textWrappingMode = TextWrappingModes.NoWrap;

        return text;
    }

    private Image CreateCriticalIcon(RectTransform parent)
    {
        GameObject iconObject = new GameObject(
            "HighDamageIcon",
            typeof(RectTransform),
            typeof(Image)
        );

        RectTransform iconRect =
            iconObject.GetComponent<RectTransform>();

        iconRect.SetParent(parent, false);
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.sizeDelta = new Vector2(62f, 62f);
        iconRect.anchoredPosition = new Vector2(-68f, 0f);

        Image icon = iconObject.GetComponent<Image>();
        icon.sprite = criticalIcon;
        icon.preserveAspect = true;
        icon.raycastTarget = false;

        return icon;
    }
}
