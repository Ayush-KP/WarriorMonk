using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Threading;
using UnityEngine.Playables;

/*public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] allVirtualCameras;
    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer framingTransposer;

    [Header("Y Damping Setting for player Jump/Fall: ")]
    [SerializeField] private float panAmount = 0.1f;
    [SerializeField] private float panTime = 0.2f;
    public float playerFallSpeedThreshold = -10;
    private float normalYDamp;
    public bool isLerpingYDamp;
    public bool hasLerpingYDamp;

   // private CinemachineTransposer framingTransposer;
    public static CameraManager Instance { get; private set; }
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        for(int i = 0; i < allVirtualCameras.Length; i++)
        {
            if (allVirtualCameras[i].enabled)
            {
                currentCamera = allVirtualCameras[i];
                framingTransposer = currentCamera.GetComponent<CinemachineFramingTransposer>();
                Debug.Log("Framing Transposer: " + framingTransposer);
            }
        }
        normalYDamp = framingTransposer.m_YDamping;
    }
    private void Start()
    {
        for(int i = 0; i < allVirtualCameras.Length; i++)
        {
            allVirtualCameras[i].Follow = PlayerController.Instance.transform;
        }
    }

    public void SwapCamera(CinemachineVirtualCamera _newCam)
    {
        currentCamera.enabled = false;
        currentCamera = _newCam;
        currentCamera.enabled = true;
    }

    public IEnumerator LerpYDamping(bool _isPlayerFalling) 
    {
        isLerpingYDamp = true;
        float _startYDamp = framingTransposer.m_YDamping;
        float _endYDamp = 0;
        if (_isPlayerFalling)
        {
            _endYDamp = panAmount;
            hasLerpingYDamp = true;
        }
        else
        {
            _endYDamp = normalYDamp;
        }
        //lerp panAmount
        float _timer = 0;
        while (_timer < panTime)
        {
            _timer += Time.deltaTime;
            float _lerpedPanAmount = Mathf.Lerp(_startYDamp, _endYDamp, (_timer / panTime));
            framingTransposer.m_YDamping = _lerpedPanAmount;
            yield return null;
        }
        isLerpingYDamp = false;
    }
}
*/


public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] allVirtualCameras;
    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer framingTransposer;

    [Header("Y Damping Setting for player Jump/Fall: ")]
    [SerializeField] private float panAmount = 0.1f;
    [SerializeField] private float panTime = 0.2f;
    public float playerFallSpeedThreshold = -10;
    private float normalYDamp;
    public bool isLerpingYDamp;
    public bool hasLerpingYDamp;

    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ensure the game object persists across scenes
        DontDestroyOnLoad(gameObject);

        // Initialize cameras and transposer
        InitializeCamerasAndTransposer();
    }

    private void InitializeCamerasAndTransposer()
    {
        for (int i = 0; i < allVirtualCameras.Length; i++)
        {
            if (allVirtualCameras[i].enabled)
            {
                currentCamera = allVirtualCameras[i];
                framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

                if (framingTransposer == null)
                {
                    Debug.LogError("CinemachineFramingTransposer component not found on " + currentCamera.name);
                    return;
                }
            }
        }

        if (framingTransposer != null)
        {
            normalYDamp = framingTransposer.m_YDamping;
        }
        else
        {
            Debug.LogError("No enabled virtual camera with a CinemachineFramingTransposer found.");
        }
    }

    private void Start()
    {
        if (PlayerController.Instance == null)
        {
            Debug.LogError("PlayerController instance is null.");
            return;
        }

        for (int i = 0; i < allVirtualCameras.Length; i++)
        {
            allVirtualCameras[i].Follow = PlayerController.Instance.transform;
        }
    }

    public void SwapCamera(CinemachineVirtualCamera _newCam)
    {
        currentCamera.enabled = false;
        currentCamera = _newCam;
        currentCamera.enabled = true;
    }

    public IEnumerator LerpYDamping(bool _isPlayerFalling)
    {
        if (framingTransposer == null)
        {
            Debug.LogError("CinemachineFramingTransposer is not initialized.");
            yield break;
        }

        isLerpingYDamp = true;
        float _startYDamp = framingTransposer.m_YDamping;
        float _endYDamp = _isPlayerFalling ? panAmount : normalYDamp;
        hasLerpingYDamp = _isPlayerFalling;

        float _timer = 0;
        while (_timer < panTime)
        {
            _timer += Time.deltaTime;
            framingTransposer.m_YDamping = Mathf.Lerp(_startYDamp, _endYDamp, _timer / panTime);
            yield return null;
        }
        isLerpingYDamp = false;
    }
}



