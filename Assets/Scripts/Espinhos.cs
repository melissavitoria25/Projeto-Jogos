
using System.Collections;
using UnityEngine;

public class Espinhos : MonoBehaviour
{
    [Header("Personagem que pode ser atingido")]
    [SerializeField] Player jogadorPermitido;

    [Header("Referências")]
    [SerializeField] Player player1;
    [SerializeField] Player player2;

    [SerializeField] GerenciadorCenario gerenciadorCenario;

    bool processandoContato = false;

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (processandoContato)
        {
            return;
        }

        // Identifica qual personagem encostou
        Player personagem = outro.GetComponentInParent<Player>();

        if (personagem == null)
        {
            return;
        }

        // Impede que o personagem errado morra
        if (personagem != jogadorPermitido)
        {
            return;
        }

        // Confere as referências
        if (player1 == null || player2 == null ||
            gerenciadorCenario == null)
        {
            Debug.LogError("Referências dos espinhos não configuradas!");
            return;
        }

        processandoContato = true;

        Debug.Log(personagem.name + " atingiu seus espinhos!");

        // Restaura os papéis iniciais
        player1.ReiniciarPersonagem(true);
        player2.ReiniciarPersonagem(false);

        // Retorna os personagens aos respectivos pontos
        player1.Respawn();
        player2.Respawn();

        // Retorna o cenário ao mundo do Player1
        gerenciadorCenario.ReiniciarCenario();

        // Libera novos contatos depois do respawn
        StartCoroutine(LiberarContato());
    }

    IEnumerator LiberarContato()
    {
        yield return new WaitForSeconds(0.5f);

        processandoContato = false;
    }
}
