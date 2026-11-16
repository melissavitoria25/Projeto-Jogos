
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    // Nome da cena que contém a fase
    [SerializeField] string nomeCenaJogo = "SampleScene";

    // Inicia o jogo
    public void Jogar()
    {
        SceneManager.LoadScene(nomeCenaJogo);
    }

    // Fecha o jogo
    public void Sair()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
