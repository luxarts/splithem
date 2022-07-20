using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    static public GameHandler Instance;
    public GameObject enemyPrefab;

    [Header("UIs")]
    public GameObject uiHUD;
    public GameObject uiPause;
    public GameObject uiVictory;
    public GameObject uiDefeat;

    [Header("HUD elements")]
    public TextMeshProUGUI scoreField;
    public TextMeshProUGUI timerField;
    public TextMeshProUGUI alertField;
    public TextMeshProUGUI enemiesField;
    public Image[] diamonds;

    [Space(10)]
    public AudioClip playerHitSound;
    public bool isPaused = true;
    public AudioSource audioSource;
    public int enemiesCount = 1;

    [Header("Settings")]
    public float lightIntensityFactor = 10;
    public float lightRangeFactor = 2;
    public int lifes = 3;
    public float alertTime = 3f;
    public float enemyScaleFactor = 0.5f;
    public float forceOnHit = 2f;
    public int maxSplits = 3;

    private int lifesRemaining;
    private int score = 0;
    private float startTime;
    private float minEnemyScale = 0;
    private string timer;
    private int totalShoots = 0;
    private int kills = 0;

    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }
        
        lifesRemaining = lifes;

        audioSource = gameObject.GetComponent<AudioSource>();

        minEnemyScale = enemyPrefab.transform.localScale.x / Mathf.Pow(1 / enemyScaleFactor, maxSplits);

        Pause();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        StartCoroutine(StartGame());
    }
    private void FixedUpdate()
    {
        timer = SecondsToString(Time.fixedTime - startTime);
        timerField.text = timer;
        enemiesField.text = enemiesCount.ToString();
        if (lifesRemaining == 0)
        {
            ShowDefeat();
        }
        if(enemiesCount == 0)
        {
            ShowVictory();
        }
    }
    public string GetTimer()
    {
        return timer;
    }
    public int GetEnemies()
    {
        return enemiesCount;
    }
    public int GetKills()
    {
        return kills;
    }
    public void AddShoot()
    {
        totalShoots++;
    }
    public float GetAccuracy()
    {
        if (totalShoots == 0) return 0f;

        return ((float)kills / (float)totalShoots) * 100f;
    }

    public void RemoveLife()
    {
        Color color = new Color(0, 0, 0, 0.5f);
        diamonds[lifesRemaining-1].color = color;

        lifesRemaining--;
        string message = "Ouch! " + lifesRemaining + " lifes remaining.";
        audioSource.PlayOneShot(playerHitSound);
        if (lifesRemaining == 1)
        {
            message = "LAST LIFE REMAINING, BE CAREFUL!";
        }
        if (lifesRemaining > 0)
        {
            StartCoroutine(ShowAlert(message));
        }
        
    }
    public void AddScore(int amount)
    {
        score += amount;
        scoreField.text = score.ToString();
    }
    public int GetScore()
    {
        return score;
    }
    private IEnumerator ShowAlert(string message)
    {
        alertField.text = message;
        alertField.gameObject.SetActive(true);

        yield return new WaitForSeconds(alertTime);

        alertField.gameObject.SetActive(false);
    }
    private IEnumerator StartGame()
    {
        alertField.gameObject.SetActive(true);
        alertField.text = "Get ready!";

        yield return new WaitForSecondsRealtime(1);

        alertField.text = "3";

        yield return new WaitForSecondsRealtime(1);

        alertField.text = "2";

        yield return new WaitForSecondsRealtime(1);

        alertField.text = "1";

        yield return new WaitForSecondsRealtime(1);

        alertField.text = "SPLIT THEM!";
        Time.timeScale = 1;
        startTime = Time.fixedTime;
        isPaused = false;

        yield return new WaitForSecondsRealtime(1);

        alertField.gameObject.SetActive(false);
        timerField.gameObject.SetActive(true);
    }
    public void Restart()
    {
        SceneManager.GoToGameplay();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void Unpause()
    {
        Time.timeScale = 1;
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private string SecondsToString(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        string minutesStr = minutes < 10 ? "0" + minutes.ToString() : minutes.ToString();

        int secondsInt = Mathf.FloorToInt(seconds % 60);
        string secondsStr = secondsInt < 10 ? "0" + secondsInt.ToString() : secondsInt.ToString();
        
        return minutesStr+":"+secondsStr;
    }

    public void EnemyHit(Collider enemyCollider, Ray r)
    {
        kills++;
        int amount = enemyCollider.GetComponent<Face>().GetNumber();
        AddScore(amount);
        if (enemyCollider.transform.parent.localScale.x != minEnemyScale)
        {
            enemiesCount += amount + 1;
            HandleSplit(enemyCollider.transform.parent.gameObject, enemyCollider.transform.parent.position, r.origin, enemyScaleFactor, amount + 1);
        }

        enemiesCount--;
        Destroy(enemyCollider.transform.parent.gameObject);
    }

    private void HandleSplit(GameObject obj, Vector3 pos, Vector3 playerPos, float scaleFactor, int amount)
    {
        Vector3 scale = new Vector3(obj.transform.localScale.x * scaleFactor, obj.transform.localScale.y * scaleFactor, obj.transform.localScale.z * scaleFactor);
        for (int i = 0; i < amount; i++)
        {
            GameObject cube = Instantiate(obj, pos, Quaternion.Euler(45, 45, 45));
            cube.transform.localScale = scale;
            cube.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(pos - playerPos) * forceOnHit, ForceMode.Impulse);
        }
    }

    public void ShowHUD()
    {
        if (uiVictory.activeInHierarchy || uiDefeat.activeInHierarchy) return;

        Unpause();

        uiHUD.SetActive(true);
        uiPause.SetActive(false);
        uiVictory.SetActive(false);
        uiDefeat.SetActive(false);
    }
    public void ShowPause()
    {
        if (uiVictory.activeInHierarchy || uiDefeat.activeInHierarchy) return;
        Pause();

        uiHUD.SetActive(false);
        uiPause.SetActive(true);
        uiVictory.SetActive(false);
        uiDefeat.SetActive(false);

        uiPause.GetComponent<WindowPause>().ReloadValues();
    }
    public void ShowVictory()
    {
        Pause();

        uiHUD.SetActive(false);
        uiPause.SetActive(false);
        uiVictory.SetActive(true);
        uiDefeat.SetActive(false);

        uiVictory.GetComponent<WindowVictory>().ReloadValues();
    }
    public void ShowDefeat()
    {
        Pause();

        uiHUD.SetActive(false);
        uiPause.SetActive(false);
        uiVictory.SetActive(false);
        uiDefeat.SetActive(true);

        uiDefeat.GetComponent<WindowDefeat>().ReloadValues();
    }
}
