using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap _baseTilemap;

    [SerializeField]
    private List<TileBase> _baseTileRules;

    [SerializeField]
    private Tilemap _spikeTilemap;

    [SerializeField]
    private List<TileBase> _spikeTileRules;

    [SerializeField]
    private Tilemap _breakableTilemap;

    [SerializeField]
    private List<TileBase> _breakableTileRules;

    [SerializeField]
    private Color _transitionColor = new Color(1, 0, 0, 1);

    [SerializeField]
    private float _transitionDuration = 0.1f;
    public static GameManager Instance { get; private set; }

    private int index = 0; // temporary

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void StateChanged(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            index = (index + 1) % _baseTileRules.Count;

            // here change the state depending on what you need
            UpdateTileMaps();
        }
    }

    private void UpdateTileMaps()
    {
        DOTween.To(() => _baseTilemap.color,
            x => _baseTilemap.color = x,
            _transitionColor,
            _transitionDuration);

        // Its not effective but this will have to do for now
        foreach (Vector3Int position in _baseTilemap.cellBounds.allPositionsWithin)
        {
            if (_baseTilemap.HasTile(position))
            {  
                //_baseTilemap.GetTile(position).
                _baseTilemap.SetTile(position, _baseTileRules[index]);
            }
        }

        //_baseTilemap.
        DOTween.To(() => _baseTilemap.color,
            x => _baseTilemap.color = x,
            new Color(1, 1, 1, 1),
            _transitionDuration).SetDelay(_transitionDuration);


        foreach (Vector3Int position in _spikeTilemap.cellBounds.allPositionsWithin)
        {
            if (_spikeTilemap.GetTile(position) != null)
            {
                _spikeTilemap.SetTile(position, _spikeTileRules[index]);
            }
        }
    }
}
