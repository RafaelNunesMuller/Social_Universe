using UnityEngine;
using UnityEngine.SceneManagement; // Importe para carregar cenas

public class MainMenuManager : MonoBehaviour
{
    // Certifique-se de que o nome da sua cena principal do jogo está correto
    public string gameSceneName = "Scene1"; // <<< MUDE PARA O NOME DA SUA CENA DE JOGO

    void Start()
    {
        // Opcional: Garante que o cursor esteja visível e desbloqueado no menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayGame()
    {
        Debug.Log("Carregando jogo...");
        SceneManager.LoadScene("Stage1"); // Carrega a cena do jogo
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit(); // Fecha a aplicação (funciona apenas em builds)

        // Se estiver no Editor, use Debug.Break() para parar a execução
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}