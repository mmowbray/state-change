using UnityEngine;
using System;

namespace RobotStuff
{
	public class RobotStrategyC : RobotStrategy
	{
		public RobotStrategyC(GameObject gameObject, Transform target) : base(gameObject, target)
		{
			Debug.Log("New RobotC");
		}

		public RobotStrategyC(GameObject gameObject, Transform target, float followRange, float arriveThreshold, float followSpeed) : base(gameObject, target, followRange, arriveThreshold, followSpeed)
		{
			Debug.Log("New RobotC");
		}

		public override void Start()
		{
			
		}

		public override void Update()
		{
			Vector3 direction = _target.transform.position - _myGameObject.transform.position;
			if(direction.magnitude <= _followRange)
			{
				if(direction.magnitude > _arriveThreshold)
				{
					if(!_myGameObject.GetComponent<Robot>().isAttacking && _myNavMeshAgent.isActiveAndEnabled)
					{
						_myNavMeshAgent.SetDestination(_target.position);
					}

                    _myGameObject.GetComponent<Robot>().isAttacking = false;
                }
				else
				{
					if(_myNavMeshAgent.isActiveAndEnabled)
					{
						_myNavMeshAgent.SetDestination(_myGameObject.transform.position);
					}

                    _myGameObject.GetComponent<Robot>().isAttacking = true;
				}
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
						Vector3 dir = direction.normalized;
						dir.y = 0;
						_myGameObject.transform.Translate(dir * _followSpeed * Time.deltaTime, Space.World);
					}
					else
					{
						Vector3 tarPos = _target.transform.position;
						tarPos.y = 0;
						_myGameObject.transform.position = tarPos;
					}
				}
			}
		}
	}
}

