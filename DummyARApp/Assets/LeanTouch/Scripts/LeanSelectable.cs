using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Lean.Touch
{
	// This component allows you to select this GameObject via another component
	public class LeanSelectable : MonoBehaviour
	{
		// Event signature
		[System.Serializable] public class LeanFingerEvent : UnityEvent<LeanFinger> {}

		public static List<LeanSelectable> Instances = new List<LeanSelectable>();

		[Tooltip("Should IsSelected temporarily return false if the selecting finger is still being held?")]
		public bool HideWithFinger;

		public bool IsSelected
		{
			get
			{
				// Hide IsSelected?
				if (HideWithFinger == true && isSelected == true && SelectingFinger != null)
				{
					return false;
				}

				return isSelected;
			}
		}

		// This stores the finger that began selection of this LeanSelectable
		// This will become null as soon as that finger releases, which you can detect via OnSelectUp
		[System.NonSerialized]
		public LeanFinger SelectingFinger;

		// Called when selection begins (finger = the finger that selected this)
		public LeanFingerEvent OnSelect;

		// Called when the selecting finger goes up (finger = the finger that selected this)
		public LeanFingerEvent OnSelectUp;

		// Called when this is deselected, if OnSelectUp hasn't been called yet, it will get called first
		public UnityEvent OnDeselect;

		// Is this selectable selected?
		[SerializeField]
		private bool isSelected;

		[ContextMenu("Select")]
		public void Select()
		{
			Select(null);
		}

		// NOTE: Multiple selection is allowed
		public void Select(LeanFinger finger)
		{
			Global.curObj = null;
            Debug.Log("ModelRotate=" + Global.ModelRotateFlag);
            if(!Global.ModelRotateFlag){
                return;
            }
			isSelected      = true;
			SelectingFinger = finger;
            Debug.Log("Enable");
			Transform obj = Global.FindRootModel (this.transform);
			/*
			for (int i = 0; i < GameObject.Find ("ModelParent").transform.childCount; i++) {
				GameObject tmp = GameObject.Find ("ModelParent").transform.GetChild (i).gameObject;
				if (tmp != obj.gameObject) {
					MeshRenderer[] rendererComponents = tmp.transform.GetComponentsInChildren<MeshRenderer>(true);
					foreach (MeshRenderer component in rendererComponents) {
						component.gameObject.GetComponent<Lean.Touch.LeanSelectable> ().isSelected = false;
					}
				}
			}*/
			Global.curObj = obj.gameObject;

			if (OnSelect != null)
			{
				OnSelect.Invoke(finger);
			}
		}

		[ContextMenu("Deselect")]
		public void Deselect()
		{
            if(Global.curObj == null || Global.curObj.activeSelf == false){
                return;
            }
			StartCoroutine(deSelect ());
		}

		private IEnumerator deSelect(){
			yield return new WaitForSeconds(0.1f);
			// Make sure we don't deselect multiple times
			if (isSelected == true)
			{
				isSelected = false;

				if (SelectingFinger != null && OnSelectUp != null)
				{
					OnSelectUp.Invoke(SelectingFinger);
				}

				if (OnDeselect != null)
				{
					OnDeselect.Invoke();
				}

				for (var i = LeanSelect.Instances.Count - 1; i >= 0; i--)
				{
					LeanSelect.Instances[i].RemoveSelectable(this);
				}

				Debug.Log("Disable");
				Global.curObj = null;

				// NOTE: SelectingFinger is set to null in LateUpdate
			}
		}

		protected virtual void OnEnable()
		{
			// Register instance
			Instances.Add(this);
		}

		protected virtual void OnDisable()
		{
			// Unregister instance
			Instances.Remove(this);
			if (isSelected == true)
			{
				Deselect();
			}
		}

		protected virtual void LateUpdate()
		{
			// Null the selecting finger?
			// NOTE: This is done in LateUpdate so certain OnFingerUp actions that require checking SelectingFinger can still work properly
			if (SelectingFinger != null)
			{
				if (SelectingFinger.Set == false || isSelected == false)
				{
					SelectingFinger = null;
				}
			}
		}
	}
}