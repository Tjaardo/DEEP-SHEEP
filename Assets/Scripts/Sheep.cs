using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
	GameObject player;
	public float distance;
	public float speed;
	

	protected void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	protected void Update()
	{
		if(Vector3.Distance(transform.position, player.transform.position) <= distance)
		{
			Vector3 direction = new Vector3();
			direction = transform.position - player.transform.position;
			
			transform.Translate(direction * speed * Time.deltaTime);
			//transform.position = transform.position + direction * speed * Time.deltaTime;
		}
	}

	
}
