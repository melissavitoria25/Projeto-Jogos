
using UnityEngine;

public class Chave : MonoBehaviour
{
    [Header("Quem pode coletar")]
    [SerializeField] Player jogadorPermitido;

    [Header("Gerenciador do jogo")]
    [SerializeField] GerenciadorJogo gerenciadorJogo;

    bool foiColetada = false;

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (foiColetada)
        {
            return;
        }

        Player personagem = outro.GetComponentInParent<Player>();

        // Somente o Player1 pode coletar a chave
        if (personagem != jogadorPermitido)
        {
            return;
        }

        if (gerenciadorJogo == null)
        {
            Debug.LogError("GerenciadorJogo não configurado na chave!");
            return;
        }

        foiColetada = true;

        gerenciadorJogo.RegistrarChave();

        // Faz a chave desaparecer
        gameObject.SetActive(false);
    }
}
