using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HoloAim : MonoBehaviour
{
    public GameObject holoBlock;
    public GameObject realBlock;
    public GameObject aimer;

    [SerializeField]
    private float m_ScaleSpeed;
    private GameObject highlightedBlock;
    private bool targetting;
    private ParticleSystem chargeEffects;
    private float charge;

    public Text counterText;

    private int blockLimit;
    private int numBlocks;

    void Start()
    {
        holoBlock.SetActive(false);
        targetting = true;
        chargeEffects = gameObject.GetComponentInChildren<ParticleSystem>();
        charge = 0f;
        blockLimit = 100;
        numBlocks = 0;
        SetBlockText();
    }

    void Update()
    {

        if (Input.GetButtonDown("ActivateAiming"))
            targetting = !targetting;

        holoBlock.SetActive(targetting);

        if (Input.GetKey(KeyCode.Mouse0) && numBlocks < blockLimit)
        {
            charge += Time.fixedDeltaTime;
            if (Input.GetKey(KeyCode.Mouse1))
                charge = 0f;
            if (!chargeEffects.isPlaying)
                chargeEffects.Play();
        }

        RaycastHit hit; //where the intersection is in world coords

        if (Physics.Raycast(transform.position, aimer.transform.forward, out hit))
        { //there was a collision with something in the scene

            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Changeable")) //raycast intersected an extrudable wall
            {
                holoBlock.transform.localScale = charge > 0 ? new Vector3(1.0f, 1.0f + charge * m_ScaleSpeed, 1.0f) : Vector3.one;
                holoBlock.transform.position = hit.point;
                holoBlock.transform.rotation = hit.transform.gameObject.transform.rotation;
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("MassBlock"))
            {
                var gazedAtBlock = hit.transform.gameObject.GetComponentInChildren<Block>();
                if (gazedAtBlock)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        Destroy(gazedAtBlock.gameObject);
                        numBlocks--; // decrement the number of blocks on the scene when it is destroyed
                        SetBlockText();
                        return; // if a block is being removed then we are done and don't anything else.
                    }
                    else
                    {
                        gazedAtBlock.alertGazed();
                        holoBlock.SetActive(false); //hide holo block if we are looking at an existing block
                    }

                }

            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && numBlocks < blockLimit)
            {

                GameObject newBlock = Instantiate(realBlock, holoBlock.transform.position, holoBlock.transform.rotation) as GameObject;
                newBlock.transform.localScale = holoBlock.transform.localScale;

                charge = 0; //reset the charge
                numBlocks++; // increase the number of blocks that is on the scene
                chargeEffects.Stop();
                SetBlockText();
            }
        }
    }

    void SetBlockText()
    {
        counterText.text = "Block Limit: " + blockLimit.ToString() + "\n" + "Number of Blocks on Scene: " + numBlocks.ToString();

    }
}
