using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variable
{
	public string name;

	public enum types{BOOL, INT, FLOAT}
	public types type;

	public bool boolValue;
	public int intValue;
	public float floatValue;
}

