using DG.Tweening;
using MarkusSecundus.Utils.Behaviors.GameObjects;
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
    private AreaManager _areaManager;
    
    private Tilemap _breakableTilemap => _areaManager.CurrentInstance.GetComponent<Tilemap>();

    [SerializeField]
    private List<TileBase> _breakableTileRules;

    [SerializeField]
    private SpriteRenderer _background;

    [SerializeField]
    private List<Sprite> _backgroundImages;


    [SerializeField]
    private Color _transitionColor = new Color(1, 0, 0, 1);

    [SerializeField]
    private float _transitionDuration = 0.1f;

    [SerializeField]
    private PlayerController _playerController;

    private SoundtrackManager _soundtrackManager;

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
        _soundtrackManager = Object.FindAnyObjectByType<SoundtrackManager>();
    }

    public void StateChanged(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            index = (index + 1) % _baseTileRules.Count;

            // here change the state depending on what you need
            UpdateTileMaps();
            UpdateCharacterSkills();
            _soundtrackManager.SwitchToWorld(index);
            _playerController.SetCharacter(index);
        }
    }

    private void UpdateCharacterSkills()
    {

        //very awful code, no time throw it into a dictionary or something though
        switch (index)
        {
            case 0:
                _playerController.doubleJump = true;
                _playerController.dash = false;
                _playerController.strike = false;
                break;
            case 1:
                _playerController.doubleJump = false;
                _playerController.dash = true;
                _playerController.strike = false;
                break;
            case 2:
                _playerController.doubleJump = false;
                _playerController.dash = false;
                _playerController.strike = true;
                break;
        }
    }

    private void UpdateTileMaps()
    {
        DOTween.To(() => _baseTilemap.color,
            x => _baseTilemap.color = x,
            _transitionColor,
            _transitionDuration).SetEase(Ease.InOutBounce);

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
            _transitionDuration).SetDelay(_transitionDuration).SetEase(Ease.InOutBounce);


        foreach (Vector3Int position in _spikeTilemap.cellBounds.allPositionsWithin)
        {
            if (_spikeTilemap.HasTile(position))
            {
                _spikeTilemap.SetTile(position, _spikeTileRules[index]);
            }
        }

        foreach (Vector3Int position in _breakableTilemap.cellBounds.allPositionsWithin)
        {
            if (_breakableTilemap.HasTile(position))
            {
                //_baseTilemap.GetTile(position).
                _breakableTilemap.SetTile(position, _breakableTileRules[index]);
            }
        }

        // set background image
        _background.sprite = _backgroundImages[index];
    }
}
