
using UnityEngine;

public class Porta : MonoBehaviour
{
    [Header("Personagem autorizado")]
    [SerializeField] Player jogadorPermitido;

    [Header("Gerenciador do jogo")]
    [SerializeField] GerenciadorJogo gerenciadorJogo;

    bool aberta = false;

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (aberta)
        {
            return;
        }

        Player personagem = outro.GetComponentInParent<Player>();

        // Somente o Player2 pode abrir a porta
        if (personagem != jogadorPermitido)
        {
            return;
        }

        if (gerenciadorJogo == null)
        {
            Debug.LogError("GerenciadorJogo não configurado na porta!");
            return;
        }

        // Verifica se a chave já foi coletada
        if (!gerenciadorJogo.PossuiChave)
        {
            Debug.Log("A porta está trancada! Procure a chave no mundo do Player1.");
            return;
        }

        aberta = true;

        Debug.Log("Porta aberta!");

        // Conclui a fase
        gerenciadorJogo.ConcluirFase();

        // Remove a porta da cena
        gameObject.SetActive(false);
    }
}
