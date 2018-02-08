using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveElement : ActiveMoveElement {
	

	[HideInInspector]
	public int status;

	void Awake() {
		DoAwake ();
	}

	public override void DoAwake() {
		base.DoAwake ();

		status = Status_Inactive;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override bool IsEnemy() {
		return true;
	}



	public override bool IsColliseWithOthers() {
		return true;
	}
}
