using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    // - - -
    // Variables
    // - - -
    public static GameBoard Instance;

    [Header("Data")]
    [SerializeField] private GameObject _background;
    [Space]
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private GameObject _markerPrefab;
    [SerializeField] private Transform  _markerParent;
    [Space]
    [SerializeField] private List<Material> _cardColors;
    
    
    // - - -
    // Internals
    // - - -
    
    private float _mouseX;
    private float _mouseY;

    // private float _maxX = 11.3f;
    // private float _maxY = 6.31f;

    private int _maxX = 11;
    private int _maxY =  6;

    private bool[,] _grid; // negatives stored at max + n*-1
    private GameObject[,] _markers;

    // - - -
    #region Initialize
    // - - -

    void Start()
    {
        CacheData();
    }

    private void CacheData()
    {
        Instance = this;
        InitGridArrays();
        GenerateMarkers();
    }

    private void InitGridArrays()
    {
        int gX = (int)(_maxX * 2) + 1;
        int gY = (int)(_maxY * 2) + 1;

        _grid = new bool[gX, gY];
        _markers = new GameObject[gX, gY];
    }

    private void GenerateMarkers()
    {
        int maxX = (int) _maxX;
        int maxY = (int) _maxY;

        for ( int x = maxX * -1; x <= maxX; x++ )
        {
            for ( int y = maxY * -1; y <= maxY; y++ )
            {
                int ix = x < 0 ? maxX + x * -1 : x ;
                int iy = y < 0 ? maxY + y * -1 : y ;

                _markers[ix, iy] = Instantiate(_markerPrefab, new Vector3(x, y, 0), Quaternion.identity, _markerParent);
                _markers[ix, iy].name = $"[ {x} , {y} ]";
            }
        }

        _markerParent.localPosition = new Vector3(0,0,0.139f);
    }

    // - - -
    #endregion
    // - - -

    // - - -
    #region Game Loop 
    // - - -

    void Update()
    {
        ProcessMousePosition();
        ProcessDebugging();
    }

    private void ProcessDebugging()
    {
        int c = Random.Range(1,5);

        if ( Input.GetKey(KeyCode.Alpha1 ) ) { c = 1; }
        if ( Input.GetKey(KeyCode.Alpha2 ) ) { c = 2; }
        if ( Input.GetKey(KeyCode.Alpha3 ) ) { c = 3; }
        if ( Input.GetKey(KeyCode.Alpha4 ) ) { c = 4; }
        if ( Input.GetKey(KeyCode.Alpha5 ) ) { c = 5; }
        if ( Input.GetKey(KeyCode.Alpha6 ) ) { c = 6; }

        if ( Input.GetKeyDown(KeyCode.E) )
        {
            List<string> basicElements = Mixer.Instance.GetAllBasics();

            Material m = c == 0 ? _cardColors[Random.Range(0, _cardColors.Count)] : _cardColors[c-1];

            Card newCard = SpawnCard(Random.Range(-3, 4), 7);

            newCard.SetFace(m);
            newCard.SetElement(basicElements[c-1]);

            PlaceCard( newCard, Random.Range(-5, 5), Random.Range(-5, 6) );
        }

        if ( Input.GetKeyDown(KeyCode.F) )
        {
            bool line = Input.GetKey(KeyCode.LeftShift);
            Card newCard = null;

            for ( int x = (_maxX-2) * -1; x <= (_maxX-2); x++ )
            {
                for ( int y = (_maxY-2) * -1; y <= (_maxY-2); y++ )
                {
                    CovertXYToIndices(x, y, out int i, out int j);
                    if ( !_grid[i, j] ) 
                    {
                        newCard = SpawnCard(Random.Range(-3, 4), 7);

                        newCard.SetFace(_cardColors[5]);
                        newCard.SetElement(" - X - ");

                        PlaceCard( newCard, x, y );
                    }
                    if ( newCard != null && !line ) break;
                }
                if ( newCard != null ) break;
            }
        }
    }

    private void ProcessMousePosition()
    {
        if ( Input.GetMouseButton(0) )
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Water")))
            {
                _mouseX = Mathf.Clamp(hit.point.x, -_maxX, _maxX);
                _mouseY = Mathf.Clamp(hit.point.y, -_maxY, _maxY);
            }
        }
    }

    // - - -
    #endregion
    // - - -

    // - - -
    #region Game Logic
    // - - -

    public Card SpawnCard(float x, float y)
    {
        return Instantiate(_cardPrefab, new Vector3(x, y, 0), Quaternion.identity, transform).GetComponent<Card>();
    }

    public void PlaceCard(Card card) { PlaceCard(card, _mouseX, _mouseY); }
    public void PlaceCard(Card card, float x, float y)
    {
        if ( ToggleOnGrid(true, x, y, out x, out y) )
        {
            card.SetIdlePosition(new Vector3(x, y, 0));
        }
        else
        {
            ToggleOnGrid(true, card.IdleX, card.IdleY, out _, out _);
        }

        card.RefreshName();
    }

    public void PickUpCard(float x, float y)
    {
        ToggleOnGrid(false, x, y, out x, out y);
    }

    private bool ToggleOnGrid(bool toggle, float x, float y, out float rx, out float ry)
    {
        x = Mathf.Round(x); rx = x;
        y = Mathf.Round(y); ry = y;

        CovertXYToIndices(x, y, out int i, out int j);

        if (_grid[i, j] && toggle ) { return false; }
            _grid[i, j]  = toggle;    return true ;
    }

    private void CovertXYToIndices(float x, float y, out int i, out int j)
    {
        i = x < 0 ? _maxX + (int) x * -1 : (int) x ;
        j = y < 0 ? _maxY + (int) y * -1 : (int) y ;
    }

    // - - - 
    #endregion
    // - - - 

    // - - - 
    #region Static
    // - - -

    public static Vector3 GetMouseBoardPosition()
    {
        return new Vector3(Instance._mouseX, Instance._mouseY, 0);
    }

    // - - -
    #endregion
    // - - -
}
