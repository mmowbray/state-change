using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	[SerializeField] private float m_ScaleSpeed;
	public Vector3 wallPoint;
	private bool gazedAt;

	void Start(){
		gazedAt = false;
	}

	void Update(){
		if (gazedAt) {
			GetComponent<Renderer>().material.color = Color.green;
			gazedAt = false;
		} else {
			GetComponent<Renderer> ().material.color = Color.red;
		}
	}

	public void StretchBy(float amount){
		transform.parent.localScale += new Vector3 (0, amount * m_ScaleSpeed, 0);
	}

	void OnCollisionEnter(Collision col){
		Debug.Log("Bullet");
		if (col.gameObject.tag == "Bullet") {
			Destroy (col.gameObject);
			Destroy (gameObject);
		}
	}

	public void alertGazed()
	{
		this.gazedAt = true;
	}
}
