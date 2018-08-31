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
	public enum ConditionOperator{MoreThan, LessThan, MoreThanEquals, LessThanEquals}
	public Direction MOVE_direction;
	public float MOVE_speed;
	public Queue pipeline;

	void Start () 
	{
		pipeline = new Queue ();
	}

	void Update () 
	{
		Debug.Log (pipeline.Count);
		Queue executedPipeline = new Queue ();
		while(pipeline.Count > 0)
		{
			System.Object task = pipeline.Dequeue();
			executedPipeline.Enqueue (task);
			//Need reform
			WhileForm whileCommand = (task as WhileForm);
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

		Debug.Log ("Command: " +Command);

		//Command = cleaner (Command);
		Command = Clean(Command);

		Debug.Log ("Command after clean: " +Command);

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
		
	void whileInterpreter(String condition, String corpus)
	{
		WhileForm whileObj = new WhileForm ();
		whileObj.Corpus = corpus;
		//Debug.Log ("Condição: " + condition);
		if (condition.StartsWith ("true")) 
		{
			Expression whileTrueExpression = new Expression();
			whileTrueExpression.condition = Expression.ConditionOperator.TRUE;
			whileObj.mainExpression = whileTrueExpression;
		}
		else 
		{			
			bool conditionTest = false;
			string arg1Send = "";
			string arg2Send = ""; 
			ConditionOperator operatorSend = ConditionOperator.LessThan;
			int indexOperator = -99;
			for(int iterator = 0; iterator < condition.Length; iterator++)
			{
				if(condition[iterator].Equals('>') || condition[iterator].Equals('<'))
				{
					Debug.Log ("Founded a contitional " +iterator);
					indexOperator = iterator;
					if (condition [iterator + 1].Equals ('=')) 
					{						
						//Debug.Log ("firs Substring: 0 - " +indexOperator);
						//Debug.Log ("second Substring: " +(indexOperator+2) +" - " +(condition.Length - indexOperator+2));
						//Debug.Log(condition.Length + " \\ " +indexOperator);
						arg1Send = condition.Substring (0, indexOperator);
						arg2Send = condition.Substring ((indexOperator+2), (condition.Length - (indexOperator+2)));
						if (condition [iterator].Equals ('>')) 
						{
							operatorSend = ConditionOperator.MoreThanEquals;
						} 
						else
						{
							operatorSend = ConditionOperator.LessThanEquals;
						}
						break;					
					}
					else 
					{
						//Debug.Log ("firs Substring: 0 - " +indexOperator);
						//Debug.Log ("second Substring: " +(indexOperator+1) +" - " +(condition.Length - indexOperator+1));
						//Debug.Log(condition.Length + " \\ " +indexOperator);
						arg1Send = condition.Substring (0, indexOperator);
						arg2Send = condition.Substring ((indexOperator+1), (condition.Length - (indexOperator+1)));
						if (condition [iterator].Equals ('>')) 
						{
							operatorSend = ConditionOperator.MoreThan;
						} 
						else
						{
							operatorSend = ConditionOperator.LessThan;
						}
						break;
					}
				}
			}
			conditonInterpreter (arg1Send, arg2Send, operatorSend, whileObj);
			//conditionTest 
			//Debug.Log("Condição: " +conditionTest);
			//Interp the condition
		}	
	}


	void whileAct(WhileForm whileCommand)
	{
		
		if (expressionCheck (whileCommand.mainExpression)) 
		{
			Queue<Command> Executed = new Queue<Command> ();
			while (whileCommand.commands.Count > 0) 
			{
				Command next = whileCommand.commands.Dequeue ();
				Executed.Enqueue (next);
				switch (next.name) {
				case "MOVE":				
					object firstParam = next.commandParams.Dequeue();
					object secondParam = next.commandParams.Dequeue();
					command_move ((firstParam as string), float.Parse((secondParam as string)));
					next.commandParams.Enqueue (firstParam);
					next.commandParams.Enqueue (secondParam);
					break;
				}
			}
			whileCommand.commands = Executed;

		}
	}


	void conditonInterpreter(String arg1, String arg2, ConditionOperator operator1 , WhileForm whileObj)
	{
		Expression whileNormalExpression = new Expression();

		arg1 = Clean(arg1);
		arg2 = Clean(arg2);

		int argInt1 = Int32.Parse (arg1);
		int argInt2 = Int32.Parse (arg2);

		//Debug.Log("condition tested: " +argInt1 +" " +operator1 +" " +argInt2);
		switch(operator1)
		{
			case ConditionOperator.LessThan:
				whileNormalExpression.condition = Expression.ConditionOperator.LessThan;				
				break;
			case ConditionOperator.LessThanEquals:
				whileNormalExpression.condition = Expression.ConditionOperator.LessThanEquals;
				break;
			case ConditionOperator.MoreThan:
				whileNormalExpression.condition = Expression.ConditionOperator.LessThan;
				break;
			case ConditionOperator.MoreThanEquals:
				whileNormalExpression.condition = Expression.ConditionOperator.LessThan;
				break;
		}
		whileObj.mainExpression = whileNormalExpression;
		corpusInterpreter (whileObj.Corpus, whileObj);
	}


	void command_move(string direction, float moveSpeed)
	{
		//Debug.Log ("enter in comannd move");
		if (MOVE_direction == Direction.LEFT) 
		{
			transform.Translate (Vector2.left * MOVE_speed * Time.deltaTime);
		}
		else if (MOVE_direction == Direction.RIGHT) 
		{
			transform.Translate (Vector2.right * MOVE_speed * Time.deltaTime);
		}
		else if (MOVE_direction == Direction.UP) 
		{
			transform.Translate (Vector2.up * MOVE_speed * Time.deltaTime);
		}
		else
		{
			transform.Translate (Vector2.down * MOVE_speed * Time.deltaTime);
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

	bool expressionCheck(Expression toCheck)
	{
		float Number1 = 0;
		float Number2 = 0;
		bool elseReturn = false;
		switch (toCheck.condition) 
		{
			case Expression.ConditionOperator.Different:
				switch (toCheck.firstArgument.type) 
				{
					case Argument.types.NUMBER:
						Number1 = toCheck.firstArgument.numberValue;
						break;
					case Argument.types.VARIABLE:
						//not yet
						break;
					case Argument.types.OPERATION:
						//not yet
						break;
					}
				switch (toCheck.secondArgument.type) 
				{
					case Argument.types.NUMBER:
						Number2 = toCheck.secondArgument.numberValue;
						break;
					case Argument.types.VARIABLE:
						//not yet
						break;
					case Argument.types.OPERATION:
						//not yet
						break;
				}
				if(Number1 != Number2)
				{
					return true;
				}
				break;
			case Expression.ConditionOperator.Equals:
				switch (toCheck.firstArgument.type) 
				{
					case Argument.types.NUMBER:
						Number1 = toCheck.firstArgument.numberValue;
						break;
					case Argument.types.VARIABLE:
						//not yet
						break;
					case Argument.types.OPERATION:
						//not yet
						break;
				}
				switch (toCheck.secondArgument.type) 
				{
					case Argument.types.NUMBER:
						Number2 = toCheck.secondArgument.numberValue;
						break;
					case Argument.types.VARIABLE:
						//not yet
						break;
					case Argument.types.OPERATION:
						//not yet
						break;
				}
				if(Number1 == Number2)
				{
					return true;
				}
				break;
			case Expression.ConditionOperator.TRUE:
				return true;
				break;
			case Expression.ConditionOperator.FALSE:
				break;
			case Expression.ConditionOperator.LessThan:
				switch (toCheck.firstArgument.type) 
				{
					case Argument.types.NUMBER:
						Number1 = toCheck.firstArgument.numberValue;
						break;
					case Argument.types.VARIABLE:
						//not yet
						break;
					case Argument.types.OPERATION:
						//not yet
						break;
				}
				switch (toCheck.secondArgument.type) 
				{
					case Argument.types.NUMBER:
						Number2 = toCheck.secondArgument.numberValue;
						break;
					case Argument.types.VARIABLE:
						//not yet
						break;
					case Argument.types.OPERATION:
						//not yet
						break;
				}
				if(Number1 < Number2)
				{
					return true;
				}
				break;
			case Expression.ConditionOperator.LessThanEquals:
				switch (toCheck.firstArgument.type) 
				{
					case Argument.types.NUMBER:
						Number1 = toCheck.firstArgument.numberValue;
						break;
					case Argument.types.VARIABLE:
						//not yet
						break;
					case Argument.types.OPERATION:
						//not yet
						break;
				}
				switch (toCheck.secondArgument.type) 
				{
					case Argument.types.NUMBER:
						Number2 = toCheck.secondArgument.numberValue;
						break;
					case Argument.types.VARIABLE:
						//not yet
						break;
					case Argument.types.OPERATION:
						//not yet
						break;
				}
				if(Number1 <= Number2)
				{
					return true;
				}
				break;
			case Expression.ConditionOperator.MoreThan:
				switch (toCheck.firstArgument.type) 
				{
					case Argument.types.NUMBER:
						Number1 = toCheck.firstArgument.numberValue;
						break;
					case Argument.types.VARIABLE:
						//not yet
						break;
					case Argument.types.OPERATION:
						//not yet
						break;
				}
				switch (toCheck.secondArgument.type) 
				{
					case Argument.types.NUMBER:
						Number2 = toCheck.secondArgument.numberValue;
						break;
					case Argument.types.VARIABLE:
						//not yet
						break;
					case Argument.types.OPERATION:
						//not yet
						break;
				}
				if(Number1 > Number2)
				{
					return true;
				}
				break;
			case Expression.ConditionOperator.MoreThanEquals:
				switch (toCheck.firstArgument.type) 
				{
					case Argument.types.NUMBER:
						Number1 = toCheck.firstArgument.numberValue;
						break;
					case Argument.types.VARIABLE:
						//not yet
						break;
					case Argument.types.OPERATION:
						//not yet
						break;
				}
				switch (toCheck.secondArgument.type) 
				{
					case Argument.types.NUMBER:
						Number2 = toCheck.secondArgument.numberValue;
						break;
					case Argument.types.VARIABLE:
						//not yet
						break;
					case Argument.types.OPERATION:
						//not yet
						break;
				}
				if(Number1 >= Number2)
				{
					return true;
				}
				break;
		}
		return elseReturn;
	}


	void corpusInterpreter(string corpus, WhileForm whileObj)
	{
		if (corpus.StartsWith ("move")) 
		{
			int stringIndexInitParam1 = 0;
			int stringIndexEndParam1 = 0;
			int stringIndexEndParam2 = 0;


			bool find1Param = true;
			bool find2Param = false;

			for (int i = 0; i < corpus.Length; i++) 
			{
				if (find1Param) 
				{
					if (corpus [i].Equals ('(')) 
					{
						stringIndexInitParam1 = i+1;
						//find1Param = false;
						//find2Param = true;
						continue;
					}
					if (corpus [i].Equals (',')) 
					{
						stringIndexEndParam1 = i;
						find1Param = false;
						find2Param = true;
						continue;
					}
				}
				else if(find2Param)
				{
					if (corpus [i].Equals (')')) 
					{
						stringIndexEndParam2 = i-1;
						find1Param = true;
						find2Param = false;
						break;
					}
				}
			}
			String firstParam = corpus.Substring (stringIndexInitParam1, stringIndexEndParam1 - stringIndexInitParam1);
			String secondParam = corpus.Substring (stringIndexEndParam1 + 1, stringIndexEndParam2 - stringIndexEndParam1);
			// Debug.Log ("1param: " + corpus.Substring (stringIndexInitParam1, stringIndexEndParam1 - stringIndexInitParam1));
			// Debug.Log ("2param: " + corpus.Substring (stringIndexEndParam1+1, stringIndexEndParam2 - stringIndexEndParam1));
			Command moveCommand = new Command();
			moveCommand.name = "MOVE";
			moveCommand.commandParams.Enqueue (firstParam);
			moveCommand.commandParams.Enqueue (secondParam);
			whileObj.commands.Enqueue (moveCommand);
		}
		pipeline.Enqueue (whileObj);
	}

}
