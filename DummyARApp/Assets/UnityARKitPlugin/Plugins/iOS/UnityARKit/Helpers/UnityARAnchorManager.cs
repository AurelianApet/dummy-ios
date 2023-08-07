using System;
using System.Collections.Generic;
using System.Linq;
using Collections.Hybrid.Generic;

namespace UnityEngine.XR.iOS
{
	public class UnityARAnchorManager 
	{


		private LinkedListDictionary<string, ARPlaneAnchorGameObject> planeAnchorMap;


        public UnityARAnchorManager ()
		{
			planeAnchorMap = new LinkedListDictionary<string,ARPlaneAnchorGameObject> ();
			UnityARSessionNativeInterface.ARAnchorAddedEvent += AddAnchor;
			UnityARSessionNativeInterface.ARAnchorUpdatedEvent += UpdateAnchor;
			UnityARSessionNativeInterface.ARAnchorRemovedEvent += RemoveAnchor;

		}


		public void AddAnchor(ARPlaneAnchor arPlaneAnchor)
		{
            if (Global.ModelRotateFlag)
                return;
			GameObject go = UnityARUtility.CreatePlaneInScene (arPlaneAnchor);
			go.AddComponent<DontDestroyOnLoad> ();  //this is so these GOs persist across scene loads
			ARPlaneAnchorGameObject arpag = new ARPlaneAnchorGameObject ();
			arpag.planeAnchor = arPlaneAnchor;
			arpag.gameObject = go;
			planeAnchorMap.Add (arPlaneAnchor.identifier, arpag);

            if(GameObject.Find("Manage").GetComponent<UIManager>().DesObj.activeInHierarchy == false)
                GameObject.Find("Manage").GetComponent<UIManager>().ShowDescription();
		}

		public void RemoveAnchor(ARPlaneAnchor arPlaneAnchor)
		{
			if (planeAnchorMap.ContainsKey (arPlaneAnchor.identifier)) {
				ARPlaneAnchorGameObject arpag = planeAnchorMap [arPlaneAnchor.identifier];
				GameObject.Destroy (arpag.gameObject);
				planeAnchorMap.Remove (arPlaneAnchor.identifier);
			}
		}

		public void UpdateAnchor(ARPlaneAnchor arPlaneAnchor)
		{
            if (Global.ModelRotateFlag)
                return;
			if (planeAnchorMap.ContainsKey (arPlaneAnchor.identifier)) {
				ARPlaneAnchorGameObject arpag = planeAnchorMap [arPlaneAnchor.identifier];
				UnityARUtility.UpdatePlaneWithAnchorTransform (arpag.gameObject, arPlaneAnchor);
				arpag.planeAnchor = arPlaneAnchor;
				planeAnchorMap [arPlaneAnchor.identifier] = arpag;
			}
            else
            {
                if (GameObject.Find("Manage").GetComponent<UIManager>().DesObj.activeInHierarchy == false)
                    GameObject.Find("Manage").GetComponent<UIManager>().ShowDescription();
            }
		}

        public void UnsubscribeEvents()
        {
            UnityARSessionNativeInterface.ARAnchorAddedEvent -= AddAnchor;
            UnityARSessionNativeInterface.ARAnchorUpdatedEvent -= UpdateAnchor;
            UnityARSessionNativeInterface.ARAnchorRemovedEvent -= RemoveAnchor;
        }

        public void Destroy()
        {
            foreach (ARPlaneAnchorGameObject arpag in GetCurrentPlaneAnchors()) {
                GameObject.Destroy (arpag.gameObject);
            }

            planeAnchorMap.Clear ();
            UnsubscribeEvents();
        }

		public LinkedList<ARPlaneAnchorGameObject> GetCurrentPlaneAnchors()
		{
			return planeAnchorMap.Values;
		}

        public void DisablePlaneObjs()
        {
            
        }
	}
}

