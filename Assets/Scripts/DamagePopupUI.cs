using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamagePopupUI : MonoBehaviour
{
    private const float Duration = 0.9f;
    private const float FloatDistance = 95f;

    private RectTransform popupRect;
    private CanvasGroup canvasGroup;
    private RectTransform popupLayer;
    private Canvas rootCanvas;
    private Camera worldCamera;
    private Vector3 worldPosition;
    private Vector2 randomOffset;

    public void Initialize(
        RectTransform rect,
        CanvasGroup group,
        TMP_Text damageText,
        Image criticalIcon,
        RectTransform layer,
        Canvas canvas,
        Camera camera,
        Vector3 hitWorldPosition)
    {
        popupRect = rect;
        canvasGroup = group;
        popupLayer = layer;
        rootCanvas = canvas;
        worldCamera = camera;
        worldPosition = hitWorldPosition;
        randomOffset = new Vector2(
            Random.Range(-42f, 42f),
            Random.Range(-18f, 26f)
        );

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        float elapsed = 0f;

        while (elapsed < Duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / Duration);

            if (!UpdateScreenPosition(normalizedTime))
            {
                Destroy(gameObject);
                yield break;
            }

            float scale = Mathf.Lerp(
                1.35f,
                1f,
                Mathf.Clamp01(normalizedTime * 4f)
            );

            popupRect.localScale = Vector3.one * scale;
            canvasGroup.alpha = normalizedTime < 0.55f
                ? 1f
                : 1f - Mathf.InverseLerp(0.55f, 1f, normalizedTime);

            yield return null;
        }

        Destroy(gameObject);
    }

    private bool UpdateScreenPosition(float normalizedTime)
    {
        if (worldCamera == null || popupLayer == null)
            return false;

        Vector3 screenPosition =
            worldCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z <= 0f)
            return false;

        Camera canvasCamera = rootCanvas.renderMode ==
            RenderMode.ScreenSpaceOverlay
            ? null
            : rootCanvas.worldCamera;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                popupLayer,
                screenPosition,
                canvasCamera,
                out Vector2 canvasPosition))
        {
            return false;
        }

        float easedFloat = 1f -
            Mathf.Pow(1f - normalizedTime, 2f);

        popupRect.anchoredPosition =
            canvasPosition +
            randomOffset +
            Vector2.up * FloatDistance * easedFloat;

        popupRect.localRotation = Quaternion.identity;
        return true;
    }
}
