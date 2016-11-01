using UnityEngine;
using System.Collections;

public class HoloAim : MonoBehaviour
{
    private bool targetting;
    private bool blockVisible;
    public GameObject holoBlock;
    public GameObject aimer;
    private PlayerExtension parentPlayer;
    [SerializeField] private float m_ScaleSpeed;
    private GameObject highlightedBlock;
    // Use this for initialization
    void Start ()
    {
        targetting = false;
        blockVisible = false;
        parentPlayer = transform.parent.transform.parent.gameObject.GetComponent<PlayerExtension>();
        //Instantiate(holoBlock, new Vector3(0.0f, -4.0f, 0.0f), this.transform.rotation); //The 0, -3, 0 is just somewhere invisible so we don't have to keep reinstantiating it
    }
	
	// Update is called once per frame
	void Update ()
    {
        RaycastHit hit;
        if (Input.GetButtonUp("ActivateAiming"))
        {
            Debug.Log("Targetting");
            if (!targetting)
                targetting = true;
            else
            {
                targetting = false;
                holoBlock.transform.position = new Vector3(0.0f, -4.0f, 0.0f);
            }
        }

        if (targetting)
        {
            if (Physics.Raycast(transform.position, aimer.transform.forward, out hit) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Changeable")) //draw line from pos in the fwrd direction, store collision info in "hit"
            {
                if (parentPlayer.getCharge() == 0)
                    holoBlock.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                else
                    holoBlock.transform.localScale = new Vector3(1.0f, 1.0f + (parentPlayer.getCharge() * m_ScaleSpeed), 1.0f);
                holoBlock.transform.position = hit.point;
                holoBlock.transform.rotation = hit.transform.gameObject.transform.rotation;
                if (hit.transform.gameObject != highlightedBlock)
                {
                    if (highlightedBlock != null)
                        highlightedBlock.GetComponent<Renderer>().material.color = Color.red;
                    highlightedBlock = null;
                }
            }
            else if (Physics.Raycast(transform.position, aimer.transform.forward, out hit) && hit.transform.gameObject.layer == LayerMask.NameToLayer("MassBlock"))
            {
                hit.transform.gameObject.GetComponent<Renderer>().material.color = Color.green;
                highlightedBlock = hit.transform.gameObject;
                if (hit.transform.gameObject != highlightedBlock)
                {
                    highlightedBlock.GetComponent<Renderer>().material.color = Color.red;
                    highlightedBlock = hit.transform.gameObject;
                }
            }
        }
    }
}
