using UnityEngine;
using System;

namespace RobotStuff
{
	public abstract class RobotStrategy
	{
		protected GameObject _myGameObject;
		protected Transform _target;
		protected NavMeshAgent _myNavMeshAgent;
		protected float _angularSpeed = 180.0f;
		protected float _maxRayDistance = 500.0f;
		protected bool _follow = false;
		protected bool _touching = false;

		protected float _followRange = 25.0f;
		protected float _arriveThreshold = 7.0f;
		protected float _followSpeed = 2.0f;

		public RobotStrategy(GameObject gameObject, Transform target)
		{
			_myGameObject = gameObject;
			_target = target;
			_myNavMeshAgent = _myGameObject.GetComponent<NavMeshAgent>();
		}

		public RobotStrategy(GameObject gameObject, Transform target, float followRange, float arriveThreshold, float followSpeed) : this(gameObject, target)
		{
			if(followRange != null && arriveThreshold != null && followSpeed != null)
			{
				_followRange = followRange;
				_arriveThreshold = arriveThreshold;
				_followSpeed = followSpeed;
			}
		}

		public abstract void Start();

		public abstract void Update();
	}
}

