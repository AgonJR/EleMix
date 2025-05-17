using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Z 0 0 M")]
    [SerializeField] private float _zoomSpeed = 10f;
    [SerializeField] private float _zoomLerpSpeed = 5f;
    [Space]
    [SerializeField] private float _minZoom = -20f;
    [SerializeField] private float _maxZoom = -5f;
    [Space]
    [Header("P A N")]
    [SerializeField] private float _panSpeed = 1f;
    [SerializeField] private float _maxX = 5f;
    [SerializeField] private float _maxY = 5f;
    [SerializeField] private float _panDecay = 5f;
                     private Vector2 _panVelocity;

    private float _targetZoom;

    private void Start()
    {
        _targetZoom = transform.position.z;
    }

    private void Update()
    {
        ProcessZoom();
        ProcessPan(); 
    }

    private void ProcessZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            _targetZoom += scroll * _zoomSpeed;
            _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
        }

        Vector3 currentPosition = transform.position;
        currentPosition.z = Mathf.Lerp(currentPosition.z, _targetZoom, Time.deltaTime * _zoomLerpSpeed);
        transform.position = currentPosition;
    }



    private void ProcessPan()
    {
        Vector2 input = new Vector2((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow )) ?  1 :
                                    (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow  )) ? -1 : 0,
                                    (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow    )) ?  1 :
                                    (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow  )) ? -1 : 0);

        if (input.sqrMagnitude > 1f) input.Normalize();

        if (input != Vector2.zero)
        {
            _panVelocity = input * _panSpeed;
        }
        else
        {
            _panVelocity = Vector2.Lerp(_panVelocity, Vector2.zero, Time.deltaTime * _panDecay);
        }

        Vector3 move = new Vector3(_panVelocity.x, _panVelocity.y, 0f) * Time.deltaTime;
        Vector3 nextPos = transform.position + move;

        nextPos.x = Mathf.Clamp(nextPos.x, -_maxX, _maxX);
        nextPos.y = Mathf.Clamp(nextPos.y, -_maxY, _maxY);

        transform.position = nextPos;
    }

}
