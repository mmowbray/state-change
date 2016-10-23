using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	[SerializeField] private float m_ScaleSpeed;

	void Start(){
		int x = 0;
	}

	void OnMouseDrag(){
		Vector3 stretchY = new Vector3(0,1,0) * 0.5f;
		transform.parent.localScale += stretchY;
	}

	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Bullet") {
			Destroy (col.gameObject);
			Destroy (gameObject);
		}
	}
}
