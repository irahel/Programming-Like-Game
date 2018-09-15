using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Argument
{
	public enum types{VARIABLE, NUMBER, EXPRESSION, OPERATION, NONE}
	public types type;

	public Variable variableValue;
	public float numberValue;
	public Expression expressionValue;
	public Operation operationValue;

	public Argument ()
	{
		type = types.NONE;
	}

}
