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
	public ArrayList VariableScope;

	void Start () 
	{
		Variable test = new Variable ();
		test.name = "chatuba";
		test.type = Variable.types.FLOAT;
		test.floatValue = 69f;

		pipeline = new Queue ();
		VariableScope = new ArrayList ();

		VariableScope.Add (test);
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

	public void compiller(String Command)
	{
		//String Command = Terminal.gameObject.GetComponentInChildren<Text>().text;

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
		else if(float.TryParse(toSolve, out finalArgument.numberValue))
		{			
			finalArgument.type = Argument.types.NUMBER;
			//finalArgument.numberValue = float.Parse(toSolve);
		}
		else
		{
			Debug.Log ("Variavel encontrada");
			finalArgument.type = Argument.types.VARIABLE;
			foreach (Variable globalElement in VariableScope) 
			{
				if (globalElement.name.Equals (toSolve)) 
				{					
					Debug.Log ("Variavel definida");
					finalArgument.variableValue = globalElement;							
				}
			}
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
				switch (next.name) 
				{
					case "MOVE":
						//Debug.Log ("enter here case");
						object firstParam = next.commandParams.Dequeue ();
						object secondParam = next.commandParams.Dequeue ();
						//Debug.Log ("commands  :" +firstParam + "  "  +secondParam);
						command_move ((firstParam as string), float.Parse((secondParam as string)));
						next.commandParams.Enqueue (firstParam);
						next.commandParams.Enqueue (secondParam);
						break;
					case "DECLARATION":
						object typeParam = next.commandParams.Dequeue ();
						object varNameParam = next.commandParams.Dequeue ();						

						switch (typeParam as string) 
						{
							case "bool":
								command_declaration (varNameParam as string, Variable.types.BOOL);
								break;
							case "float":
								command_declaration (varNameParam as string, Variable.types.FLOAT);
								break;
							case "int":
								command_declaration (varNameParam as string, Variable.types.INT);
								break;
						}
						next.commandParams.Enqueue (typeParam);
						next.commandParams.Enqueue (varNameParam);						
						break;
				case "ASSIGNMENT":
					object nameVarA = next.commandParams.Dequeue ();
					Debug.Log ("var: " +nameVarA as string);
					if (next.commandParams.Count == 3) {
						Debug.Log ("operation");
						object receive1 = next.commandParams.Dequeue ();
						object opA = next.commandParams.Dequeue ();
						object receive2 = next.commandParams.Dequeue ();

						Debug.Log ("rec 1: " + receive1 as string);
						Debug.Log ("op: " + opA as string);
						Debug.Log ("rec 2: " + receive2 as string);

						float item1F = 0;
						float item2F = 0;
						int item1I = 0;
						int item2I = 0;

						if (float.TryParse (receive1 as string, out item1F)) 
						{
							item1F = float.Parse (receive1 as string);
							item1I = (int) item1F;
						} else {
							foreach (Variable elem in VariableScope) {
								if (elem.name.Equals (receive1 as string)) {
									switch (elem.type) {
										case Variable.types.FLOAT:
											item1F = elem.floatValue;
											item1I = (int)item1F;
											break;
										case Variable.types.INT:
											item1F = elem.intValue;
											item1I = elem.intValue;
											break;
									}
								}
							}
						}

						if (float.TryParse (receive2 as string, out item2F)) {
							item2F = float.Parse (receive2 as string);
							item2I = (int) item2F;
						} else {
							foreach (Variable elem in VariableScope) {
								if (elem.name.Equals (receive2 as string)) {
									switch (elem.type) {
									case Variable.types.FLOAT:
										item2F = elem.floatValue;
										item2I = (int)item2F;
										break;
									case Variable.types.INT:
										item2F = elem.intValue;
										item2I = elem.intValue;
										break;
									}
								}
							}
						}
						Debug.Log ("OPERARARARARA: " +opA as string);
						char op = (opA.ToString())[0];
						command_assignment_operation (nameVarA as string, item1I, item1F, item2I, item2F, op);
						next.commandParams.Enqueue (nameVarA);
						next.commandParams.Enqueue (receive1);
						next.commandParams.Enqueue (opA);
						next.commandParams.Enqueue (receive2);
					} else {
						object receive = next.commandParams.Dequeue ();
						float number = 0;
						if (float.TryParse (receive as string, out number)) {
							command_assignment (nameVarA as string, int.Parse (receive as string), float.Parse (receive as string), false);
						} else {
							foreach (Variable elem in VariableScope) {
								if (elem.name.Equals (receive as string)) {
									switch (elem.type) {
									case Variable.types.BOOL:
										command_assignment (nameVarA as string, 0, 0, elem.boolValue);
										break;
									case Variable.types.FLOAT:
										command_assignment (nameVarA as string, 0, elem.floatValue, false);
										break;
									case Variable.types.INT:
										command_assignment (nameVarA as string, elem.intValue, 0, false);
										break;
									}
									break;
								}
							}
						}
						next.commandParams.Enqueue (nameVarA);
						next.commandParams.Enqueue (receive);
					}

						break;
				}
			}
			whileCommand.commands = Executed;

		}
	}

	void command_assignment_operation(string variableName, int valueI1, float valueF1, int valueI2, float valueF2, char OP)
	{
		Debug.Log ("operationinin: " + OP);
		Debug.Log ("enter in comand assing");
		foreach (Variable elem in VariableScope) 
		{
			if (elem.name.Equals (variableName)) 
			{
				Debug.Log ("encontrou variavel increment " +elem.name);
				switch (elem.type) 
				{			
					case Variable.types.FLOAT:
					switch (OP) 
						{
					case '+':
							Debug.Log ("kct");
								elem.floatValue = valueF1 + valueF2;
								break;
							case '-':
								elem.floatValue = valueF1 - valueF2;
								break;
							case '*':
								elem.floatValue = valueF1 * valueF2;
								break;
							case '/':
								elem.floatValue = valueF1 / valueF2;
								break;
						}
						break;
					case Variable.types.INT:
						switch (OP) 
						{

							case '+':
						Debug.Log ("kcilda");
								elem.intValue = valueI1 + valueI2;
								break;
							case '-':
								elem.intValue = valueI1 + valueI2;
								break;
							case '*':
								elem.intValue = valueI1 + valueI2;
								break;
							case '/':
								elem.intValue = valueI1 + valueI2;
								break;
						}
						break;
				}

				Debug.Log ("Variavel atribuida");

				break;
			}
		}
	}

	void command_assignment(string variableName, int valueI, float valueF, bool valueB)
	{
		foreach (Variable elem in VariableScope) 
		{
			if (elem.name.Equals (variableName)) 
			{
				
				switch (elem.type) 
				{
					case Variable.types.BOOL:
						elem.boolValue = valueB;
						break;
					case Variable.types.FLOAT:
						elem.floatValue = valueF;
						break;
					case Variable.types.INT:
						elem.intValue = valueI;
						break;
				}

				Debug.Log ("Variavel atribuida");

				break;
			}
		}

	}

	void command_declaration(string newVariableName, Variable.types typeNew)
	{
		Variable newVar = new Variable ();
		newVar.name = newVariableName;
		newVar.type = typeNew;
		//Debug.Log ("Variavel adicionada " +newVar.name);
		VariableScope.Add (newVar);
		//Debug.Log("kappe");
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
				return f_Item1 == f_Item2;		
			case Expression.ConditionOperator.LessThan:
				Debug.Log (f_Item1 + " < "  +f_Item2);
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

				for (int i = 0; i < item.Length; i++) {
					if (find1Param) {
						if (item [i].Equals ('(')) {
							stringIndexInitParam1 = i + 1;
							//find1Param = false;
							//find2Param = true;
							continue;
						}
						if (item [i].Equals (',')) {
							stringIndexEndParam1 = i;
							find1Param = false;
							find2Param = true;
							continue;
						}
					} else if (find2Param) {
						if (item [i].Equals (')')) {
							stringIndexEndParam2 = i - 1;
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
				Command moveCommand = new Command ();
				moveCommand.initiate ();

				moveCommand.name = "MOVE";
				moveCommand.commandParams.Enqueue (firstParam);
				moveCommand.commandParams.Enqueue (secondParam);
				whileObj.commands.Enqueue (moveCommand);
			}
			else if (item.StartsWith ("bool") || item.StartsWith ("int") || item.StartsWith ("float")) 
			{
				int initIndex = 0;
				Command variableCommand = new Command ();
				variableCommand.initiate ();
				variableCommand.name = "DECLARATION";
				//assignment params, [type, variable, operator 1, operation, operator 2]

				//need name
				//type command
				//value
				//bool isBool = false;
				if (item.StartsWith ("bool")) {
					//isBool = true;
					initIndex = 3;
					variableCommand.commandParams.Enqueue ("bool");
				} else if (item.StartsWith ("int")) {
					initIndex = 2;
					variableCommand.commandParams.Enqueue ("int");
				} else {
					initIndex = 4;
					variableCommand.commandParams.Enqueue ("float");
				}

				int nameVariableIndex = item.Length;
				//int operator1 = initIndex;
				//int operation = initIndex;
				//bool findName = true;
				//bool findOp1 = false;

							
				//else if (!isBool && findOp1) 
				//{
				//	if (item [index].Equals ('+') || item [index].Equals ('-') || item [index].Equals ('*') || item [index].Equals ('/')) 
				//	{
				//		operator1 = index;
				//		findOp1 = false;
				//		operation = index;
				//	}
				//}

			
				//Debug.Log ("VARIABLE 1 ASSIGNMET NAME: " + item.Substring ((initIndex + 1), (nameVariableIndex - (initIndex + 1))));
				//Debug.Log ("OPERATOR 1 ASSIGNMET NAME: " + item.Substring((
				//	nameVariableIndex+1), item.Length-(nameVariableIndex+1)));
				//Debug.Log ("VARIABLE ASSIGNMET OPERATION: " + item[operation]);
				//Debug.Log ("OPERATOR 2 ASSIGNMET NAME: " + item.Substring((operation+1), item.Length - (operation+1)));


				variableCommand.commandParams.Enqueue (item.Substring ((initIndex + 1), (nameVariableIndex - (initIndex + 1))));
				//variableCommand.commandParams.Enqueue (item.Substring((nameVariableIndex+1), item.Length-(nameVariableIndex+1)));
				//variableCommand.commandParams.Enqueue (item[operation]);
				//variableCommand.commandParams.Enqueue (item.Substring((operation+1), item.Length - (operation+1)));


				whileObj.commands.Enqueue (variableCommand);
			}
			else 
			{
				Command variableCommand = new Command ();
				variableCommand.initiate ();
				variableCommand.name = "ASSIGNMENT";

				int equalIndex = 0;
				for(int index = 0; index < item.Length; index++)
				{
					if(item[index].Equals('='))
					{
						equalIndex = index;
					}
				}
				string variable = item.Substring (0, equalIndex);
				string operationOverVariable = item.Substring (equalIndex+1, item.Length-(equalIndex+1));
				foreach(Variable elem in VariableScope)
				{
					if(elem.name.Equals(variable))
					{
						variableCommand.commandParams.Enqueue (variable);
						Debug.Log ("VARIAVEL ENCONTRADA");
						if (operationOverVariable.Contains ('+') || operationOverVariable.Contains ('-') || operationOverVariable.Contains ('*') || operationOverVariable.Contains ('/')) 
						{
							char[] keys = { '*', '/', '+', '-' };
							int opIndex = operationOverVariable.IndexOfAny (keys);
											
							variableCommand.commandParams.Enqueue (operationOverVariable.Substring (0, opIndex));
							variableCommand.commandParams.Enqueue (operationOverVariable [opIndex]);
							variableCommand.commandParams.Enqueue (operationOverVariable.Substring (opIndex + 1, operationOverVariable.Length - (opIndex + 1)));

							Debug.Log ("command: " + operationOverVariable.Substring (0, opIndex) + "  " + operationOverVariable [opIndex] + "  " + operationOverVariable.Substring (opIndex + 1, operationOverVariable.Length - (opIndex + 1)));
						}
						else
						{
							variableCommand.commandParams.Enqueue (operationOverVariable);
							Debug.Log ("command: " + operationOverVariable);
						}
						break;
					}
				}
				whileObj.commands.Enqueue (variableCommand);
			}

		}
			
		pipeline.Enqueue (whileObj);
	}

}
