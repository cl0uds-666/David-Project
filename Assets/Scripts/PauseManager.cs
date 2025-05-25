using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    private float originalVolume;

    void Start()
    {
        // Save the starting volume level
        originalVolume = AudioListener.volume;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        AudioListener.volume = originalVolume;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        originalVolume = AudioListener.volume;
        AudioListener.volume = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.volume = originalVolume;
        SceneManager.LoadScene("MainMenu"); // Replace with your actual menu scene name
    }
}
