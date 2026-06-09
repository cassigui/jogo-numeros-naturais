using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GerenciadorContas : MonoBehaviour
{
    [Header("Referências da Fase")]
    [SerializeField] private Fase03 gerenciadorFase;

    [Header("Textos das Contas (Esquerda)")]
    [SerializeField] private TextMeshProUGUI[] textosContas; // Tamanho 3

    [Header("Textos dos Resultados (Direita)")]
    [SerializeField] private TextMeshProUGUI[] textosResultados; // Tamanho 3

    // Estruturas internas
    private List<int> respostasCorretas = new List<int>();
    private int botaoContaSelecionado = -1;
    private int botaoResultadoSelecionado = -1;

    // Guarda quais contas (índice 0, 1 ou 2) receberam o resultado correto via Drop
    private Dictionary<int, int> combinacoesAtuais = new Dictionary<int, int>();

    void Start()
    {
        if (gerenciadorFase == null)
            gerenciadorFase = Object.FindAnyObjectByType<Fase03>();

        GerarNovasContasAleatorias();
    }

    public void GerarNovasContasAleatorias()
    {
        respostasCorretas.Clear();
        combinacoesAtuais.Clear();
        ResetarSelecoes();

        List<string> contasUsadas = new List<string>();

        // 1. Gerar 3 contas e armazenar os seus resultados corretos
        for (int i = 0; i < 3; i++)
        {
            bool contaValida = false;
            int num1 = 0; int num2 = 0; int resultado = 0;
            string textoDaConta = "";
            bool ehSoma = false;

            int tentativasSeguranca = 0;
            while (!contaValida && tentativasSeguranca < 100)
            {
                tentativasSeguranca++;
                ehSoma = Random.Range(0, 2) == 0;

                if (ehSoma)
                {
                    num1 = Random.Range(0, 11);
                    num2 = Random.Range(0, 11 - num1);
                    resultado = num1 + num2;
                    textoDaConta = $"{num1} + {num2}";
                }
                else
                {
                    num1 = Random.Range(0, 11);
                    num2 = Random.Range(0, num1 + 1);
                    resultado = num1 - num2;
                    textoDaConta = $"{num1} - {num2}";
                }

                if (!respostasCorretas.Contains(resultado) && !contasUsadas.Contains(textoDaConta))
                {
                    contaValida = true;
                }
            }

            respostasCorretas.Add(resultado);
            contasUsadas.Add(textoDaConta);
            textosContas[i].text = textoDaConta;
        }

        // 2. Criar uma lista para misturar os resultados no ecrã
        List<int> respostasEmbaralhadas = new List<int>(respostasCorretas);

        // Força o re-embaralhamento até que NENHUM item coincida com o índice original
        bool temItemAlinhado = true;
        int travaLoop = 0;

        while (temItemAlinhado && travaLoop < 50)
        {
            travaLoop++;
            EmbaralharLista(respostasEmbaralhadas);

            temItemAlinhado = false;
            for (int i = 0; i < respostasCorretas.Count; i++)
            {
                if (respostasEmbaralhadas[i] == respostasCorretas[i])
                {
                    temItemAlinhado = true;
                    break;
                }
            }
        }

        // 3. Aplicar nos botões da direita (agora 100% desalinhados)
        for (int i = 0; i < textosResultados.Length; i++)
        {
            textosResultados[i].text = respostasEmbaralhadas[i].ToString();
        }

        // 4. Inicializa os componentes DraggableResultado com os novos valores numéricos gerados
        foreach (var textoRes in textosResultados)
        {
            if (textoRes != null)
            {
                DraggableResultado dragComp = textoRes.GetComponentInParent<DraggableResultado>();
                if (dragComp != null) dragComp.InicializarValor();
            }
        }
    }

    public void ValidarDropMatematico(int indexConta, int valorDoResultado, DraggableResultado draggable)
    {
        if (indexConta >= 0 && indexConta < respostasCorretas.Count)
        {
            if (respostasCorretas[indexConta] == valorDoResultado)
            {
                Debug.Log($"Acertou! Conta {indexConta} recebeu o resultado correto: {valorDoResultado}");

                if (!combinacoesAtuais.ContainsKey(indexConta))
                {
                    combinacoesAtuais.Add(indexConta, valorDoResultado);
                }
                draggable.MarcarComoCorreto(true); // Fica verde!
            }
            else
            {
                Debug.Log("Resposta errada para esta conta!");
                draggable.MarcarComoCorreto(false); // Mantém ou volta ao original
                if (combinacoesAtuais.ContainsKey(indexConta)) combinacoesAtuais.Remove(indexConta);

                if (gerenciadorFase != null) gerenciadorFase.SenhaIncorretaErrou();
            }
        }
    }

    public void ValidarDropMatematicoInversao(int indexConta, DraggableResultado draggable)
    {
        if (indexConta >= 0 && indexConta < respostasCorretas.Count)
        {
            if (respostasCorretas[indexConta] == draggable.ValorResultado)
            {
                if (!combinacoesAtuais.ContainsKey(indexConta))
                {
                    combinacoesAtuais.Add(indexConta, draggable.ValorResultado);
                }
                draggable.MarcarComoCorreto(true); // Fica verde!
            }
            else
            {
                // Se na inversão automática o item foi parar em um lugar incorreto, desliga o verde dele
                if (combinacoesAtuais.ContainsKey(indexConta)) combinacoesAtuais.Remove(indexConta);
                draggable.MarcarComoCorreto(false);
            }
        }
    }

    // Caso o jogador puxe o item para fora do slot de volta para o limbo, limpa o acerto antigo
    public void RemoverValidacaoDaConta(int indexConta)
    {
        if (combinacoesAtuais.ContainsKey(indexConta))
        {
            combinacoesAtuais.Remove(indexConta);
            Debug.Log($"Resultado removido da conta {indexConta}");
        }
    }

    public void ValidarBotaoLigarLuz()
    {
        if (combinacoesAtuais.Count == 3)
        {
            if (gerenciadorFase != null) gerenciadorFase.SenhaCorretaAcionarVitoria();
        }
        else
        {
            if (gerenciadorFase != null) gerenciadorFase.SenhaIncorretaErrou();
        }
    }

    private void ResetarSelecoes()
    {
        botaoContaSelecionado = -1;
        botaoResultadoSelecionado = -1;
    }

    private void EmbaralharLista(List<int> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            int tmp = lista[i];
            lista[i] = lista[r];
            lista[r] = tmp;
        }
    }
}