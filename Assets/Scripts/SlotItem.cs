using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotItem : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image slotImage;
    [SerializeField] private Sprite spriteVazio;
    [SerializeField] private Sprite spriteOcupado;

    private void Awake()
    {
        if (slotImage == null) slotImage = GetComponent<Image>();
        slotImage.sprite = spriteVazio;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DraggableItem item = eventData.pointerDrag.GetComponent<DraggableItem>();

            if (item != null)
            {

                item.parentAfterDrag = transform;
                item.SetSlotAtual(this);
                item.rectTransform.position = GetComponent<RectTransform>().position;
                TrocarImagem(true);
            }
        }
    }

    public void TrocarImagem(bool ocupado)
    {
        if (slotImage != null)
            slotImage.sprite = ocupado ? spriteOcupado : spriteVazio;
    }
}