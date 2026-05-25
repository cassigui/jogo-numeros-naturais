using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotItem : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image slotImage;
    [SerializeField] private Sprite spriteVazio;
    [SerializeField] private Sprite spriteOcupado;
    public bool isOcupped { get; private set; }

    private void Awake()
    {
        if (slotImage == null) slotImage = GetComponent<Image>();
        slotImage.sprite = spriteVazio;
        TrocarImagem(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isOcupped) 
        {
            Debug.Log($"Slot {name} já está ocupado! Bloqueando nova pilha.");
            return; 
        }

        if (eventData.pointerDrag != null)
        {
            DraggableItem item = eventData.pointerDrag.GetComponent<DraggableItem>();

            if (item != null)
            {
                // 🔔 AQUI ESTÁ O SEGREDO: Avisa a pilha que o slot aceitou ela!
                item.foiAceitaNoSlot = true;

                item.parentAfterDrag = transform;
                item.SetSlotAtual(this);
                item.rectTransform.position = GetComponent<RectTransform>().position;
                TrocarImagem(true);
            }
        }
    }

    public void TrocarImagem(bool ocupado)
    {   
        isOcupped = ocupado;
        if (slotImage != null)
            slotImage.sprite = ocupado ? spriteOcupado : spriteVazio;
    }
}