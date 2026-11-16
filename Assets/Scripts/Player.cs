
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Rigidbody2D playerRB;
    Animator playerAnimator;
    BoxCollider2D playerFeet;

    Collider2D[] colisoesPersonagem;

    float gravidadeOriginal;
    float xDir;
    float direcaoOlhar = 1f;

    [Header("Movimentação")]
    [SerializeField] float xSpeed = 5f;
    [SerializeField] float jumpIntensity = 6f;

    bool canDoubleJump;

    [Header("Referências")]
    [SerializeField] Transform outroPlayer;
    Player outroScript;

    [SerializeField] GerenciadorCenario gerenciadorCenario;
    [SerializeField] Transform pontoRespawn;

    [Header("Seguidor")]
    [SerializeField] float distanciaSeguidor = 1.2f;
    [SerializeField] float alturaSeguidor = 0.6f;
    [SerializeField] float velocidadeSeguidor = 8f;

    // Identifica quem está sendo controlado
    bool isPrincipal;

    void Awake()
    {
        // Obtém os componentes do próprio personagem
        playerRB = GetComponent<Rigidbody2D>();

        playerAnimator = GetComponentInChildren<Animator>();

        playerFeet = GetComponentInChildren<BoxCollider2D>(true);

        colisoesPersonagem = GetComponentsInChildren<Collider2D>(true);

        gravidadeOriginal = playerRB.gravityScale;

        // Identifica o personagem principal inicial
        isPrincipal = CompareTag("Principal");

        // Obtém o script do outro personagem
        if (outroPlayer != null)
        {
            outroScript = outroPlayer.GetComponent<Player>();
        }

        // Configura a física conforme o papel inicial
        AplicarPapel();
    }

    void FixedUpdate()
    {
        if (isPrincipal)
        {
            Movimentar();
        }
        else
        {
            SeguirPrincipal();
        }
    }

    // Recebe os comandos horizontais do teclado
    void OnMove(InputValue inputValue)
    {
        float direcao = inputValue.Get<Vector2>().x;

        // Descobre quem deve receber o comando
        Player controlado = isPrincipal ? this : outroScript;

        if (controlado != null)
        {
            controlado.xDir = direcao;
        }
    }

    // Movimenta o personagem principal
    void Movimentar()
    {
        playerRB.linearVelocityX = xDir * xSpeed;

        bool isRunning = Mathf.Abs(xDir) > 0.01f;

        playerAnimator.SetBool("IsRunning", isRunning);

        if (isRunning)
        {
            direcaoOlhar = Mathf.Sign(xDir);
            FlipSprite(direcaoOlhar);
        }
    }

    // Inverte somente o visual
    void FlipSprite(float direcao)
    {
        Vector3 escala = playerAnimator.transform.localScale;

        escala.x = Mathf.Abs(escala.x) * Mathf.Sign(direcao);

        playerAnimator.transform.localScale = escala;
    }

    // Faz o seguidor flutuar atrás do principal
    void SeguirPrincipal()
    {
        if (outroScript == null || outroPlayer == null)
        {
            return;
        }

        // Calcula a posição desejada atrás e acima do principal
        Vector2 posicaoAlvo = (Vector2)outroPlayer.position;

        posicaoAlvo.x -= outroScript.direcaoOlhar * distanciaSeguidor;
        posicaoAlvo.y += alturaSeguidor;

        // Aproxima o seguidor da posição desejada
        Vector2 novaPosicao = Vector2.MoveTowards(
            playerRB.position,
            posicaoAlvo,
            velocidadeSeguidor * Time.fixedDeltaTime
        );

        playerRB.MovePosition(novaPosicao);

        // O seguidor não precisa executar animação de corrida
        playerAnimator.SetBool("IsRunning", false);

        // Mantém a orientação visual do principal
        FlipSprite(outroScript.direcaoOlhar);
    }

    // Recebe o comando de pulo
    void OnJump()
    {
        Player controlado = isPrincipal ? this : outroScript;

        if (controlado != null)
        {
            controlado.ExecutarPulo();
        }
    }

    // Executa o primeiro pulo ou o pulo duplo
    void ExecutarPulo()
    {
        bool estaNoChao = Physics2D.OverlapBox(
            playerFeet.bounds.center,
            playerFeet.bounds.size,
            0f,
            LayerMask.GetMask("Ground")
        );

        if (estaNoChao)
        {
            playerRB.linearVelocityY = jumpIntensity;

            canDoubleJump = true;

            return;
        }

        if (canDoubleJump)
        {
            playerRB.linearVelocityY = jumpIntensity;

            canDoubleJump = false;
        }
    }

    // Troca instantaneamente os personagens
    void OnSwap()
    {
        if (outroScript == null || gerenciadorCenario == null)
        {
            Debug.LogError("Referências da troca não configuradas!");
            return;
        }

        // Guarda as posições atuais
        Vector2 minhaPosicao = playerRB.position;
        Vector2 outraPosicao = outroScript.playerRB.position;

        // Troca as posições imediatamente
        playerRB.position = outraPosicao;
        outroScript.playerRB.position = minhaPosicao;

        // Interrompe os movimentos anteriores
        xDir = 0;
        outroScript.xDir = 0;

        playerRB.linearVelocity = Vector2.zero;
        outroScript.playerRB.linearVelocity = Vector2.zero;

        // Inverte quem é principal
        bool novoPapel = !isPrincipal;

        isPrincipal = novoPapel;
        outroScript.isPrincipal = !novoPapel;

        // Atualiza as Tags
        AtualizarTag();
        outroScript.AtualizarTag();

        // Atualiza a física dos dois personagens
        AplicarPapel();
        outroScript.AplicarPapel();

        // Atualiza o cenário
        gerenciadorCenario.TrocarMundo();

        // Atualiza imediatamente as posições na física
        Physics2D.SyncTransforms();

        Debug.Log("Personagens e mundos trocados!");
    }

    // Atualiza a Tag conforme o papel do personagem
    void AtualizarTag()
    {
        gameObject.tag = isPrincipal ? "Principal" : "Seguidor";
    }

    // Configura a física conforme o papel atual
    void AplicarPapel()
    {
        if (isPrincipal)
        {
            // Personagem principal utiliza física normal
            playerRB.bodyType = RigidbodyType2D.Dynamic;
            playerRB.gravityScale = gravidadeOriginal;

            // Ativa os colliders
            foreach (Collider2D colisor in colisoesPersonagem)
            {
                colisor.enabled = true;
            }
        }
        else
        {
            // Seguidor flutua sem gravidade
            playerRB.linearVelocity = Vector2.zero;
            playerRB.bodyType = RigidbodyType2D.Kinematic;
            playerRB.gravityScale = 0f;

            // Desativa todos os colliders do seguidor
            foreach (Collider2D colisor in colisoesPersonagem)
            {
                colisor.enabled = false;
            }
        }
    }

    // Retorna o personagem ao ponto inicial
    public void Respawn()
    {
        playerRB.linearVelocity = Vector2.zero;

        xDir = 0;

        if (pontoRespawn != null)
        {
            playerRB.position = pontoRespawn.position;
        }

        canDoubleJump = false;

        playerAnimator.SetBool("IsRunning", false);
    }

    // Restaura o papel de cada personagem
    public void ReiniciarPersonagem(bool principal)
    {
        StopAllCoroutines();

        isPrincipal = principal;

        xDir = 0;

        playerRB.linearVelocity = Vector2.zero;

        canDoubleJump = false;

        playerAnimator.SetBool("IsRunning", false);

        AtualizarTag();

        AplicarPapel();
    }
}
