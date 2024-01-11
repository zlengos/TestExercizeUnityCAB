using TreeEditor;
using Unity.Mathematics;
using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        #region Fields

        [Header("Player Settings")] 

        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float offsetYGrounded = 0.6f;
        [SerializeField] private float wallCheckRayDistance = 0.6f;
        [SerializeField] private float rayOffsetYHigh = 0.25f;
        [SerializeField] private float rayOffsetYLow = 0.68f;
        [SerializeField] private float jumpCooldown = 0.5f;
        [SerializeField] private Vector2 isGroundedCheckSize = new (0.7f, 0.3f);
        [SerializeField] private ParticleSystem jumpParticles;
        
        
        [Header("Required Components")] 
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;
      
        [Header("Debug")]
        private bool _isGrounded;
        
        private bool _isTouchingLeft, _isTouchingRight;

        private int _jumpsCountAllowed = 1;
        private int _jumpsDeal;
        public float speed = 7f;
        private float _lastJumpTime;

        #endregion

        #region Cache

        private static readonly int Speed = Animator.StringToHash("Speed");

        #endregion

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            rb ??= GetComponent<Rigidbody2D>();
            animator ??= GetComponent<Animator>();
        }

        private void Update()
        {
            HandleMovement();
            HandleAnimations();
            HandleJump();
        }

        #region Movement, Render

        private void FixedUpdate() => CheckIsGrounded();
        
        private void HandleMovement()
        {
            var horizontalInput = Input.GetAxis("Horizontal");
            var movement = new Vector2(horizontalInput, 0);

            if (CanMove())
                rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);

            UpdateCharacterScale();
        }

        private void UpdateCharacterScale()
        {
            transform.localScale = new Vector3(Mathf.Sign(rb.velocity.x), 1, 1);
        }

        private void HandleJump()
        {
            if (Input.GetButtonDown("Jump") && _jumpsDeal < _jumpsCountAllowed)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                _jumpsDeal++;
                _lastJumpTime = Time.time;
                Instantiate(jumpParticles, transform.position, Quaternion.Euler(90f, 0f, 0f));
            }
        }

        private void HandleAnimations()
        {
            animator.SetFloat(Speed, Mathf.Abs(Input.GetAxis("Horizontal")));
        }

        private bool CanMove()
        {
            var horizontalInput = Input.GetAxis("Horizontal");
            var pos = transform.position;

            RaycastHit2D hitHigh = Physics2D.Raycast(
                new Vector2(pos.x, pos.y + rayOffsetYHigh),
                new Vector2(horizontalInput * wallCheckRayDistance, 0), wallCheckRayDistance,
                LayerMask.GetMask("Ground"));

            RaycastHit2D hitLow = Physics2D.Raycast(
                new Vector2(pos.x, pos.y - rayOffsetYLow),
                new Vector2(horizontalInput * wallCheckRayDistance, 0), wallCheckRayDistance,
                LayerMask.GetMask("Ground"));

#if UNITY_EDITOR
            DebugDrawRays();
#endif

            return !hitHigh && !hitLow;
        }

        public void SetNewCountJumps(int count)
        {
            _jumpsCountAllowed = count;
        }

        private void CheckIsGrounded()
        {
            var position = transform.position;

            if (_isGrounded && Time.time - _lastJumpTime > jumpCooldown)
                _jumpsDeal = 0;
            
            _isGrounded = Physics2D.OverlapBox(new Vector2(position.x, position.y - offsetYGrounded),
                isGroundedCheckSize, 0, LayerMask.GetMask("Ground"));
        }
            
        #endregion

        #region Debug

#if UNITY_EDITOR

        private void DebugDrawRays()
        {
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayOffsetYHigh),
                new Vector2(Input.GetAxis("Horizontal") * wallCheckRayDistance, 0));
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - rayOffsetYLow),
                new Vector2(Input.GetAxis("Horizontal") * wallCheckRayDistance, 0));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            var position = transform.position;

            Gizmos.DrawCube(new Vector2(position.x, position.y - offsetYGrounded),
                new Vector2(0.7f, 0.3f));
        }

#endif

        #endregion
    }
}