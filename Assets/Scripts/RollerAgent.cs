using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RollerAgent : Agent
{
    
    public Text countText;
    public Text winText;
    public Transform target;
    private Rigidbody rb;
    private int count;    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();         
    }

    
    public override void OnEpisodeBegin()
    {
        this.transform.localPosition = new Vector3(0f,0.6f,0f);
        count = 0;
        SetCountText();
        winText.text = "New Episode";

        foreach (Transform child in target)
         {
             child.gameObject.SetActive(true);             
         }

        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
    }

    public float speed;
    public override void OnActionReceived(float[] action)
    {          
        moveAgent(action);

        // Fell off platform
        if (this.transform.localPosition.y < 0)
        {   
            //SetReward(-1f);         
            EndEpisode();
        }

    }

    public void moveAgent(float[] action){
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = action[0];
        controlSignal.z = action[1];
        rb.AddForce(controlSignal * speed); 
    }


    private void OnTriggerEnter(Collider other)

    {
        if(other.gameObject.CompareTag("Pick Up")){ 
            AddReward(0.1f);           
            other.gameObject.SetActive(false);
            winText.text = "";
            count++;            
            SetCountText();
        }        
    }


    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if(count == 12)
        {
            SetReward(1.0f);                     
            EndEpisode();            
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * speed);
    }
    
}
