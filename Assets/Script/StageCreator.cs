using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StageCreator : MonoBehaviour
{
    //Game Object
    [SerializeField] private GameObject floorObject;
    [SerializeField] private GameObject ceilingObject;
    private Camera camera;

    //Variables
    public float stageDifficulty = 0;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;

        floorObject.transform.DOMoveY(Mathf.Log(stageDifficulty / 30f + 1f) / 1.5f, GameManager.stageMoveDuration);
        ceilingObject.transform.DOMoveY(-Mathf.Log(stageDifficulty / 30f + 1f) / 1.5f, GameManager.stageMoveDuration);
    }

    // Update is called once per frame
    void Update()
    {
        if (camera.transform.position.x - transform.position.x > 30f) Destroy(gameObject);
    }

    //Fade Out
    public void FadeOut()
    {
        floorObject.transform.DOMoveY(-1f, GameManager.stageMoveDuration);
        ceilingObject.transform.DOMoveY(1f, GameManager.stageMoveDuration).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
