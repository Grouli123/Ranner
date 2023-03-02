using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Text _text;
    private Animator _animator;
    private new Rigidbody rigidbody;
    private float _lineOffset = 2.5f;
    private float _lineChangeSpeed = 15f;
    private Vector3 _startGamePosition;
    private Quaternion _startGameRotation;
    private float _pointStart;
    private float _pointFinish;
    private bool _isMoving = false;
    private Coroutine _movingCoroutine;
    private float _lastVectorX;
    private bool _isJumping;
    private float _jumpForce = 12f;
    private float _jumpGravity = -40f;
    private float _realGravity = -9.8f;

    private int _coins;

    private void Start() 
    {
        _animator = GetComponent<Animator>();
        _startGamePosition = transform.position;
        _startGameRotation = transform.rotation;
        rigidbody = GetComponent<Rigidbody>();
        SwipeController.instance.MoveEvent += MovePlayer;    
    }

    private void Update() 
    {
        _text.text = _coins.ToString();    
    }

    public void StartGame()
    {
        _animator.SetTrigger("Run");
    }

    public void StartLevel()
    {
        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        RoadGenerator.instance.StartLevel();
    }

    public void ResetGame()
    {
        rigidbody.velocity = Vector3.zero;
        _pointStart = 0;
        _pointFinish = 0;

        _animator.SetTrigger("Idle");
        RoadGenerator.instance.ResetLevel();
        transform.position = _startGamePosition;
        transform.rotation = _startGameRotation;
    }

    private void MovePlayer(bool[] swipe)
    {
        if(swipe[(int)SwipeController.Direction.Up] && _isJumping == false)
        {
            Jump();
        }
        if(swipe[(int)SwipeController.Direction.Left] && _pointFinish > -_lineOffset)
        {
            MoveHorizontal(-_lineChangeSpeed);
        }
         if(swipe[(int)SwipeController.Direction.Right] && _pointFinish < _lineOffset)
        {
            MoveHorizontal(_lineChangeSpeed);
        }
    }

    private void Jump()
    {
        _animator.SetTrigger("Jump");
        _isJumping = true;
        rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        Physics.gravity = new Vector3(0, _jumpGravity, 0);
        StartCoroutine(StopJumpCoroutine());

    }

    private IEnumerator StopJumpCoroutine()
    {
        do
        {
            yield return new WaitForSeconds(0.2f);
        }
        while (rigidbody.velocity.y !=0);

        _isJumping = false;

        Physics.gravity = new Vector3(0, _realGravity, 0);
    }

    private void MoveHorizontal(float speed)
    {
        _pointStart = _pointFinish;

        _pointFinish += Mathf.Sign(speed) * _lineOffset;

        if(_isMoving) { StopCoroutine(_movingCoroutine); _isMoving = false; }

        _movingCoroutine = StartCoroutine(MoveCoroutine(speed));
    }

    private IEnumerator MoveCoroutine(float vectorX)
    {
        _isMoving = true;
        while(Mathf.Abs(_pointStart - transform.position.x) < _lineOffset)
        {
            yield return new WaitForFixedUpdate();

            rigidbody.velocity = new Vector3(vectorX, rigidbody.velocity.y, 0);
            _lastVectorX = vectorX;

            float x = Mathf.Clamp(transform.position.x, Mathf.Min(_pointStart, _pointFinish), Mathf.Max(_pointStart, _pointFinish));

            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        rigidbody.velocity = Vector3.zero;

        transform.position = new Vector3(_pointFinish, transform.position.y, transform.position.z);

        if(transform.position.y > 1)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, -10, rigidbody.velocity.z);
        }

        _isMoving = false;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "Ramp")
        {
            rigidbody.constraints |= RigidbodyConstraints.FreezePositionZ;
        }

        if(other.gameObject.tag == "Gold")
        {
            _coins += 1;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.gameObject.tag == "Ramp")
        {
            rigidbody.constraints &= ~ RigidbodyConstraints.FreezePositionZ;
        }
        if(other.gameObject.tag == "Lose")
        {       
            _animator.SetTrigger("Death");        
            //RoadGenerator.instance.ResetLevel();
            ResetGame();
        }
    }

    private void OnCollisionEnter(Collision collision) 
    {
        if(collision.gameObject.tag == "Ground")
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        }

        if(collision.gameObject.tag == "NoLose")
        {
            MoveHorizontal(-_lastVectorX);
        }
    }

    private void OnCollisionExit(Collision collicion)
    {
        if(collicion.gameObject.tag == "RampPlane")
        {
            if(rigidbody.velocity.x == 0 && _isJumping == false)
            {
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, -10, rigidbody.velocity.z);
            }
        }
    }


}
