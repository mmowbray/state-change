using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	private float m_chargeTime = 0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setChargeTime(float chargeTime){
		m_chargeTime = chargeTime;
	}

	public float getChargeTime(){
		return m_chargeTime;
	}
}
