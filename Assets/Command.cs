using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command
{
	public string name;
	public Queue commandParams;

	public void initiate ()
	{
		name = "";
		commandParams = new Queue();
	}
}
