using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Transform memberPrefab;
    public Transform enemyPrefab;
    public Transform foodPrefab;
    //public Transform playerPrefab;

    public int numberOfMembers;
    public int numberOfEnemies;
    public int numberOfFood;

    public List<Member> members;
    public List<Enemy> enemies;
    public List<Food> food;

    public float bounds;
    public float spawnRadius;
    
    //public Player player;

    

    void Start()
    {
        members = new List<Member>();
        enemies = new List<Enemy>();
        food = new List<Food>();

        Spawn(memberPrefab, numberOfMembers);
        Spawn(enemyPrefab, numberOfEnemies);
        Spawn(foodPrefab, numberOfFood);
        //Spawn(playerPrefab, 1);

        members.AddRange(FindObjectsOfType<Member>());
        enemies.AddRange(FindObjectsOfType<Enemy>());
        food.AddRange(FindObjectsOfType<Food>());

        //player = FindObjectOfType<Player>();
    }

    void Spawn(Transform prefab, int count)
    {
        for(int i = 0; i < count; i++)
        {
            Instantiate(prefab, new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius)), Quaternion.identity);
        }
    }

    public List<Member> GetNeighbors(Member member, float radius)
    {
        List<Member> NeighborsFound = new List<Member>();

        foreach(var otherMember in members)
        {
            if(otherMember == member)
            continue;

            if(Vector3.Distance(member.transform.position, otherMember.transform.position) <= radius)
            {
                NeighborsFound.Add(otherMember);
            }
        }

        return NeighborsFound;
    }

    public List<Enemy> GetEnemies(Member member, float radius)
    {
        List<Enemy> returnEnemies = new List<Enemy>();

        foreach(var enemy in enemies)
        {
            if(Vector3.Distance(member.transform.position, enemy.transform.position) <= radius)
            {
                returnEnemies.Add(enemy);
            }
        }

        return returnEnemies;
    }

    public List<Food> GetFood(Member member, float radius)
    {
        List<Food> foodFound = new List<Food>();
        
        foreach(var food in food)
        {
            if(food != null)
            {
            if(Vector3.Distance(member.transform.position, food.transform.position) <= radius)
            {
                foodFound.Add(food);
            }
            }
        }
        

        return foodFound;
    }

    


}
