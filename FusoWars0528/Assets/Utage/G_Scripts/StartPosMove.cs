using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPosMove : MonoBehaviour {

	//private Vector3 m_PosStart = new Vector3(-800, 40, 0);
	private Vector3 m_PosEnd = new Vector3(0, 40, 0);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(transform.localPosition, m_PosEnd)<0.1f)
		{
			Destroy(this);
			return;
		}
		transform.localPosition = Vector3.Lerp(transform.localPosition, m_PosEnd, 0.075f);
	}
}
