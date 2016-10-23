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
			default :
				this._robotStrategy = new RobotStrategyA(gameObject, target);
				break;	
		}
	}
}
