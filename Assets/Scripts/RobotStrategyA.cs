using UnityEngine;
using System;

namespace RobotStuff
{
	public class RobotStrategyA : RobotStrategy
	{
		public RobotStrategyA(GameObject gameObject, Transform target) : base(gameObject, target)
		{
			Debug.Log("New RobotA");
		}

		public override void Start()
		{
			_followRange = 7.0f;
			_arriveThreshold = 0.05f;
			_followSpeed = 1.0f;
		}

		public override void Update()
		{
			RaycastHit hit;

			Ray ray = new Ray(_myGameObject.transform.position, _myGameObject.transform.forward);

			if (Physics.Raycast(ray, out hit, _maxRayDistance))
			{
				if(hit.collider.CompareTag("Player"))
				{
					_follow = true;
				}
			}

			if(_follow && !_touching)
			{
				FollowTarget();
			}
		}

		public void FollowTarget()
		{
			if(_target != null)
			{
				Vector3 direction = _target.transform.position - _myGameObject.transform.position;
				if(direction.magnitude <= _followRange)
				{
					_myGameObject.transform.rotation = Quaternion.RotateTowards(_myGameObject.transform.rotation, Quaternion.LookRotation(direction), _angularSpeed * Time.deltaTime);

					if(direction.magnitude > _arriveThreshold)
					{
						_myGameObject.transform.Translate(direction.normalized * _followSpeed * Time.deltaTime, Space.World);
					}
					else
					{
						_myGameObject.transform.position = _target.transform.position;
					}
				}
				else
				{ 
					_follow = false; // Out of Range. 
				}
			}
		}
	}
}

