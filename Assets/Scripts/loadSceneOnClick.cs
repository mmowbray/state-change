using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class loadSceneOnClick : MonoBehaviour {

public void LoadByIndex(int sceneIndex)
    {
        SceneManager.LoadScene("Level1");
    }
}
