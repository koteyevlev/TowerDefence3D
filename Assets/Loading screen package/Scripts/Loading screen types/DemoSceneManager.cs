using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoSceneManager : MonoBehaviour
{
    private bool _canCheckForInput = false;
    private LoadingScreenManager _loadingManager;

    private void Start()
    {
        _loadingManager = transform.GetComponentInChildren<LoadingScreenManager>();
    }

    private void Update()
    {
        if (_canCheckForInput)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _canCheckForInput = false;
                _loadingManager.RevealLoadingScreen();
            }
        }
    }

    public void OnLoadingScreenRevealed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnLoadingScreenHided()
    {
        _canCheckForInput = true;
    }
}
