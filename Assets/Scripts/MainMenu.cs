using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject quickGuidePanel;

    void Start()
    {
        ShowMain();
    }

    // -------------------------------
    // Main Buttons
    // -------------------------------
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene"); // Replace with your actual game scene name
    }

    public void OpenOptions()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void OpenQuickGuide()
    {
        mainPanel.SetActive(false);
        quickGuidePanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

    // -------------------------------
    // Quick Guide Buttons
    // -------------------------------
    public void OpenPerksGuide()
    {
        SceneManager.LoadScene("QuickGuide_Perks"); // You can replace this with a UI panel instead
    }

    public void OpenDropsGuide()
    {
        SceneManager.LoadScene("QuickGuide_Drops"); // You can replace this with a UI panel instead
    }

    public void BackFromQuickGuide()
    {
        quickGuidePanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    // -------------------------------
    // Options Back Button
    // -------------------------------
    public void BackFromOptions()
    {
        optionsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    // -------------------------------
    // Reset
    // -------------------------------
    private void ShowMain()
    {
        mainPanel.SetActive(true);
        optionsPanel.SetActive(false);
        quickGuidePanel.SetActive(false);
    }
}
