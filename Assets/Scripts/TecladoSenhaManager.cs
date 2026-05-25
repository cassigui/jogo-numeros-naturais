using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TecladoSenhaManager : MonoBehaviour
{
    [Header("Configurações de UI")]
    [SerializeField] private TMP_InputField inputSenhaVisor;
    [SerializeField] private int limiteDigitos = 4;

    [Header("Configuração da Senha")]
    [SerializeField] private string senhaCorreta = "1234";

    [Header("Dependências do Jogo")]
    [SerializeField] private Fase02 faseManager;

    [Header("Botões do Teclado Numérico")]
    [Tooltip("Arraste aqui os seus botões de 1 a 9 na ordem correta")]
    [SerializeField] private Button[] botoesNumericos;

    [Tooltip("Arraste aqui o botão do Cadeado Rosa")]
    [SerializeField] private Button botaoConfirmarCadeado;

    void Start()
    {
        // 1. Configura o Visor com segurança
        if (inputSenhaVisor != null)
        {
            inputSenhaVisor.text = "";
            inputSenhaVisor.interactable = false;
        }

        // 2. Configura os botões numéricos via código (Evita bugs do Inspector)
        if (botoesNumericos != null)
        {
            for (int i = 0; i < botoesNumericos.Length; i++)
            {
                if (botoesNumericos[i] != null)
                {
                    string numeroDoBotao = (i + 1).ToString(); // Converte o índice atual no número correspondente
                    botoesNumericos[i].onClick.RemoveAllListeners(); // Limpa lixo de memória
                    botoesNumericos[i].onClick.AddListener(() => ClicouNumero(numeroDoBotao));
                }
            }
        }

        // 3. Configura o botão do Cadeado via código
        if (botaoConfirmarCadeado != null)
        {
            botaoConfirmarCadeado.onClick.RemoveAllListeners();
            botaoConfirmarCadeado.onClick.AddListener(ConfirmarSenha);
        }
    }

    public void ClicouNumero(string numero)
    {
        if (inputSenhaVisor == null) return;

        if (inputSenhaVisor.text.Length < limiteDigitos)
        {
            inputSenhaVisor.text += numero;
        }
    }

    public void ConfirmarSenha()
    {
        if (inputSenhaVisor == null) return;

        if (inputSenhaVisor.text == senhaCorreta)
        {
            Debug.Log("🔓 SENHA CORRETA!");
            if (faseManager != null)
            {
                faseManager.SenhaCorretaAcionarVitoria();
            }
        }
        else
        {
            Debug.LogWarning("❌ SENHA INCORRETA!");
            LimparVisor();

            if (faseManager != null)
            {
                faseManager.SenhaIncorretaErrou();
            }
        }
    }

    public void LimparVisor()
    {
        if (inputSenhaVisor != null)
        {
            inputSenhaVisor.text = "";
        }
    }
}