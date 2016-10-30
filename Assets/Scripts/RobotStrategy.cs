using UnityEngine;
using System;

namespace RobotStuff
{
	public abstract class RobotStrategy
	{
		protected GameObject _myGameObject;
		protected Transform _target;
		protected float _followRange = 7.0f;
		protected float _arriveThreshold = 0.05f;
		protected float _followSpeed = 1.0f;
		protected float _angularSpeed = 180.0f;
		protected float _maxRayDistance = 500.0f;
		protected bool _follow = false;
		protected bool _touching = false;

		public RobotStrategy(GameObject gameObject, Transform target)
		{
			_myGameObject = gameObject;
			_target = target;
		}

		public abstract void Start();

		public abstract void Update();

		public virtual void OnTriggerEnter(Collider col)
		{
			_touching = true;
		}

		public virtual void OnTriggerStay(Collider col)
		{
			_touching = true;
			GoAround();
		}

		public virtual void OnTriggerExit(Collider col)
		{
			_touching = false;
		}

		public virtual void GoAround()
		{
			RaycastHit hit;
			Ray rightRay = new Ray(_myGameObject.transform.position, Vector3.right);
			Ray leftRay = new Ray(_myGameObject.transform.position, Vector3.left);

			if(!Physics.Raycast(rightRay, out hit, 1))
			{
				_myGameObject.transform.rotation = Quaternion.RotateTowards(_myGameObject.transform.rotation, Quaternion.LookRotation(Vector3.right), _angularSpeed * Time.deltaTime);
				_myGameObject.transform.Translate(Vector3.right * _followSpeed * Time.deltaTime, Space.World);
			}
			else if(!Physics.Raycast(leftRay, out hit, 1))
			{
				_myGameObject.transform.rotation = Quaternion.RotateTowards(_myGameObject.transform.rotation, Quaternion.LookRotation(Vector3.left), _angularSpeed * Time.deltaTime);
				_myGameObject.transform.Translate(Vector3.left * _followSpeed * Time.deltaTime, Space.World);
			}
		}
	}
}

