using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class levelLoaderScript : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadLevelIndex(object lvl)
    {
        StartCoroutine(LoadLevel(lvl));
    }

    IEnumerator LoadLevel(object idx)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        if (idx.GetType() == typeof(string)) 
        {
            SceneManager.LoadScene((string)idx); 
        }
        else if (idx.GetType() == typeof(int))
        {
            SceneManager.LoadScene((int)idx);
        }
        else
        {
            Debug.LogError("ERROR: INVALID INDEX TO LOAD LEVEL.\n(Type not STRING nor INT)");
        }

    }
}

