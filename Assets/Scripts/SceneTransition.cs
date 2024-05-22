using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{ 
    [SerializeField] private string transitionTo;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Vector2 exitDirection;
    [SerializeField] private float exitTime;
    [SerializeField] private float transitionCooldown = 3f; // Add a cooldown to prevent immediate re-triggering
    private bool isTransitioning = false; // To check if a transition is already in progress
    private Collider2D transitionCollider;

    private void Start()
    {
        transitionCollider = GetComponent<Collider2D>();
        if (transitionTo == GameManager.Instance.transitionedFromScene)
        {
            PlayerController.Instance.transform.position = startPoint.position;
            StartCoroutine(PlayerController.Instance.WalkIntoNewScene(exitDirection,exitTime));
            if (transitionCollider != null)
            {
                transitionCollider.enabled = false;
                StartCoroutine(EnableColliderAfterDelay(transitionCooldown));
            }
        }
        //StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));   
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {

        if (_other.CompareTag("Player") && !isTransitioning)
        {
            CheckShadeData();
           
            //GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;
            //PlayerController.Instance.pState.cutscene = true;
            ////StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo));
            //if (UIManager.Instance.sceneFader != null)
            //}
                //StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo));
                StartCoroutine(HandleTransition());
            //}
           
        }
    }

    void CheckShadeData()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        for(int i = 0; i < enemyObjects.Length; i++)
        {
            if (enemyObjects[i].GetComponent<Shade>() != null)
            {
                SaveData.Instance.SaveShadeData();
            }
        }
    }

    private IEnumerator HandleTransition()
    {
        isTransitioning = true; // Set the transitioning flag to true
        GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;
        PlayerController.Instance.pState.cutscene = true;

        yield return UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo);

        yield return new WaitForSeconds(transitionCooldown); // Wait for cooldown duration
        isTransitioning = false; // Reset the transitioning flag
    }

    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (transitionCollider != null)
        {
            transitionCollider.enabled = true;
        }
    }
}
