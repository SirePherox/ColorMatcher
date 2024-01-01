using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : SingletonCreator<SceneLoader>
{
    [Header("Variables")]
    private float sceneLoadDelay = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator LoadSceneAsync(int sceneIndex)
    {
        yield return new WaitForSeconds(sceneLoadDelay);

       AsyncOperation asyncOp =  SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncOp.isDone)
        {
            Debug.Log(asyncOp.progress);
            yield return null;
        }
    }

    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public void ReloadCurrentScenee()
    {
        SceneManager.LoadScene(GetCurrentSceneIndex());
    }
}
