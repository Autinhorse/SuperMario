using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapElementWallBoard : MapElement {

	void Awake() {
		type = Type_WallBoard;
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public override int IsBlock( int dir ) {
		if (dir == Dir_Up) {
			return Block_Hard;
		}
		return Block_None;
	}
}
