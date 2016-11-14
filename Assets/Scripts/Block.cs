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

	void Start(){
		gazedAt = false;
	}

	void Update(){
		if (gazedAt) {
			GetComponent<Renderer>().material.color = Color.green;
			gazedAt = false;
		} else {
			GetComponent<Renderer> ().material.color = Color.red;
		}

	}

	public void StretchBy(float amount){
		transform.parent.localScale += new Vector3 (0, amount * m_ScaleSpeed, 0);
    }

    void finishedExpanding() // called by Animator
    {
        isExpanding = false;
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
