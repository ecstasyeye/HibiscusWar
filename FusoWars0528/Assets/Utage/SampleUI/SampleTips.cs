using UnityEngine;
using System.Collections;

namespace Utage
{

	public class SampleTips : MonoBehaviour
	{

		public void OnClickTips(UguiNovelTextHitArea hit)
		{
			Debug.Log(hit.Arg);
		}
	}
}
