using System.Threading.Tasks;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    private Animator _animatorComponent;

    private void Start()
    {
        _animatorComponent = transform.GetComponent<Animator>();
        RevealLoadingScreen();
        // Remove it if you don't want to hide it in the Start function and call it elsewhere
        HideLoadingScreen();
    }

    public void RevealLoadingScreen()
    {
        _animatorComponent.SetTrigger("Reveal");
    }

    public void HideLoadingScreen()
    {
        // Call this function, if you want start hiding the loading screen
        _animatorComponent.SetTrigger("Hide");
    }

    //public void OnFinishedReveal()
    //{
    //    // TODO: remove it and load your own scene !!
    //    transform.parent.GetComponent<DemoSceneManager>().OnLoadingScreenRevealed();
    //}

    //public void OnFinishedHide()
    //{
    //    // TODO: remove it and call your functions 
    //    transform.parent.GetComponent<DemoSceneManager>().OnLoadingScreenHided();
    //}

}
