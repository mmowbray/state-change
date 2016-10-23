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

		public RobotStrategy(GameObject gameObject, Transform target)
		{
			_myGameObject = gameObject;
			_target = target;
		}

		public abstract void Start();

		public abstract void Update();
	}
}

