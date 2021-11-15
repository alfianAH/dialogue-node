using UnityEngine;
using UnityEngine.AI;

namespace Character
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float angularSpeed = 1000f;
        
        // private Animator animator;
        private Camera mainCamera;
        private NavMeshAgent navPlayer;

        private bool isWalking;
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");

        private void Awake()
        {
            // animator = GetComponent<Animator>();
            mainCamera = Camera.main;
            navPlayer = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            navPlayer.speed = speed;
            navPlayer.angularSpeed = angularSpeed;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit))
                {
                    Move(hit.point);
                }
            }

            // Set animation
            isWalking = !(navPlayer.remainingDistance <= navPlayer.stoppingDistance);
            // animator.SetBool(IsWalking, isWalking);
        }
        
        /// <summary>
        /// Move to destination
        /// </summary>
        /// <param name="destination"></param>
        private void Move(Vector3 destination)
        {
            navPlayer.SetDestination(destination);
        }
    }
}
