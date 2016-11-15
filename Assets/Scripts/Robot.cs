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
    public float minimumForceToDestroy = 100;
    public float minimumForceToStun = 10;

	private RobotStrategy _robotStrategy;

    //public bool defeatTrigger = false;
    private bool isDefeated = false;
    private float disappearTimer = 0;
    public float defeatDisappearanceDelayTime = 5;
    public bool isReeling = false;
    private float timeSpentReeling = 0;
    private float timeToSpendReeling;
    private float startingYPos = 0;
    private bool inContactWithSomething = false;
    private bool safeContactOccurred = false;

    public GameObject DizzyEffectPrefab;
    private GameObject dizzyEffect;

    private Animator mAnimator;
    private System.Collections.Generic.List<Transform> pieces;// for falling apart and removal of body parts after

    [SerializeField] AudioSource destroySound;
    bool alreadyDestroyed = false;
    [SerializeField] AudioSource errorNoise;
    [SerializeField] AudioSource sawNoise;

    // Use this for initialization
    void Start() 
	{
		this.SetRobotStrategy(robotStrategyName);
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDefeated && !isReeling)
        {
            _robotStrategy.Update();
            {
                if (isAttacking) // currently only set in inspector, needs to be switched on/off by navigation logic when in attack range
                {
                    sawNoise.Play();
                    GetComponent<NavMeshAgent>().enabled = false;
                    mAnimator.SetBool("isAttacking", true);
                    Collider[] cols = GetComponentsInChildren<Collider>();
                    foreach (Collider col in cols)
                        if (col.gameObject.tag == "Damaging Component")
                            col.enabled = true;
                }
                else
                {
                    if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Standard Pose"))
                        mAnimator.applyRootMotion = true;
                    else
                        mAnimator.SetBool("isAttacking", false);
                    Collider[] cols = GetComponentsInChildren<Collider>();
                    foreach (Collider col in cols)
                        if (col.gameObject.tag == "Damaging Component")
                            col.enabled = false;
                }

            }
        }
        else if (!isDefeated && isReeling)
        {
            isAttacking = false;
            if(inContactWithSomething)
                timeSpentReeling += Time.deltaTime; //only recovers from stun when on something
            if (timeSpentReeling >= timeToSpendReeling)
            {
                Destroy(dizzyEffect);
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;

                transform.up = Vector3.MoveTowards(transform.up, Vector3.up, 0.1f);
                
                
                // *** below might cause issues when robot is knocked onto another floor, fine if room is flat
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, startingYPos, transform.position.z), 0.1f);

                if (Mathf.Approximately(transform.rotation.eulerAngles.x, 0) && Mathf.Approximately(transform.rotation.eulerAngles.z, 0) && Mathf.Abs(transform.position.y - startingYPos) <= 0.5f)
                {
                    mAnimator.applyRootMotion = false;
                    GetComponent<NavMeshAgent>().enabled = true;
                    startingYPos = 0;
                    isReeling = false;
                    CorrectPosture();

                    mAnimator.SetBool("isStunned", false);

                    timeToSpendReeling = timeSpentReeling = 0;
                    Debug.Log(gameObject.name + "'s back to business!");
                }
            }
        }
        /* 

        ***uncomment this block to allow on-click-defeat from inspector***
        ***also uncomment the public variable defeatTrigger***

        //if (defeatTrigger && !isDefeated) // makes robot fall apart from inspector on command
            {
                isAttacking = false;
                GetComponent<NavMeshAgent>().enabled = false;
                mAnimator.SetBool("isAttacking", false);
                mAnimator.Stop();

                isDefeated = true;
                fallApart(null);
            }
            */
        //if above block is uncommented, change below line to 'else if'
        if (isDefeated)
        {
            disappearTimer += Time.deltaTime;
            if (!destroySound.isPlaying && !alreadyDestroyed)
            {
                alreadyDestroyed = true;
                destroySound.Play();
            }
        }
        if (disappearTimer > defeatDisappearanceDelayTime)
        {
            // shrinks from view until invisible, then removes game object
            // shrink applied piece by piece, otherwise shrinking animation has all pieces cluster together
            Transform[] pieceArray = pieces.ToArray() as Transform[];
            foreach (Transform piece in pieceArray)
            {
                if (piece != null)
                {
                    if (piece != transform) // to make sure "this" is deleted last so script can complete first
                        piece.localScale = Vector3.Scale(piece.localScale, new Vector3(0.9f, 0.9f, 0.9f));
                    // Destroys each part's GameObject when sufficiently small. 
                    if (piece.localScale.magnitude < 0.001 || pieces.Count == 1) 
                    {
                        Destroy(piece.gameObject);
                        pieces.Remove(piece);
                    }
                }
            }
        }

	}

    private void fallApart(Collision col)
    {
        

        SphereCollider[] spheres = GetComponentsInChildren<SphereCollider>() as SphereCollider[];
        foreach (SphereCollider s in spheres)
            if (s.gameObject.tag == "Damaging Component")
            {
                Destroy(s.gameObject); // removes player-damaging hitboxes
            }

        // deactivates universal collider to allow children to handle themselves
        GetComponent<Collider>().enabled = false;

        Rigidbody[] objects = GetComponentsInChildren<Rigidbody>() as Rigidbody[];

        pieces = new System.Collections.Generic.List<Transform>(GetComponentsInChildren<Transform>()); // saves references to all children's Transform components
        foreach (Transform t in pieces)
        {
            t.parent = null; // detaches from parent, otherwise physics animations look weird
            if (t.gameObject.GetComponent<SpriteRenderer>() != null)
                Destroy(t.gameObject);
        }

        // Falls apart; physics engine takes over for a few seconds. Update function will eventually destroy object
        foreach (Rigidbody r in objects)
        {
            r.isKinematic = false;
            r.useGravity = true;
            
            r.AddExplosionForce(col != null ? col.impulse.magnitude : 10, col != null ? col.collider.ClosestPointOnBounds(transform.position) : transform.position, 10,0,ForceMode.Impulse);
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
				this._robotStrategy = new RobotStrategyC(gameObject, target);
				break;	
		}
    }

    void CorrectPosture()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0,transform.rotation.eulerAngles.y,0));
    }

    void OnCollisionStay(Collision col)
    {
        inContactWithSomething = true;
        if (col.gameObject.tag == "Block" && !isReeling && safeContactOccurred)
        {
            Block block = col.gameObject.GetComponent<Block>(); // knocked over temporarily
            if (block == null)
                block = col.gameObject.GetComponentInChildren<Block>(); //sometimes collision registers with parent StretchableBlock and not Block
            if (block != null && !block.IsExpanding)
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
                safeContactOccurred = false;
                CorrectPosture();
                Debug.Log(gameObject.name + " is done being pushed around.");
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        inContactWithSomething = false;
        if (col.gameObject.tag == "Block" && !isReeling && safeContactOccurred)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            safeContactOccurred = false;
            CorrectPosture();
            Debug.Log(gameObject.name + " is done being pushed around.");
        }

    }

    void OnCollisionEnter(Collision col)
    {
        inContactWithSomething = true;
        Debug.Log(col.impulse.magnitude + "N of force\n");
        if (col.impulse.magnitude > minimumForceToDestroy) // will always fall apart upon a great enough force (block smash, falling)
        {
            safeContactOccurred = false;
            if (!isDefeated)
            {
                isAttacking = false;
                GetComponent<NavMeshAgent>().enabled = false;
                mAnimator.SetBool("isAttacking", false);
                mAnimator.Stop();

                isDefeated = true;
                Destroy(dizzyEffect);
                fallApart(col);

            }
        }
        else if (col.gameObject.tag == "Block" && col.impulse.magnitude >= minimumForceToStun)
        {
            safeContactOccurred = false;
            Block block = col.gameObject.GetComponent<Block>(); // knocked over temporarily
            if (block == null)
                block = col.gameObject.GetComponentInChildren<Block>(); //sometimes collision registers with parent StretchableBlock and not Block
            if (block != null && block.IsExpanding == true)
            {
                if (Mathf.Approximately(startingYPos,0))
                    startingYPos = transform.position.y;
                GetComponent<NavMeshAgent>().enabled = false;
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = true;

                foreach (ContactPoint p in col.contacts)
                    rb.AddForceAtPosition(col.impulse, p.point);
                isReeling = true;
                isAttacking = false;
                mAnimator.SetBool("isAttacking", false);
                errorNoise.Play();
                mAnimator.SetBool("isStunned", true);
                mAnimator.applyRootMotion = true;
                timeSpentReeling -= 0;
                if (timeToSpendReeling < (col.impulse.magnitude / 100) * 2)
                    timeToSpendReeling = (col.impulse.magnitude / 100) * 2;

                if (dizzyEffect == null)
                    dizzyEffect = Instantiate(DizzyEffectPrefab, transform, false) as GameObject;

                Debug.Log("Knocked out for " + timeToSpendReeling + " seconds");
            }
        }
        else if (col.gameObject.tag == "Block")
        {
            Block block = col.gameObject.GetComponent<Block>(); // gentle push
            if (block == null)
                block = col.gameObject.GetComponentInChildren<Block>(); //sometimes collision registers with parent StretchableBlock and not Block
            if (block != null && block.IsExpanding)
            {
                safeContactOccurred = true;
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = true;
                Debug.Log(gameObject.name + " is being pushed gently.");
            }
        }
    }

    public void reenableNavMeshAgent()
    {
        GetComponent<NavMeshAgent>().enabled = true;
        isAttacking = false;
    }
    /*
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
*/
}
