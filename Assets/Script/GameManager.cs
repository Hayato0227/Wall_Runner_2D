using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Instance
    public static GameManager Instance;

    //Game Object
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject stagePrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject stepPrefab;
    [SerializeField] private GameObject playerPrefab;
    private Camera camera;

    //Componets
    [SerializeField] private CanvasGroup titleCanvasGroup;
    [SerializeField] private Text titleText;
    [SerializeField] private Text scoreText;
    [SerializeField] private CanvasGroup scoreCanvasGroup;
    public Text scoreGameText;

    //Variables
    public int score = 0;
    private float previousCameraPositionXCount = 1;
    
    //Constants
    private static float uiDuration = 0.5f;
    public static float stepDuration = 3f;
    public static float playerFadeInDuration = 1f;
    public static float stageMoveDuration = 2f;
    public static float wallDuration = 5f;

    //Audio
    public AudioClip deadAudio;
    public AudioClip enterAudio;
    public AudioClip hookAudio;

    // Start is called before the first frame update
    void Start()
    {
        //Initialization
        Instance = this;
        camera = Camera.main;

        //Calculate High Score
        int highScore = PlayerPrefs.GetInt("Score");
        PlayerPrefs.SetInt("Score", highScore);

        //Set Score
        scoreText.text = "High-Score:" + highScore.ToString("000");

        //Fade In Title
        titlePanel.SetActive(true);
        titlePanel.SetActive(true);
        titleCanvasGroup.DOFade(1f, uiDuration).OnComplete(() =>
        {
            titleCanvasGroup.interactable = true;
        });

    }

    // Update is called once per frame
    void Update()
    {
        if(camera.transform.position.x + 8f > previousCameraPositionXCount * 16)
        {
            previousCameraPositionXCount++;

            //Set Stage
            Vector2 stagePos = new Vector2(previousCameraPositionXCount * 16f - 8f, 0f);
            GameObject stage = CreateObject(stagePrefab, stagePos);
            stage.GetComponent<StageCreator>().stageDifficulty = score;

            //Set Step
            if(Random.value > 0.5f)
            {
                GameObject obj = CreateObject(stepPrefab, stagePos + new Vector2(Random.value, -3f + Random.value));
            }
            else
            {
                GameObject obj1 = CreateObject(stepPrefab, stagePos + new Vector2(-4f + Random.value, -3f + Random.value));
                GameObject obj2 = CreateObject(stepPrefab, stagePos + new Vector2(4f + Random.value, -3f + Random.value));
            }

            //Set Wall
            GameObject obj4 = CreateObject(wallPrefab, stagePos + new Vector2(-5f + Random.value, 3f * Random.value));
            GameObject obj5 = CreateObject(wallPrefab, stagePos + new Vector2(Random.value, 3f * Random.value));
            GameObject obj6 = CreateObject(wallPrefab, stagePos + new Vector2(5f + Random.value, 3f * Random.value));
        }
    }


    //Game Start
    public void StartGame()
    {
        //Fade Out Title
        titleCanvasGroup.interactable = false;
        scoreCanvasGroup.DOFade(1f, uiDuration);
        titleCanvasGroup.DOFade(0f, uiDuration).OnComplete(() =>
        {
            titlePanel.SetActive(false);
        });

        //Sound
        SoundManager.Instance.audioSource.PlayOneShot(enterAudio);

        //Set Stage
        CreateObject(stagePrefab, new Vector2(-8f, 0f));
        CreateObject(stagePrefab, new Vector2(8f, 0f));

        //Set Wall
        CreateObject(wallPrefab, new Vector2(0f, 1f));
        CreateObject(wallPrefab, new Vector2(5f, 1f));
        CreateObject(wallPrefab, new Vector2(10f, 1f));
        CreateObject(wallPrefab, new Vector2(15f, 1f));

        //Set Step
        CreateObject(stepPrefab, new Vector2(2.5f, -3f));
        CreateObject(stepPrefab, new Vector2(7.5f, -3f));
        CreateObject(stepPrefab, new Vector2(12.5f, -3f));

        //Set Player
        CreateObject(playerPrefab, new Vector2(-5f, 1));
        CreateObject(stepPrefab, new Vector2(-5f, 0f));
    }

    //Create Object
    private GameObject CreateObject(GameObject obj, Vector2 pos)
    {
        return Instantiate(obj, pos, Quaternion.identity);
    }

    //End Game
    public void EndGame()
    {
        StartCoroutine(EndCoroutine());
    }

    //End Coroutine
    private IEnumerator EndCoroutine()
    {
        //Set High Score
        int highScore = PlayerPrefs.GetInt("Score", 0);
        if (highScore < score) PlayerPrefs.SetInt("Score", score);


        yield return new WaitForSeconds(1f);

        scoreCanvasGroup.DOFade(0f, uiDuration);

        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Stage"))
        {
            obj.GetComponent<StageCreator>()?.FadeOut();
        }

        yield return new WaitForSeconds(stageMoveDuration + 2f);

        SceneManager.LoadScene("Game");
    }
}
