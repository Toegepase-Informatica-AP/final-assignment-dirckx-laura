using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject tennisball;
    private GameObject tennisballContainer;
    private Dog dog;

    public void OnEnable()
    {

        tennisballContainer = transform.Find("TennisContainer").gameObject;
        dog = transform.GetComponentInChildren<Dog>();
    }
    public void ClearEnvironment()
    {
        foreach (Transform ball in tennisballContainer.transform)
        {
            Debug.Log("Destroying ball");
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
        GameObject newMenhir = Instantiate(tennisball.gameObject);
        newMenhir.transform.SetParent(tennisballContainer.transform);
        newMenhir.transform.localPosition = RandomPosition(0.2f);
        newMenhir.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }
}