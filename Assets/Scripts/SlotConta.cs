using UnityEngine;
using UnityEngine.EventSystems;

public class SlotConta : MonoBehaviour, IDropHandler
{
    [Header("Configuração da Linha (0, 1 ou 2)")]
    [SerializeField] private int indexLinha; // Defina 0 para a primeira linha, 1 para a segunda, etc.

    private DraggableResultado resultadoAtual;
    private GerenciadorContas gerenciadorContas;

    private void Awake()
    {
        gerenciadorContas = Object.FindAnyObjectByType<GerenciadorContas>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject objetoArrastado = eventData.pointerDrag;
        
        if (objetoArrastado != null)
        {
            DraggableResultado novoResultado = objetoArrastado.GetComponent<DraggableResultado>();

            if (novoResultado != null)
            {
                // CASO DE TROCA: Se este slot JÁ TEM um resultado dentro dele
                if (transform.childCount > 0)
                {
                    DraggableResultado resultadoAntigo = GetComponentInChildren<DraggableResultado>();

                    if (resultadoAntigo != null)
                    {
                        Transform paiAntigoDoNovo = novoResultado.ObterParentBeforeDrag();

                        // Manda o resultado antigo para o lugar antigo do novo resultado
                        resultadoAntigo.transform.SetParent(paiAntigoDoNovo);
                        resultadoAntigo.rectTransform.anchoredPosition = Vector2.zero;
                        
                        if (paiAntigoDoNovo.TryGetComponent<SlotConta>(out var slotAntigo))
                        {
                            resultadoAntigo.SetSlotAtual(slotAntigo);
                            slotAntigo.ConfigurarResultadoAtual(resultadoAntigo);
                            
                            // Avisa o gerenciador que o item antigo agora está no slot antigo
                            if (gerenciadorContas != null)
                            {
                                gerenciadorContas.ValidarDropMatematicoInversao(slotAntigo.indexLinha, resultadoAntigo);
                            }
                        }
                        else
                        {
                            // Se voltou para fora (limbo), remove a validação antiga dele
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

                // O novoResultado vai rodar a própria validação visual e do gerenciador no OnEndDrag dele!
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