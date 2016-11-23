using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
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
	private RaycastHit frontOfHitGameObject;
	private bool targetting;

	private float charge;
	private bool ignoreMouse0KeyUp; // this was needed to ignore the mouse up when it was used to delete blocks
	private bool isPuzzleMode = true;
	private int numBlocks;
	private List<GameObject> blocksList;

    [SerializeField] private AudioSource shootSound;
    [SerializeField] private AudioSource switchSound;
    [SerializeField] private AudioSource retractSound;
    [SerializeField] private AudioSource chargeSound;

    void Start()
    {
        holoBlock.SetActive(false);
        targetting = true;
        chargeEffects = gameObject.GetComponentInChildren<ParticleSystem>();
        charge = 0f;
        //blockLimit = 10;
        numBlocks = 1;
        SetBlockText();
		blocksList = new List<GameObject>();
    }

	void Update()
	{

        if (Input.GetButtonDown("ActivateAiming"))
        {
            targetting = !targetting;
            switchSound.Play();
        }

		holoBlock.SetActive (targetting);

		if (Physics.Raycast (transform.parent.position, transform.parent.forward, out hit)) { //there was a collision with something in the scene

			//update block growth if charge > 0 or we are looking at an extrudable wall
			if (charge > 0 || hit.transform.gameObject.layer == LayerMask.NameToLayer("Changeable")) //raycast intersected an extrudable wall
			{
				Physics.Raycast (hit.point, hit.normal, out frontOfHitGameObject); // find object in front of the "hit" object
				float blockMaxYScale = frontOfHitGameObject != null? frontOfHitGameObject.distance: float.MaxValue; // in case nothing is found in front of the "hit" object, set blockMaxYValue to float.MaxValue. in other words, the scale will have no limit
				float blockYScale;
				if (isPuzzleMode) {
					blockYScale = m_FixedLength > blockMaxYScale? blockMaxYScale : m_FixedLength;
					holoBlock.transform.localScale = new Vector3 (1.0f, blockYScale, 1.0f);

				} else{
					ChargeGun (); // get charge value
					blockYScale = (1.0f + (charge * m_ScaleSpeed)) > blockMaxYScale ? blockMaxYScale : (1.0f + (charge * m_ScaleSpeed)) ; // make sure the block length doesn't exceed the limit
					holoBlock.transform.localScale = charge > 0 ? new Vector3 (1.0f, blockYScale, 1.0f) : Vector3.one;
				}

				//keep the position and rotation fixed when we begin charging
				if (charge == 0) {
					holoBlock.transform.position = hit.point;
					holoBlock.transform.rotation = hit.transform.gameObject.transform.rotation;
				}
			}
			else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("MassBlock"))
			{
				var gazedAtBlock = hit.transform.gameObject.GetComponentInChildren<Block> ();
				if (gazedAtBlock && charge == 0) { //we are looking at a block and we are not currently charging up
					if (CrossPlatformInputManager.GetButtonDown("Fire1")) {
						GameObject blockToDelete = gazedAtBlock.gameObject.transform.parent.gameObject;
						blocksList.Remove (blockToDelete);
						Destroy (blockToDelete);
                        ignoreMouse0KeyUp = true; // ignore one Mouse0 KeyUp event. This is to prevent block creation
						numBlocks = blocksList.Count; // decrement the number of blocks on the scene when it is destroyed
						SetBlockText();
						charge = 0; //reset the charge
                        retractSound.Play();
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
		if (CrossPlatformInputManager.GetButtonDown("DeleteAllBlocks")) {
			int totalBlocks = blocksList.Count;
			for(int x = 0; x < totalBlocks; x++){
				GameObject blockToDestroy = blocksList [0];
				if (blocksList.Remove (blockToDestroy)) { // if block was found and removed
					Destroy (blockToDestroy);
				}
			}
			numBlocks = blocksList.Count;
			SetBlockText();
            retractSound.Play();
        }
	}

	void DeletePreviousBlock(){
		if (CrossPlatformInputManager.GetButtonDown("DeletePreviousBlock")) {
			GameObject blockToDestroy = blocksList [0];
			if (blocksList.Remove (blockToDestroy)) { // if block was found and removed
				Destroy (blockToDestroy);
                retractSound.Play();
            }
		}
		numBlocks = blocksList.Count;
		SetBlockText();
    }

	void createBlock(){
		
		if (CrossPlatformInputManager.GetButtonUp("Fire1") && numBlocks < blockLimit && ignoreMouse0KeyUp == false) {

			GameObject newBlock = Instantiate (realBlock, holoBlock.transform.position, holoBlock.transform.rotation) as GameObject;
			newBlock.transform.localScale = holoBlock.transform.localScale;
			blocksList.Add (newBlock);
			charge = 0; //reset the charge
			numBlocks = blocksList.Count;
			SetBlockText();
			chargeEffects.Stop ();
            chargeSound.Stop();
            shootSound.Play();
		}else if (CrossPlatformInputManager.GetButtonUp("Fire1")) {
			ignoreMouse0KeyUp = false;
		}

	}

	void ChargeGun(){

		if (CrossPlatformInputManager.GetButton("Fire1") && ignoreMouse0KeyUp == false && numBlocks < blockLimit) {
			charge += Time.fixedDeltaTime;

			if (Input.GetKey (KeyCode.Mouse1)) {
				charge = 0f;
			}

			if (!chargeEffects.isPlaying) {
				chargeEffects.Play ();
                chargeSound.Play();
            }
		}

	}

	void GunMode (){
		if (CrossPlatformInputManager.GetButtonDown("GunMode")) {
			isPuzzleMode = !isPuzzleMode;
            switchSound.Play();
        }
	}

	void SetBlockText()
	{
		counterText.text = "Block Limit: " + blockLimit.ToString() + "\n" + "Number of Blocks on Scene: " + numBlocks.ToString();

	}

}
