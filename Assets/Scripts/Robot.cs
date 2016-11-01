using UnityEngine;
using System.Collections;
using RobotStuff;

public class Robot : MonoBehaviour 
{
	public string robotStrategyName = "RobotStrategyA";
	public Transform target;
	public float followRange = 3.0f;
	public float arriveThreshold = 0.05f;

	private RobotStrategy _robotStrategy;

	// Use this for initialization
	void Start() 
	{
		this.SetRobotStrategy(robotStrategyName);
		_robotStrategy.Start();
	}
	
	// Update is called once per frame
	void Update() 
	{
		_robotStrategy.Update();
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
			case "RobotStrategyC": 
				this._robotStrategy = new RobotStrategyC(gameObject, target);
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
