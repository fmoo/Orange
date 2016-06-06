using UnityEngine;
using System.Collections;
using System.Collections.Generic;

sealed public class MeshUnoptimizer {
	public static Mesh Unoptimize(Mesh mesh) {

		List<Vector3> vertices = new List<Vector3>(mesh.vertices);
		List<Vector2> uvs = new List<Vector2>(mesh.uv);
		List<int> triangles = new List<int>(mesh.triangles);

		HashSet<int> allTrisSeen = new HashSet<int>();
		HashSet<int> quadTrisSeen = new HashSet<int>();

		var numAdded = 0;
		for (var i = 0; i < mesh.triangles.Length; i+= 6) {
			quadTrisSeen.Clear();
			for (var j = 0; j < 6; j++) {
				var vi = mesh.triangles[i+j];

				// If we've seen this vertex in the current quad, just skip it 
				//if (quadTrisSeen.Contains(vi)) {
				//	continue;
				//}

				// Otherwise, if we *haven't* seen this vertex in other quads, it's
				// unique (for now).  Remember it for both and keep going.
				if (!allTrisSeen.Contains(vi)) {
					//quadTrisSeen.Add(vi);
					allTrisSeen.Add(vi);
					continue;
				}
					
				// Ok... This is a non-unique vertex.  Let's make some copies.
				numAdded++;
				var vi2 = vertices.Count;
				vertices.Add(vertices[vi]);
				uvs.Add(uvs[vi]);
				triangles[i+j] = vi2;
			}
		}

		Debug.Log("Unoptimized " + numAdded + " vertices.");

		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = triangles.ToArray();

		return mesh;
	}
}

abstract public class BaseMeshUnoptomizerBehavior {

	abstract protected Mesh getMesh();
	abstract protected void setMesh(Mesh m);

	// Use this for initialization
	void Start () {
		var m = getMesh();
		setMesh(MeshUnoptimizer.Unoptimize(m));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
