using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MouseController : MonoBehaviour
{
    public float jetpackForce;
    public ParticleSystem jetpack;
    public float forwardMovementSpeed;
    public float forwardMovementSpeedfever;
    private uint level = 0;
    public TMP_Text levels;

    private Rigidbody2D rb;

    public Transform groundCheckTransform;
    public LayerMask groundCheckLayerMask;
    private bool grounded;
    private Animator animator;
    bool fever = false;

    private bool dead = false;

    private uint coins = 0;
    public TMP_Text textCoins;
    public TMP_Text fevertext;

    public Button buttonRestart;
    public Button buttonmain;

    public AudioClip coinCollectSound;
    public AudioSource jetpackAudio;
    public AudioSource footstepsAudio;

    public ParakaxScroll parakaxScroll;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        textCoins.text = coins.ToString();
        levels.text = $"LV. {level}";

        StartCoroutine(LevelCount());
        StartCoroutine(Fever());
    }

    private void FixedUpdate()
    {
        bool jetpackActive = Input.GetButton("Fire1");

        if (jetpackActive && !dead)
        {
            rb.AddForce(jetpackForce * Vector2.up);
        }

        if (!dead)
        {
            Vector2 newVeloctiy = rb.velocity;
            if (fever)
            {
                newVeloctiy.x = forwardMovementSpeedfever;
                fevertext.gameObject.SetActive(true);
            }

            else
            {
                newVeloctiy.x = forwardMovementSpeed;
                fevertext.gameObject.SetActive(false);
            }

            rb.velocity = newVeloctiy;
        }

        UpdateGroundedStatus();
        AdjustJetpack(jetpackActive);
        DisplayRestartButton();
        DisplayMaimenuButton();
        AdjustFootstepsAndJetpackSound(jetpackActive);

        forwardMovementSpeedfever = forwardMovementSpeed * 1.5f;
        parakaxScroll.offset = transform.position.x;
    }

    //fire1을 눌렀을때 활성화되는 jetpackActive 값을 넘겨준다.
    private void AdjustJetpack(bool jetpackActive)
    {
        var emission = jetpack.emission;
        emission.enabled = !grounded;
        emission.rateOverTime = jetpackActive ? 300f : 75f;
    }

    private void UpdateGroundedStatus()
    {
        grounded = Physics2D.OverlapCircle(
            groundCheckTransform.position, 0.1f, groundCheckLayerMask);
        animator.SetBool("grounded",grounded);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coins")
        {
            CollectCoin(collision);
        }
        else
            HitByLaser(collision);
    }

    private void CollectCoin(Collider2D coinCollider)
    {
        ++coins;
        Destroy(coinCollider.gameObject);
        textCoins.text = coins.ToString();

        AudioSource.PlayClipAtPoint(coinCollectSound, transform.position);
    }
    private void HitByLaser(Collider2D laserCollider)
    {
        if (!dead)
        {
            AudioSource laser = laserCollider.GetComponent<AudioSource>();
            laser.Play();
        }
        dead = true;
        animator.SetBool("dead", true);
    }

    private void DisplayRestartButton()
    {
        bool active = buttonRestart.gameObject.activeSelf;
        if (grounded && dead && !active)
            buttonRestart.gameObject.SetActive(true);
    }

    public void OnClickedRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void DisplayMaimenuButton()
    {
        bool active = buttonmain.gameObject.activeSelf;
        if (grounded && dead && !active)
            buttonmain.gameObject.SetActive(true);
    }

    public void OnClickedMainmenuButton()
    {
        SceneManager.LoadScene("Menu");
    }

    private void AdjustFootstepsAndJetpackSound(bool jetpackActive)
    {
        footstepsAudio.enabled = !dead && grounded;
        jetpackAudio.enabled = !dead && !grounded;
        jetpackAudio.volume = jetpackActive ? 1f : 0.5f;
    }

    IEnumerator LevelCount()
    {
        while (true)
        {
            yield return new WaitForSeconds(20f);
            ++level;
            levels.text = $"LV. {level}";
            forwardMovementSpeed = forwardMovementSpeed * 1.1f;
        }
    }
    IEnumerator Fever()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            fever = true;
            yield return new WaitForSeconds(10f);
            fever = false;
        }
    }
}
