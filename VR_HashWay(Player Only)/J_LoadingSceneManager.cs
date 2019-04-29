using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class J_LoadingSceneManager : MonoBehaviour
{
    public Slider slider;
    bool IsDone = false;
    float fTime = 0f;
    AsyncOperation async_operation;
    J_SceneManager sceneManager;
    void Start()
    {
        sceneManager = GameObject.Find("J_SceneManager").GetComponent<J_SceneManager>();
        StartCoroutine(StartLoad(sceneManager.SceneIndex));
    }

    void Update()
    {
        fTime += Time.deltaTime;
        slider.value = fTime;

        if (fTime >= 5)
        {
            async_operation.allowSceneActivation = true;
        }
    }

    public IEnumerator StartLoad(int sceneIndex)
    {
        async_operation = SceneManager.LoadSceneAsync(sceneIndex);
        async_operation.allowSceneActivation = false;

        if (IsDone == false)
        {
            IsDone = true;

            while (async_operation.progress < 0.9f)
            {
                slider.value = async_operation.progress;
                yield return true;
            }
        }
    }
}