using UnityEngine;
using UnityEngine.AI;

namespace HeneGames.DialogueSystem
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ClickMovement : MonoBehaviour
    {
        private NavMeshAgent nav;

        [SerializeField] private LayerMask pointMask;

        private void Start()
        {
            nav = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            ClickMovementInput();
        }

        private void ClickMovementInput()
        {
            //Mouse left click
            if (Input.GetMouseButtonDown(0))
            {
                //Ray from the camera to the mouse position
                Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                //Set navmesh destination to raycast hit point
                if (Physics.Raycast(_ray, out RaycastHit _hit, 100f, pointMask))
                {
                    if(nav.enabled)
                    {
                        nav.SetDestination(_hit.point);
                    }
                }
            }
        }

        public void StopMovement(bool _value)
        {
            nav.isStopped = _value;
        }
    }
}