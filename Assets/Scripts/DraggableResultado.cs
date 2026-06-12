using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DraggableResultado : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;

    private Transform parentBeforeDrag;
    private Vector2 positionBeforeDrag;
    private Canvas canvas;
    [HideInInspector] public RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private SlotConta slotVinculado;

    [HideInInspector] public bool foiAceitaNoSlot;

    public int ValorResultado { get; private set; }

    [Header("Configuração de Imagens")]
    [SerializeField] private Sprite spriteOriginal;
    public Sprite SpriteOriginal => spriteOriginal;
    [SerializeField] private Sprite spriteCorreto;
    private Image imagemComponente;
    private GerenciadorContas gerenciadorContas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        imagemComponente = GetComponent<Image>();
        gerenciadorContas = Object.FindAnyObjectByType<GerenciadorContas>();

        if (spriteOriginal == null && imagemComponente != null)
        {
            spriteOriginal = imagemComponente.sprite;
        }
    }

    public void InicializarValor()
    {
        TextMeshProUGUI texto = GetComponentInChildren<TextMeshProUGUI>();
        if (texto != null && int.TryParse(texto.text, out int valor))
        {
            ValorResultado = valor;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void SetSlotAtual(SlotConta slot)
    {
        slotVinculado = slot;
    }

    public Transform ObterParentBeforeDrag()
    {
        return parentBeforeDrag;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        foiAceitaNoSlot = false;

        if (slotVinculado != null)
        {
            slotVinculado.RemoverResultado();
            slotVinculado = null;
        }

        MudarSprite(spriteOriginal);

        parentBeforeDrag = transform.parent; // Salva o slot de onde ele está saindo AGORA
        positionBeforeDrag = rectTransform.anchoredPosition;
        parentAfterDrag = parentBeforeDrag;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // Move para o canvas temporariamente para renderizar por cima de tudo
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (foiAceitaNoSlot)
        {
            transform.SetParent(parentAfterDrag);
            rectTransform.anchoredPosition = Vector2.zero;

            if (slotVinculado != null)
            {
                slotVinculado.ConfigurarResultadoAtual(this);

                if (gerenciadorContas != null)
                {
                    // Garante que usa ObterIndexLinha() para o SlotConta original
                    gerenciadorContas.ValidarDropMatematico(slotVinculado.ObterIndexLinha(), ValorResultado, this);
                }
            }
        }
        else
        {
            transform.SetParent(parentBeforeDrag);
            rectTransform.anchoredPosition = positionBeforeDrag;
            MudarSprite(spriteOriginal);
        }
    }

    public void MudarSprite(Sprite novoSprite)
    {
        if (imagemComponente != null && novoSprite != null)
        {
            imagemComponente.sprite = novoSprite;
        }
    }

    public void MarcarComoCorreto(bool correto)
    {
        MudarSprite(correto ? spriteCorreto : spriteOriginal);
    }

    public void OnPointerDown(PointerEventData eventData) { }
}