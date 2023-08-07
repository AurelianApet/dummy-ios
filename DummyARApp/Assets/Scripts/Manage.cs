using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Manage : MonoBehaviour {
	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void quitUnity();

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TaskOnClick()
	{
		Global.curObj = null;
		Global.curModelId = -1;
		Global.modelId = -1;
		Global.is_set_param = false;
		Global.gModelInfoList = null;
		Global.ModelRotateFlag = false;
		GameObject.Find ("Canvas/ModelRotate").GetComponent<UnityEngine.UI.Toggle>().isOn = false;
		StartCoroutine (initializeParam ());
	}

	IEnumerator initializeParam(){
		Debug.Log ("start init--");
		GameObject scrObj = GameObject.Find ("Canvas/ScrollView/Content");
		for (int i = 0; i < scrObj.transform.childCount; i++) {
			DestroyImmediate (scrObj.transform.GetChild(i).gameObject);
			yield return new WaitForSeconds (0.1f);
			i--;
		}
		scrObj = GameObject.Find ("ModelParent");
		for (int i = 0; i < scrObj.transform.childCount; i++) {
			if (scrObj.transform.GetChild (i).gameObject.name.ToString () != "HitObject(Clone)") {
				DestroyImmediate (scrObj.transform.GetChild (i).gameObject);
				yield return new WaitForSeconds (0.1f);
				i--;
			} else {
				GameObject tmp = GameObject.Find ("ModelParent/HitObject(Clone)");
				for (int j = 0; j < tmp.transform.childCount; j++) {
					DestroyImmediate (tmp.transform.GetChild (j).gameObject);
					yield return new WaitForSeconds (0.1f);
					j--;
				}
			}
		}
		//Global.is_init = true;
		Debug.Log("end init--");
		#if UNITY_IOS && !UNITY_EDITOR
		quitUnity();
		#endif
	}

    public void setParam(string pid)
    {
		Global.is_init = true;
		if (pid != "") {
			if (int.Parse (pid) > 0) {
				Global.modelId = int.Parse(pid);
				Global.ModelUrl = Global.getModel + "?id=" + Global.modelId;
				Debug.Log("parameter successfully set.");
			}
		}
		Global.is_set_param = true;
    }
}
