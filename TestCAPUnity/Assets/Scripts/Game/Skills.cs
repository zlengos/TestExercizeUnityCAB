using System.Collections;
using System.Linq;
using Configs;
using Unity.Collections;
using UnityEngine;

namespace Game
{
    public class Skills : MonoBehaviour
    {
        #region Fields

        [Header("Dash Settings")] 
        [SerializeField] private float dashSpeed = 10f;
        [SerializeField] private float dashDuration = 0.5f;

        [Header("Player Components")]
        [SerializeField] private Player playerCharacter;
        [SerializeField] private Rigidbody2D playerRb;
        [SerializeField] private SkillsConfig skillsConfig;

        [SerializeField] private ParticleSystem dashParticles;
        
        [SerializeField] private DistanceJoint2D joint2D;
        [SerializeField] private LineRenderer lineRenderer;


        [Header("Debug")]
        [ReadOnly] private bool _isDashing;
        [ReadOnly] private Skill _activeSkill;

        private float _horizontalInput;
        private bool _isCheeking = true;
        private Camera _camera;
        private Vector3 _mousePosition, _tempPosition; 

        #endregion
        
        private void Start()
        {
            lineRenderer.positionCount = 0;
            _camera = Camera.main;
            joint2D.enabled = false;
            _activeSkill = GetActiveSkill();

            playerCharacter ??= GetComponent<Player>();
            playerRb ??= GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            PerformSkill();
            UpdateHorizontalInput();
        }
        
        private Skill GetActiveSkill()
        {
            return skillsConfig.AllSkills.FirstOrDefault(skill => skill.IsEquipped);
        }

        private void PerformSkill()
        {
            if (Input.GetButtonDown("Ability"))
            {
                switch (_activeSkill?.Name)
                {
                    case "Dash":
                        Dash();
                        break;
                    case "HookCat":
                        HookCat();
                        break;
                    case "DoubleJump":
                        playerCharacter.SetNewCountJumps(2);
                        break;
                }
            }

            DrawHookLine();
            if (Input.GetButtonDown("StopAbility"))
            {
                if (_isCheeking == false)
                    StopGrappling();
            }
        }

        #region Dash

        private void Dash()
        {
            if (!_isDashing)
            {
                StartCoroutine(DashCoroutine());
            }
        }

        private IEnumerator DashCoroutine()
        {
            
            _isDashing = true;

            dashParticles.Play();
            //TODO: Effect

            float startTime = Time.time;

            while (Time.time < startTime + dashDuration)
            {
                playerRb.AddForce(new Vector2(_horizontalInput * dashSpeed, 0), ForceMode2D.Impulse);
                dashParticles.gameObject.transform.localScale = new Vector3(-Mathf.Sign(_horizontalInput),1,1);
                yield return null;
            }

            dashParticles.Stop();
            
            _isDashing = false;
            playerRb.velocity = Vector2.zero;
        }

        private void UpdateHorizontalInput() => _horizontalInput = Input.GetAxis("Horizontal");
        
        #endregion

        #region Grapple

        private void HookCat()
        {
            GetMousePosition();
            var hit = Physics2D.Raycast(_camera.transform.position, _mousePosition, Mathf.Infinity,
                LayerMask.GetMask("Ground"));

            playerCharacter.speed = 14;
            if (hit && _isCheeking)
            {
                joint2D.enabled = true;

                joint2D.connectedAnchor = _mousePosition;
                lineRenderer.positionCount = 2;
                _tempPosition = _mousePosition;
                _isCheeking = false;
            }

            DrawHookLine();
        }

        private void StopGrappling()
        {
            joint2D.enabled = false;
            _isCheeking = true;
            lineRenderer.positionCount = 0;
        }

        private void DrawHookLine()
        {
            if (lineRenderer.positionCount <= 0)
                return;

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, _tempPosition);
        }

        private void GetMousePosition() => _mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        #endregion
    }
}
