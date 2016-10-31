using UnityEngine;
using System.Collections;

public class PlayerExtension : MonoBehaviour {
	[SerializeField] private GameObject m_BulletPrefab;
	[SerializeField] private Transform m_BulletSpawnLocation;
	[SerializeField] private float m_BulletSpeed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			GameObject bulletCopy = Instantiate (m_BulletPrefab) as GameObject;
			bulletCopy.transform.position = m_BulletSpawnLocation.position;
			bulletCopy.GetComponent<Rigidbody> ().velocity = m_BulletSpawnLocation.forward * m_BulletSpeed;
		}
	}
}
