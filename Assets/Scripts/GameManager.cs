using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [SerializeField]
    private int _ballInLevel = 0;

    private int currentSceneIndex = 0;

    private int totalPoints = 0;

    private int pointAtLevelStart = 0;


    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Evita duplicati
            return;
        }

        Instance = this;
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Destroy game manager if we are on mainMenu
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            Destroy(gameObject);
        }
    }

    public void RegisterBall()
    {
        _ballInLevel++;
    }


    public void DeregisterBall()
    {
        _ballInLevel--;

        StartCoroutine(DelayedCheckAdvanceLevel());
    }


    private IEnumerator DelayedCheckAdvanceLevel()
    {
        yield return null; // aspetta un frame
        if (_ballInLevel == 0)
        {

            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            // Check if the next scene exists
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                UIManager.Instance?.showStageClearedText();
                pointAtLevelStart = totalPoints;
                LoadLevel(nextSceneIndex);
            }
            else
            {
                UIManager.Instance?.showWinText();
                ReturnToMenu();
            }
        }
    }

    public void DeregisterAllBall()
    {
        _ballInLevel = 0;
    }

    public void ReloadCurrentLevel()
    {
        SceneManager.LoadScene(currentSceneIndex);
        DeregisterAllBall();
    }

    public void ReturnToMenu()
    {
        LoadLevel(0);
    }

    public void LoadLevel(int buildIndexLevel)

    {
        StartCoroutine(DelayLoadLevel(buildIndexLevel));

        currentSceneIndex = buildIndexLevel;
        DeregisterAllBall();
    }


    private IEnumerator DelayLoadLevel(int buildIndexLevel)
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(buildIndexLevel);
    }

    public void UpdatePoints(int points)
    {
        totalPoints += points;
        UIManager.Instance.updatePoints(totalPoints);
    }

    public void ResetPoints()
    {
        totalPoints = pointAtLevelStart;
        UIManager.Instance.updatePoints(totalPoints);
    }

}
