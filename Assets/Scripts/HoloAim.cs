using UnityEngine;
using System.Collections;

public class HoloAim : MonoBehaviour
{
    private bool targetting;
    private bool blockVisible;
    public GameObject holoBlock;
    public GameObject massGun;

	// Use this for initialization
	void Start ()
    {
        targetting = false;
        blockVisible = false;
        //Instantiate(holoBlock, new Vector3(0.0f, -4.0f, 0.0f), this.transform.rotation); //The 0, -3, 0 is just somewhere invisible so we don't have to keep reinstantiating it
    }
	
	// Update is called once per frame
	void Update ()
    {
        RaycastHit hit;
        if (Input.GetButtonUp("ActivateAiming"))
        {
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
            if (Physics.Raycast(transform.position, massGun.transform.forward, out hit) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Changeable")) //draw line from pos in the fwrd direction, store collision info in "hit"
            {
                holoBlock.transform.position = hit.point;
                holoBlock.transform.rotation = hit.transform.gameObject.transform.rotation;
            }
        }
    }
}
