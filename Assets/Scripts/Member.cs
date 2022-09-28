using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Member : MonoBehaviour
{
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 acceleration;

    public Level level;
    public MemberConfig conf;

    public bool hasEaten;

    Vector3 wanderTarget;

    private float walkingTime;

    private Animator anim;


    

    void Start()
    {
        level = FindObjectOfType<Level>();
        conf = FindObjectOfType<MemberConfig>();

        anim = GetComponent<Animator>();

        position = transform.position;
        velocity = new Vector3(Random.Range(-3,3), 0, Random.Range(-3,3));

        walkingTime = Random.Range(conf.walkingTimeMin, conf.walkingTimeMax);

        hasEaten = false;
    }

    void Update()
    {
        //walkingTime -= Time.deltaTime;

        //if(walkingTime > 0f)
        if(!hasEaten)
        {
            anim.SetBool("isEating", false);
            Debug.DrawLine(transform.position, Combine(), Color.white, .1f);
            acceleration = Combine();
            acceleration = Vector3.ClampMagnitude(acceleration, conf.maxAcceleration);
            velocity = velocity + acceleration * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, conf.maxVelocity);
            position = position + velocity * Time.deltaTime;
            WrapAround(ref position, -level.bounds, level.bounds);
            transform.position = position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Combine().normalized), Time.deltaTime);
        }
        else
        {
            anim.SetBool("isEating", true);
            StartCoroutine(Eat());
        }

        

    }

    IEnumerator Eat()
    {
       
        yield return new WaitForSeconds(Random.Range(conf.eatingTimeMin, conf.eatingTimeMax));
        //walkingTime = Random.Range(conf.walkingTimeMin, conf.walkingTimeMax);
        
        Vector3 scaleChange = new Vector3(0.1f, 0.1f, 0.1f);
        transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        hasEaten = false;
    }

    protected Vector3 Wander()
    {
        float jitter = conf.wanderJitter * Time.deltaTime;
        wanderTarget += new Vector3(RandomBinomial() * jitter, 0, RandomBinomial() * jitter);
        
        wanderTarget = wanderTarget.normalized;
        wanderTarget *= conf.wanderRadius;
        Vector3 targetInLocalSpace = wanderTarget + new Vector3(conf.wanderDistance, 0, conf.wanderDistance);
        Vector3 targetInWorldSpace = transform. TransformPoint(targetInLocalSpace);
        targetInWorldSpace -= this.position;
        return targetInWorldSpace.normalized;
    }

    Vector3 Cohesion()
    {
        Vector3 cohesionVector = new Vector3();
        int countMembers = 0;
        var neighbors = level.GetNeighbors(this, conf.cohesionRadius);
        if(neighbors.Count == 0)
            return cohesionVector;
        foreach(var member in neighbors)
        {
            if(IsInFOV(member.position))
            {
                cohesionVector += member.position;
                countMembers++;
            }
        }
        if(countMembers == 0)
            return cohesionVector;

        cohesionVector /= countMembers;
        cohesionVector = cohesionVector - this.position;
        cohesionVector = Vector3.Normalize(cohesionVector);
        return cohesionVector;
    }

    Vector3 Alignment()
    {
        Vector3 alignVector = new Vector3();
        var members = level.GetNeighbors(this, conf.alignmentRadius);

        if(members.Count == 0)
            return alignVector;
        foreach(var member in members)
        {
            if(IsInFOV(member.position))
            alignVector += member.velocity;
        }
        return alignVector.normalized;
    }

    Vector3 Separation()
    {
        Vector3 separateVector = new Vector3();
        Vector3 normal = new Vector3();
        var members = level.GetNeighbors(this, conf.separationRadius);
        if(members.Count == 0)
            return separateVector;
        foreach(var member in members)
        {
            if(IsInFOV(member.position))
            {
                Vector3 movingTowards = this.position - member.position;
                if(movingTowards.magnitude > 0)
                {
                    separateVector += movingTowards.normalized / movingTowards.magnitude;
                }
            }
        }
        float angle = Vector3.Angle(separateVector, transform.forward);
        if(angle > 130f)
        {
            return transform.forward;
        }
        else
        {
            return separateVector.normalized;  
        }
         
    }

    Vector3 Avoidance()
    {
        Vector3 avoidVector = new Vector3();
        var enemyList = level.GetEnemies(this, conf.avoidanceRadius);
        if(enemyList.Count == 0)
            return avoidVector;
        foreach(var enemy in enemyList)
        {
            avoidVector += RunAway(enemy.transform.position);
        }
        return avoidVector.normalized;
    }

    Vector3 RunAway(Vector3 target)
    {
        Vector3 neededVelocity = (position - target).normalized * conf.maxVelocity;
        neededVelocity.y = 0;
        return neededVelocity - velocity;
    }

    Vector3 Food()
    {
        Vector3 foodVector = new Vector3();
        var foodList = level.GetFood(this, conf.foodRadius);
        float distance = Mathf.Infinity;
        Food closest = null;
        if(foodList.Count == 0)
        {
            return foodVector;
        }
        foreach(var food in foodList)
        {
            Vector3 diff = food.transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;
            if(curDistance < distance)
            {
                closest = food;
                distance = curDistance;
            }
        }
        foodVector = closest.transform.position - transform.position;
        return foodVector;
    }

    Vector3 Bounds()
    {
        Vector3 boundsVector = new Vector3();
        Vector3 center = Vector3.zero;
        boundsVector = center - transform.position;
        float t = boundsVector.magnitude / conf.boundsRadius;
        if(t < 0.9f)
        {
            return Vector3.zero;
        }
        return boundsVector * t * t;
        
    }

    virtual protected Vector3 Combine()
    {
        Vector3 finalVec = conf.cohesionPriority * Cohesion() + conf.wanderPriority * Wander() 
            + conf.alignmentPriority * Alignment() + conf.separaitionPriority * Separation()
                + conf.avoidancePriority * Avoidance() + conf.boundsPriority * Bounds()
                    + conf.foodPriority * Food();
        return finalVec;
    }

    void WrapAround(ref Vector3 vector, float min, float max)
    {
        vector.x = WrapAroundFloat(vector.x, min, max);
        vector.y = WrapAroundFloat(vector.y, min, max);
        vector.z = WrapAroundFloat(vector.z, min, max);
    }

    float WrapAroundFloat(float value, float min, float max)
    {
        if(value > max)
            value = min;
        else if(value < min)
            value = max;
        return value;
    }

    float RandomBinomial()
    {
        return Random.Range(0f, 1f) - Random.Range(0f, 1f);
    }

    bool IsInFOV(Vector3 vec)
    {
        return Vector3.Angle(this.velocity, vec - this.position) <= conf.maxFOV;
    }
}
