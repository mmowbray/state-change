using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	[SerializeField] private float m_ScaleSpeed;

	void OnMouseDrag(){
		transform.localScale += transform.forward * m_ScaleSpeed;
	}
}
