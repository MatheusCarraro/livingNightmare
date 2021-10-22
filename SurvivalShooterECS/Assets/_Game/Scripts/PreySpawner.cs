using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class PreySpawner : MonoBehaviour {
    public GameObject preyPrefab;
    // Start is called before the first frame update
    void Start() {
        StartCoroutine(SpawnPrey());
    }

    // Update is called once per frame
    void Update() {

    }

    IEnumerator SpawnPrey() {
        yield return new WaitForSeconds(2f);
        if (FindObjectsOfType<PreyBehaviour>().Length < 30) {
            Vector2 position;
            float hwidth = Miscellaneous.HalfWidthCamera(CameraControllerBase.instance.camera);
            if (Random.Range(0, 2) == 0) {
                position = new Vector2(Random.Range(-(hwidth + 2), hwidth + 2), Random.Range(0, 2) == 0 ? -(hwidth + 2) : hwidth + 2);
            } else {
                position = new Vector2(Random.Range(0, 2) == 0 ? -(hwidth + 2) : hwidth + 2, Random.Range(-(hwidth + 2), hwidth + 2));
            }
            Instantiate(preyPrefab, position, Quaternion.identity);
        }

        StartCoroutine(SpawnPrey());
    }
}