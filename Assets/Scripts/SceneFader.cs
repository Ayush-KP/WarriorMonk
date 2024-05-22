using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField] public float fadeTime;

    private Image fadeOutUIImage;
    public enum FadeDirection
    {
        In,
        Out
    }
    // Start is called before the first frame update
    void Start()
    {
        fadeOutUIImage = GetComponent<Image>();
        fadeOutUIImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallFadeAndLoadScene(string _sceneToLoad)
    {
        StartCoroutine(FadeAndLoadScene(FadeDirection.In, _sceneToLoad));
    }
    public IEnumerator Fade(FadeDirection _fadeDirection)
    {
        float _alpha = _fadeDirection ==  FadeDirection.Out? 1 : 0;
        float _fadeEndValue = _fadeDirection == FadeDirection.Out ? 0 : 1;
        
        if (_fadeDirection == FadeDirection.Out)
        {
            fadeOutUIImage.enabled = false;
            Debug.Log("Outbound");
            while (_alpha >= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDirection);
                yield return null;
            }
            
            Debug.Log("Image Disabled");
        }
        else
        {
            Debug.Log("Inbound");
            fadeOutUIImage.enabled = true;
            while (_alpha <= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDirection);
                yield return null;
            }
        }
    }

    public IEnumerator FadeAndLoadScene(FadeDirection _fadeDirection, string _sceneToLoad)
    {
        fadeOutUIImage.enabled = true;
       // Debug.Log("Fade Direction " + _fadeDirection);
        yield return Fade(_fadeDirection);
        SceneManager.LoadScene(_sceneToLoad);
        fadeOutUIImage.enabled = false;
    }
    void SetColorImage(ref float _alpha, FadeDirection _fadeDirection)
    {
        fadeOutUIImage.color = new Color(fadeOutUIImage.color.r, fadeOutUIImage.color.g, fadeOutUIImage.color.b, _alpha);
        _alpha += Time.deltaTime * (1/fadeTime) * (_fadeDirection == FadeDirection.Out ? -1 : 1);
    }
}
