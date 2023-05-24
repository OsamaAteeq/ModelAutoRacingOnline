using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused = false;

    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private CanvasRenderer _pauseMenu;


    private void Start()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient) { _pauseButton.gameObject.SetActive(false); }
        else
        {
            _pauseButton.onClick.AddListener(PauseFunction);
            _resumeButton.onClick.AddListener(ResumeFunction);
            _exitButton.onClick.AddListener(MenuFunction);
            _restartButton.onClick.AddListener(RestartFunction);
        }
    }

    private void RestartFunction()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void MenuFunction()
    {

        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    private void PauseFunction() 
    {
        if (!IsPaused)
        {
            _pauseMenu.gameObject.SetActive(true);
            IsPaused = true;
            Time.timeScale = 0f;
        }
    }

    private void ResumeFunction() 
    {
        if (IsPaused)
        {
            _pauseMenu.gameObject.SetActive(false);
            IsPaused = false;
            Time.timeScale = 1f;
        }
    }
}
