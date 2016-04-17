using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Transform))]

public class Node : MonoBehaviour {

	public bool discovered;

    public Node[] connections;

	//used for A*
	public double f;
	public double g;
	public double h;
	public Node parent;

	// Use this for initialization
	void Start () {
		//connections = new Node[0];
	}
	
	// Update is called once per frame
	void Update () {
		if(connections.Length < 1)
		{
			ConnectToClosest();
		}
	}

	public void ConnectToClosest()
	{
		Node closestNode = FindClosestVisible(position);
		closestNode.AddConnection(this);
		AddConnection(closestNode);

		//Array.Resize(ref closestNode.connections, closestNode.connections.Length + 1);
		//closestNode.connections[closestNode.connections.Length - 1] = this;
		//Array.Resize(ref connections, connections.Length + 1);
		//connections[connections.Length - 1] = closestNode;
	}

	/// <summary>
	/// Findes the closest node to a given position that is visible (not obstructed by terrain)
	/// </summary>
	/// <param name="pos">Position to check from</param>
	/// <returns>Closest visible node</returns>
	public static Node FindClosestVisible(Vector3 pos)
	{
		//Get a list of all nodes in the scene
		GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

		//Arrange them from nearest to farthest
		Array.Sort(waypoints, delegate (GameObject a, GameObject b)
			{ return (a.GetComponent<Node>().position - pos).magnitude.CompareTo((b.GetComponent<Node>().position - pos).magnitude); }
		);

		//Next we have to check if the closest one is visible
		Node closest;
		RaycastHit hit;
		int waypointIndex = 0;

		bool hitsTerrain = false;
		do
		{
			hitsTerrain = false;
			//get the current node while incrementing index
			closest = waypoints[waypointIndex++].GetComponent<Node>();

			float checkHeight = 10;
			Vector3 raisedStart = new Vector3(pos.x, checkHeight, pos.z);
			Vector3 raisedDest = new Vector3(closest.position.x, checkHeight, closest.position.z);

			//Draw a line from the starting position to the desired node
			Physics.Linecast(raisedStart, raisedDest, out hit);

			//If this line intersects the terrain, it is obstructed and we move on
			if (hit.collider != null)
			{
				hitsTerrain = (hit.collider.GetType() == typeof(TerrainCollider));
				Debug.DrawLine(raisedStart, raisedDest, Color.cyan, 10);
				Debug.Log("Node Obstructed: " + closest.ToString());
			}
            

		} while (hitsTerrain);

		Debug.Log(closest.ToString());

		return closest;
	}

	public void AddConnection(Node node)
	{
		Node[] newConnections = new Node[connections.Length + 1];
		connections.CopyTo(newConnections, 0);
		newConnections[newConnections.Length - 1] = node;
		Debug.Log(this.ToString() + "Adding Connection:" + node.ToString() + "(" + connections.Length + "->" + newConnections.Length + ")");
		connections = newConnections;
	}

	public Vector3 position
	{
		get { return transform.position; }
	}

	public void ResetHeuristics()
	{
		f = 0;
		g = 0;
		h = 0;
		parent = null;
	}

	public static void ResetAllNodeHeuristics()
	{
		Node[] nodes = Component.FindObjectsOfType<Node>();

		for(int f = 0; f < nodes.Length; f++)
		{
			nodes[f].ResetHeuristics();
		}
	}
}
