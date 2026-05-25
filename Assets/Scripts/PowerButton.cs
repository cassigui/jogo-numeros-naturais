using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PowerButton : MonoBehaviour
{
    [SerializeField] private Image slotImage;
    [SerializeField] private Sprite spriteOff;
    [SerializeField] private Sprite spriteOn;
    public bool IsOn { get; private set; }

    private void Awake()
    {
        if (slotImage == null) slotImage = GetComponent<Image>();
        
        DesligarBotao();
    }

    public void LigarBotao()
    {
        Debug.Log("TENTANDO LIGAR O BOTAO");

        if (slotImage != null && spriteOn != null)
        {
            slotImage.sprite = spriteOn;
            IsOn = true;
            Debug.Log("⚡ Botão Power: LIGADO");
        }
    }

    public void DesligarBotao()
    {
        if (slotImage != null && spriteOff != null)
        {
            slotImage.sprite = spriteOff;
            IsOn = false;
            Debug.Log("💤 Botão Power: DESLIGADO");
        }
    }

    public void AlternarEstado()
    {
        if (IsOn)
            DesligarBotao();
        else
            LigarBotao();
    }

}