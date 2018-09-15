using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expression
{
	public enum ConditionOperator{MoreThan, LessThan, MoreThanEquals, LessThanEquals, Equals, Different, And, Or, TRUE, FALSE, NONE}
	public ConditionOperator condition;

	public Argument firstArgument;
	public Argument secondArgument;


	public void initiate ()
	{
		condition = ConditionOperator.NONE;
		firstArgument = new Argument ();
		secondArgument = new Argument ();
	}

}

