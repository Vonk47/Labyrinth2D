using JSAM;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TestGame.Gameplay.Controllers
{
    public sealed class PlayerController : MonoBehaviour
    {
        private bool[,] _maze;
        private Tilemap _groundTilemap;
        private Vector2Int _gridPosition;

        private Vector2 _moveInput;
        private Vector2 _screenCenter;
        private InputController _inputController;
        private bool _buttonPressed;

        private List<Vector2Int> _exits;

        private void Start()
        {
            _screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            _inputController = new();
        }

        private void SubscribeEvents()
        {
            _inputController.OnClick += Trigger_Move;
        }

        private void UnsubscribeEvents()
        {
            _inputController.OnClick -= Trigger_Move;
        }

        public void EnablePlayer(bool enable)
        {
            if (enable)
            {
                SubscribeEvents();
            }
            else
            {
                UnsubscribeEvents();
            }
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
            _inputController.Dispose();
        }

        public void Initialize(in Vector2Int startPos, in bool[,] mazeData, in Tilemap groundMap, in List<Vector2Int> exits)
        {
            _maze = mazeData;
            _groundTilemap = groundMap;
            _gridPosition = startPos;
            _exits = exits;
            transform.position = _groundTilemap.GetCellCenterWorld((Vector3Int)_gridPosition);
        }

        //from button
        public void Trigger_Move(int direction)
        {
            _moveInput = direction switch
            {
                0 => new Vector2(0, 1),
                1 => new Vector2(0, -1),
                2 => new Vector2(-1, 0),
                3 => new Vector2(1, 0),
            };
            TryMove();
        }

        //from Position
        public void Trigger_Move(Vector2 screenPos)
        {
            _moveInput = Vector2.zero;

            Vector2 delta = screenPos - _screenCenter;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                // Horizontal
                if (delta.x > 0)
                    _moveInput = Vector2.right;
                else
                    _moveInput = Vector2.left;
            }
            else
            {
                // Vertical
                if (delta.y > 0)
                    _moveInput = Vector2.up;
                else
                    _moveInput = Vector2.down;
            }

            TryMove();
        }

        private void TryMove()
        {
            if (_maze == null) return;

            Vector2Int direction = Vector2Int.zero;
            if (_moveInput.x > 0.5f) direction = Vector2Int.right;
            else if (_moveInput.x < -0.5f) direction = Vector2Int.left;
            else if (_moveInput.y > 0.5f) direction = Vector2Int.up;
            else if (_moveInput.y < -0.5f) direction = Vector2Int.down;

            if (direction == Vector2Int.zero) return;

            Vector2Int nextPos = _gridPosition + direction;

            if (IsInBounds(nextPos) && !_maze[nextPos.x, nextPos.y])
            {
                _gridPosition = nextPos;
                transform.position = _groundTilemap.GetCellCenterWorld((Vector3Int)_gridPosition);
                GameController.Instance.AddSteps();
                if (_exits.Contains(nextPos))
                    GameController.Instance.Win();

                AudioManager.PlaySound(AudioLibrarySounds.HopSounds);
            }
        }

        private bool IsInBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 &&
                   pos.x < _maze.GetLength(0) && pos.y < _maze.GetLength(1);
        }
    }
}