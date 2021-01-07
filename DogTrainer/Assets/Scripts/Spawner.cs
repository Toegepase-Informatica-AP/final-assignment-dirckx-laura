using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject tennisball;
    private GameObject tennisballContainer;
    private Dog dog;

    private void Update()
    {

        if (tennisball.transform.position.y < 0)
        {
            ClearEnvironment();
            SpawnBall();
        }

    }

    public void OnEnable()
    {
        tennisballContainer = transform.Find("TennisContainer").gameObject;
        dog = transform.GetComponentInChildren<Dog>();
    }
    public void ClearEnvironment()
    {
        foreach (Transform ball in tennisballContainer.transform)
        {
            GameObject.Destroy(ball.gameObject);
        }
    }
    public Vector3 RandomPosition(float up)
    {
        float x = Random.Range(-9.75f, 9.75f);
        float z = Random.Range(-9.75f, 9.75f);
        return new Vector3(x, up, z);
    }

    public void SpawnBall()
    {
        GameObject ball = Instantiate(tennisball.gameObject);
        ball.transform.SetParent(tennisballContainer.transform);

        ball.transform.localPosition = new Vector3(0.1804f, -0.0674f, -0.0028f);
    }


}