using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    
    private Transform parentBeforeDrag; 
    private Vector2 positionBeforeDrag; 
    private Canvas canvas;
    public RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private SlotItem slotVinculado;

    // Nova variável para controlar se o drop foi bem-sucedido
    [HideInInspector] public bool foiAceitaNoSlot;

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
        // Reseta o estado ao começar a arrastar
        foiAceitaNoSlot = false;

        slotVinculado = GetComponentInParent<SlotItem>();
        if (slotVinculado != null)
        {
            slotVinculado.TrocarImagem(false);
            Debug.Log("Item saindo do slot: " + slotVinculado.name);
        }

        // Salva de onde a pilha veio
        parentBeforeDrag = transform.parent;
        positionBeforeDrag = rectTransform.anchoredPosition; 
        parentAfterDrag = parentBeforeDrag; 

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(canvas.transform);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Se o SlotItem aceitou a pilha, ele terá marcado 'foiAceitaNoSlot' como true
        if (foiAceitaNoSlot)
        {
            // O drop deu certo! Fixa no novo pai (o slot)
            transform.SetParent(parentAfterDrag);
        }
        else
        {
            // O slot estava ocupado ou o drop foi fora de um slot. Volta para o início!
            Debug.Log("Drop recusado ou inválido. Retornando ao início.");
            transform.SetParent(parentBeforeDrag);
            rectTransform.anchoredPosition = positionBeforeDrag;
        }
    }

    public void OnPointerDown(PointerEventData eventData) { }
}