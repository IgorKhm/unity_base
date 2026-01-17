using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject titlePanel;
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Gameplay Root (optional)")]
    public GameObject gameplayRoot; // put Player+Enemy under this if you want easy enable/disable

    
    
    void OnEnable()
    {
        Health.AnyDeath += OnAnyDeath;
    }

    void OnDisable()
    {
        Health.AnyDeath -= OnAnyDeath;
    }

    private void OnAnyDeath(GameObject dead)
    {
        // Win if an Enemy died
        if (dead.layer == LayerMask.NameToLayer("Enemy"))
            Win();
    }

    void Start()
    {
        ShowTitle();
        if (losePanel) losePanel.SetActive(false);
    }
    
    public void Lose()
    {
        Time.timeScale = 0f;
        if (losePanel) losePanel.SetActive(true);
    }

    void ShowTitle()
    {
        Time.timeScale = 0f;
        if (titlePanel) titlePanel.SetActive(true);
        if (winPanel) winPanel.SetActive(false);
        if (gameplayRoot) gameplayRoot.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
    }

    public void StartGame()
    {
        if (titlePanel) titlePanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        if (gameplayRoot) gameplayRoot.SetActive(true);

        Time.timeScale = 1f;
    }

    public void Win()
    {
        Time.timeScale = 0f;
        if (winPanel) winPanel.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}