/* JR AM
 * Clash for Crust Scene Controller
 * 01-21-24
 */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControl : MonoBehaviour
{
    private static SceneControl instance;
    public static SceneControl Instance { get { return instance; } }

    private bool firstLoad = true;
    public bool loading;
    [SerializeField] private string currentSceneName;
    private int currentSceneIndex;

    [SerializeField] private List<string> sceneNames;

    [SerializeField] private GameObject loadingScreen;

    [SerializeField] private Animator curtainAnim;
    [SerializeField] private Animation curtainAnimation;
    [SerializeField] private Image curtainImg;

    [SerializeField] private GameObject openCurtainGO1;
    [SerializeField] private GameObject openCurtainGO2;

    [SerializeField] private GameObject closeCurtainGO1;
    [SerializeField] private GameObject closeCurtainGO2;

    [SerializeField] private Animator openCurtainAnimator1;
    [SerializeField] private Animator openCurtainAnimator2;

    [SerializeField] private Animator closeCurtainAnimator1;
    [SerializeField] private Animator closeCurtainAnimator2;

    [SerializeField] private string prevScene;

    [SerializeField] private AudioListener listener;


    private void Awake()
    {
        if (!instance)
            instance = this;

        //Debug.Log("Scene Control Awake");
        StartNewGame();
        //currentSceneName = "Menu";
    }

    private void StartNewGame()
    {
        currentSceneIndex = 0;
        StartCoroutine(LoadScene(sceneNames[0]));
    }

    public void LoadAScene(int index, string name)
    {
        currentSceneIndex = index;
        prevScene = currentSceneName;
        StartCoroutine(LoadScene(name));
    }

    public void LoadScene(int index)
    {
        StartCoroutine(LoadScene(sceneNames[index]));
    }

    public string GetPrevScene()
    {
        return prevScene;
    }

    IEnumerator LoadScene(string sceneName)
    {
        loading = true; //change to true

        if (string.IsNullOrEmpty(currentSceneName))
            loading = false;


        //Debug.Log("Current scene name: " + currentSceneName);
        prevScene = currentSceneName;

        if (loading)
        {
            listener.enabled = false;
            loadingScreen.gameObject.SetActive(true);

            if (!firstLoad)
            {
                closeCurtainGO1.gameObject.SetActive(true);
                closeCurtainGO2.gameObject.SetActive(true);

                closeCurtainAnimator1.Play("Curtain");
                closeCurtainAnimator2.Play("Curtain");
                yield return new WaitWhile(() => closeCurtainAnimator1.GetCurrentAnimatorStateInfo(0).IsName("Curtain"));
            }
        }
            

        //Checking to see if current scene is loaded
        if (!string.IsNullOrEmpty(currentSceneName))
        {
            //Unload current scene
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentSceneName);

            
            //waiting for unloading to finish
            while (!asyncUnload.isDone)
            {
                yield return null;
            }

        }

        //Debug.Log("Before the async load");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        //Debug.Log("After the async load");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        //Turn loading screen off here
        currentSceneName = sceneName;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName)); //Set current active scene to the new scene

        //Loading ends
        loading = false;

        if (!loading)
        {
            if (!firstLoad)
            {
                listener.enabled = true;
                loadingScreen.gameObject.SetActive(false);
                closeCurtainGO1.gameObject.SetActive(false);
                closeCurtainGO2.gameObject.SetActive(false);
                openCurtainGO1.gameObject.SetActive(true);
                openCurtainGO2.gameObject.SetActive(true);
                openCurtainAnimator1.Play("Curtain");
            }

            if (firstLoad)
                firstLoad = false;
        }

        yield return new WaitWhile(() => openCurtainAnimator1.GetCurrentAnimatorStateInfo(0).IsName("Curtain"));
        openCurtainGO1.SetActive(false);
        openCurtainGO2.SetActive(false);


    }

    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }

    public void LoadSceneFromName(string sceneName)
    {
        bool sceneExists = false;
        
        for (int i = 0; i < sceneNames.Count; i++)
        {
            if (sceneName == sceneNames[i]) 
            {
                sceneExists = true;
                currentSceneIndex = i;
            }
        }

        if (sceneExists)
        {
            StartCoroutine(LoadScene(sceneName));
        }
    }
}
