using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject cam;
    public GameObject enemyPrefab;
    public TextMeshProUGUI message;

    [Header("Settings")]
    public int amountOfSpawnedEnemies = 10;
    public float camSpeed = 20f;
    public float messageRotationSpeed = 1f;

    private float[] scales = {2, 1, 0.5f};
    void Start()
    {
        for(int i=0;i<amountOfSpawnedEnemies; i++)
        {
            GameObject cube = Instantiate(enemyPrefab, new Vector3(0, 17, 0), Quaternion.Euler(45,45,45));
            float size = scales[Random.Range(0, scales.Length)];
            Vector3 scale = new Vector3(size, size, size);
            cube.transform.localScale = scale;

            cube.GetComponent<Enemy>().bounceSound = null;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Camera rotation
        cam.transform.RotateAround(new Vector3(0,0,0), Vector3.up, 1*Time.fixedDeltaTime*camSpeed);

        // TODO Message rotation
    }
}
