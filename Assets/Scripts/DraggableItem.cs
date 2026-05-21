using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    private Canvas canvas;
    public RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private SlotItem slotVinculado;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void SetSlotAtual(SlotItem slot)
    {
        slotVinculado = slot;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        slotVinculado = GetComponentInParent<SlotItem>();

        if (slotVinculado != null)
        {
            slotVinculado.TrocarImagem(false);
            Debug.Log("Item saindo do slot: " + slotVinculado.name);
        }

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        parentAfterDrag = transform.parent;
        transform.SetParent(canvas.transform);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
    public void OnPointerDown(PointerEventData eventData) { }
}