using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StyleMouse : MonoBehaviour {

    public Texture2D ponteiroPadrao;
    public Texture2D ponteiroNoTablet;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSport = Vector2.zero;

    // Use this for initialization
    void Start () {
        Cursor.SetCursor(ponteiroPadrao,hotSport,cursorMode);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseEnter()
    {
        if (gameObject.tag == "tablet")
        {
            Cursor.SetCursor(ponteiroNoTablet,hotSport,cursorMode);
        }
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(ponteiroPadrao, hotSport, cursorMode);
    }
}
