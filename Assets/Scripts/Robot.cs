using UnityEngine;
using System.Collections;
using RobotStuff;

public class Robot : MonoBehaviour 
{
	public string robotStrategyName = "RobotStrategyA";
	public Transform target;
	public float followRange = 3.0f;
	public float arriveThreshold = 0.05f;
    public bool isAttacking;

	private RobotStrategy _robotStrategy;

    public bool defeatTrigger = false;
    private bool defeated = false;
    private float disappearTimer = 0;

    private Animator mAnimator;

	// Use this for initialization
	void Start() 
	{
		this.SetRobotStrategy(robotStrategyName);
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!defeated)
        {
            _robotStrategy.Update();
            if (isAttacking)
                mAnimator.SetBool("isAttacking", true);
            else mAnimator.SetBool("isAttacking", false);
        }
        if (defeatTrigger && !defeated)
        {
            mAnimator.SetBool("isAttacking", false);
            mAnimator.Stop();
            defeated = true;
            fallApart();

        }
        else if (defeated)
            disappearTimer += Time.deltaTime;
        if (disappearTimer > 5)
        {
            // shrinks from view until invisible, then removes game object
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(0.9f, 0.9f, 0.9f ));
            if (transform.localScale.magnitude < 0.001)
                Destroy(gameObject);
        }

	}

    private void fallApart()
    {
        SphereCollider[] spheres = GetComponentsInChildren<SphereCollider>() as SphereCollider[];
        foreach (SphereCollider s in spheres)
            Component.Destroy(s);

        Rigidbody[] objects = GetComponentsInChildren<Rigidbody>() as Rigidbody[];

        foreach (Rigidbody r in objects)
        {
            r.isKinematic = false;
            r.useGravity = true;
            r.AddExplosionForce(Random.value * 4, transform.position, 3);
        }
    }

	public void SetRobotStrategy(string robotStrategyName)
	{
		switch(robotStrategyName)
		{
			case "RobotStrategyA": 
				this._robotStrategy = new RobotStrategyA(gameObject, target);
				break;	
			case "RobotStrategyB": 
				this._robotStrategy = new RobotStrategyB(gameObject, target);
				break;	
			default :
				this._robotStrategy = new RobotStrategyA(gameObject, target);
				break;	
		}
	}

	void OnTriggerEnter(Collider col)
	{
		_robotStrategy.OnTriggerEnter(col);
	}

	void OnTriggerStay(Collider col)
	{
		_robotStrategy.OnTriggerStay(col);
	}

	void OnTriggerExit(Collider col)
	{
		_robotStrategy.OnTriggerExit(col);
	}
}
