using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public GameObject Planet;
    [Range(0, 1)]
    public float Speed = 1;

    public float distance = 20;
    public float angleX = 0;
    public float angleY = 90;

    public Vector2 MinMaxYAngle = new Vector2(30, 150);
    public Vector2 MinMaxDistance = new Vector2(10, 25);
    
    public float ReturnAutoDuration = 3;

    [Header("Private Element")]
    [ReadOnly]
    [SerializeField]
    private Vector2 _previousPos;
    [ReadOnly]
    [SerializeField]
    private Vector2 _deltaPos = Vector2.zero;
    [ReadOnly]
    [SerializeField]
    private Vector2 _previousOnePos;
    [ReadOnly]
    [SerializeField]
    private Vector2 _deltaOnePos = Vector2.zero;
    [ReadOnly] 
    [SerializeField] 
    private float _previousDistance = 0;
    [ReadOnly] 
    [SerializeField] 
    private float _deltaDistance = 0;
    [ReadOnly] 
    [SerializeField] 
    private bool _autoRotate = true;
    [ReadOnly] 
    [SerializeField] 
    private float _timeAwait = 0;

    void Awake()
    {
        _timeAwait = ReturnAutoDuration;
    }

    // Update is called once per frame
    void Update()
    {
        KeyboardMovement();
#if UNITY_EDITOR
        MouseMovement();
#elif !UNITY_EDITOR
        TouchMovement();
#endif
        AutoRotate();

        angleY = Mathf.Clamp(angleY, MinMaxYAngle.x, MinMaxYAngle.y);
        distance = Mathf.Clamp(distance, MinMaxDistance.x, MinMaxDistance.y);
        
        Vector2 mover = new Vector2(angleX * Mathf.Deg2Rad, angleY * Mathf.Deg2Rad);
        this.transform.position = arounding(mover, Planet.transform.position, distance);
        this.transform.LookAt(Planet.transform);

        _timeAwait += Time.deltaTime;
        _autoRotate = _timeAwait >= ReturnAutoDuration;
    }

    public Vector3 arounding(Vector2 move, Vector3 target, float distance)
    {
        float x = distance * Mathf.Cos(move.x) * Mathf.Sin(move.y) + target.x;
        float y = distance * Mathf.Cos(move.y) + target.y; 
        float z = distance * Mathf.Sin(move.x) * Mathf.Sin(move.y) + target.z;
        return new Vector3(x, y, z);
    }

    public float Remap(float value, float oMin, float oMax, float nMin, float nMax)
    {
        float result = nMin + (value - oMin) * (nMax - nMin) / (oMax - oMin);
        return result;
    }

    public void AutoRotate()
    {
        if (!_autoRotate) return;
        float speed = Remap(Speed, 0, 1, 0, 40);
        angleX += Time.deltaTime * speed / 10;
        angleY += Time.deltaTime * (((MinMaxYAngle.x + MinMaxYAngle.y) / 2) - angleY) * speed / 10;
    }

    public void KeyboardMovement()
    {
        float speed = Remap(Speed, 0, 1, 0, 40);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            distance -= Input.GetAxis("Vertical") * Time.deltaTime * speed;
        }
        else
        {
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) _timeAwait = 0;
            float vertical = Input.GetAxis("Vertical") * Time.deltaTime * speed;
            float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
            angleX += horizontal;
            angleY += vertical;
        }
    }

    public void MouseMovement()
    {
        float speed = Remap(Speed, 0, 1, 0, 40);
        if (Input.GetMouseButtonDown(0))
        {
            _timeAwait = 0;
            _previousPos = Input.mousePosition;
        } else if (Input.GetMouseButton(0))
        {
            _timeAwait = 0;
            _deltaPos = (Vector2)Input.mousePosition - _previousPos;
            angleX -= _deltaPos.x * Time.deltaTime * speed;
            angleY += _deltaPos.y * Time.deltaTime * speed;
            _previousPos = Input.mousePosition;
        } else if (Input.GetMouseButtonUp(0))
        {
            _previousPos = Vector2.zero;
            _deltaPos = Vector2.zero;
        }

        distance -= Input.mouseScrollDelta.y * Time.deltaTime * speed;
    }

    public void TouchMovement()
    {
        if (Input.touches.Length <= 0) return;
        _timeAwait = 0;
        float speed = Remap(Speed, 0, 1, 0, 40);
        if (Input.touches.Length == 1)
        {
            switch (Input.touches[0].phase)
            {
                case TouchPhase.Began:
                    _previousPos = Input.touches[0].position;
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    _deltaPos = Input.touches[0].position - _previousPos;
                    angleX -= _deltaPos.x * Time.deltaTime * speed;
                    angleY += _deltaPos.y * Time.deltaTime * speed;
                    _previousPos = Input.touches[0].position;
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    _previousPos = Vector2.zero;
                    _deltaPos = Vector2.zero;
                    break;
            }
        }

        if (Input.touches.Length == 2)
        {
            switch (Input.touches[1].phase)
            {
                case TouchPhase.Began:
                    _previousPos = Input.touches[0].position;
                    _previousOnePos = Input.touches[1].position;
                    _previousDistance = Vector2.Distance(_previousPos, _previousOnePos);
                    _deltaDistance = 0;
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    _deltaDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position) - _previousDistance;
                    distance -= _deltaDistance * Time.deltaTime * speed / 2;

                    Debug.Log(_deltaDistance + " " + Vector2.Distance(Input.touches[0].position, Input.touches[1].position) + " " + _previousDistance);
                    
                    _previousPos = Input.touches[0].position;
                    _previousOnePos = Input.touches[1].position;
                    _previousDistance = Vector2.Distance(_previousPos, _previousOnePos);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    _previousPos = Vector2.zero;
                    _deltaPos = Vector2.zero;
                    _previousOnePos = Vector2.zero;
                    _deltaOnePos = Vector2.zero;
                    _deltaDistance = 0;
                    break;
            }
        }
    }
}
