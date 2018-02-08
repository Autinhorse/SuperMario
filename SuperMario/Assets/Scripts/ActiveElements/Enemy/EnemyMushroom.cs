using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class EnemyMushroom : EnemyMoveElement {

	void Awake() {
		DoAwake ();
		colliseWidth = 0.8f;
		colliseHeight = 0.8f;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
		//DoFixedUpdate ();
	}

	public override bool JumpOnHead() {
		if (_status == Status_Dead) {
			return false;
		}

		_status = Status_Dead;

		DOTween.Play (SptMain.transform.DOScaleY (0, 0.45f));
		DOTween.Play (SptMain.transform.DOLocalMoveY (-0.5f, 0.45f).OnComplete(()=>{
			gameObject.SetActive( false );
		} ) );

		return false;
	}

	public override void Reset() {
		gameObject.SetActive (true);
		SptMain.transform.localScale = Vector3.one;
		SptMain.transform.localPosition = Vector3.zero;
		transform.position = StartPos;
		Color color = SptMain.color;
		color.a = 1.0f;
		SptMain.color = color;

		_status = Status_Inactive;
	}
}
