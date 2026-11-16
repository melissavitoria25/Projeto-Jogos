
using UnityEngine;
using TMPro;

public class GerenciadorJogo : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] TMP_Text textoStatus;
    [SerializeField] TMP_Text textoVitoria;

    // Progresso da fase
    int orbesColetadas = 0;

    bool possuiChave = false;
    bool faseConcluida = false;

    // Permite que outros scripts consultem a chave
    public bool PossuiChave => possuiChave;

    void Start()
    {
        AtualizarInterface();

        // A mensagem de vitória começa escondida
        if (textoVitoria != null)
        {
            textoVitoria.gameObject.SetActive(false);
        }
    }

    // Registra uma orbe coletada
    public void RegistrarOrbe()
    {
        if (faseConcluida)
        {
            return;
        }

        orbesColetadas++;

        AtualizarInterface();

        Debug.Log("Orbes coletadas: " + orbesColetadas);
    }

    // Registra a coleta da chave
    public void RegistrarChave()
    {
        if (possuiChave)
        {
            return;
        }

        possuiChave = true;

        AtualizarInterface();

        Debug.Log("Chave coletada! Procure a porta no mundo do Player2.");
    }

    // Conclui a fase quando a porta é aberta
    public void ConcluirFase()
    {
        if (faseConcluida || !possuiChave)
        {
            return;
        }

        faseConcluida = true;

        if (textoVitoria != null)
        {
            textoVitoria.text = "FASE CONCLUÍDA!";
            textoVitoria.gameObject.SetActive(true);
        }

        Debug.Log("VITÓRIA! Fase concluída.");
    }

    // Atualiza as informações exibidas na tela
    void AtualizarInterface()
    {
        if (textoStatus != null)
        {
            textoStatus.text =
                "Orbes: " + orbesColetadas +
                " | Chave: " + (possuiChave ? "Sim" : "Não");
        }
    }
}
