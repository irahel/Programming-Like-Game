using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoorActivator : MonoBehaviour 
{
	public BoxCollider2D DOOR_COLLIDER;
	public Animator ANIM_CONTROL_door_left;
	public Animator ANIM_CONTROL_door_right;
	public Animator ANIM_CONTROL_door_activator;

	public GameObject Terminal;
	public player playerScript;

	void Start () 
	{
		
	}
	

	void Update () 
	{
		
	}

	//Mouse Click
	void OnMouseDown()
	{
		Terminal.SetActive (true);
		//Terminal.SetActive (true);
		playerScript.TerminalOn = true;
	}

	void doWhatUNeed()
	{
		DOOR_COLLIDER.enabled = false;
		ANIM_CONTROL_door_left.SetTrigger ("Open");
		ANIM_CONTROL_door_right.SetTrigger ("Open");
		ANIM_CONTROL_door_activator.SetTrigger ("Active");
		Terminal.SetActive (false);
		playerScript.TerminalOn = false;
		Debug.Log ("End Well");

	}

	public void getEntry(string entry)
	{
		try
		{
			if (entry.StartsWith("atribuicao"))
			{
				Debug.Log("1 verification");
				ArrayList parts = new ArrayList(entry.Split(':'));
				Debug.Log(parts.Count);
				Debug.Log(parts[0]);
				Debug.Log(parts[1]);
				parts[1] = (parts[1] as string).Replace(")", "");
				parts[1] = (parts[1] as string).Replace(" ", "");
				parts[1] = (parts[1] as string).Replace(";", "");
				Debug.Log(parts[1]);
				if((parts[1] as string).ToLower().Equals("verdadeiro"))
				{
					Debug.Log("2 verification");
					ArrayList sub_parts = new ArrayList((parts[0] as string).Split(' '));
					Debug.Log(sub_parts.Count);
					Debug.Log(sub_parts[2]);
					if (sub_parts[1].Equals("energia"))
					{
						Debug.Log("3 verification");
						doWhatUNeed();
					}
					else
					{
						throw new System.Exception("Dumb error");
					}
				}
				else
				{
					throw new System.Exception("Type error");
				}
			}
			else
			{
				throw new System.Exception("Start error");
			};
		}
		catch (Exception e)
		{
			Terminal.SetActive (false);
			Debug.Log ("False");
		}		
	}
}
