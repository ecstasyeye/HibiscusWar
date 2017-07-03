using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAni : MonoBehaviour {

	public bool m_Need = false;

	// Update is called once per frame
	void Update () {
		if (m_Need)
		{
			float sizeMath = Mathf.PingPong(Time.time / 4, 0.15f);
			transform.localScale = new Vector3(1 + sizeMath, 1 + sizeMath);
		}
	}
}
