using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;

public class PlattaformTest : MonoBehaviour 
{
	public GameObject Terminal;
	public player playerScript;
	public enum Direction{LEFT, RIGHT, UP, DOWN}
	public enum ConditionOperator{MoreThan, LessThan, MoreThanEquals, LessThanEquals, Equals}
	public Direction MOVE_direction;
	public float MOVE_speed;
	public Queue pipeline;

	void Start () 
	{
		pipeline = new Queue ();
	}

	void Update () 
	{
		//Debug.Log ("pipe lenght " +pipeline.Count);
		Queue executedPipeline = new Queue ();
		while(pipeline.Count > 0)
		{
			System.Object task = pipeline.Dequeue();
			executedPipeline.Enqueue (task);
			//Need reform
			WhileForm whileCommand = (task as WhileForm);
			//Debug.Log ("corpus: "+whileCommand.Corpus);
			whileAct (whileCommand);
		}
		pipeline = executedPipeline;


	}

	void OnMouseDown()
	{
		Terminal.SetActive (true);
		playerScript.TerminalOn = true;
	}

	public void compiller()
	{
		String Command = Terminal.gameObject.GetComponentInChildren<Text>().text;

		//Debug.Log ("Command: " +Command);

		//Command = cleaner (Command);
		Command = Clean(Command);

		//Debug.Log ("Command after clean: " +Command);

		if (Command.StartsWith ("while")) 
		{						
			int stringIndexInitCondition = 0;
			int stringIndexEndCondition = 0;
			int stringIndexInitCorpus = 0;
			int stringIndexEndCorpus = 0;

			bool findInitCondition = true;
			bool findEndCondition = false;
			bool findInitCorpus = false;
			bool findEndCorpus = false;

			for (int i = 0; i < Command.Length; i++) 
			{
				//Debug.Log (Command [i]);
				if (findInitCondition) 
				{					
					if(Command[i].Equals('('))
					{
						stringIndexInitCondition = i+1;
						findInitCondition = false;
						findEndCondition = true;
						continue;
					}
				}else if(findEndCondition)
				{
					if(Command[i].Equals(')'))
					{
						stringIndexEndCondition = i;
						findEndCondition = false;
						findInitCorpus = true;
						continue;
					}
				}else if(findInitCorpus)
				{
					if(Command[i].Equals('{'))
					{
						stringIndexInitCorpus = i+1;
						findInitCorpus = false;
						findEndCorpus = true;
						continue;
					}
				}else if(findEndCorpus)
				{
					if(Command[i].Equals('}'))
					{
						stringIndexEndCorpus = i;
						findEndCorpus = false;
						findInitCondition = true;
						break;
					}	
				}

			}
			String conditionSend = Command.Substring (stringIndexInitCondition, stringIndexEndCondition - stringIndexInitCondition);
			String corpusSend = Command.Substring (stringIndexInitCorpus, stringIndexEndCorpus - stringIndexInitCorpus);
			whileInterpreter (conditionSend, corpusSend);
		}
			
		// Debug.Log (Command);
		Terminal.SetActive (false);
		playerScript.TerminalOn = false;
	}
	Expression expressionInterpreter(string condition)
	{		
		condition = Clean (condition);
		Expression finalExpression = new Expression ();
		Debug.Log ("condition: " +condition);

		if (condition.Contains ('>') || condition.Contains ('<') || condition.Contains ('=') || condition.Contains ('&') || condition.Contains ('|')) 
		{			
			Debug.Log ("enter in condition if");
			char[] reservedKeys = { '>', '<', '=', '!', '&', '|' };
			int index = condition.IndexOfAny (reservedKeys);
			int cutIndex = index + 1;
			switch (condition [index]) {
			case '>':
				if (condition [index + 1].Equals ('=')) {
					finalExpression.condition = Expression.ConditionOperator.MoreThanEquals;
					cutIndex += 1;
				} else {
					finalExpression.condition = Expression.ConditionOperator.MoreThan;
				}
				Debug.Log ("Applied condition >");
				break;
			case '<':
				if (condition [index + 1].Equals ('=')) {
					finalExpression.condition = Expression.ConditionOperator.LessThanEquals;
					cutIndex += 1;
				} else {
					finalExpression.condition = Expression.ConditionOperator.LessThan;
				}
				Debug.Log ("Applied condition <");
				break;
			case '=':
				if (condition [index + 1].Equals ('=')) {
					finalExpression.condition = Expression.ConditionOperator.Equals;
					cutIndex += 1;
				} else {						
					//error
				}
				Debug.Log ("Applied condition =");
				break;
			case '!':
				if (condition [index + 1].Equals ('=')) {
					finalExpression.condition = Expression.ConditionOperator.Different;
					cutIndex += 1;
				} else {						
					//error
				}
				Debug.Log ("Applied condition !=");
				break;
			case '&':
				if (condition [index + 1].Equals ('&')) {
					finalExpression.condition = Expression.ConditionOperator.And;
					cutIndex += 1;
				} else {						
					//error
				}
				Debug.Log ("Applied condition &");
				break;
			case '|':
				if (condition [index + 1].Equals ('|')) {
					finalExpression.condition = Expression.ConditionOperator.Or;
					cutIndex += 1;
				} else {						
					//error
				}
				Debug.Log ("Applied condition |");
				break;
			}
			Debug.Log ("first argument string: " +condition.Substring (0, index));
			Debug.Log ("second argument string: " +condition.Substring (cutIndex, condition.Length - cutIndex));
			finalExpression.firstArgument = argumentInterpreter (condition.Substring (0, index));
			finalExpression.secondArgument = argumentInterpreter (condition.Substring (cutIndex, condition.Length - cutIndex));

			//Debug.Log (condition.Substring(0, index));
			//Debug.Log (condition[index]);
			//Debug.Log (condition.Substring(index+1, condition.Length - index));
		} 
		else if (Clean (condition).Equals ("true")) 
		{
			Debug.Log ("enter in true if");
			finalExpression.condition = Expression.ConditionOperator.TRUE;
		} 
		else if (Clean (condition).Equals ("false")) 
		{
			Debug.Log ("enter in false if");
			finalExpression.condition = Expression.ConditionOperator.FALSE;
		}

		return finalExpression;

	}

	Argument argumentInterpreter(string toSolve)
	{
		Argument finalArgument = new Argument ();
		if (toSolve.Contains ('>') || toSolve.Contains ('<') || toSolve.Contains ('=') || toSolve.Contains ('&') || toSolve.Contains ('|')) 
		{
			finalArgument.type = Argument.types.EXPRESSION;
			finalArgument.expressionValue = expressionInterpreter (toSolve);
		}
		else if (toSolve.Contains ('+') || toSolve.Contains ('-') || toSolve.Contains ('*') || toSolve.Contains ('/')) 
		{
			finalArgument.type = Argument.types.OPERATION;
			finalArgument.operationValue = operationInterpreter(toSolve);
		}
		else 
		{
			//need implement variables
			finalArgument.type = Argument.types.NUMBER;
			finalArgument.numberValue = float.Parse(toSolve);
		}
		return finalArgument;
	}

	Operation operationInterpreter(string toSolve)
	{
		Operation finalOperation = new Operation ();

		char[] firsPriority = { '*', '/' };
		char[] secondPriority = { '+', '-'};

		if (toSolve.Contains ('*') || toSolve.Contains ('/')) 
		{
			int index = toSolve.IndexOfAny (firsPriority);
			if (toSolve [index].Equals ('*')) 
			{
				finalOperation.operator_ = Operation.Operators.MULT;
			}
			else 
			{
				finalOperation.operator_ = Operation.Operators.DIV;
			}
			//solve argumment 1
			if (toSolve.Substring (0, index).Contains ('+') || toSolve.Substring (0, index).Contains ('-')) 
			{
				finalOperation.type_argumment_1 = Operation.types.OPERATION;
				finalOperation.operation_value_1 = operationInterpreter (toSolve.Substring (0, index));
			}
			else 
			{
				//need implement variables
				finalOperation.type_argumment_1 = Operation.types.NUMBER;
				finalOperation.number_value_1 = float.Parse (toSolve.Substring (0, index));
			}

			//solve argumment 2
			if (toSolve.Substring (index + 1, toSolve.Length - (index+1)).Contains ('+') || toSolve.Substring (index + 1, toSolve.Length - (index+1)).Contains ('-')) 
			{
				finalOperation.type_argumment_2 = Operation.types.OPERATION;
				finalOperation.operation_value_2 = operationInterpreter (toSolve.Substring (index + 1, toSolve.Length - (index+1)));
			}
			else 
			{
				//need implement variables
				finalOperation.type_argumment_2 = Operation.types.NUMBER;
				finalOperation.number_value_2 = float.Parse (toSolve.Substring (index + 1, toSolve.Length - (index+1)));
			}

		} 
		else if (toSolve.Contains ('+') || toSolve.Contains ('-')) 
		{
			int index = toSolve.IndexOfAny (secondPriority);
			if (toSolve [index].Equals ('+')) 
			{
				finalOperation.operator_ = Operation.Operators.PLUS;
			}
			else 
			{
				finalOperation.operator_ = Operation.Operators.MINUS;
			}
			//solve argumment 1
			finalOperation.type_argumment_1 = Operation.types.NUMBER;
			finalOperation.number_value_1 = float.Parse (toSolve.Substring (0, index));


			//solve argumment 2
			if (toSolve.Substring (index + 1, toSolve.Length - (index+1)).Contains ('+') || toSolve.Substring (index + 1, toSolve.Length - (index+1)).Contains ('-')) 
			{
				finalOperation.type_argumment_2 = Operation.types.OPERATION;
				finalOperation.operation_value_2 = operationInterpreter (toSolve.Substring (index + 1, toSolve.Length - (index+1)));
			}
			else
			{
				//need implement variables
				finalOperation.type_argumment_2 = Operation.types.NUMBER;
				finalOperation.number_value_2 = float.Parse (toSolve.Substring (index + 1, toSolve.Length - (index+1)));
			}
		}

		return finalOperation;
	}

	void whileInterpreter(String condition, String corpus)
	{
		WhileForm whileObj = new WhileForm ();
		whileObj.initiate ();

		whileObj.Corpus = corpus;
		//Debug.Log ("Condição: " + condition);
		whileObj.mainExpression = expressionInterpreter(condition);
		corpusInterpreter (whileObj);
	}


	void whileAct(WhileForm whileCommand)
	{

		//Debug.Log ("enter in act");
		//Debug.Log (whileCommand.mainExpression.firstArgument.numberValue + "  " + whileCommand.mainExpression.condition + "  " + whileCommand.mainExpression.secondArgument.numberValue);
		if (expressionCheck (whileCommand.mainExpression)) 
		{
			Debug.Log ("enter in exp");
			Queue<Command> Executed = new Queue<Command> ();
			while (whileCommand.commands.Count > 0) 
			{
				//Debug.Log ("enter in commands " +whileCommand.commands.Count);
				Command next = whileCommand.commands.Dequeue ();
				//Debug.Log ("enter in commands " +whileCommand.commands.Count);
				Executed.Enqueue (next);
				//	Debug.Log ("named " +next.name);
				switch (next.name) {
				case "MOVE":
					//Debug.Log ("enter here case");
					object firstParam = next.commandParams.Dequeue ();
					object secondParam = next.commandParams.Dequeue ();
					//Debug.Log ("commands  :" +firstParam + "  "  +secondParam);
					command_move ((firstParam as string), float.Parse((secondParam as string)));
					next.commandParams.Enqueue (firstParam);
					next.commandParams.Enqueue (secondParam);
					break;
				}
			}
			whileCommand.commands = Executed;

		}
	}

	void command_move(string direction, float moveSpeed)
	{
		//moveSpeed = moveSpeed * 10000;
		//	Debug.Log ("enter in comannd move");
		if (direction == "left") 
		{
			transform.Translate (Vector2.left * moveSpeed * Time.deltaTime);
			//Debug.Log ("moving left");
		}
		else if (direction == "right") 
		{
			transform.Translate (Vector2.right * moveSpeed * Time.deltaTime);
			//	Debug.Log ("moving right");
		}
		else if (direction == "up") 
		{
			transform.Translate (Vector2.up * moveSpeed * Time.deltaTime);
			//Debug.Log ("moving up");
		}
		else
		{
			transform.Translate (Vector2.down * moveSpeed * Time.deltaTime);
			//Debug.Log ("moving down");
		}			
	}

	string Clean(string enter)
	{		
		enter = enter.Replace (" ", "");
		enter = enter.Replace ("\n", "");
		enter = enter.Replace ("\t", "");
		return enter;
	}
	// while(true){move(left, 1)}
	// while(12 > 1){move(left, 1)}

	float solveOperation(Operation toSolve)
	{
		float Number1 = 0;
		float Number2 = 0;
		//solve number 1
		switch (toSolve.type_argumment_1) 
		{
			case Operation.types.NUMBER:
				Number1 = toSolve.number_value_1;
				break;
			case Operation.types.VARIABLE:
				switch (toSolve.variable_value_1.type) {
				case Variable.types.FLOAT:
					Number1 = toSolve.variable_value_1.floatValue;
					break;
				case Variable.types.INT:
					Number1 = toSolve.variable_value_1.intValue;
					break;
				case Variable.types.BOOL:
					//error
					break;
				}
				break;
			case Operation.types.OPERATION:
				Number1 = solveOperation (toSolve.operation_value_1);
				break;
		}
		//solve number 2
		switch (toSolve.type_argumment_2) 
		{
			case Operation.types.NUMBER:
				Number2 = toSolve.number_value_2;
				break;
			case Operation.types.VARIABLE:
				switch (toSolve.variable_value_2.type) {
				case Variable.types.FLOAT:
					Number2 = toSolve.variable_value_2.floatValue;
					break;
				case Variable.types.INT:
					Number2 = toSolve.variable_value_2.intValue;
					break;
				case Variable.types.BOOL:
					//error
					break;
				}
				break;
			case Operation.types.OPERATION:
				Number2 = solveOperation (toSolve.operation_value_2);
				break;
		}		
		//solve operation
		switch (toSolve.operator_) 
		{
			case Operation.Operators.PLUS:			
				return Number1 + Number2;
			case Operation.Operators.MINUS:
				return Number1 - Number2;				
			case Operation.Operators.MULT:
				return Number1 * Number2;				
			case Operation.Operators.DIV:
				return Number1 / Number2;				
			default:
			//implement error here
				return 0;
		}
	}

	bool expressionCheck(Expression toCheck)
	{
		if (toCheck.condition == Expression.ConditionOperator.TRUE) 
		{
			return true;
		}
		else if (toCheck.condition == Expression.ConditionOperator.FALSE) 
		{
			return false;
		}

		float f_Item1 = 0;
		float f_Item2 = 0;

		bool b_Item1 = false;
		bool b_Item2 = false;

		//solve item 1
		switch (toCheck.firstArgument.type) 
		{
			case Argument.types.NUMBER:
				f_Item1 = toCheck.firstArgument.numberValue;
				break;
			case Argument.types.VARIABLE:
				switch (toCheck.firstArgument.variableValue.type) 
				{
				case Variable.types.FLOAT:
					f_Item1 = toCheck.firstArgument.variableValue.floatValue;
					break;
				case Variable.types.INT:
					f_Item1 = toCheck.firstArgument.variableValue.intValue;
					break;
				case Variable.types.BOOL:
					b_Item1 = toCheck.firstArgument.variableValue.boolValue;
					break;
				}
				break;
			case Argument.types.OPERATION:
				f_Item1 = solveOperation (toCheck.firstArgument.operationValue);
				break;
			case Argument.types.EXPRESSION:
				b_Item1 = expressionCheck (toCheck.firstArgument.expressionValue);
				break;
		}
		//solve item 2
		switch (toCheck.secondArgument.type) 
		{
			case Argument.types.NUMBER:
				f_Item2 = toCheck.secondArgument.numberValue;
				break;
			case Argument.types.VARIABLE:
				switch (toCheck.secondArgument.variableValue.type) 
					{
					case Variable.types.FLOAT:
						f_Item2 = toCheck.secondArgument.variableValue.floatValue;
						break;
					case Variable.types.INT:
						f_Item2 = toCheck.secondArgument.variableValue.intValue;
						break;
					case Variable.types.BOOL:
						b_Item2 = toCheck.secondArgument.variableValue.boolValue;
						break;
				}
				break;
			case Argument.types.OPERATION:
				f_Item2 = solveOperation (toCheck.secondArgument.operationValue);
				break;
			case Argument.types.EXPRESSION:
				b_Item2 = expressionCheck (toCheck.secondArgument.expressionValue);
				break;
		}

		 //solve final condition
		switch (toCheck.condition) 
		{
			case Expression.ConditionOperator.Different:
				return f_Item1 != f_Item2;								
			case Expression.ConditionOperator.Equals:
				return f_Item1 != f_Item2;		
			case Expression.ConditionOperator.LessThan:
				return f_Item1 < f_Item2;					
			case Expression.ConditionOperator.LessThanEquals:
				return f_Item1 <= f_Item2;
			case Expression.ConditionOperator.MoreThan:
				return f_Item1 > f_Item2;
			case Expression.ConditionOperator.MoreThanEquals:
				return f_Item1 >= f_Item2;
			case Expression.ConditionOperator.And:
				return b_Item1 && b_Item2;
			case Expression.ConditionOperator.Or:
				return b_Item1 || b_Item2;
			default:
			//implement error here
				return false;
		}
	}


	void corpusInterpreter(WhileForm whileObj)
	{
		string corpus = whileObj.Corpus;

		ArrayList commands = new ArrayList(corpus.Split(';'));
		//Debug.Log ("corpus :" + corpus);

		foreach( string item in commands)
		{
			//Debug.Log ("corpus parted :" + item);
			if (item.StartsWith ("move")) 
			{
				int stringIndexInitParam1 = 0;
				int stringIndexEndParam1 = 0;
				int stringIndexEndParam2 = 0;


				bool find1Param = true;
				bool find2Param = false;

				for (int i = 0; i < item.Length; i++) 
				{
					if (find1Param) 
					{
						if (item [i].Equals ('(')) 
						{
							stringIndexInitParam1 = i+1;
							//find1Param = false;
							//find2Param = true;
							continue;
						}
						if (item [i].Equals (',')) 
						{
							stringIndexEndParam1 = i;
							find1Param = false;
							find2Param = true;
							continue;
						}
					}
					else if(find2Param)
					{
						if (item [i].Equals (')')) 
						{
							stringIndexEndParam2 = i-1;
							find1Param = true;
							find2Param = false;
							break;
						}
					}
				}
				String firstParam = item.Substring (stringIndexInitParam1, stringIndexEndParam1 - stringIndexInitParam1);
				String secondParam = item.Substring (stringIndexEndParam1 + 1, stringIndexEndParam2 - stringIndexEndParam1);
				// Debug.Log ("1param: " + corpus.Substring (stringIndexInitParam1, stringIndexEndParam1 - stringIndexInitParam1));
				// Debug.Log ("2param: " + corpus.Substring (stringIndexEndParam1+1, stringIndexEndParam2 - stringIndexEndParam1));
				Command moveCommand = new Command();
				moveCommand.initiate ();

				moveCommand.name = "MOVE";
				moveCommand.commandParams.Enqueue (firstParam);
				moveCommand.commandParams.Enqueue (secondParam);
				whileObj.commands.Enqueue (moveCommand);
			}
		}
			
		pipeline.Enqueue (whileObj);
	}

}
