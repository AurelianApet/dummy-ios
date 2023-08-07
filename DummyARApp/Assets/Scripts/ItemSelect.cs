using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelect : MonoBehaviour {
	public int itemID;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnMouseClick()
	{
		Debug.Log("Selected Item : " + itemID + " ----");
		Global.curModelId = itemID;
	}
}
