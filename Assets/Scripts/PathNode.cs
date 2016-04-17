using UnityEngine;
using System.Collections;

public class PathNode {

	public Node node;
	public double f;
	public double g;
	public double h;

	public PathNode(Node aNode)
	{
		node = aNode;

		f = 0;
		g = 0;
		h = 0;
	}
}
