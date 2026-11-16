
using UnityEngine;
using UnityEngine.Tilemaps;

public class GerenciadorCenario : MonoBehaviour
{
    [Header("Plataformas")]
    [SerializeField] GameObject plataformaPlayer1;
    [SerializeField] GameObject plataformaPlayer2;

    [Header("Espinhos")]
    [SerializeField] GameObject espinhosPlayer1;
    [SerializeField] GameObject espinhosPlayer2;

    [Header("Itens e saída")]
    [SerializeField] GameObject chavePlayer1;
    [SerializeField] GameObject portaPlayer2;

    [Header("Transparência")]
    [Range(0f, 1f)]
    [SerializeField] float alphaMundoInativo = 0.35f;

    bool mundoPlayer1 = true;

    void Start()
    {
        AtualizarCenario();
    }

    public void TrocarMundo()
    {
        mundoPlayer1 = !mundoPlayer1;
        AtualizarCenario();
    }

    public void ReiniciarCenario()
    {
        mundoPlayer1 = true;
        AtualizarCenario();
    }

    void AtualizarCenario()
    {
        // Plataformas
        ConfigurarElemento(plataformaPlayer1, mundoPlayer1);
        ConfigurarElemento(plataformaPlayer2, !mundoPlayer1);

        // Espinhos
        ConfigurarElemento(espinhosPlayer1, mundoPlayer1);
        ConfigurarElemento(espinhosPlayer2, !mundoPlayer1);

        // Chave somente no mundo do Player1
        ConfigurarElemento(chavePlayer1, mundoPlayer1);

        // Porta somente no mundo do Player2
        ConfigurarElemento(portaPlayer2, !mundoPlayer1);
    }

    void ConfigurarElemento(GameObject elemento, bool ativo)
    {
        if (elemento == null)
        {
            return;
        }

        float alpha = ativo ? 1f : alphaMundoInativo;

        // Atualiza a transparência da Tilemap
        Tilemap tilemap = elemento.GetComponent<Tilemap>();

        if (tilemap != null)
        {
            Color cor = tilemap.color;
            cor.a = alpha;
            tilemap.color = cor;
        }

        // Atualiza a transparência de Sprites
        SpriteRenderer sprite = elemento.GetComponent<SpriteRenderer>();

        if (sprite != null)
        {
            Color cor = sprite.color;
            cor.a = alpha;
            sprite.color = cor;
        }

        // Ativa ou desativa as colisões
        Collider2D colisor = elemento.GetComponent<Collider2D>();

        if (colisor != null)
        {
            colisor.enabled = ativo;
        }
    }
}
