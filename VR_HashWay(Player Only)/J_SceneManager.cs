using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class J_SceneManager : MonoBehaviour
{

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private int _sceneIndex;
    public int SceneIndex
    {
        get { return _sceneIndex; }
        set { _sceneIndex = value; }
    }
    // Use this for initialization
    private void OnEnable()
    {
        SceneManager.LoadScene(1);
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 3)
        {
            this.enabled = false;
        }
    }
}