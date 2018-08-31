using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expression : MonoBehaviour 
{
	public enum ConditionOperator{MoreThan, LessThan, MoreThanEquals, LessThanEquals, Equals, Different, TRUE, FALSE}
	public ConditionOperator condition;

	public Argument firstArgument;
	public Argument secondArgument;


}
