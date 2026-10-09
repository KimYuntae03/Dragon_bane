using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Image pauseButtonIcon;

    [Header("Icons")]
    [SerializeField] private Sprite pauseIcon;
    [SerializeField] private Sprite playIcon;

    [Header("Scene")]
    [SerializeField] private SceneLoader sceneLoader;

    private bool isPaused = false;

    private void Start()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        UpdatePauseIcon();
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;

        Time.timeScale = 0f;

        if (pauseMenu != null)
            pauseMenu.SetActive(true);

        UpdatePauseIcon();
    }

    public void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1f;

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        UpdatePauseIcon();
    }

    public void ExitToMainMap()
    {
        Time.timeScale = 1f;

        if (sceneLoader != null)
            sceneLoader.ReturnToMainMap();
    }

    private void UpdatePauseIcon()
    {
        if (pauseButtonIcon == null)
            return;

        pauseButtonIcon.sprite =
            isPaused ? playIcon : pauseIcon;
    }
}