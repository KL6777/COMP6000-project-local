using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using System.Collections;

public class MusicController : MonoBehaviour
{
    // Singleton instance to ensure only one MusicController exists
    public static MusicController instance;

    // References to the background music AudioSource and volume slider
    public AudioSource backgroundMusic;
    public Slider volumeSlider; 

    // Duration for fade out effect when muting the music
    private float fadeDuration = 1.5f;

    void Awake()
    {
        // Ensure only one instance of the MusicController exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scenes
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instances
            return;
        }
    }

    void Start()
    {
        // Ensures the OnSceneLoaded method runs when new scenes load
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Load previously saved volume setting if it exists
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume");

            // Set the background music volume to the saved value
            if (backgroundMusic != null)
            {
                backgroundMusic.volume = savedVolume;
                backgroundMusic.Play();
            }

            // Ensure the volume slider matches the saved volume
            if (volumeSlider != null)
            {
                volumeSlider.value = savedVolume;
                volumeSlider.onValueChanged.AddListener(SetVolume);  // Add listener for volume changes
                UpdateSliderHandleColor(savedVolume);  // Update slider handle colour
            }
        }
    }

    // Called whenever a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find all sliders in the new scene
        Slider[] sliders = GameObject.FindObjectsOfType<Slider>();

        foreach (Slider slider in sliders)
        {
            // Check if the slider is the Volume Slider
            if (slider.name == "VolumeSlider")
            {
                // Connect the slider to the SetVolume method
                slider.onValueChanged.AddListener(SetVolume);

                // Sync the slider value with the background music's current volume
                slider.value = backgroundMusic != null ? backgroundMusic.volume : 0f;

                // Update the slider's handle colour
                UpdateSliderHandleColor(slider.value);
            }
        }

        // Ensure the background music resumes playing when the "Level01" scene loads
        if (scene.name == "Level01" && backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }

    // Reconnects the volume slider inside the Pause Menu
    public void ReconnectSlider()
    {
        // Find the slider in the Pause Menu dynamically
        Slider pauseMenuSlider = GameObject.Find("VolumeSlider")?.GetComponent<Slider>();

        if (pauseMenuSlider != null)
        {
            // Remove any existing listeners to prevent duplicate volume changes
            pauseMenuSlider.onValueChanged.RemoveAllListeners();

            // Add a new listener for volume changes
            pauseMenuSlider.onValueChanged.AddListener(SetVolume);

            // Sync the slider value with the current background music volume
            pauseMenuSlider.value = backgroundMusic != null ? backgroundMusic.volume : 0f;

            // Update the slider's handle colour
            UpdateSliderHandleColor(pauseMenuSlider.value);
        }
    }

    // Sets the background music volume and saves the value
    public void SetVolume(float volume)
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = volume;
        }

        // Save the new volume setting
        PlayerPrefs.SetFloat("MusicVolume", volume);

        // Update the slider handle colour
        UpdateSliderHandleColor(volume);
    }

    // Updates the slider handle colour to indicate if the music is muted
    void UpdateSliderHandleColor(float volume)
    {
        // Find all sliders in the scene
        Slider[] sliders = GameObject.FindObjectsOfType<Slider>();

        foreach (Slider slider in sliders)
        {
            // Check if the slider is the Volume Slider
            if (slider.name == "VolumeSlider")
            {
                // Change the handle colour to red if muted, white otherwise
                Image handleImage = slider.handleRect.GetComponent<Image>();
                if (handleImage != null)
                {
                    handleImage.color = volume == 0 ? Color.red : Color.white;
                }
            }
        }
    }

    // Stops the background music and destroys the MusicController when changing scenes
    public void StopMusicOnSceneChange()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();  // Stop the music immediately
        }
        Destroy(gameObject);  // Remove the MusicController to prevent duplicates
    }
}
