
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Rigidbody2D playerRB;
    Animator playerAnimator;
    BoxCollider2D playerFeet;

    float xDir;

    [SerializeField] float xSpeed;
    [SerializeField] float jumpIntensity;

    bool canDoubleJump;

    // Referência ao outro personagem
    [SerializeField] Transform outroPlayer;
    Player outroScript;

    //Referência ao gerenciador do cenário
    [SerializeField] GerenciadorCenario gerenciadorCenario;

    //Posição em que o personagem reaparece
    [SerializeField] Transform pontoRespawn;

    // Configurações do seguidor
    [SerializeField] float distanciaSeguidor = 2f;
    [SerializeField] float velocidadeSeguidor = 5f;

    //melhorando o pulo, pro player2 não ficar travado na borda da plataforma
    //define o alcance da detecção e quais camadas possuem obstáculos que o seguidor deve detectar.
    [SerializeField] float distanciaDeteccao = 0.8f;
    [SerializeField] LayerMask camadaObstaculos;

    // Configurações da troca
    [SerializeField] float tempoTroca = 0.5f;

    // Identifica o personagem principal e se está trocando
    // Variaveis que controlam a troca
    bool isPrincipal;
    bool isSwapping;

    void Awake()
    {
        // Obtém os componentes do próprio personagem
        playerRB = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponentInChildren<Animator>();
        playerFeet = GetComponentInChildren<BoxCollider2D>();

        // Verifica se o personagem começa como principal
        isPrincipal = CompareTag("Principal");

        // Obtém o script Player do outro personagem
        if (outroPlayer != null)
        {
            outroScript = outroPlayer.GetComponent<Player>();
        }
    }

    void FixedUpdate()
    {
        // Impede movimentação normal durante a troca
        if (isSwapping)
        {
            return;
        }

        // Decide se o personagem é controlado ou seguidor
        if (isPrincipal)
        {
            Movimentar();
        }
        else
        {
            SeguirPrincipal();
        }
    }

    // Recebe a direção enviada pelo Player Input
    void OnMove(InputValue inputValue)
    {
        if (isSwapping)
        {
            return;
        }

        float direcao = inputValue.Get<Vector2>().x;

        // Player1 continua recebendo as entradas do teclado
        // e encaminha os comandos quando Player2 é principal
        if (isPrincipal)
        {
            xDir = direcao;
        }
        else
        {
            if (outroScript != null)
            {
                outroScript.xDir = direcao;
            }
        }
    }

    // Movimenta o personagem principal
    void Movimentar()
    {
        playerRB.linearVelocityX = xDir * xSpeed;

        bool isRunning =
            Mathf.Abs(playerRB.linearVelocityX) > Mathf.Epsilon;

        playerAnimator.SetBool("IsRunning", isRunning);

        if (isRunning)
        {
            FlipSprite();
        }
    }

    // Vira o personagem para a direção do movimento
    void FlipSprite()
    {
        float sinal = Mathf.Sign(playerRB.linearVelocityX);

        //inverte apenas a imagem do personagem
        playerAnimator.transform.localScale = new Vector3(sinal, 1, 1);
    }

    // Faz o seguidor acompanhar o principal
    void SeguirPrincipal()
    {
        if (outroPlayer == null)
        {
            return;
        }

        // Calcula a distancia no eixo x entre os personagens
        float distanciaX = outroPlayer.position.x - transform.position.x;

        // Obtém a distância sem considerar o sinal
        float distanciaAtual = Mathf.Abs(distanciaX);

        // Verifica se o seguidor está longe demais
        if (distanciaAtual > distanciaSeguidor)
        {
            // Identifica se deve andar para direita ou esquerda
            float direcao = Mathf.Sign(distanciaX);

            // player2 recebe uma velocidade horizontal para alcançar o player1
            playerRB.linearVelocityX = direcao * velocidadeSeguidor;

            playerAnimator.SetBool("IsRunning", true);

            FlipSprite();

            // Verifica se precisa pular algum obstáculo
            VerificarObstaculo(direcao);
        }
        else
        {
            // Para ao alcançar a distância desejada
            playerRB.linearVelocityX = 0;

            playerAnimator.SetBool("IsRunning", false);
        }
    }

    //Função que detecta obstaculos pra ajudar na movimentação do player2 em plataformas
    //Essa função reutiliza a função ExecutarPulo().
    void VerificarObstaculo(float direcao)
    {
        // Define a origem do raio perto dos pés do personagem
        Vector2 origem = (Vector2)transform.position
                     + Vector2.down * 0.3f;

        // Define a direção horizontal do raio
        Vector2 direcaoRaio = new Vector2(direcao, 0);

        // Verifica se existe algum obstáculo à frente
        RaycastHit2D obstaculo = Physics2D.Raycast(
            origem,
            direcaoRaio,
            distanciaDeteccao,
            camadaObstaculos
        );

        // Se encontrar um obstáculo, tenta pular
        if (obstaculo.collider != null)
        {
            ExecutarPulo();
        }
    }

    // Recebe o comando de pulo
    void OnJump()
    {
        if (isSwapping)
        {
            return;
        }

        // Descobre quem é o principal atualmente
        Player principal = this;

        if (!isPrincipal)
        {
            principal = outroScript;
        }

        if (principal == null)
        {
            return;
        }

        // Apenas o personagem principal tenta pular
        principal.ExecutarPulo();
    }

    // Executa o pulo e o pulo duplo
    void ExecutarPulo()
    {

        // O feetcollider está tocando algum collider da Layer Ground? Se for False, o primeiro pulo não acontece
        if (Physics2D.OverlapBox(
            playerFeet.bounds.center,
            playerFeet.bounds.size,
            0f,
            LayerMask.GetMask("Ground")))

        {
            playerRB.linearVelocityY = jumpIntensity;

            canDoubleJump = true;

            return;
        }

        // Segundo pulo: personagem está no ar
        if (canDoubleJump)
        {
            playerRB.linearVelocityY = jumpIntensity;

            canDoubleJump = false;
        }
    }

    // Recebe o comando de troca dos personagens
    void OnSwap()
    {
        // Impede começar outra troca durante a animação
        if (isSwapping)
        {
            return;
        }

        if (outroScript == null)
        {
            return;
        }

        StartCoroutine(TrocarPersonagens());
    }

    // Anima a troca de posições
    IEnumerator TrocarPersonagens()
    {
        // Bloqueia a movimentação dos dois personagens
        isSwapping = true;
        outroScript.isSwapping = true;

        // Guarda as posições iniciais
        Vector2 posicaoInicial = playerRB.position;
        Vector2 posicaoOutro = outroScript.playerRB.position;

        // Guarda os tipos originais dos Rigidbody2D
        RigidbodyType2D tipoOriginal = playerRB.bodyType;
        RigidbodyType2D tipoOutro = outroScript.playerRB.bodyType;

        // Interrompe os movimentos atuais
        playerRB.linearVelocity = Vector2.zero;
        outroScript.playerRB.linearVelocity = Vector2.zero;

        xDir = 0;
        outroScript.xDir = 0;

        // Configura movimento cinemático durante a troca
        playerRB.bodyType = RigidbodyType2D.Kinematic;
        outroScript.playerRB.bodyType = RigidbodyType2D.Kinematic;

        float tempo = 0f;

        // Executa a animação pelo tempo configurado
        while (tempo < tempoTroca)
        {
            tempo += Time.fixedDeltaTime;

            float porcentagem = Mathf.Clamp01(
                tempo / Mathf.Max(tempoTroca, 0.01f));

            // Calcula as posições intermediárias
            Vector2 novaPosicao = Vector2.Lerp(
                posicaoInicial,
                posicaoOutro,
                porcentagem);

            Vector2 novaPosicaoOutro = Vector2.Lerp(
                posicaoOutro,
                posicaoInicial,
                porcentagem);

            // Move os dois personagens
            playerRB.MovePosition(novaPosicao);
            outroScript.playerRB.MovePosition(novaPosicaoOutro);

            yield return new WaitForFixedUpdate();
        }

        // Restaura os tipos originais dos Rigidbody2D
        playerRB.bodyType = tipoOriginal;
        outroScript.playerRB.bodyType = tipoOutro;

        // Inverte qual personagem é principal
        bool eraPrincipal = isPrincipal;

        isPrincipal = !eraPrincipal;
        outroScript.isPrincipal = eraPrincipal;

        // Atualiza as Tags dos personagens
        if (isPrincipal)
        {
            gameObject.tag = "Principal";
            outroScript.gameObject.tag = "Seguidor";
        }
        else
        {
            gameObject.tag = "Seguidor";
            outroScript.gameObject.tag = "Principal";
        }

        //Atualiza as plataformas do cenário após a troca
        //O mundo será alterado depois que a animação de troca de posições terminar
        if(gerenciadorCenario != null)
        {
            gerenciadorCenario.TrocarMundo();
        }

        // Limpa os movimentos anteriores
        xDir = 0;
        outroScript.xDir = 0;

        playerRB.linearVelocity = Vector2.zero;
        outroScript.playerRB.linearVelocity = Vector2.zero;

        playerAnimator.SetBool("IsRunning", false);
        outroScript.playerAnimator.SetBool("IsRunning", false);

        // Libera novamente a movimentação
        isSwapping = false;
        outroScript.isSwapping = false;
    }

    //Função que interrompe o movimento e reposiciona o personagem
    public void Respawn()
    {
        // Interrompe o movimento atual
        playerRB.linearVelocity = Vector2.zero;

        // Limpa a direção de movimento
        xDir = 0;

        // Posiciona o personagem no ponto de respawn
        if (pontoRespawn != null)
        {
            playerRB.position = pontoRespawn.position;
        }

        // Reinicia o pulo duplo
        canDoubleJump = false;

        // Para a animação de corrida
        playerAnimator.SetBool("IsRunning", false);
    }

    // Restaura o papel inicial de cada personagem
    // A função recebe um parametro chamado principal do tipo bool
    // Quando usamos player1.ReiniciarPersonagem(true) o player1 volta a ser o principal e quando usamos player2.ReiniciarPersonagem(false) o player2 volta a ser o seguidor
    public void ReiniciarPersonagem(bool principal)
    {
        //Interrompe qualquer corrotina de troca (animação de troca) se estiver acontecendo
        StopAllCoroutines();

        //Restaura o comportamento físico normal do personagem, porque durante a troca usamos Kinematic
        playerRB.bodyType = RigidbodyType2D.Dynamic;

        // Libera o personagem para se movimentar novamente
        isSwapping = false;

        // Define quem será o personagem principal
        isPrincipal = principal;

        // Limpa a direção e a velocidade
        // Interrompe qualquer movimento horizontal
        xDir = 0;
        // Zera a velocidade do personagem
        playerRB.linearVelocity = Vector2.zero;

        // Reinicia o controle do pulo duplo
        canDoubleJump = false;

        // Para a animação de corrida
        playerAnimator.SetBool("IsRunning", false);

        // Atualiza a Tag do personagem
        if (isPrincipal)
        {
            gameObject.tag = "Principal";
        }
        else
        {
            gameObject.tag = "Seguidor";
        }
    }

}