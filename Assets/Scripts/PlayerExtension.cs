using UnityEngine;
using System.Collections;

public class PlayerExtension : MonoBehaviour {
	[SerializeField] private GameObject m_BulletPrefab;
	[SerializeField] private Transform m_BulletSpawnLocation;
	[SerializeField] private float m_BulletSpeed;

	private float m_BulletChargeTime = 0f;
	private ParticleSystem m_chargeEffets;

	// Use this for initialization
	void Start () {
		m_chargeEffets = gameObject.GetComponentInChildren<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		// calculate bullet charge time
		if (Input.GetKey(KeyCode.Mouse0)) {
			
			m_BulletChargeTime += Time.deltaTime;

			if (!m_chargeEffets.isPlaying) {
				m_chargeEffets.Play ();
			}

			if(Input.GetKey(KeyCode.Mouse1))
				m_BulletChargeTime = 0; //reset the charge-up on right-click
		}

		if (Input.GetKeyUp(KeyCode.Mouse0)) { 	// at release charge, launch bullet
			
			GameObject bulletCopy = Instantiate (m_BulletPrefab) as GameObject;
			bulletCopy.transform.position = m_BulletSpawnLocation.position;
			bulletCopy.GetComponent<Rigidbody> ().velocity = m_BulletSpawnLocation.forward * m_BulletSpeed;
			bulletCopy.GetComponent<Bullet> ().setChargeTime (m_BulletChargeTime);
			m_BulletChargeTime = 0f; // reset charge time
			m_chargeEffets.Stop();
		}
	}

    public float getCharge()
    {
        return m_BulletChargeTime;
    }
}
