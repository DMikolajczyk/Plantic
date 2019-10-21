using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merger : MonoBehaviour {

	// Use this for initialization
	void Start () {  
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length-1];

        //////////// INFO: Parent do którego podpinamy skrypt nie ma mesha. Dlatego i = 1 i wyżej Lenght-1.
        int i = 1;
        while (i < meshFilters.Length)
        {
            combine[i - 1].mesh = meshFilters[i].sharedMesh;
            combine[i - 1].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i - 1].gameObject.SetActive(false);
            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
        transform.localEulerAngles = new Vector3(0, 0, 0);
    }
	
}
