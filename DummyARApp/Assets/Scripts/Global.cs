using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global {
    public static bool ModelRotateFlag = false;
    public static int modelId = -1;
    public static GameObject curObj = null;
	public static string getModel = "https://aura.doors.vision/Aura_Business/getRelatedProductsApi.php";
//	public static string getModel = "http://103.237.72.18:8088/Aura/getRelatedProductsApi.php";
	public static string ModelUrl = "";
    public static List<ModelInfo> gModelInfoList;
	public static bool asset_downflag = true;
	public static GameObject rootObj;
    public static bool is_set_param = false;
	public static int curModelId = -1;
	public static bool is_init = false;
	public static bool is_init_processing = false;

	public static Transform FindRootModel(Transform obj){
		while (obj.gameObject.name != "RootNode") {
			obj = obj.parent.transform;
		}
		return obj.parent.transform;
	}
}

public class ModelInfo
{
    public string model_id;
	public string model_imagelink;
    public string model_assetlink;
}