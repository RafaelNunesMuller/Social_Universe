using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Arraste seu PauseMenuCanvas aqui
    public string mainMenuSceneName = "MainMenu"; // Nome da cena do menu principal

    public static bool GameIsPaused = false; // Flag estática para verificar o estado do jogo

    // Referência ao script de movimento do player para desativar/ativar controles
    public Player_Move playerMoveScript;

    void Start()
    {
        // Tenta encontrar o Player_Move se não for atribuído no Inspector
        if (playerMoveScript == null)
        {
            playerMoveScript = FindFirstObjectByType<Player_Move>();
            if (playerMoveScript == null)
            {
                Debug.LogError("Player_Move script não encontrado na cena para o PauseMenuManager!");
            }
        }

        // Garante que o menu de pausa está desativado no início
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        GameIsPaused = false;
        Time.timeScale = 1f; // Garante que o tempo esteja correndo no início
        Debug.Log("PauseMenuManager: Jogo iniciado, não pausado.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) // Detecta a tecla ESC
        {
            if (GameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false); // Desativa o UI do menu de pausa
        }
        Time.timeScale = 1f; // Volta o tempo ao normal
        GameIsPaused = false;

        // Reativa os controles do player
        if (playerMoveScript != null)
        {
            playerMoveScript.canMove = true;
            playerMoveScript.canRotate = true;
            playerMoveScript.canJump = true;
            playerMoveScript.canAttack = true;
            playerMoveScript.canDefend = true;
            Debug.Log("Controles do player reativados.");
        }

        Cursor.lockState = CursorLockMode.Locked; // Bloqueia o cursor
        Cursor.visible = false; // Esconde o cursor
        Debug.Log("Jogo Despausado.");
    }

    void PauseGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true); // Ativa o UI do menu de pausa
        }
        Time.timeScale = 0f; // Congela o tempo do jogo
        GameIsPaused = true;

        // Desativa os controles do player
        if (playerMoveScript != null)
        {
            playerMoveScript.canMove = false;
            playerMoveScript.canRotate = false;
            playerMoveScript.canJump = false;
            playerMoveScript.canAttack = false;
            playerMoveScript.canDefend = false;
            Debug.Log("Controles do player desativados.");
        }

        Cursor.lockState = CursorLockMode.None; // Desbloqueia o cursor
        Cursor.visible = true; // Mostra o cursor
        Debug.Log("Jogo Pausado.");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Garante que o tempo esteja normal antes de recarregar
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recarrega a cena atual
        Debug.Log("Reiniciando jogo...");
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Garante que o tempo esteja normal antes de carregar
        SceneManager.LoadScene(mainMenuSceneName); // Carrega a cena do menu principal
        Debug.Log("Voltando ao menu principal...");
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}