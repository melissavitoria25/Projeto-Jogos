
using UnityEngine;

public class OrbeLuz : MonoBehaviour
{
    [SerializeField] GerenciadorJogo gerenciadorJogo;

    bool foiColetada = false;

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (foiColetada)
        {
            return;
        }

        // Identifica o personagem que encostou na orbe
        Player personagem = outro.GetComponentInParent<Player>();

        if (personagem == null)
        {
            return;
        }

        if (gerenciadorJogo == null)
        {
            Debug.LogError("GerenciadorJogo não configurado na orbe!");
            return;
        }

        foiColetada = true;

        // Atualiza o contador
        gerenciadorJogo.RegistrarOrbe();

        // Faz a orbe desaparecer
        gameObject.SetActive(false);
    }
}
