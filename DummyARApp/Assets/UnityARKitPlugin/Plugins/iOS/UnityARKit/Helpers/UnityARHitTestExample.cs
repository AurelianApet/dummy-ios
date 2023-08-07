using System;
using System.Collections;
using System.Collections.Generic;
using TriLib;
using Lean.Touch;
namespace UnityEngine.XR.iOS
{
	public class UnityARHitTestExample : MonoBehaviour
	{
		public Transform m_HitTransform;
		public float maxRayDistance = 30.0f;
        public LayerMask collisionLayer = 1 << 10;  //ARKitPlane layer
        //public GameObject handObjPrefab;
		GameObject curObj;

		void Start()
        {
			
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

        bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes)
        {
            Debug.Log("2222222");

            List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
            if (hitResults.Count > 0) {
                Debug.Log("333333");
                foreach (var hitResult in hitResults) {
                    Debug.Log ("Got hit!");
                    //Create a new gameobject and set as child of current object.
//					GameObject rootObj = Instantiate(this.gameObject);
					GameObject newHandObj = Instantiate(curObj);
                    newHandObj.transform.parent = this.transform;
                    newHandObj.transform.position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                    newHandObj.transform.localScale = new Vector3(1, 1, 1);
                    newHandObj.transform.rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
                    newHandObj.SetActive(true);
                    this.GetComponent<UnityARHitTestExample>().enabled = false;
                    if (GameObject.Find("Manage").GetComponent<UIManager>().DesObj.activeInHierarchy == true)
                        GameObject.Find("Manage").GetComponent<UIManager>().DesObj.SetActive(false);
                    //m_HitTransform.position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
                    //m_HitTransform.rotation = UnityARMatrixOps.GetRotation (hitResult.worldTransform);
                    GameObject.Find("Manage").GetComponent<ObjectManager>().AddObj();
                    Debug.Log (string.Format ("x:{0:0.######} y:{1:0.######} z:{2:0.######}", m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));
					Global.curModelId = -1;
                    return true;
                }
            }
            return false;
        }

        public bool IsPointerOverUI()
        {
            EventSystems.EventSystem system = EventSystems.EventSystem.current;
            if(system != null)
            {
                if(Application.isMobilePlatform)
                {
                    if(Input.touchCount > 0)
                    {
                        return system.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
                    }
                }
                else
                {
                    return system.IsPointerOverGameObject();
                }
            }
            return false;
        }

        // Update is called once per frame
        void Update()
        {
			//if (!Global.is_download_model || Global.curModelId == -1)
			//	return;
#if UNITY_EDITOR   //we will only use this script on the editor side, though there is nothing that would prevent it from working on device
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                //we'll try to hit one of the plane collider gameobjects that were generated by the plugin
                //effectively similar to calling HitTest with ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent
                if (Physics.Raycast(ray, out hit, maxRayDistance, collisionLayer))
                {
                    //we're going to get the position from the contact point
                    m_HitTransform.position = hit.point;
                    Debug.Log(string.Format("x:{0:0.######} y:{1:0.######} z:{2:0.######}", m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));

                    //and the rotation from the transform of the plane collider
                    m_HitTransform.rotation = hit.transform.rotation;
                }
            }
#else

            if (IsPointerOverUI() == true)
                return;
            
			if (Input.touchCount > 0 && m_HitTransform != null)
			{
				var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)// || touch.phase == TouchPhase.Moved)
				{
					Debug.Log("modelId=" + Global.curModelId);
					StartCoroutine(LoadModel());
					if(curObj == null)
						return;
					var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
					ARPoint point = new ARPoint {
						x = screenPosition.x,
						y = screenPosition.y
					};

                    // prioritize reults types
                    ARHitTestResultType[] resultTypes = {
						//ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingGeometry,
                        ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
                        // if you want to use infinite planes use this:
                        //ARHitTestResultType.ARHitTestResultTypeExistingPlane,
                        //ARHitTestResultType.ARHitTestResultTypeEstimatedHorizontalPlane, 
						//ARHitTestResultType.ARHitTestResultTypeEstimatedVerticalPlane, 
						//ARHitTestResultType.ARHitTestResultTypeFeaturePoint
                    };
					
                    foreach (ARHitTestResultType resultType in resultTypes)
                    {
                        Debug.Log("111111");
                        if (HitTestWithResultType (point, resultType))
                        {
                            return;
                        }
                    }
				}
			}
#endif
        }
	}
}

