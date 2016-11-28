using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour
{
    private GameObject player;
    public float openDist;
    public string level;

    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= openDist)
        {
            Debug.Log("Loading");
            SceneManager.LoadScene(level, LoadSceneMode.Single);
        }

    }
}
