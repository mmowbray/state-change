using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	[SerializeField] private float m_ScaleSpeed;

<<<<<<< HEAD
	void Start(){
	}

	public void StretchBy(float amount){
		transform.parent.localScale += new Vector3 (0, amount * m_ScaleSpeed, 0);
	}
=======
	public Vector3 wallPoint;
>>>>>>> master

	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Bullet") {
			Destroy (col.gameObject);
			Destroy (gameObject);
		}
	}
}
