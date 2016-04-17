using UnityEngine;
using System.Collections;
using System;

public class ColonyHub : MonoBehaviour {

	public float mineralCount;

	public GameObject[] waypoints;
	private GameObject[] mineralNodes;

	public GameObject[] buildSites;
	public int numBuildSites = 12;

	public const int ColonyPodCost = 20;

	public int droneSpawnCount = 5;
	public int mineralNodeCount = 1;

	Camera[] cameras;
	int curCameraIndex;

	public GameObject[] obstacles;

	// Use this for initialization
	void Start () {
		//the nodes for pathfinding
		waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

		mineralNodes = new GameObject[mineralNodeCount];

		float spawnRange = 650;
		for (int i = 0; i < mineralNodeCount; i++)
		{
			float x;
			float z;

			do
			{
				x = UnityEngine.Random.Range(20, spawnRange);
				z = UnityEngine.Random.Range(20, spawnRange);
			}
			while (Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z)) > 5);

			mineralNodes[i] = (GameObject)GameObject.Instantiate(Resources.Load("MineralNode", typeof(GameObject)), new Vector3(x, Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z)), z), Quaternion.identity);
		}

		Vector3 pos = transform.position;
		//Arrange them from nearest to farthest
		Array.Sort(mineralNodes, delegate (GameObject a, GameObject b)
			{ return (a.GetComponent<Node>().position - pos).magnitude.CompareTo((b.GetComponent<Node>().position - pos).magnitude); }
		);

		buildSites = GenerateBuildSites();

		SpawnDrones(droneSpawnCount);

		cameras = GameObject.FindObjectsOfType<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		//switch camera on space bar press
		if(Input.GetKeyDown("space"))
		{
			if(curCameraIndex++ >= cameras.Length)
			{
				curCameraIndex = 0;
			}

			for (int c = 0; c < cameras.Length; c++)
			{
				cameras[c].enabled = false;
				cameras[c].tag = "Untagged";

				if(c == curCameraIndex)
				{
					cameras[c].enabled = true;
					cameras[c].tag = "MainCamera";
				}
			}
		}

		//find obstacles
		obstacles = GameObject.FindGameObjectsWithTag("Solid");
	}

	public void SpawnDrones(int numDrones)
	{
		float degreeIncrement = 360 / numDrones;
		float spawnDist = 10;

		for (int d = 0; d < numDrones; d++)
		{
			float x = transform.position.x + Mathf.Cos(degreeIncrement * d) * spawnDist;
			float z = transform.position.z + Mathf.Sin(degreeIncrement * d) * spawnDist;
			float y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));

			Vector3 position = new Vector3(x, y + 3, z);

			GameObject.Instantiate(Resources.Load("Drone", typeof(GameObject)), position, Quaternion.identity);
		}
	}

	public GameObject[] GenerateBuildSites()
	{
		GameObject[] sites = new GameObject[numBuildSites];

		Vector3 startDisplacement = new Vector3(17, 0, -5);
		Vector3 unitDisplacement = new Vector3(12, 0, 12);

		int xUnits = (int)Mathf.Sqrt(numBuildSites);
		int zUnits = numBuildSites / xUnits;

		for (int x = 0; x < xUnits; x++)
		{
			for (int z = 0; z < zUnits; z++)
			{
				float xPos = transform.position.x + startDisplacement.x + x * unitDisplacement.x;
				float zPos = transform.position.z + startDisplacement.z + z * unitDisplacement.z;
				float yPos = Terrain.activeTerrain.SampleHeight(new Vector3(xPos, 0, zPos));

				Vector3 position = new Vector3(xPos, yPos + 3, zPos);

				sites[x * zUnits + z] = (GameObject)GameObject.Instantiate(Resources.Load("BuildSite", typeof(GameObject)), position, Quaternion.identity);
			}
		}

		return sites;
	}

	public GameObject GetOpenMineralNode()
	{
		for (int i = 0; i < mineralNodes.Length; i++)
		{
			if (mineralNodes[i].GetComponent<MineralNode>().minerals > 0)
			{
				int minerCount = 0;
				GameObject[] drones = GameObject.FindGameObjectsWithTag("Drone");
				foreach(GameObject drone in drones)
				{
					if(drone.GetComponent<Drone>() != null && drone.GetComponent<Drone>().target 
						== mineralNodes[i].GetComponent<Node>())
					{
						minerCount++;
					}
				}

				if(minerCount < 1)
				{
					return mineralNodes[i];
				}
			}
		}
		return null;
	}
}
