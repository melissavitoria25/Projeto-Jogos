using UnityEngine;

public class GerenciadorCenario : MonoBehaviour
{
    // Referências às plataformas dos dois mundos
    [SerializeField] GameObject plataformaPlayer1;
    [SerializeField] GameObject plataformaPlayer2;

    // Começamos no mundo do Player1
    bool mundoPlayer1 = true;

    void Start()
    {
        AtualizarCenario();
    }

    // Será chamada quando os personagens trocarem
    public void TrocarMundo()
    {
        mundoPlayer1 = !mundoPlayer1;

        AtualizarCenario();
    }

    // Ativa as plataformas do mundo correspondente
    void AtualizarCenario()
    {
        plataformaPlayer1.SetActive(mundoPlayer1);
        plataformaPlayer2.SetActive(!mundoPlayer1);
    }

}
