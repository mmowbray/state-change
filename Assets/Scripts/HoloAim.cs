﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HoloAim : MonoBehaviour
{
	[SerializeField] private float m_ScaleSpeed;
	[SerializeField] float m_FixedLength;
	[SerializeField] int blockLimit;

    public GameObject holoBlock;
    public GameObject realBlock;
    public GameObject aimer;
	public Text counterText;

    private GameObject highlightedBlock;
	private ParticleSystem chargeEffects;
	private RaycastHit hit; //where the intersection is in world coords
	private bool targetting;

	private float charge;
	private bool ignoreMouse0KeyUp;
	private bool isPuzzleMode = false;
	private int numBlocks;

    void Start()
    {
        holoBlock.SetActive(false);
        targetting = true;
        chargeEffects = gameObject.GetComponentInChildren<ParticleSystem>();
        charge = 0f;
        blockLimit = 10;
        numBlocks = 0;
        SetBlockText();
    }

	void Update()
	{
		GunMode (); 

		if (Input.GetButtonDown ("ActivateAiming"))
			targetting = !targetting;

		holoBlock.SetActive (targetting);

		ChargeGun ();


		if (Physics.Raycast (transform.position, aimer.transform.forward, out hit)) { //there was a collision with something in the scene

			if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Changeable")) //raycast intersected an extrudable wall
			{
				if (isPuzzleMode == false) {
					holoBlock.transform.localScale = charge > 0 ? new Vector3 (1.0f, 1.0f + charge * m_ScaleSpeed, 1.0f) : Vector3.one;
				} else {
					holoBlock.transform.localScale = new Vector3 (1.0f,m_FixedLength , 1.0f);
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
						ignoreMouse0KeyUp = true; // ignore one Mouse0 KeyUp event. This is to prevent block creation
						numBlocks--; // decrement the number of blocks on the scene when it is destroyed
						SetBlockText();
					}
					else {
						gazedAtBlock.alertGazed ();
						holoBlock.SetActive (false); //hide holo block if we are looking at an existing block
					}

				}
					
			}

			createBlock ();

		}
    }

	void createBlock(){
		
		if (Input.GetKeyUp (KeyCode.Mouse0) && numBlocks < blockLimit) {
			if (ignoreMouse0KeyUp == true) { // Mouse0 KeyUp needs to be ignored if Mouse0 was pressed to delete the block
				ignoreMouse0KeyUp = false; 
			} else {
				GameObject newBlock = Instantiate (realBlock, holoBlock.transform.position, holoBlock.transform.rotation) as GameObject;
				newBlock.transform.localScale = holoBlock.transform.localScale;

				charge = 0; //reset the charge
				numBlocks++;
				SetBlockText();
				chargeEffects.Stop ();
			}
		}
	}

	void ChargeGun(){
		if (Input.GetKey (KeyCode.Mouse0)) {
			charge += Time.fixedDeltaTime;

			if (Input.GetKey (KeyCode.Mouse1)) {
				charge = 0f;
			}

			if (!chargeEffects.isPlaying) {
				chargeEffects.Play ();
			}
		}
	}

	void GunMode (){
		if (Input.GetKeyDown (KeyCode.Q)) {
			isPuzzleMode = !isPuzzleMode;
		}
	}

	void SetBlockText()
	{
		counterText.text = "Block Limit: " + blockLimit.ToString() + "\n" + "Number of Blocks on Scene: " + numBlocks.ToString();

	}

}
