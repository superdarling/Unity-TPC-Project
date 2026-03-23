using UnityEngine;
using UnityEngine.EventSystems;

public class OnScreenStick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Ò¡¸Ë±³¾°")]
    public RectTransform stickBackground;
    [Header("Ò¡¸Ë°´Å¥")]
    public RectTransform stickHandle;

    private Vector2 input = Vector2.zero;  // Ò¡¸ËÊä³ö
    public float Horizontal { get { return input.x; } }
    public float Vertical { get { return input.y; } }

    private Canvas canvas;
    private float radius;

    void Start()
    {
        if (stickBackground == null)
            stickBackground = GetComponent<RectTransform>();

        if (stickHandle == null)
            Debug.LogError("StickHandle Î´ÉèÖÃ£¡");

        // ÕÒµ½×î½üµÄ Canvas
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("ÕÒ²»µ½¸¸ Canvas£¡");

        // Ò¡¸Ë°ë¾¶£¨RectTransform.width/2£©
        radius = stickBackground.sizeDelta.x * 0.5f;
    }

    // ×ª»»ÆÁÄ»×ø±êµ½Ò¡¸Ë±¾µØ×ø±ê
    private Vector2 ScreenPointToLocalVector2(Vector2 screenPos)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            stickBackground, screenPos, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, out localPoint
        );
        return localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint = ScreenPointToLocalVector2(eventData.position);
        Vector2 clamped = Vector2.ClampMagnitude(localPoint, radius);

        stickHandle.localPosition = clamped;
        input = clamped / radius;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        stickHandle.localPosition = Vector2.zero;
    }
}