using UnityEngine;

namespace ILib.Audio
{
	internal class DestroyObserver : MonoBehaviour
	{
		public System.Action OnDestroyEvent;
		private void OnDestroy()
		{
			OnDestroyEvent?.Invoke();
		}
	}
}