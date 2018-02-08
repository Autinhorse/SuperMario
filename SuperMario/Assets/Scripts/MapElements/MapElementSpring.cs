using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapElementSpring : MapElement {

	void Awake() {
		type = Type_Spring;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override int IsBlock( int dir ) {
		return Block_Hard;
	}
}
