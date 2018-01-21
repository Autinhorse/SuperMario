using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapElementWallFixed : MapElement {

	void Awake() {
		type = Type_WallFixed;
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
