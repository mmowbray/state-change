using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	[SerializeField] private float m_ScaleSpeed;
	public Vector3 wallPoint;
	private bool gazedAt;
    private bool isExpanding = true; // on creation, so we know it can harm enemies
    public bool IsExpanding
    {
        get { return isExpanding; }
    }
    private Animator anim;

	void Start(){
		gazedAt = false;
        anim = GetComponent<Animator>();
	}

	void Update(){
		if (gazedAt) {
			GetComponent<Renderer>().material.color = Color.green;
			gazedAt = false;
		} else {
			GetComponent<Renderer> ().material.color = Color.red;
		}
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Expand On Creation") || anim.GetCurrentAnimatorStateInfo(0).IsName("RemovalShrink"))
        {

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("RemovalShrink"))
                foreach (SpriteRenderer sp in GetComponentsInChildren<SpriteRenderer>())
                {
                    sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 1 - anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
                }
            else if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.75)
                foreach (SpriteRenderer sp in GetComponentsInChildren<SpriteRenderer>())
                {
                    sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, (anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.75f) / 0.25f );
                }
            else
                foreach (SpriteRenderer sp in GetComponentsInChildren<SpriteRenderer>())
                {
                    sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 0);
                }
        }

	}

	public void StretchBy(float amount){
		transform.parent.localScale += new Vector3 (0, amount * m_ScaleSpeed, 0);
    }

    void finishedExpanding() // called by Animator
    {
        isExpanding = false;
    }

    void finishedShrinking() // called by Animator
    {
        FindObjectOfType<HoloAim>().alertBlockDisappeared(transform.parent.gameObject);
        Destroy(transform.parent.gameObject);
    }

	void OnCollisionEnter(Collision col){
		Debug.Log("Bullet");
		if (col.gameObject.tag == "Bullet") {
			Destroy (col.gameObject);
			Destroy (gameObject);
		}
	}

    public void alertGazed()
	{
		this.gazedAt = true;
	}
}
