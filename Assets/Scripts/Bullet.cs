using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	private float m_chargeTime = 0f;

	public void setChargeTime(float chargeTime){
		m_chargeTime = chargeTime;
	}

	public float getChargeTime(){
		return m_chargeTime;
	}
}
