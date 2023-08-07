using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using System.IO;
using UnityEngine.UI;
using LitJson;
using TriLib;
using Lean.Touch;

public class UIManager : MonoBehaviour {
	public GameObject ScrollObj;
    public GameObject MenuObj;
    public GameObject OpenBtnObj;
    public GameObject CloseBtnObj;
    public GameObject DesObj;
    public GameObject GeneratePlaneObj;
    public GameObject RmvObj;
	public GameObject LoadingObj;
	public GameObject PercentObj;
	public GameObject SContentObj;
	GameObject curObj;
	// Use this for initialization
	IEnumerator Start () {
		if (Global.is_init) {
			yield return null;
		}
		Global.is_init_processing = true;
		if(UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX || Screen.height > Screen.width * 2.0f || Screen.width > Screen.height * 2.0f)
		{
			ScrollObj.GetComponent<RectTransform>().offsetMin = new Vector2(ScrollObj.GetComponent<RectTransform>().offsetMin.x+125, ScrollObj.GetComponent<RectTransform>().offsetMin.y);
		}

		if (GameObject.Find("TextureBufferCamera"))
		{
			GameObject.Destroy(GameObject.Find("TextureBufferCamera"));
		}
		//Resources.UnloadUnusedAssets();
		LoadingObj.SetActive(false);
        while (!Global.is_set_param)
        {
            yield return new WaitForSeconds(0.1f);
        }
		if (Global.modelId == -1) {
			Global.is_init_processing = false;
			yield break;
		}
		WWW www = new WWW(Global.ModelUrl);
        Debug.Log("param set");
		yield return LoadModelInfo(www);
		//StartCoroutine ("LoadModel");
	}
		
	IEnumerator LoadModel()
	{
		curObj = null;
		if (Global.ModelRotateFlag)
			yield break;
		Debug.Log("start load model--" + Global.curModelId);
		var assetLoaderOptions = AssetLoaderOptions.CreateInstance();
		//assetLoaderOptions.DontLoadCameras = false;
		//assetLoaderOptions.DontLoadLights = false;
		assetLoaderOptions.UseCutoutMaterials = true;

		GameObject gbo = null;
		using (var assimpLoader = new AssetLoader())
		{
			try
			{
				#if (UNITY_WINRT && !UNITY_EDITOR_WIN)
				_rootGameObject = assimpLoader.LoadFromMemory(fileBytes, filename, assetLoaderOptions);
				#else
				gbo = assimpLoader.LoadFromFile(Global.gModelInfoList[Global.curModelId].model_assetlink, assetLoaderOptions);
				#endif
			}
			catch (System.Exception exception)
			{
				Debug.Log("----Error : " + exception.Message + "----");
			}
		}

		if (gbo != null)
		{
			MeshRenderer[] rendererComponents = gbo.transform.GetComponentsInChildren<MeshRenderer>(true);
			// Enable rendering:
			foreach (MeshRenderer component in rendererComponents)
			{
				for (int i = 0; i < component.materials.Length; i++)
				{
					//component.materials[i].shader = Shader.Find("Legacy Shaders/Self-Illumin/Diffuse");
					//component.materials[i].color = new Color((float)150 / 255, (float)150 / 255, (float)150 / 255, 1);
				}
				component.enabled = true;
				component.gameObject.AddComponent<MeshCollider> ();
				//gbo.AddComponent<Lean.Touch.LeanScale> ();
				component.gameObject.AddComponent<Lean.Touch.LeanSelectable> ();
				component.gameObject.AddComponent<Lean.Touch.LeanRotate> ();
				component.gameObject.AddComponent<Lean.Touch.LeanTranslate> ();
				component.gameObject.GetComponent<Lean.Touch.LeanTranslate> ().RequiredFingerCount = 1;
				component.gameObject.GetComponent<Lean.Touch.LeanRotate> ().RequiredSelectable = gbo.GetComponent<Lean.Touch.LeanSelectable>();
			}
			gbo.transform.parent = this.transform;
			gbo.transform.localScale = new Vector3(1f, 1, 1);
			gbo.transform.localPosition = new Vector3(0, 0, 0);
			gbo.transform.eulerAngles = new Vector3 (0, 0, 0);
			gbo.SetActive(false);
			curObj = gbo;
			Debug.Log("end load model" + Global.curModelId);
		}
	}

	// Update is called once per frame
	IEnumerator initFun(){
		Global.is_init_processing = true;
		Debug.Log ("init thread");
		if(UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX || Screen.height > Screen.width * 2.0f || Screen.width > Screen.height * 2.0f)
		{
			ScrollObj.GetComponent<RectTransform>().offsetMin = new Vector2(ScrollObj.GetComponent<RectTransform>().offsetMin.x+125, ScrollObj.GetComponent<RectTransform>().offsetMin.y);
		}

		if (GameObject.Find("TextureBufferCamera"))
		{
			GameObject.Destroy(GameObject.Find("TextureBufferCamera"));
		}
		//Resources.UnloadUnusedAssets();

		LoadingObj.SetActive(false);
		Debug.Log ("waiting for setting param--");
		while (!Global.is_set_param)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (Global.modelId == -1) {
			Global.is_init_processing = false;
			yield break;
		}
		Debug.Log ("init set param");
		WWW www = new WWW(Global.ModelUrl);
		Debug.Log("param set");
		yield return LoadModelInfo(www);
	}

	void Update () {
		if (Global.is_init && !Global.is_init_processing) {
			Global.is_init = false;
			StopCoroutine ("DownloadFile");
			StopCoroutine ("initFun");
			StartCoroutine (initFun ());
		}
	}

    public void OnOpenBtnClick()
    {
        OpenBtnObj.SetActive(false);
        CloseBtnObj.SetActive(true);

        MenuObj.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void OnCloseBtnClick()
    {
        OpenBtnObj.SetActive(true);
        CloseBtnObj.SetActive(false);

        MenuObj.transform.localPosition = new Vector3(200, 0, 0);
    }

	public void OnModel(int mid)
    {
        GeneratePlaneObj.GetComponent<UnityARGeneratePlane>().OnDisable();
        GeneratePlaneObj.GetComponent<UnityARGeneratePlane>().OnEnable();
		Global.modelId = mid;
        //OnCloseBtnClick();
    }

    public void OnHorizontalBtnClick()
    {
        GeneratePlaneObj.GetComponent<UnityARGeneratePlane>().OnDisable();
        GeneratePlaneObj.GetComponent<UnityARGeneratePlane>().OnEnable();

        GameObject.Find("ARCameraManager").GetComponent<UnityARCameraManager>().planeDetection = UnityARPlaneDetection.Horizontal;
        GameObject.Find("ARCameraManager").GetComponent<UnityARCameraManager>().InitCamera();

        OnCloseBtnClick();
    }

    public void OnVerticalBtnClick()
    {
        GeneratePlaneObj.GetComponent<UnityARGeneratePlane>().OnDisable();
        GeneratePlaneObj.GetComponent<UnityARGeneratePlane>().OnEnable();

        GameObject.Find("ARCameraManager").GetComponent<UnityARCameraManager>().planeDetection = UnityARPlaneDetection.Vertical;
        GameObject.Find("ARCameraManager").GetComponent<UnityARCameraManager>().InitCamera();
        OnCloseBtnClick();
    }

    public void OnRoofBtnClick()
    {
        GeneratePlaneObj.GetComponent<UnityARGeneratePlane>().OnDisable();
        GeneratePlaneObj.GetComponent<UnityARGeneratePlane>().OnEnable();

        GameObject.Find("ARCameraManager").GetComponent<UnityARCameraManager>().planeDetection = UnityARPlaneDetection.HorizontalAndVertical;
        GameObject.Find("ARCameraManager").GetComponent<UnityARCameraManager>().InitCamera();
        OnCloseBtnClick();
    }

    public void ShowDescription()
    {
        //DesObj.SetActive(true);
    }

    public void OnCheckBoxChanged()
    {
		if (GameObject.Find ("Canvas/ModelRotate").GetComponent<UnityEngine.UI.Toggle> ().isOn) {
			Global.ModelRotateFlag = true;
		} else {
			Global.ModelRotateFlag = false;
		}
        if(Global.ModelRotateFlag)
        {
            RmvObj.SetActive(true);
            GeneratePlaneObj.SetActive(false);
        }
        else
        {
            RmvObj.SetActive(false);
            GeneratePlaneObj.SetActive(true);
        }
    }

    public void OnRemove(){
        Debug.Log("1111");
        if(Global.curObj && Global.ModelRotateFlag){
            Debug.Log("222");
            DestroyObject(Global.curObj);
        }
    }

	IEnumerator LoadModelInfo(WWW www)
	{
		if (Global.gModelInfoList != null && Global.gModelInfoList.Count > 0)
		{
			while (Global.gModelInfoList.Count > 0)
				Global.gModelInfoList.RemoveAt(0);
			Global.gModelInfoList = null;
		}
		Global.gModelInfoList = new List<ModelInfo>();

		#if UNITY_IPHONE
		Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.Gray);
		#elif UNITY_ANDROID
		Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
		#endif
		Handheld.StartActivityIndicator();
		yield return new WaitForSeconds(0.5f);

		yield return www;
		if (www.error == null)
		{
			JsonData json_info = JsonMapper.ToObject(www.text);
			Debug.Log ("count=" + json_info.Count);
			if (json_info.Count > 0)
			{
				//Create directory
				string thumb_folder = Application.persistentDataPath + "/thumbs/";
				if (!System.IO.Directory.Exists(thumb_folder))
				{
					System.IO.Directory.CreateDirectory(thumb_folder);
				}
				int initedModelCount = 0;
				LoadingObj.SetActive (true);
				for (int i = 0; i < json_info.Count; i++)
				{
					Debug.Log ("start---" + i);
					ModelInfo newModel = new ModelInfo();
					newModel.model_id = json_info[i]["id"].ToString();
					newModel.model_assetlink = json_info[i]["model"].ToString();
					newModel.model_imagelink = json_info[i]["image"].ToString();
					string thumbName = "";
					if (newModel.model_imagelink.Trim().Equals("") || newModel.model_assetlink.Trim().Equals(""))
						continue;
					if (!newModel.model_assetlink.Trim().Contains(".fbx"))
						continue;
					thumbName = newModel.model_imagelink.Split('/')[newModel.model_imagelink.Split('/').Length - 1];

					Global.asset_downflag = true;

					Debug.Log("Thumb file : " + thumb_folder + thumbName);
					if (!File.Exists(thumb_folder + thumbName))
						yield return StartCoroutine(DownloadFile(thumb_folder, thumbName, newModel.model_imagelink));

					newModel.model_imagelink = thumb_folder + thumbName;
					Global.gModelInfoList.Add(newModel);
					InitModelList(initedModelCount);
					initedModelCount++;
					/////////Download Models
					string assetName = "";
					assetName = Global.gModelInfoList[i].model_assetlink.Split('/')[Global.gModelInfoList[i].model_assetlink.Split('/').Length - 1];
					string asset_Folder = Application.persistentDataPath + "/Assets/";
					if (!System.IO.Directory.Exists(asset_Folder))
					{
						System.IO.Directory.CreateDirectory(asset_Folder);
					}

					Debug.Log("Asset File : " + asset_Folder + assetName);
					if (!File.Exists(asset_Folder + assetName))
						yield return StartCoroutine(DownloadFile(asset_Folder, assetName, Global.gModelInfoList[i].model_assetlink));

					Handheld.StopActivityIndicator();

					Debug.Log ("i=" + i + ":" + Global.asset_downflag);
					if (Global.asset_downflag == true)
					{
						Debug.Log ("Download model finished:" + asset_Folder + assetName);
						Global.gModelInfoList[i].model_assetlink = asset_Folder + assetName;
						PercentObj.GetComponent<Text>().text = "100%";
					}
					else
					{
						PercentObj.SetActive(false);
						LoadingObj.SetActive(false);
						yield break;
					}
					///////////Finish download
					yield return new WaitForSeconds(0.2f);
					Debug.Log ("end---" + i);
				}
			}
			else
			{
				Debug.Log("---Data is empty---");
			}
			LoadingObj.SetActive (false);
			Debug.Log ("Download finished");
			Global.is_init_processing = false;
		}
		else
		{
            Debug.Log("----Json Error----" + www.error);
		}

		Handheld.StopActivityIndicator();
	}

	public void InitModelList(int id)
	{
		Texture2D texture;
		texture = new Texture2D(2, 2);
		texture.LoadImage(loadImage(Global.gModelInfoList[id].model_imagelink));
		Rect r = new Rect(Vector2.zero, new Vector2(texture.width, texture.height));
		ScrollObj.GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, 250);
		GameObject sObj = (GameObject)Instantiate(SContentObj, new Vector3(0, 0, 0), Quaternion.identity);
        Vector3 sp = sObj.transform.position;
        sp.x += 250 * id;
        sp.y = 125;
        sObj.transform.position = sp;
		sObj.transform.parent = ScrollObj.transform;
		sObj.transform.Find("Image").GetComponent<Image>().sprite = Sprite.Create(texture, r, Vector2.one * 0.5f);
		sObj.GetComponent<ItemSelect>().itemID = id;
		texture = null;
		Destroy(texture);
		sObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        ScrollObj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 250 * (id + 1));
	}

	public byte[] loadImage(string path)
	{
		byte[] dataByte = null;

		//Exit if Directory or File does not exist
		if (!Directory.Exists(Path.GetDirectoryName(path)))
		{
			Debug.LogWarning("Directory does not exist");
			return null;
		}

		if (!File.Exists(path))
		{
			Debug.Log("File does not exist");
			return null;
		}

		try
		{
			dataByte = File.ReadAllBytes(path);
			Debug.Log("Loaded Data from: " + path.Replace("/", "\\"));
		}
		catch (Exception e)
		{
			Debug.LogWarning("Failed To Load Data from: " + path.Replace("/", "\\"));
			Debug.LogWarning("Error: " + e.Message);
		}

		return dataByte;
	}

    IEnumerator DownloadFile(string targetPath, string fileName, string fileURL)
    {
		PercentObj.SetActive(true);
		Debug.Log ("start download file--" + fileURL);
		WWW file_www = new WWW(fileURL);
		while(!file_www.isDone)
		{
			if (Global.is_init) {
				Debug.Log ("stop downloading--");
				Global.asset_downflag = true;
				break;
			}
			Debug.Log (((int)(file_www.progress * 100)).ToString() + "%");
			PercentObj.GetComponent<Text>().text = ((int)(file_www.progress * 100)).ToString() + "%";
			yield return null;
		}
		if (!Global.is_init) {
			yield return file_www;
			Debug.Log ("end download file" + fileURL);
			if (file_www.error == null)
			{
				byte[] fileByte = file_www.bytes;
				File.WriteAllBytes(targetPath + fileName, fileByte);
				Global.asset_downflag = true;
				Debug.Log("Saved file : " + targetPath + fileName);
			}else
			{
				Debug.Log ("down error:" + file_www.error);
				if (fileName.Contains(".fbx"))
					Global.asset_downflag = false;
			}
		}
    }
}
