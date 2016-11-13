using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

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
	private bool ignoreMouse0KeyUp; // this was needed to ignore the mouse up when it was used to delete blocks
	private bool chargeMassGun = true;
	private bool isPuzzleMode = false;
	private int numBlocks;

	private List<GameObject> blocksList;

    void Start()
    {
        holoBlock.SetActive(false);
        targetting = true;
        chargeEffects = gameObject.GetComponentInChildren<ParticleSystem>();
        charge = 0f;
        blockLimit = 10;
        numBlocks = 0;
        SetBlockText();
		blocksList = new List<GameObject>();
    }

	void Update()
	{

		if (Input.GetButtonDown ("ActivateAiming"))
			targetting = !targetting;

		holoBlock.SetActive (targetting);

		if (Physics.Raycast (transform.position, aimer.transform.forward, out hit)) { //there was a collision with something in the scene


			if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Changeable")) //raycast intersected an extrudable wall
			{
				
				if (isPuzzleMode == false) {
					ChargeGun ();
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
						GameObject blockToDelete = gazedAtBlock.gameObject.transform.parent.gameObject;
						blocksList.Remove (blockToDelete);
						Destroy (blockToDelete);
						ignoreMouse0KeyUp = true; // ignore one Mouse0 KeyUp event. This is to prevent block creation
						chargeMassGun = false;
						numBlocks = blocksList.Count; // decrement the number of blocks on the scene when it is destroyed
						SetBlockText();
						charge = 0; //reset the charge

					}
					else {
						gazedAtBlock.alertGazed ();
						holoBlock.SetActive (false); //hide holo block if we are looking at an existing block
					}

				}

			}

			createBlock ();

			PlayerKeyOptions ();
		}
    }

	void PlayerKeyOptions(){
		GunMode ();
		DeleteAllBlocks ();
		DeletePreviousBlock ();
	}

	void DeleteAllBlocks(){
		if (Input.GetKeyDown (KeyCode.K)) {
			int totalBlocks = blocksList.Count;
			for(int x = 0; x < totalBlocks; x++){
				GameObject blockToDestroy = blocksList [0];
				if (blocksList.Remove (blockToDestroy)) { // if block was found and removed
					Destroy (blockToDestroy);
				}
			}
			numBlocks = blocksList.Count;
			SetBlockText();
		}
	}

	void DeletePreviousBlock(){
		if (Input.GetKeyDown (KeyCode.J)) {
			GameObject blockToDestroy = blocksList [0];
			if (blocksList.Remove (blockToDestroy)) { // if block was found and removed
				Destroy (blockToDestroy);
			}
		}
		numBlocks = blocksList.Count;
		SetBlockText();
	}

	void createBlock(){
		
		if (Input.GetKeyUp (KeyCode.Mouse0) && numBlocks < blockLimit && ignoreMouse0KeyUp == false) {

			GameObject newBlock = Instantiate (realBlock, holoBlock.transform.position, holoBlock.transform.rotation) as GameObject;
			newBlock.transform.localScale = holoBlock.transform.localScale;
			blocksList.Add (newBlock);
			charge = 0; //reset the charge
			numBlocks = blocksList.Count;
			SetBlockText();
			chargeEffects.Stop ();
		}else if (Input.GetKeyUp (KeyCode.Mouse0)) {
			ignoreMouse0KeyUp = false;
		}

	}

	void ChargeGun(){

		if (Input.GetKey (KeyCode.Mouse0) && ignoreMouse0KeyUp == false && numBlocks < blockLimit) {
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
