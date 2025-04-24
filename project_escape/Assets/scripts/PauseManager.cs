using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu; // Reference to Pause Menu Panel

    // Pauses the game and shows the pause menu
    public void PauseGame()
    {
        Time.timeScale = 0f; // Pause the game
        pauseMenu.SetActive(true); // Show the Pause Menu

        // Ensure Volume Slider reconnects when the Pause Menu is opened
        FindObjectOfType<MusicController>()?.ReconnectSlider();
    }

    // Resumes the game and hides the pause menu
    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume the game
        pauseMenu.SetActive(false); // Hide the Pause Menu
    }
}
