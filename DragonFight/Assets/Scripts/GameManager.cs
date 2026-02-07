using UnityEngine;
using UnityEngine.SceneManagement; // Needed for Restart

public class GameManager : MonoBehaviour
{
    [Header("Dragons")]
    [SerializeField] private CharacterStats playerStats;
    [SerializeField] private CharacterStats enemyStats;

    [Header("UI Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    private bool _gameEnded = false;

    private void Update()
    {
        if (_gameEnded) return;

        // Check Player Death
        if (playerStats != null && playerStats.IsDead)
        {
            EndGame(false); // Player lost
        }
        // Check Enemy Death
        else if (enemyStats != null && enemyStats.IsDead)
        {
            EndGame(true); // Player won
        }
       // Quit Game on ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Quitting Game...");
            Application.Quit();
        }
    }

    private void EndGame(bool playerWon)
    {
        _gameEnded = true;

        if (playerWon)
        {
            Debug.Log("VICTORY!");
            if (winPanel != null) winPanel.SetActive(true);
        }
        else
        {
            Debug.Log("GAME OVER!");
            if (losePanel != null) losePanel.SetActive(true);
        }
    }

    // Connect this to a "Restart" button in your UI
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}