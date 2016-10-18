using UnityEngine;
using System.Collections;

public class WallCreator : MonoBehaviour {

	[SerializeField] GameObject m_Block;
	[SerializeField] float m_BlockSpeed;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter(Collision other) {
		print("Points colliding: " + other.contacts.Length);
		print("First point that collided: " + other.contacts[0].point);

		GameObject blockCopy = Instantiate(m_Block);
		blockCopy.transform.position = other.contacts[0].point;

		blockCopy.GetComponent<Rigidbody>().AddForce(transform.up * m_BlockSpeed);
		// destroying the bullet
		Destroy(other.gameObject);
		Destroy (blockCopy, 5f);
	}
}
