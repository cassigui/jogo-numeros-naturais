using UnityEngine;
using TMPro;

public class ElementoRelatorioFase : MonoBehaviour
{
    [Header("Configuração da Fase")]
    [Tooltip("O nome exato da cena que este bloco representa (ex: Fase01)")]
    [SerializeField] private string nomeDaCenaFase;

    [Header("Referências de Texto (TMPro)")]
    [SerializeField] private TextMeshProUGUI txtVitorias;
    [SerializeField] private TextMeshProUGUI txtErros;
    [SerializeField] private TextMeshProUGUI txtPartidas;
    [SerializeField] private TextMeshProUGUI txtVidasFinais;

    public string NomeDaCenaFase => nomeDaCenaFase;

    public void AtualizarVisual(int vitorias, int erros, int partidas, int vidasFinais)
    {
        if (txtVitorias != null) txtVitorias.text = $"Vitórias : {vitorias}";
        if (txtErros != null) txtErros.text = $"Falhas Cometidas : {erros}";
        if (txtPartidas != null) txtPartidas.text = $"Partidas Jogadas : {partidas}";
        if (txtVidasFinais != null) txtVidasFinais.text = $"HP/Vidas finais : {vidasFinais}";
    }

    public void DefinirComoSemDados()
    {
        if (txtVitorias != null) txtVitorias.text = "Vitórias : 0";
        if (txtErros != null) txtErros.text = "Falhas Cometidas : 0";
        if (txtPartidas != null) txtPartidas.text = "Partidas Jogadas : 0";
        if (txtVidasFinais != null) txtVidasFinais.text = "HP/Vidas finais : --";
    }
}