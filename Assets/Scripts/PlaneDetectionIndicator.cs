using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Displays a coloured dot in the bottom-left corner of the screen.
///   RED   = AR is scanning, no plane detected yet.
///   GREEN = At least one AR plane has been detected.
///
/// Assign ARPlaneManager in the Inspector (drag the XR Origin or AR Session
/// Origin object that carries ARPlaneManager). The indicator Image is
/// auto-created at runtime on the existing GameCanvas — nothing else to wire.
/// </summary>
public class PlaneDetectionIndicator : MonoBehaviour {

    [Header("AR")]
    [SerializeField] private ARPlaneManager planeManager;

    [Header("Indicator (optional – auto-created if left empty)")]
    [SerializeField] private Image indicatorImage;

    [Header("Colors")]
    [SerializeField] private Color noPlaneColor       = new Color(1f, 0.15f, 0.15f, 1f);
    [SerializeField] private Color planeDetectedColor = new Color(0.1f, 1f,  0.30f, 1f);

    [Header("Auto-Create Settings")]
    [SerializeField] private float dotSize   = 36f;
    [SerializeField] private float dotMargin = 20f;

    // Glow ring – only used in auto-created mode
    private Image _glowImage;

    // -------------------------------------------------------------------------

    private void Awake() {
        if (indicatorImage == null)
            CreateIndicatorUI();

        ApplyColor(noPlaneColor);   // start red
    }

    private void OnEnable() {
        if (planeManager != null)
            planeManager.planesChanged += OnPlanesChanged;
    }

    private void OnDisable() {
        if (planeManager != null)
            planeManager.planesChanged -= OnPlanesChanged;
    }

    // -------------------------------------------------------------------------

    private void OnPlanesChanged(ARPlanesChangedEventArgs args) {
        bool hasPlanes = planeManager.trackables.count > 0;
        ApplyColor(hasPlanes ? planeDetectedColor : noPlaneColor);
    }

    // -------------------------------------------------------------------------
    // Public API

    /// <summary>Force green (confirmed plane / game placed).</summary>
    public void SetPlaneConfirmed() => ApplyColor(planeDetectedColor);

    /// <summary>Force red (no tracking).</summary>
    public void SetNoPlane() => ApplyColor(noPlaneColor);

    // -------------------------------------------------------------------------

    private void ApplyColor(Color color) {
        if (indicatorImage != null)
            indicatorImage.color = color;

        if (_glowImage != null) {
            Color glow = color;
            glow.a = 0.35f;
            _glowImage.color = glow;
        }
    }

    // -------------------------------------------------------------------------
    // Builds the dot + glow ring on the existing Canvas at runtime

    private void CreateIndicatorUI() {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) {
            Debug.LogWarning("[PlaneDetectionIndicator] No Canvas found – cannot create indicator UI.");
            return;
        }

        Sprite circle = CreateCircleSprite();

        // ── Container anchored to bottom-left ─────────────────────────────────
        GameObject container = new GameObject("PlaneDetectionLight");
        container.transform.SetParent(canvas.transform, false);

        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin         = Vector2.zero;
        containerRect.anchorMax         = Vector2.zero;
        containerRect.pivot             = Vector2.zero;
        containerRect.sizeDelta         = new Vector2(dotSize + dotMargin * 2f,
                                                      dotSize + dotMargin * 2f);
        containerRect.anchoredPosition  = new Vector2(dotMargin, dotMargin);

        // ── Glow ring (slightly larger, semi-transparent same color) ──────────
        GameObject glowGO = new GameObject("Glow");
        glowGO.transform.SetParent(container.transform, false);

        RectTransform glowRect         = glowGO.AddComponent<RectTransform>();
        glowRect.anchorMin             = Vector2.zero;
        glowRect.anchorMax             = Vector2.one;
        glowRect.sizeDelta             = new Vector2(8f, 8f);
        glowRect.anchoredPosition      = Vector2.zero;

        _glowImage                     = glowGO.AddComponent<Image>();
        _glowImage.sprite              = circle;
        _glowImage.color               = new Color(1f, 0.15f, 0.15f, 0.35f);
        _glowImage.raycastTarget       = false;

        // ── Main dot ──────────────────────────────────────────────────────────
        GameObject dotGO = new GameObject("Dot");
        dotGO.transform.SetParent(container.transform, false);

        RectTransform dotRect          = dotGO.AddComponent<RectTransform>();
        dotRect.anchorMin              = Vector2.zero;
        dotRect.anchorMax              = Vector2.one;
        dotRect.sizeDelta              = new Vector2(-8f, -8f);
        dotRect.anchoredPosition       = Vector2.zero;

        indicatorImage                 = dotGO.AddComponent<Image>();
        indicatorImage.sprite          = circle;
        indicatorImage.color           = noPlaneColor;
        indicatorImage.raycastTarget   = false;

        Debug.Log("[PlaneDetectionIndicator] Dot indicator created at bottom-left of Canvas.");
    }

    // -------------------------------------------------------------------------
    // Procedurally generates a white anti-aliased circle sprite (no asset needed)

    private static Sprite CreateCircleSprite() {
        const int res  = 64;
        Texture2D tex  = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        Vector2 center = new Vector2(res / 2f, res / 2f);
        float   radius = res / 2f - 1f;

        for (int y = 0; y < res; y++) {
            for (int x = 0; x < res; x++) {
                float dist  = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01(radius - dist + 0.5f);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), res);
    }
}
