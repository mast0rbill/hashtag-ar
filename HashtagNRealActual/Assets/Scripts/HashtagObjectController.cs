using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HashtagObjectController : MonoBehaviour {

	[System.Serializable]
	public class HashtagObjectBridge {
		public HashtagObject json;
		public GameObject unityObj;
	}

	public static string curHashtag = "tag1";
 
	public HashtagObjectBridge[] curObjs;
	public Vector2 curLoc;
	public float curHeading;

	private HashtagObject[] lastObjs = null;
	private float lastUpdatedTime;

	// Start GPS location seq
	IEnumerator Start() {
		Input.location.Start(0.1f, 0.1f);

		while (Input.location.status == LocationServiceStatus.Initializing) {
			yield return new WaitForSeconds(1f);
		}

		if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }

		print("Location services initialized!");
		lastUpdatedTime = Time.time;
	}

	void Update () {
		if(Time.time - lastUpdatedTime >= 0.25f) {			
			string loc = Input.location.lastData.latitude + "," + Input.location.lastData.longitude;
			curLoc = LocFromString(loc);

			curHeading = Input.compass.trueHeading;

			Debug.LogError(loc + " " + curHeading);

			// store last objs
			//lastObjs = curObjs.Select(bridge => bridge.json).ToArray();

			//HashtagAPI.instance.GetObjects(loc, curHashtag, CompareAndSpawnObjects);
			lastUpdatedTime = Time.time;
		}
	}

	private Vector2 LocFromString(string s) {
		string[] split = s.Split(',');
		return new Vector2(float.Parse(split[0]), float.Parse(split[1]));
	}

	private void CompareAndSpawnObjects(HashtagObject[] newObjs) {
		HashSet<HashtagObject> oldIDSet = new HashSet<HashtagObject>();
		foreach(HashtagObject obj in lastObjs) {
			oldIDSet.Add(obj); // use location as unique identifier
		}

		foreach(HashtagObject obj in newObjs) {
			oldIDSet.Remove(obj);
		}

		foreach(HashtagObject toSpawn in oldIDSet) {
			SpawnObject(curLoc, LocFromString(toSpawn.location), toSpawn.objType, toSpawn.source);
		}
	}

	private void SpawnObject(Vector2 curLoc, Vector2 objLoc, string objType, string source) {
		Debug.LogError("Spawning object! " + objLoc.x + "," + objLoc.y + " " + objType + " " + source);
	}

	
}

public static class LongLatHelper {
	

	public static double DistanceTo(double lat1, double lon1, double lat2, double lon2)
    {
        double rlat1 = Mathf.PI * lat1 / 180;
        double rlat2 = Mathf.PI * lat2 / 180;
        double theta = lon1 - lon2;
        double rtheta = Mathf.PI * theta / 180;
        double dist =
            System.Math.Sin(rlat1) * System.Math.Sin(rlat2) + System.Math.Cos(rlat1) *
            System.Math.Cos(rlat2) * System.Math.Cos(rtheta);
        dist = System.Math.Acos(dist);
        dist = dist * 180 / System.Math.PI;
        dist = dist * 60 * 1.1515; // this apparently gets in miles
        return dist * 1.609344 * 1000f;
    }
}
