using UnityEngine;

public class OrbeLuz : MonoBehaviour
{
    // Indica se a orbe já foi coletada, pra não contabilizar várias vezes caso os colliders do player provoquem mais de um contato
    bool foiColetada = false;

    // Detecta quando algum Collider entra na orbe
    // O player encosta na orbe, OnTriggerEnter2D() detecta o contato, verificamos se o collider pertence ao player1 ou ao player2, a variável foiColetada vira true e aparece a mensagem no console.
    //Depois disso a orbe é desativada e desaparece do cenário
    void OnTriggerEnter2D(Collider2D outro)
    {
        // Impede que a mesma orbe seja coletada novamente
        if (foiColetada)
        {
            return;
        }

        // Verifica se o objeto que encostou pertence a um personagem
        Player personagem = outro.GetComponentInParent<Player>();

        if (personagem != null)
        {
            foiColetada = true;

            Debug.Log("Orbe de luz coletada!");

            // Faz a orbe desaparecer
            gameObject.SetActive(false);
        }
    }
}
