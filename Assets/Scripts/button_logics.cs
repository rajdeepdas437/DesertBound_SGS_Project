using UnityEngine;
using UnityEngine.SceneManagement; // Make sure this namespace is at the very top of your script


public class button_logics : MonoBehaviour
{
    public void end_game()
    {
        Application.Quit();

        // This will stop the "Play Mode" while you are in the Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void swtich_to_level_1()
    {
        SceneManager.LoadScene(1);
    }
}
