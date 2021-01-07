# DogTrainer
### Inhoudstafel

<!-- Start Document Outline -->

* [Inleiding](#inleiding)
* [Samenvatting](#samenvatting)
* [One-pager](#one-pager)
* [Verloop van het spel](#verloop-van-het-spel)
* [Methoden](#methoden)
	* [Park](#park)
	* [BalSpawner](#balspawner)
	* [DogAgent](#dogagent)
* [Trainen](#trainen)
	* [Training 1 - Witse](#training-1---witse)
	* [Training 2 - Wrun_9_WWalls](#training-2---wrun_9_wwalls)
	* [Training 3 - Yanu 1](#training-3---yanu-1)
	* [Training 4 - Yanu 2](#training-4---yanu-2)
	* [Training 5 - Yanu 3](#training-5---yanu-3)
	* [Training 6 - Yanu 4](#training-6---yanu-4)
* [Conclusie](#conclusie)
* [Bronvermelding](#bronvermelding)

<!-- End Document Outline -->

### Inleiding

Ons project zal een hond voorstellen waar de speler mee kan spelen. De speler kan een bal gooien, de hond zal deze dan terugbrengen. Dit kan mensen met een enorme schrik voor honden helpen om hun schrik te overwinnen.

Dit project is bedoeld om met een Oculus VR-headset te spelen om de volledige emersie te beleven.

### Samenvatting

In deze tutorial zullen wij beschrijven wat u moet weten om dit project tot een goed einde te bregen. We zullen waar nodig extra toelichten en de belangrijkste scripts tonen. Na het volgen van deze tutorial kan u zelf een gelijkaardig project maken en spelen op uw eigen VR-headset.

### One-pager

Na verder onderzoeken en denken zijn we volledig afgeweken van onze one-pager. We hebben een volledig nieuw concept uitgedacht en dit gaan we uitwerken.


### Verloop van het spel

![](spelVerloop.png)

### Methoden

#### Park

Als omgeving hebben we voor een park gekozen. Dit zal de omgeving zijn waarin zowel de hond als de speler zich kan bewegen.

#### BalSpawner

Voor het trainen is er een balSpawner geschreven. Deze zal op random plaatsen binnen het veld een bal spawnen, die de hond dan moet gaan halen en naar een bepaald punt moet brengen.
Deze balSpawner is specifiek bedoeld voor het trainen. Later kunnen we dan de getrainde hond zijn brein toevoegen aan onze game.
Finaal word deze BalSpawner gebruikt om de bal te laten spawnen op een tafel, waar de speler de bal van kan nemen en weggooien.

```cs
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
```


#### DogAgent

Voor de hond word er een leeg GameObject voorzien. Als de hond de bal heeft word deze zichtbaar gemaakt, zodat de hond met een bal in zijn mond verder loopt.

Met als referentie Obelix script van ML-agents github [kijk bronvermelding](#bronvermelding).

```cs
public class Dog : Agent
{
    private Spawner spawner;
    private Rigidbody body;
    public float speed = 10;
    public float rotationSpeed = 350;
    bool ballInMouth;
    public GameObject tBall;
    public GameObject player;


    public void Update()
    {
        fell();
    }

    public override void Initialize()
    {
        base.Initialize();
        body = GetComponent<Rigidbody>();
        spawner = GetComponentInParent<Spawner>();
        transform.localPosition = new Vector3(1.733055f, 1.3f, -17.78904f);
        body.angularVelocity = Vector3.zero;
        body.velocity = Vector3.zero;

    }

    public override void OnEpisodeBegin()
    {
        spawner.ClearEnvironment();
        spawner.SpawnBall();
        tBall.SetActive(false);
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
        if (vectorAction[0] == 0 & vectorAction[1] == 0)
        {

            AddReward(-0.001f);
            return;
        }

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
        if (collision.gameObject.CompareTag("tennisball") && !ballInMouth)
        {
            //load material of dog with ball in mouth
            // collision.gameObject.GetComponent<Renderer>().material = 
            ballInMouth = true;
            tBall.SetActive(true);
            spawner.ClearEnvironment();
            // add reward for getting ball
            AddReward(0.5f);

        }

        if (collision.gameObject.CompareTag("Player") && ballInMouth)
        {
            
            ballInMouth = false;

            //add reward for returning ball to player
            AddReward(1f);


            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Player") && !ballInMouth)
        {

            Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>());
            //ballInMouth = false;
            AddReward(-0.05f);
        }

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(ballInMouth);


    }

    public void fell()
    {
        if (GameObject.Find("Dog").transform.position.y < 0)
        {
            
            AddReward(-1f);
            ballInMouth = false;
            EndEpisode();
        }
        
        
    }

}
```

### Trainen

#### Training 1 - Witse

****yaml parameters****:
```cs
behaviors:
  Dog:
    trainer_type: ppo
    max_steps: 5.0e7
    time_horizon: 64
    summary_freq: 10000
    keep_checkpoints: 5
    checkpoint_interval: 50000
    
    hyperparameters:
      batch_size: 32
      buffer_size: 9600
      learning_rate: 3.0e-4
      learning_rate_schedule: constant
      beta: 5.0e-3
      epsilon: 0.2
      lambd: 0.95
      num_epoch: 3

    network_settings:
      num_layers: 2
      hidden_units: 128
      normalize: false
      vis_encoder_type: simple

    reward_signals:
      extrinsic:
        strength: 1.0
        gamma: 0.99
      curiosity:
        strength: 0.02
        gamma: 0.99
        encoding_size: 256
        learning_rate : 1e-3
```
****Rewardsysteem****: 
* -0.1 als de hond naar de player gaat zonder bal
* +1 als de hond naar de player gaat met bal
* +0.5 als de hond de bal pakt
* -0.001 als de hond stilstaat
* -0.1 als de hond valt van environment

![StoryBoard](StoryBoard.png)

![Graph](Graph1.png)

* Smoothed: 1.299
* Value: 1.392
* Step: 510K
* Runtime: 1h 19m 52s

****conclusie****: Zonder muren getraint, goede resultaten. Volgende keer met muren proberen trainen, zien of de resultaten even goed blijven.

#### Training 2 - Wrun_9_WWalls

****yaml parameters****:
```cs
behaviors:
  Dog:
    trainer_type: ppo
    max_steps: 5.0e7
    time_horizon: 64
    summary_freq: 10000
    keep_checkpoints: 5
    checkpoint_interval: 50000
    
    hyperparameters:
      batch_size: 32
      buffer_size: 9600
      learning_rate: 3.0e-4
      learning_rate_schedule: constant
      beta: 5.0e-3
      epsilon: 0.2
      lambd: 0.95
      num_epoch: 3

    network_settings:
      num_layers: 2
      hidden_units: 128
      normalize: false
      vis_encoder_type: simple

    reward_signals:
      extrinsic:
        strength: 1.0
        gamma: 0.99
      curiosity:
        strength: 0.02
        gamma: 0.99
        encoding_size: 256
        learning_rate : 1e-3
```
****Rewardsysteem****: 
* -0.1 als de hond naar de player gaat zonder bal
* +1 als de hond naar de player gaat met bal
* +0.5 als de hond de bal pakt
* -0.001 als de hond stilstaat
* -0.1 als de hond valt van environment

![StoryBoard](StoryBoard.png)

![GraphWithWalls](GraphWithWalls.png)


****conclusie****: Met muren getraint, minder goede resultaten.

#### Training 3 - Yanu 1

****yaml parameters****:
```cs
behaviors:
  Dog:
    trainer_type: ppo
    max_steps: 5.0e7
    time_horizon: 64
    summary_freq: 10000
    keep_checkpoints: 5
    checkpoint_interval: 50000
    
    hyperparameters:
      batch_size: 32
      buffer_size: 9600
      learning_rate: 3.0e-4
      learning_rate_schedule: constant
      beta: 5.0e-3
      epsilon: 0.2
      lambd: 0.95
      num_epoch: 3

    network_settings:
      num_layers: 2
      hidden_units: 128
      normalize: false
      vis_encoder_type: simple

    reward_signals:
      extrinsic:
        strength: 1.0
        gamma: 0.99
      curiosity:
        strength: 0.01
        gamma: 0.99
        encoding_size: 256
        learning_rate : 1e-3
```


****Rewardsysteem****: 
* -0.1 als de hond naar de player gaat zonder bal
* +1 als de hond naar de player gaat met bal
* +0.3 als de hond de bal pakt
* -0.001 als de hond stilstaat

![GraphYanu1](GraphYanu1.png)

****conclusie****: Zonder muren. Episode word opnieuw gestart als agent van het veld valt. Met curiousity strength 0.01 getraint. Ongeveer dezelfde resultaten.

#### Training 4 - Yanu 2

****yaml parameters****:
```cs
behaviors:
  Dog:
    trainer_type: ppo
    max_steps: 5.0e7
    time_horizon: 64
    summary_freq: 10000
    keep_checkpoints: 5
    checkpoint_interval: 50000
    
    hyperparameters:
      batch_size: 32
      buffer_size: 9600
      learning_rate: 3.0e-4
      learning_rate_schedule: constant
      beta: 5.0e-3
      epsilon: 0.2
      lambd: 0.95
      num_epoch: 3

    network_settings:
      num_layers: 2
      hidden_units: 128
      normalize: false
      vis_encoder_type: simple

    reward_signals:
      extrinsic:
        strength: 1.0
        gamma: 0.99
      curiosity:
        strength: 0.02
        gamma: 0.99
        encoding_size: 256
        learning_rate : 1e-3
```


****Rewardsysteem****: 
* -0.1 als de hond naar de player gaat zonder bal
* +1 als de hond naar de player gaat met bal
* +0.3 als de hond de bal pakt
* -0.001 als de hond stilstaat

![GraphYanu2](GraphYanu2.png)

****conclusie****: Zonder muren. Episode word opnieuw gestart als agent van het veld valt. Met curiousity strength 0.02 getraint. Niet erg veel verschil, gaat wel iets sneller dan de vorige.

#### Training 5 - Yanu 3

****yaml parameters****:
```cs
behaviors:
  Dog:
    trainer_type: ppo
    max_steps: 5.0e7
    time_horizon: 64
    summary_freq: 10000
    keep_checkpoints: 5
    checkpoint_interval: 50000
    
    hyperparameters:
      batch_size: 32
      buffer_size: 9600
      learning_rate: 3.0e-4
      learning_rate_schedule: constant
      beta: 5.0e-3
      epsilon: 0.2
      lambd: 0.95
      num_epoch: 3

    network_settings:
      num_layers: 2
      hidden_units: 128
      normalize: false
      vis_encoder_type: simple

    reward_signals:
      extrinsic:
        strength: 1.0
        gamma: 0.99
      curiosity:
        strength: 0.03
        gamma: 0.99
        encoding_size: 256
        learning_rate : 1e-3
```


****Rewardsysteem****: 
* -0.1 als de hond naar de player gaat zonder bal
* +1 als de hond naar de player gaat met bal
* +0.3 als de hond de bal pakt
* -0.001 als de hond stilstaat

![GraphYanu3](GraphYanu3.png)

****conclusie****: Zonder muren. Episode word opnieuw gestart als agent van het veld valt. Met curiousity strength 0.03. Niet erg veel verschil. 

#### Training 6 - Yanu 4

****yaml parameters****:
```cs
behaviors:
  Dog:
    trainer_type: ppo
    max_steps: 5.0e7
    time_horizon: 64
    summary_freq: 10000
    keep_checkpoints: 5
    checkpoint_interval: 50000
    
    hyperparameters:
      batch_size: 32
      buffer_size: 9600
      learning_rate: 3.0e-4
      learning_rate_schedule: constant
      beta: 5.0e-3
      epsilon: 0.2
      lambd: 0.95
      num_epoch: 3

    network_settings:
      num_layers: 2
      hidden_units: 128
      normalize: false
      vis_encoder_type: simple

    reward_signals:
      extrinsic:
        strength: 1.0
        gamma: 0.99
      curiosity:
        strength: 0.02
        gamma: 0.99
        encoding_size: 256
        learning_rate : 1e-3
```


****Rewardsysteem****: 
* -0.1 als de hond naar de player gaat zonder bal
* +1 als de hond naar de player gaat met bal
* +0.5 als de hond de bal pakt
* -0.001 als de hond stilstaat
* -1 als de hond van de map valt

![GraphYanu4](GraphYanu4.png)

****conclusie****: Zonder muren. Episode word opnieuw gestart als agent van het veld valt. Met curiousity strength 0.02.
final
### Conclusie
We gebruiken het brein van training 1, zonder walls.

### Bronvermelding

* Dhaese, D. D. (2020). Gedragingen van de agent en de andere spelobjecten. VR Experience (ML-Agents). https://ddhaese.github.io/ML-Agents/gedragingen-van-de-agent-en-de-andere-spelobjecten.html#obelix.cs
* Chen, L. C., & Berges, V. B. (2018, October 2). Puppo, The Corgi: Cuteness Overload with the Unity ML-Agents Toolkit. Unity Blog. https://blogs.unity3d.com/2018/10/02/puppo-the-corgi-cuteness-overload-with-the-unity-ml-agents-toolkit/
