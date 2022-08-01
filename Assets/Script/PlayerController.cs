using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    //Game Object
    private Camera camera;
    [SerializeField] private GameObject deadParticle;

    //Components
    private Collider2D wallCollider2D;
    [SerializeField] private SpringJoint2D springJoint2D;
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private LineRenderer lineRenderer;
   

    // Start is called before the first frame update
    void Start()
    {
        //Set Camera
        camera = Camera.main;

        //Fade In
        foreach(SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.DOFade(1f, GameManager.playerFadeInDuration);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Controll
        //When Clicked
        if (Input.GetMouseButtonDown(0))
        {
            //Search Wall
            Collider2D[] cols = Physics2D.OverlapCircleAll(camera.ScreenToWorldPoint(Input.mousePosition), 0.5f);
            foreach(Collider2D col in cols)
            {
                if(col.CompareTag("Wall")) {
                    wallCollider2D = col;

                    //Enable Hookshot
                    springJoint2D.enabled = true;
                    lineRenderer.enabled = true;
                    springJoint2D.connectedBody = wallCollider2D.GetComponent<Rigidbody2D>();

                    //Sound
                    SoundManager.Instance.audioSource.PlayOneShot(GameManager.Instance.hookAudio);

                    //Destroy Wall
                    SpriteRenderer spriteRenderer = wallCollider2D.GetComponent<SpriteRenderer>();
                    if (spriteRenderer.color.a == 1f)
                    {
                        //Add Score
                        GameManager.Instance.score++;
                        GameManager.Instance.scoreGameText.text = "score:" + GameManager.Instance.score.ToString("000");

                        spriteRenderer.DOFade(0f, GameManager.wallDuration).OnComplete(() =>
                        {
                            Destroy(spriteRenderer.gameObject);
                        });
                    }
                    break;
                }
            }
        }
        //Release Hookshot
        else if(Input.GetMouseButtonUp(0))
        {
            springJoint2D.enabled = false;
            lineRenderer.enabled = false;
        }

        //Camera Move
        if(transform.position.x > camera.transform.position.x)
        {
            camera.transform.position = new Vector3(transform.position.x, 0f, camera.transform.position.z);
        }

        if (wallCollider2D == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        //Draw Hookshot
        if(springJoint2D.enabled)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, wallCollider2D.transform.position);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Spike"))
        {
            Instantiate(deadParticle, transform.position, Quaternion.identity);
            GameManager.Instance.EndGame();
            Destroy(gameObject);
            SoundManager.Instance.audioSource.PlayOneShot(GameManager.Instance.deadAudio);
        }
        else if (collision.transform.CompareTag("Step"))
        {
            SpriteRenderer spriteRenderer = collision.transform.GetComponent<SpriteRenderer>();

            if (spriteRenderer == null) return;

            if (spriteRenderer.color.a != 1f) return;

            spriteRenderer.DOFade(0f, GameManager.stepDuration).OnComplete(() =>
            {
                Destroy(spriteRenderer.transform.gameObject);
            });
        }
    }
}
