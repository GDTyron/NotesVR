using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    //public static PauseUI Instance { get; private set; }
    HandPresence RightHandScript;
    HandPresence LeftHandScript;
    public GameObject RightHand;
    public GameObject LeftHand;

    [SerializeField]
    private GameObject pauseMenuUI = null;
    [SerializeField]
    private GameObject optionsMenuUI = null;
    [SerializeField]
    private bool isPaused;
    [SerializeField]
    private bool wasPressed;
    [SerializeField]
    private bool isInOptions;
    /*
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    */
    void Start()
    {
        RightHandScript = RightHand.GetComponent<HandPresence>();
        LeftHandScript = LeftHand.GetComponent<HandPresence>();
    }

    void Update()
    {
        //UnityEngine.Debug.Log("Pause says " + LeftHandScript.getPressedPrimary() || RightHandScript.getPressedPrimary());

        if (RightHandScript.getPressedPrimary() || LeftHandScript.getPressedPrimary())
        {
            if (!wasPressed)
            {
                //UnityEngine.Debug.Log("Is now pausing with " + isPaused);
                if (isPaused)
                {
                    isPaused = false;
                }
                else
                {
                    isPaused = true;
                }
            }
        }
        wasPressed = RightHandScript.getPressedPrimary() || LeftHandScript.getPressedPrimary();


        if (isPaused)
        {
            ActivateMenu();
        }
        else
        {
            DeactivateMenu();
        }
    }

    void ActivateMenu()
    {
        if(!isInOptions)
        {
            pauseMenuUI.SetActive(true);
        }
        //Time.timeScale = 0;
        //AudioListener.pause = true;
    }

    public void DeactivateMenu()
    {
        isInOptions = false;
        //Time.timeScale = 1;
        //AudioListener.pause = false;
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        isPaused = false;
    }

    public void GoToOptionsFromMenu()
    {
        isInOptions = true;
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
        isPaused = true;
    }

    public void GoToMenuFromOptions()
    {
        isInOptions = false;
        pauseMenuUI.SetActive(true);
        optionsMenuUI.SetActive(false);
        isPaused = true;
    }
}
