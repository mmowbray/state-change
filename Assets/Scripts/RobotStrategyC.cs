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

		public override void Start()
		{
			_followRange = 25.0f;
			_arriveThreshold = 0.05f;
			_followSpeed = 2.0f;

			_follow = true;
		}

		public override void Update()
		{
//			RaycastHit hit;
//
//			Ray ray = new Ray(_myGameObject.transform.position, _myGameObject.transform.forward);
//
//			if (Physics.Raycast(ray, out hit, _maxRayDistance))
//			{
//				if(hit.collider.CompareTag("Player"))
//				{
//					_follow = true;
//				}
//			}

//			if(_follow && !_touching)
//			{
//				FollowTarget();
//			}

//			if(!_touching)
//			{
//				FollowTarget();
//			}


			Vector3 direction = _target.transform.position - _myGameObject.transform.position;
			if(direction.magnitude <= _followRange)
			{
//				_myGameObject.transform.rotation = Quaternion.RotateTowards(_myGameObject.transform.rotation, Quaternion.LookRotation(direction), _angularSpeed * Time.deltaTime);

				if(direction.magnitude > _arriveThreshold)
				{
//					Vector3 dir = direction.normalized;
//					dir.y = 0;
//					_myGameObject.transform.Translate(dir * _followSpeed * Time.deltaTime, Space.World);

                    if(!_myGameObject.GetComponent<Robot>().isAttacking && _myNavMeshAgent.isActiveAndEnabled)
					    _myNavMeshAgent.SetDestination(_target.position);

                    _myGameObject.GetComponent<Robot>().isAttacking = false;
                }
				else
				{
                    //					Vector3 tarPos = _target.transform.position;
                    //					tarPos.y = 0;
                    //					_myGameObject.transform.position = tarPos;
                    if (_myNavMeshAgent.isActiveAndEnabled)
                        _myNavMeshAgent.SetDestination(_myGameObject.transform.position);
                    if (Vector3.Angle(_myGameObject.transform.forward, direction) < 45)
                        _myGameObject.GetComponent<Robot>().isAttacking = true;
                    else if (_myNavMeshAgent.isActiveAndEnabled)
                        _myNavMeshAgent.SetDestination(_target.position);
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
				else
				{ 
					_follow = false; // Out of Range. 
				}
			}
		}
	}
}

