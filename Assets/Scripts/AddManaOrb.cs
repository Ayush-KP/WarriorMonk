using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddManaOrb : MonoBehaviour
{
    [SerializeField] GameObject particles;
    [SerializeField] GameObject canvasUI;
    [SerializeField] OrbShard orbShards;
    bool used;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerController.Instance.orbShards >= 3)
        {
            //Debug.Log("destroyed");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && !used)
        {
            used = true;
            StartCoroutine(ShowUI());
        }
    }

    IEnumerator ShowUI()
    {
        GameObject _particle = Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(_particle, 0.5f);
        yield return new WaitForSeconds(0.5f);
        canvasUI.SetActive(true);
        orbShards.initialFillAmount = PlayerController.Instance.orbShards * 0.34f;
        PlayerController.Instance.orbShards++;
        orbShards.targetFillAmount = PlayerController.Instance.orbShards * 0.34f;

        StartCoroutine(orbShards.LerpFill());

        yield return new WaitForSeconds(2.5f);
        PlayerController.Instance.unlockedWallJump = true;
        SaveData.Instance.SavePlayerData();
        canvasUI.SetActive(false);
        Destroy(gameObject);
    }
}
