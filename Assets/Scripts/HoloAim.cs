using UnityEngine;
using System.Collections;

public class HoloAim : MonoBehaviour
{
    public GameObject holoBlock;
	public GameObject realBlock;
    public GameObject aimer;

    [SerializeField] private float m_ScaleSpeed;
	[SerializeField] float m_fixedLength;
    private GameObject highlightedBlock;
	private bool targetting;
	private ParticleSystem chargeEffects;
	private float charge;
	private bool isPuzzleMode = false;
    
    void Start ()
    {
		holoBlock.SetActive (false);
		targetting = true;
		chargeEffects = gameObject.GetComponentInChildren<ParticleSystem> ();
		charge = 0f;
    }

	void Update()
	{
		GunMode ();
		if (Input.GetButtonDown ("ActivateAiming"))
			targetting = !targetting;

		holoBlock.SetActive (targetting);

		if (Input.GetKey (KeyCode.Mouse0)) {
			charge += Time.fixedDeltaTime;

			if (Input.GetKey (KeyCode.Mouse1)) {
				charge = 0f;
			}

			if (!chargeEffects.isPlaying) {
				chargeEffects.Play ();
			}
		}

		RaycastHit hit; //where the intersection is in world coords

		if (Physics.Raycast (transform.position, aimer.transform.forward, out hit)) { //there was a collision with something in the scene

			if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Changeable")) //raycast intersected an extrudable wall
			{
				if (isPuzzleMode == false) {
					holoBlock.transform.localScale = charge > 0 ? new Vector3 (1.0f, 1.0f + charge * m_ScaleSpeed, 1.0f) : Vector3.one;
				} else {
					holoBlock.transform.localScale = new Vector3 (1.0f,m_fixedLength , 1.0f);
				}
				holoBlock.transform.position = hit.point;
				holoBlock.transform.rotation = hit.transform.gameObject.transform.rotation;
			}
			else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("MassBlock"))
			{
				var gazedAtBlock = hit.transform.gameObject.GetComponentInChildren<Block> ();
				if (gazedAtBlock) {
					if (Input.GetKeyDown (KeyCode.Mouse0)) {
						Destroy (gazedAtBlock.gameObject);
						return; // if a block is being removed then we are done and don't anything else.
					}
					else {
						gazedAtBlock.alertGazed ();
						holoBlock.SetActive (false); //hide holo block if we are looking at an existing block
					}

				}
					
			}

			if (Input.GetKeyDown (KeyCode.Mouse0)) {

				GameObject newBlock = Instantiate (realBlock, holoBlock.transform.position, holoBlock.transform.rotation) as GameObject;
				newBlock.transform.localScale = holoBlock.transform.localScale;

				charge = 0; //reset the charge
				chargeEffects.Stop();
			}
		}
    }

	void GunMode (){
		if (Input.GetKeyDown (KeyCode.Q)) {
			isPuzzleMode = !isPuzzleMode;
		}
	}
}
