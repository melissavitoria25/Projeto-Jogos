using UnityEngine;

public class GerenciadorCenario : MonoBehaviour
{
    // Referências às plataformas dos dois mundos
    [SerializeField] GameObject plataformaPlayer1;
    [SerializeField] GameObject plataformaPlayer2;

    //Espinhos que só ficam ativos no mundo do Player1
    [SerializeField] GameObject espinhosPlayer1;

    //Espinhos que só ficam ativos no mundo do Player2
    [SerializeField] GameObject espinhosPlayer2;

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

    // Faz o cenário retornar ao mundo inicial
    // A plataforma e os espinhos do player1 ficaram ativos, e a plataforma do player2 ficará desativada
    public void ReiniciarCenario()
    {
        mundoPlayer1 = true;
        AtualizarCenario();
    }

    // Ativa as plataformas do mundo correspondente
    void AtualizarCenario()
    {
        //Alterna as plataformas dos dois mundos
        plataformaPlayer1.SetActive(mundoPlayer1);
        plataformaPlayer2.SetActive(!mundoPlayer1);

        //Alterna os espinhos do player1
        if(espinhosPlayer1 != null)
        {
            espinhosPlayer1.SetActive(mundoPlayer1);
        }

        //Alterna os espinhos do player2
        if(espinhosPlayer2 != null)
        {
            espinhosPlayer2.SetActive(!mundoPlayer1);
        }


    }

}
