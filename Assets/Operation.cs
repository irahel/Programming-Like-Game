using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Operation 
{
public enum Operators{PLUS, MINUS, MULT, DIV}
	public Operators operator_;

	public enum types{VARIABLE, NUMBER, OPERATION, NONE}
	public types type_argumment_1, type_argumment_2;

	public Variable variable_value_1;
	public float number_value_1;
	public Operation operation_value_1;

	public Variable variable_value_2;
	public float number_value_2;
	public Operation operation_value_2;

}
