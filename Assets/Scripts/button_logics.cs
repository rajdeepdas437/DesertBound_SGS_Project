using UnityEngine;
using UnityEngine.SceneManagement; // Make sure this namespace is at the very top of your script


public class button_logics : MonoBehaviour
{
    public GameObject settings;
    public GameObject pausemenu;
    private void Start()
    {
        settings.SetActive(false);
        pausemenu.SetActive(false);
    }
    public void end_game()
    {
        Application.Quit();

        // This will stop the "Play Mode" while you are in the Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void switch_to_settings()
    {
        settings.SetActive(true);
        pausemenu.SetActive(false);

    }

    public void switch_to_back()
    {
        settings.SetActive(false);
        pausemenu.SetActive(true);

    }


    public void restart_game()
    {
        // 1. Unfreeze the game clock so physics and time update normally again
        Time.timeScale = 1f;

        // 2. Get the currently active scene build index and reload it
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
