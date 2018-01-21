using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapElementWallSlopeUp01Follow : MapElement {

	void Awake() {
		type = Type_WallSlope01Follow;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override int IsBlock( int dir ) {
		switch(dir) {
		case MapElement.Dir_Left:
		case MapElement.Dir_Up:
			return Block_None;
		}

		return Block_Hard;
	}
}
