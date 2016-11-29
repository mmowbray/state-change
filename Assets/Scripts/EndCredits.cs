using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndCredits : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(end());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator end()
	{
		yield return new WaitForSeconds(10.0f);
		SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
	}
}
