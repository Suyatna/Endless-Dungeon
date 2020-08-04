using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Manager;
    public static bool GameIsPause = false;
    
    public GameObject pauseMenuUi;
    public GameObject menuUi;
    public GameObject optionsMenu;
    public GameObject deadMenu;
    public GameObject finishMenu;
    public GameObject loadingPanel;

    public Slider loadingSlider;

    [Header("toggle")] public GameObject toggleFullScreen;

    private int _sceneIndex;

    private bool _isMainMenu = true;
    
    private void Start()
    {
        if (Manager == null)
        {
            Manager = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (toggleFullScreen == null)
        {
            toggleFullScreen = GameObject.FindWithTag("Toggle");
        }
        else
        {
            Screen.fullScreen = toggleFullScreen.GetComponent<Toggle>().isOn;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_isMainMenu)
        {
            if (GameIsPause)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Loading(int sceneIndex)
    {
        _isMainMenu = false;
        
        StartCoroutine(LoadingAsynchronously(sceneIndex));
        
        Time.timeScale = 1;
        GameIsPause = false;
    }

    public void Resume()
    {
        pauseMenuUi.SetActive(false);
        
        Time.timeScale = 1;
        GameIsPause = false;
    }
    
    private void Pause()
    {
        pauseMenuUi.SetActive(true);
        
        Time.timeScale = 0;
        GameIsPause = true;
    }

    public void Death()
    {
        deadMenu.SetActive(true);
        
        Time.timeScale = 0;
        GameIsPause = true;
    }
    
    public void ResetScene()
    {
        DataManager.Instance.isLoadScene = false;
        
        Time.timeScale = 1;
        GameIsPause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void LoadToMainMenu()
    {
        _isMainMenu = true;
        
        StartCoroutine(LoadingAsynchronously(0));
    }
    
    public void LoadFromData()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        _sceneIndex = data.sceneIndexLoad;
        
        Debug.Log("scene: " + _sceneIndex);
        
        StartCoroutine(LoadingAsynchronously(_sceneIndex));
        Time.timeScale = 1;
        GameIsPause = false;
    }
    
    public void LoadSlot(string slot)
    {
        SaveSystem.LoadSlot = "/hook" + slot + ".fun";

        DataManager.Instance.isLoadScene = true;
        DataManager.Instance.slot = slot;
        
        _isMainMenu = false;
        
        LoadFromData();
    }

    public void SaveSlot(string slot)
    {
        GameObject player = GameObject.FindWithTag("Player");
        
        SaveSystem.SaveSlot = "/hook" + slot + ".fun";
        
        player.GetComponent<Player>().SaveGame();
    }
    
    public void ActivePauseMenu()
    {
        if (_isMainMenu)
        {
            menuUi.SetActive(true);
            optionsMenu.SetActive(false);
        }
        else
        {
            pauseMenuUi.SetActive(true);
            optionsMenu.SetActive(false);
        }
    }

    public void ActiveOptionsMenu()
    {
        if (_isMainMenu)
        {
            pauseMenuUi.SetActive(false);
            optionsMenu.SetActive(true);
        }
        else
        {
            pauseMenuUi.SetActive(false);
            optionsMenu.SetActive(true);
        }
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    IEnumerator LoadingAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingPanel.SetActive(true);
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            loadingSlider.value = progress;

            yield return null;
        }
        
        loadingPanel.SetActive(false);
    }
}
