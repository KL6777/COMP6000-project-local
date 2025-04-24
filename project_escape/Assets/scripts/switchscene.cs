using UnityEngine;
using UnityEngine.SceneManagement;

public class switchscene : MonoBehaviour
{
    // Function to switch to the game screen
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Level01"); 
    }

    // Function to switch to the tutorial screen
    public void LoadTutorialScene()
    {
        SceneManager.LoadScene("Tutorial Screen"); 
    }

    // Function to switch to the menu screen
    public void LoadMenuScreen()
    {
        SceneManager.LoadScene("Menu"); 
    }

    public void LoadAboutScreen()
    {
        SceneManager.LoadScene("About");
    }
}
