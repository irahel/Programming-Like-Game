using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command : MonoBehaviour 
{
	public string name;
	public Queue commandParams;

	public Command ()
	{
		name = "";
		commandParams = new Queue();
	}
}
