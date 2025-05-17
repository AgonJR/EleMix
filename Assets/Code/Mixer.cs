using System;
using System.Collections.Generic;
using UnityEngine;

public class Mixer : MonoBehaviour
{
    // - - -
    // Variables
    // - - -
    public static Mixer Instance;

    [SerializeField] private GameObject _mixVFX;

    // - - -
    // Internals
    // - - -
    
    private Card _card1;
    private Card _card2;
    private List<Card> _overQueued;

    private Dictionary<string, string> _undiscoveredDatabase;
    private Dictionary<string, Tuple<string, string>> _mixDatabase;

    void Start()
    {
        PopulateData();
        Instance = this;
        _overQueued = new List<Card>();
    }

    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.Space) )
        {
            MixCards();
        }
    }

    // --- 
    // Mixing
    // ---

    private bool MixCards()
    {
        if ( _card1 != null && _card2 != null )
        {
            string mixResult = null;

            string mixString1 = _card1.GetElement() + _card2.GetElement();
            string mixString2 = _card2.GetElement() + _card1.GetElement();

            if ( mixResult == null )  
            { 
                _undiscoveredDatabase.TryGetValue(mixString1, out mixResult); 
            }
            if ( mixResult == null )  
            { 
                _undiscoveredDatabase.TryGetValue(mixString2, out mixResult); 
            }
            if ( mixResult == null )  { ClearQueue(); return false; }
            else 
            {
                // Remove from _undiscoveredDatabase
                // TO DO
            }

            // Spawn at Midpoint
            int mX = (int) ((_card1.IdleX + _card2.IdleX) / 2);
            int mY = (int) ((_card1.IdleY + _card2.IdleY) / 2);
            Card newCard = GameBoard.Instance.SpawnCard(mX, mY);
            newCard.SetElement(mixResult);
            PlayVFX(mX, mY);
            ClearQueue();
            return true;
        }

        return false;
    }

    private void PlayVFX(int x, int y)
    {
        Instantiate(_mixVFX, new Vector3(x, y, 0), Quaternion.identity, transform);
    }

    // ---
    // Queue
    // ---

    public int QueueForMix(Card card)
    {
        if ( _card1 == null || _card1 == card ) { _card1 = card; return 1; }
        if ( _card2 == null || _card2 == card ) { _card2 = card; return 1; }

        if ( _overQueued.Contains(card)) { return -1; }
        if (!_overQueued.Contains(card)) { _overQueued.Add(card); return -1; }

        return 0;
    }

    public int DeQueue(Card card)
    {
        bool removed = false;

             if (_card1 == card) { _card1 = null; removed = true; }
        else if (_card2 == card) { _card2 = null; removed = true; }

        if (!removed)
        {
            _overQueued.Remove(card);
        }
        else
        {
            if (_overQueued.Count > 0)
            {
                     if (_card1 == null) { _card1 = _overQueued[0]; _card1.SetQueued(1); _overQueued.RemoveAt(0); }
                else if (_card2 == null) { _card2 = _overQueued[0]; _card2.SetQueued(1); _overQueued.RemoveAt(0); }
            }
        }

        return 0;
    }

    private void ClearQueue()
    {
        _card1?.ResetSelection(); _card1 = null;
        _card2?.ResetSelection(); _card2 = null;

        foreach (var card in _overQueued ) card.ResetSelection();

        _overQueued.Clear();
    }


    // ---
    // DATA 
    // ---

    private void PopulateData()
    {
        _mixDatabase = new Dictionary<string, Tuple<string, string>>();
        _undiscoveredDatabase = new Dictionary<string, string>();

        // Tier 0
        _mixDatabase.Add("Water",   null);
        _mixDatabase.Add("Earth",   null);
        _mixDatabase.Add("Fire",    null);
        _mixDatabase.Add("Air",     null);

        // Tier 1
        _mixDatabase.Add("Mud",     new Tuple<string, string>("Water",  "Earth" ));
        _mixDatabase.Add("Steam",   new Tuple<string, string>("Water",  "Fire"  ));
        _mixDatabase.Add("Cloud",   new Tuple<string, string>("Water",  "Air"   ));
        _mixDatabase.Add("Lava",    new Tuple<string, string>("Earth",  "Fire"  ));
        _mixDatabase.Add("Soil",    new Tuple<string, string>("Earth",  "Air"   ));
        _mixDatabase.Add("Heat",    new Tuple<string, string>("Fire",   "Air"   ));

        // Tier 2
        // _mixDatabase.Add("Bog",     new Tuple<string, string>("Mud",    "Water" ));
        // _mixDatabase.Add("Swamp",   new Tuple<string, string>("Mud",    "Earth" ));
        // _mixDatabase.Add("Clay",    new Tuple<string, string>("Mud",    "Fire"  ));
        // _mixDatabase.Add("Soil",    new Tuple<string, string>("Mud",    "Air"   ));

        // How to deal with duplicates ? two combos adding up to the same result ?


        // Fill Refererence Database
        foreach ( var mixCombo in _mixDatabase )
        {
            if ( mixCombo.Value != null )
            {
                _undiscoveredDatabase.Add(mixCombo.Value.Item1 + mixCombo.Value.Item2, mixCombo.Key);
            } 
        }
    }

    public List<string> GetAllBasics()
    {
        List<string> basics = new List<string>();

        foreach ( var mixCombo in _mixDatabase )
        {
            if ( mixCombo.Value == null ) basics.Add(mixCombo.Key);
        }

        return basics;
    }

    // ---
}
