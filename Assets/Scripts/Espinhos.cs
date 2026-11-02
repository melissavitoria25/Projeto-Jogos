using UnityEngine;

public class Espinhos : MonoBehaviour
{
    // Referências aos dois personagens
    [SerializeField] Player player1;
    [SerializeField] Player player2;

    // Referência ao gerenciador do cenário
    [SerializeField] GerenciadorCenario gerenciadorCenario;

    // Impede processar vários contatos na mesma atualização de física
    bool processandoContato = false;

    // Detecta quando algum personagem toca nos espinhos
    // Primeiro interrompemos qualquer troca de personagem e restauramos os papéis. 
    // Depois reposicionamos os 2 players pro inicio da fase e reiniciamos o cenário
    // Queremos evitar que uma troca ainda em andamento continue interferindo no respawn
    void OnTriggerEnter2D(Collider2D outro)
    {
        if (processandoContato)
        {
            return;
        }

        // Verifica se o Collider pertence a um personagem
        Player personagem = outro.GetComponentInParent<Player>();

        if (personagem != null)
        {
            Debug.Log("Personagem atingiu os espinhos!");

            // Restaura os papéis iniciais e cancela possíveis trocas
            player1.ReiniciarPersonagem(true);
            player2.ReiniciarPersonagem(false);

            // Retorna os dois personagens aos pontos iniciais
            player1.Respawn();
            player2.Respawn();

            // Restaura as plataformas e os obstáculos do mundo 1 (mundo do player1)
            gerenciadorCenario.ReiniciarCenario();
        }
    }

    
    void OnEnable()
    {
        processandoContato = false;
    }
}
