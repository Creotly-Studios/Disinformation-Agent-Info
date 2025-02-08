using UnityEngine;
using UnityEngine.AI;

namespace HeneGames.DialogueSystem
{
	public class SimpleNavAgentAnimation : MonoBehaviour
	{
		NavMeshAgent agent; //A reference to the navmesh agent component
		[SerializeField] private Animator animator; //A reference to the player's animator component

		void Start()
		{
			//Get references to the local navmesh agent
			agent = GetComponent<NavMeshAgent>(); ;
		}

		void Update()
		{
			//Record the desired speed of the navmesh agent
			float speed = agent.desiredVelocity.magnitude;

			//Tell the animator how fast the navmesh agent is going
			animator.SetFloat("Speed", speed);
		}
	}
}