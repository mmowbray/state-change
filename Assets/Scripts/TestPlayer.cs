using UnityEngine;
using System.Collections;

public class TestPlayer : MonoBehaviour
{
	// Units per second
	[SerializeField]
	float mSpeed = 5.0f;

	// Degrees per second
	[SerializeField]
	float mAngularSpeed = 180.0f;

	// Idle timer variables
	[SerializeField]
	float mIdleTime = 2.0f;
	float mTimer = 0.0f;

	Vector3 mDefaultScale;

	void Start ()
	{
		// Keep a backup of the original scale
		mDefaultScale = transform.localScale;
	}

	void Update ()
	{
		MoveObject ();
		SimpleIdleAnimation ();
	}

	private void MoveObject()
	{
		// Obtain input information (See "Horizontal" and "Vertical" in the Input Manager)
		float horizontal = Input.GetAxis ("Horizontal");
		float vertical = Input.GetAxis ("Vertical");

		// Check if there is movement
		if(!Mathf.Approximately(vertical, 0.0f) || !Mathf.Approximately(horizontal, 0.0f))
		{
			Vector3 direction = new Vector3(horizontal, 0.0f, vertical);

			// Cap the magnitude of direction vector
			direction = Vector3.ClampMagnitude(direction, 1.0f);

			// Translate the game object in world space
			transform.Translate (direction * mSpeed * Time.deltaTime, Space.World);

			// Rotate the game object
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), mAngularSpeed * Time.deltaTime);

			// Reset idle timer to zero
			mTimer = 0.0f;
			transform.localScale = mDefaultScale;
		}
	}

	private void SimpleIdleAnimation()
	{
		mTimer += Time.deltaTime;
		if(mTimer >= mIdleTime)
		{
			// If time is up, change the scale of the game object
			float growth = Mathf.PingPong(Time.time, 0.10f);
			transform.localScale = mDefaultScale + transform.localScale * growth;
		}
	}
}
