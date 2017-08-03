using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SFramework
{
	public class AutoDisable : MonoBehaviour
	{
		public enum Disable { SetFalse, Destroy }

		public Disable disable = Disable.Destroy;

		public float time = 1;


		void OnEnable()
		{
			StartCoroutine(Wait());
		}

		IEnumerator Wait()
		{
			yield return new WaitForSeconds(time);
			if (disable == Disable.SetFalse)
				gameObject.SetActive(false);
			else
				Destroy(gameObject);
		}
    }
}