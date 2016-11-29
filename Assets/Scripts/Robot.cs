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
    public float guaranteedDestroyForce = 100000;
    public float blockForce = 200; // standard block strength, i.e. force applied by fixed-length (combat) block

    private RobotStrategy _robotStrategy;

    //public bool defeatTrigger = false;
    private bool isDefeated = false;
    private float disappearTimer = 0;
    public float defeatDisappearanceDelayTime = 5;
    public bool isReeling = false;
    private float timeSpentReeling = 0;
    private float timeToSpendReeling;
    private bool inContactWithSomething = false;
    private bool safeContactOccurred = false;
    private float timeToIgnoreCollisions = 0;
    private float timeSpentIgnoringCollisions = 0;

    public GameObject DizzyEffectPrefab;
    private GameObject dizzyEffect;

    private Animator mAnimator;
    private System.Collections.Generic.List<Transform> pieces;// for falling apart and removal of body parts after

    [SerializeField]
    AudioSource destroySound;
    bool alreadyDestroyed = false;
    [SerializeField]
    AudioSource errorNoise;
    [SerializeField]
    AudioSource sawNoise;

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
                if (isAttacking)
                {
                    sawNoise.Play();
                    if (safeContactOccurred)
                    {
                        NavMeshHit hit = new NavMeshHit();
                        NavMesh.SamplePosition(transform.position, out hit, 10, NavMesh.AllAreas);
                        transform.position = hit.position;
                    }
                    GetComponent<NavMeshAgent>().enabled = false;
                    mAnimator.SetBool("isAttacking", true);

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
            Collider[] cols = GetComponentsInChildren<Collider>();
            foreach (Collider col in cols)
                if (col.gameObject != gameObject)
                    col.enabled = false;

            timeSpentIgnoringCollisions += Time.deltaTime;
            if (inContactWithSomething)
                timeSpentReeling += Time.deltaTime; //only recovers from stun when on something
            if (timeSpentReeling >= timeToSpendReeling)
            {
                Destroy(dizzyEffect);
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;

                transform.up = Vector3.MoveTowards(transform.up, Vector3.up, 0.1f);
                if (transform.up == -Vector3.up)
                    transform.up += new Vector3(0.1f, -0.9f, 0.1f);
                GetComponent<NavMeshAgent>().enabled = true;

                NavMeshHit hit = new NavMeshHit();
                int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
                NavMesh.SamplePosition(transform.position, out hit, 10, walkableMask);

                if (Mathf.Approximately(transform.rotation.eulerAngles.x, 0) && Mathf.Approximately(transform.rotation.eulerAngles.z, 0)/* && Mathf.Abs(transform.position.y - hit.position.y) <= 1.1f  && Mathf.Approximately(transform.position.x,hit.position.x) && Mathf.Approximately(transform.position.z, hit.position.z)*/)
                {
                    foreach (Collider col in cols)
                        if (col.gameObject.tag != "Damaging Component")
                            col.enabled = true;



                    rb.isKinematic = true;
                    mAnimator.applyRootMotion = false;
                    GetComponent<NavMeshAgent>().enabled = true;
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
                else pieces.Remove(piece);
            }
        }

    }

    private void fallApart(Collision col, float overrideForce = 0)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        Destroy(dizzyEffect);

        Collider[] colliders = GetComponentsInChildren<Collider>() as Collider[];
        foreach (Collider c in colliders)
            if (c.gameObject.tag == "Damaging Component")
            {
                c.isTrigger = false;
                Destroy(c.gameObject); // removes player-damaging hitboxes
            }
            else c.enabled = true;

        // deactivates universal collider to allow children to handle themselves
        foreach (Collider c in GetComponents<Collider>())
            c.enabled = false;

        Rigidbody[] objects = GetComponentsInChildren<Rigidbody>() as Rigidbody[];

        pieces = new System.Collections.Generic.List<Transform>(GetComponentsInChildren<Transform>()); // saves references to all children's Transform components
        foreach (Transform t in pieces)
        {
            t.parent = null; // detaches from parent, otherwise physics animations look weird
            if (t.gameObject.GetComponent<SpriteRenderer>() != null)
                Destroy(t.gameObject);
        }

        Block block = col.gameObject.GetComponent<Block>();
        if (block == null)
            block = col.gameObject.GetComponentInChildren<Block>(); //sometimes collision registers with parent StretchableBlock and not Block

        // Falls apart; physics engine takes over for a few seconds. Update function will eventually destroy object
        foreach (Rigidbody r in objects)
        {
            r.isKinematic = false;
            r.useGravity = true;
            if (r.gameObject == gameObject)
            {
                r.constraints = RigidbodyConstraints.FreezeAll;
            }

            if (block != null)
            {
                float currentSize = block.transform.localScale.y * block.transform.parent.localScale.y;
                r.AddExplosionForce(overrideForce != 0 ? overrideForce : col.impulse.magnitude, transform.position - Vector3.up, 0, 2, ForceMode.Impulse);
            }
            else
                r.AddExplosionForce(col != null ? col.impulse.magnitude : 10, col != null ? col.collider.ClosestPointOnBounds(transform.position) : transform.position, 0, 2, ForceMode.Impulse);
        }
    }

    public void SetRobotStrategy(string robotStrategyName)
    {
        switch (robotStrategyName)
        {
            case "RobotStrategyA":
                this._robotStrategy = new RobotStrategyA(gameObject, target);
                break;
            case "RobotStrategyB":
                this._robotStrategy = new RobotStrategyB(gameObject, target);
                break;
            default:
                this._robotStrategy = new RobotStrategyC(gameObject, target);
                break;
        }
    }

    void CorrectPosture()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
    }

    void BladeSpinning()
    {
        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (Collider col in cols)
            if (col.gameObject.tag == "Damaging Component")
                col.enabled = true;
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.name.Contains("Floor"))
            inContactWithSomething = true;
        Debug.Log(col.impulse.magnitude + "N of force\n");
        if (col.impulse.magnitude > guaranteedDestroyForce) // will always fall apart upon a great enough force (block smash, falling)
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
        else if (col.gameObject.tag == "Block" && !isDefeated)
        {
            Block block = col.gameObject.GetComponent<Block>();
            if (block == null)
                block = col.gameObject.GetComponentInChildren<Block>(); //sometimes collision registers with parent StretchableBlock and not Block

            if (block != null && block.IsExpanding)
            {
                float fixedSize = FindObjectOfType<HoloAim>().m_FixedLength;
                Animator blockAnim = block.GetComponent<Animator>();
                float currentSize = block.transform.localScale.y * block.transform.parent.localScale.y;
                float intendedSize = block.transform.parent.localScale.y;
                if (blockAnim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.75f) // point of expansion with enough force
                {
                    float decider = fixedSize;
                    if (block.transform.up == Vector3.up)
                        decider *= 1;
                    else
                        decider *= 0.5f;
                    if (intendedSize >= decider) // growing to a size fit for attack
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
                            fallApart(col,(blockForce * intendedSize / decider));
                        }
                    }
                    else
                    {
                        safeContactOccurred = true;
                        Debug.Log(gameObject.name + " is being pushed gently.");
                    }
                }
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.name.Contains("Floor"))
            inContactWithSomething = false;
        if (col.gameObject.tag == "Block" && !isDefeated && !isReeling && safeContactOccurred)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            NavMeshHit hit = new NavMeshHit();
            NavMesh.SamplePosition(transform.position, out hit, 10, NavMesh.AllAreas);
            transform.position = hit.position;
            rb.isKinematic = true;
            rb.useGravity = false;
            safeContactOccurred = false;
            CorrectPosture();
            Debug.Log(gameObject.name + " is done being pushed around.");
        }

    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name.Contains("Floor"))
            inContactWithSomething = true;
        Debug.Log(col.impulse.magnitude + "N of force\n");
        if (col.impulse.magnitude > guaranteedDestroyForce) // will always fall apart upon a great enough force (block smash, falling)
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
        else if (col.gameObject.tag == "Block" && !isDefeated)
        {
            Block block = col.gameObject.GetComponent<Block>();
            if (block == null)
                block = col.gameObject.GetComponentInChildren<Block>(); //sometimes collision registers with parent StretchableBlock and not Block

            if (block != null && block.IsExpanding)
            {
                float fixedSize = FindObjectOfType<HoloAim>().m_FixedLength;
                Animator blockAnim = block.GetComponent<Animator>();
                float currentSize = block.transform.localScale.y * block.transform.parent.localScale.y;
                float intendedSize = block.transform.parent.localScale.y;
                if (blockAnim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.75f) // point of expansion with enough force
                {
                    float decider = fixedSize;
                    if (block.transform.up == Vector3.up)
                        decider *= 1f;
                    else
                        decider *= 0.5f;
                    if (intendedSize >= decider) // growing to a size fit for attack
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
                            fallApart(col, (blockForce * intendedSize / decider));
                        }
                    }
                    else // if smaller than standard
                    {
                        safeContactOccurred = true;
                        Debug.Log(gameObject.name + " is being pushed gently.");
                    }
                }
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
