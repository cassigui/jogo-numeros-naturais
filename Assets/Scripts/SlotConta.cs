using UnityEngine;
using UnityEngine.EventSystems;

public class SlotConta : MonoBehaviour, IDropHandler
{
    [Header("Configuração da Linha (0, 1 ou 2)")]
    [SerializeField] private int indexLinha; 

    private DraggableResultado resultadoAtual;
    private GerenciadorContas gerenciadorContas;

    private void Awake()
    {
        gerenciadorContas = Object.FindAnyObjectByType<GerenciadorContas>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"O mouse soltou algo em cima do slot: {gameObject.name}");

        GameObject objetoArrastado = eventData.pointerDrag;
        
        if (objetoArrastado != null)
        {
            DraggableResultado novoResultado = objetoArrastado.GetComponent<DraggableResultado>();

            if (novoResultado != null)
            {
                // CASO DE TROCA: Se este slot JÁ TEM um número dentro dele
                if (transform.childCount > 0)
                {
                    DraggableResultado resultadoAntigo = GetComponentInChildren<DraggableResultado>();

                    if (resultadoAntigo != null)
                    {
                        Transform paiAntigoDoNovo = novoResultado.ObterParentBeforeDrag();

                        // Move o número antigo para o slot de onde o novo veio
                        resultadoAntigo.transform.SetParent(paiAntigoDoNovo);
                        resultadoAntigo.rectTransform.anchoredPosition = Vector2.zero;
                        
                        if (paiAntigoDoNovo.TryGetComponent<SlotConta>(out var slotAntigo))
                        {
                            resultadoAntigo.SetSlotAtual(slotAntigo);
                            slotAntigo.ConfigurarResultadoAtual(resultadoAntigo);
                            
                            if (gerenciadorContas != null)
                            {
                                gerenciadorContas.ValidarDropMatematicoInversao(slotAntigo.ObterIndexLinha(), resultadoAntigo);
                            }
                        }
                        else
                        {
                            resultadoAntigo.SetSlotAtual(null);
                            if (gerenciadorContas != null) gerenciadorContas.RemoverValidacaoDaConta(indexLinha);
                            resultadoAntigo.MudarSprite(resultadoAntigo.SpriteOriginal);
                        }
                    }
                }

                // Configura o NOVO resultado arrastado neste slot
                novoResultado.parentAfterDrag = transform;
                novoResultado.foiAceitaNoSlot = true;
                novoResultado.SetSlotAtual(this);
                ConfigurarResultadoAtual(novoResultado);
            }
            else
            {
                Debug.LogWarning("❌ O objeto solto NÃO possui o componente DraggableResultado!");
            }
        }
    }

    public int ObterIndexLinha() => indexLinha;

    public void ConfigurarResultadoAtual(DraggableResultado resultado)
    {
        resultadoAtual = resultado;
    }

    public void RemoverResultado()
    {
        resultadoAtual = null;
        if (gerenciadorContas != null) gerenciadorContas.RemoverValidacaoDaConta(indexLinha);
    }
}