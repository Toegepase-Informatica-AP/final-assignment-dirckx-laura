# DogTrainer

### BalSpawner

### DogAgent

Met als referentie Obelix script van ML-agents github [kijk bronvermelding](#bronvermelding).

```cs
public class Dog : Agent
{
    private Rigidbody body;
    public float speed = 10;
    public float rotationSpeed = 350;
    bool ballInMouth;
    

    public override void Initialize()
    {
        base.Initialize();
        body = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(1.733055f, 0.5f, -17.78904f);
        body.angularVelocity = Vector3.zero;
        body.velocity = Vector3.zero;
    }
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = 0f;
        actionsOut[1] = 0f;

        if (Input.GetKey(KeyCode.UpArrow)) // Moving fwd
        {
            actionsOut[0] = 2f;
        }
        else if (Input.GetKey(KeyCode.DownArrow)) // Turning left
        {
            actionsOut[0] = 1f;
        }
        else if (Input.GetKey(KeyCode.LeftArrow)) // Turning left
        {
            actionsOut[1] = 1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow)) // Turning right
        {
            actionsOut[1] = 2f;
        }
    }

   
    //code van Meneer Dhaese bij Obelix.cs - MLAgents - VR Experience github
    public override void OnActionReceived(float[] vectorAction)
    {
        //bij stilstaan afstraffen, nog niet zeker of dit nodig is
        /*if (vectorAction[0] == 0 & vectorAction[1] == 0)
        {
            
            //AddReward(-0.001f);
            return;
        }*/

        if (vectorAction[0] != 0)
        {
            Vector3 translation = transform.forward * speed * (vectorAction[0] * 2 - 3) * Time.deltaTime;
            transform.Translate(translation, Space.World);
        }

        if (vectorAction[1] != 0)
        {
            float rotation = rotationSpeed * (vectorAction[1] * 2 - 3) * Time.deltaTime;
            transform.Rotate(0, rotation, 0);
        }


    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ball") && !ballInMouth)
        {
            //load material of dog with ball in mouth
            // collision.gameObject.GetComponent<Renderer>().material = 
            ballInMouth = true;

            // add reward for getting ball
            AddReward(+0.5f);          

        }

        if (collision.gameObject.CompareTag("player") && ballInMouth)
        {
            //add reward for returning ball to player
            AddReward(+1f);
            ballInMouth = false;
            
            EndEpisode();
        }
       
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(ballInMouth);

        
    }

}
```


### Bronvermelding

* Dhaese, D. D. (2020). Gedragingen van de agent en de andere spelobjecten. VR Experience (ML-Agents). https://ddhaese.github.io/ML-Agents/gedragingen-van-de-agent-en-de-andere-spelobjecten.html#obelix.cs
* Chen, L. C., & Berges, V. B. (2018, October 2). Puppo, The Corgi: Cuteness Overload with the Unity ML-Agents Toolkit. Unity Blog. https://blogs.unity3d.com/2018/10/02/puppo-the-corgi-cuteness-overload-with-the-unity-ml-agents-toolkit/
