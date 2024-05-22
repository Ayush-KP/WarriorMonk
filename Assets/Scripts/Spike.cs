using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.CompareTag("Player")) 
        {
            StartCoroutine(RespawnPoint());
        }
    }

    IEnumerator RespawnPoint() 
    {
        Debug.Log("Respawn process initiated");
        PlayerController.Instance.pState.cutscene = true;
        PlayerController.Instance.pState.invincible = true;
        PlayerController.Instance.rb.velocity = Vector2.zero;
        //Time.timeScale = 0;
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.In));
        Debug.Log("Fade in");
        PlayerController.Instance.TakeDamage(5);
        yield return new WaitForSeconds(1);
        PlayerController.Instance.transform.position = GameManager.Instance.platformRespawnPoint;
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        Debug.Log("Fade out");
        yield return new WaitForSeconds(UIManager.Instance.sceneFader.fadeTime);
        PlayerController.Instance.pState.cutscene = false;
        PlayerController.Instance.pState.invincible = false;
        //Time.timeScale = 1;

    }
}