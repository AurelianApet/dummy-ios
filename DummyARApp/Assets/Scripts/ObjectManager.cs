using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class ObjectManager : MonoBehaviour {
    //public GameObject ObjPrefab;
    public GameObject hitParent;
    public GameObject model;

	// Use this for initialization
	void Start () {
        //AddHitObject();
        AddObj();
	}
	
	// Update is called once per frame
	void Update () {		
	}

    public void AddObj(){
        Debug.Log("AddObj");
        GameObject hitObj = Instantiate(model);
        hitObj.transform.parent = hitParent.transform;
        //hitObj.transform.position = new Vector3(0, 0, 0);
        hitObj.transform.localScale = new Vector3(1f, 1f, 1f);
        hitObj.transform.eulerAngles = new Vector3(0, 0, 0);
        //hitObj.GetComponent<UnityARHitTestExample>().handObjPrefab = ObjPrefab;
        hitObj.GetComponent<UnityARHitTestExample>().m_HitTransform = hitParent.transform;
    }
}
