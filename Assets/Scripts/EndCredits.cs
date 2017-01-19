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
		if (transform.position.y <= 1950.0f) {
			transform.Translate(Vector3.up * Time.deltaTime * 100.0f);
		}
	}

	public IEnumerator end()
	{
		yield return new WaitForSeconds(25.0f);
		SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
	}
}
