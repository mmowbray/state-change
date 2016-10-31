using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	[SerializeField] private float m_ScaleSpeed;

	public Vector3 wallPoint;

	void OnMouseDrag(){
		Vector3 stretchY = new Vector3(0,1,0) * m_ScaleSpeed;
		transform.localScale += stretchY;
	}
}
