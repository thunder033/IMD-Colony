using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path {

	public List<PathNode> nodes;

	public Vector3 displacement;

	public Path()
	{
		nodes = new List<PathNode>();
	}

}
