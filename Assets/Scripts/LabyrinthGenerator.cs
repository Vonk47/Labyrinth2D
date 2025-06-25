using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TestGame.Gameplay.Model;
using TestGame.Gameplay.Controllers;
using System;

namespace TestGame.Gameplay
{
    [RequireComponent(typeof(Tilemap), typeof(TilemapRenderer))]
    public sealed class LabyrinthGenerator : MonoBehaviour
    {

        [Header("Player")]
        [SerializeField] private PlayerController _playerController;

        [Header("Grid & Tilemap References")]
        [SerializeField] private Grid _grid;
        [SerializeField] private Tilemap _groundTilemap;
        [SerializeField] private Tilemap _wallTilemap;
        [SerializeField] private Tilemap _exitTilemap;

        [Header("Tiles")]
        [SerializeField] private TileBase[] _groundTiles;
        [SerializeField] private TileBase _wallTile;
        [SerializeField] private TileBase _exitTile;
        [SerializeField] private ushort _tilesOutsideMazeRange = 4;

        [Header("Directional Wall Tiles")]
        /*
        [SerializeField] private TileBase _wallTop;
        [SerializeField] private TileBase _wallBottom;
        [SerializeField] private TileBase _wallLeft;
        [SerializeField] private TileBase _wallRight;
        [SerializeField] private TileBase _wallCornerTL;
        [SerializeField] private TileBase _wallCornerTR;
        [SerializeField] private TileBase _wallCornerBL;
        [SerializeField] private TileBase _wallCornerBR;
        */

        [Header("Maze Settings")]
        [SerializeField] private ushort _width = 21;
        [SerializeField] private ushort _height = 21;
        [SerializeField] private byte _numberOfExits = 3;
        [SerializeField] private byte _minDistanceBetweenExits = 5;
        [Range(0, 100)]
        [SerializeField]
        private ushort _additionalPassages = 15;
        [Range(0, 50)]
        [SerializeField] private ushort _roomChance = 20;
        [SerializeField] private bool _useRandomSeed = true;
        [SerializeField] private int _seed = 12345;

        private bool[,] _maze;
        public bool[,] Maze => _maze;
        private List<Vector2Int> _exits = new List<Vector2Int>();

        private const string SavePath = "maze_settings.json";

        private void GenerateMaze()
        {
            if (_width % 2 == 0) _width++;
            if (_height % 2 == 0) _height++;

            _maze = new bool[_width, _height];

            InitializeMaze();
            CarveMaze();
            AddExtraPassages();
            GenerateRooms();
            PlaceExits();
            PaintTiles();

        }

        public void StartGame()
        {
            if (_useRandomSeed)
                UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
            else
                UnityEngine.Random.InitState(_seed);

            LoadData();
            GenerateMaze();
            SpawnPlayerAtCenter();
        }

        public void LoadData()
        {
            var currentSettings = SaveSystem.Load<MazeSettings>(SavePath);
            _width = currentSettings.width;
            _height = currentSettings.height;
            _numberOfExits = currentSettings.numberOfExits;
            _minDistanceBetweenExits = currentSettings.minDistanceBetweenExits;
            _additionalPassages = currentSettings.additionalPassages;
            _roomChance = currentSettings.roomChance;
            _useRandomSeed = currentSettings.useRandomSeed;
            _seed = currentSettings.seed;
        }

        // walls
        private void InitializeMaze()
        {
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    _maze[x, y] = (x == 0 || y == 0 || x == _width - 1 || y == _height - 1 ||
                                  x % 2 == 0 || y % 2 == 0);
        }

        //Recursive backtracking using depth first search
        private void CarveMaze()
        {
            var visited = new HashSet<Vector2Int>();
            var stack = new Stack<Vector2Int>();
            var start = new Vector2Int(_width / 2, _height / 2);
            if (start.x % 2 == 0) start.x--;
            if (start.y % 2 == 0) start.y--;

            stack.Push(start);
            visited.Add(start);
            _maze[start.x, start.y] = false;

            while (stack.Count > 0)
            {
                var current = stack.Peek();
                var neighbors = GetUnvisitedNeighbors(current, visited);
                if (neighbors.Count > 0)
                {
                    var next = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];
                    var wall = (current + next) / 2;
                    _maze[wall.x, wall.y] = false;
                    _maze[next.x, next.y] = false;
                    visited.Add(next);
                    stack.Push(next);
                }
                else
                    stack.Pop();
            }
        }

        private List<Vector2Int> GetUnvisitedNeighbors(in Vector2Int current, HashSet<Vector2Int> visited)
        {
            var dirs = new[] { new Vector2Int(2, 0), new Vector2Int(-2, 0),
                           new Vector2Int(0, 2), new Vector2Int(0, -2) };


            var list = new List<Vector2Int>();
            foreach (var direction in dirs)
            {
                var cell = current + direction;
                if (cell.x > 0 && cell.x < _width - 1 && cell.y > 0 && cell.y < _height - 1 && !visited.Contains(cell))
                    list.Add(cell);
            }
            return list;
        }

        private void AddExtraPassages()
        {
            var candidates = new List<Vector2Int>();
            for (int x = 1; x < _width - 1; x++)
                for (int y = 1; y < _height - 1; y++)
                    if (_maze[x, y] && IsBetweenPassages(x, y))
                        candidates.Add(new Vector2Int(x, y));

            int removeCount = Mathf.RoundToInt(candidates.Count * (_additionalPassages / 100f));
            for (int i = 0; i < removeCount && candidates.Count > 0; i++)
            {
                var idx = UnityEngine.Random.Range(0, candidates.Count);
                var p = candidates[idx];
                _maze[p.x, p.y] = false;
                candidates.RemoveAt(idx);
            }
        }

        private bool IsBetweenPassages(in int x, in int y)
        {
            if (x % 2 == 1 && y % 2 == 0)
                return !_maze[x, y - 1] && !_maze[x, y + 1];
            if (x % 2 == 0 && y % 2 == 1)
                return !_maze[x - 1, y] && !_maze[x + 1, y];
            return false;
        }

        private void GenerateRooms()
        {
            int attempts = UnityEngine.Random.Range(2, 5);
            for (int i = 0; i < attempts; i++)
                if (UnityEngine.Random.Range(0, 100) < _roomChance)
                    CarveRoom();
        }

        private void CarveRoom()
        {
            // room  parameters (random)
            int roomWidth = UnityEngine.Random.Range(3, 7);
            int roomHeight = UnityEngine.Random.Range(3, 7);

            int startX = UnityEngine.Random.Range(1, _width - roomWidth - 1);
            int startY = UnityEngine.Random.Range(1, _height - roomHeight - 1);

            // ensure the starting position is odd to align with maze structure
            if (startX % 2 == 0) startX++;
            if (startY % 2 == 0) startY++;

            // carve out the room (make the area walkable)
            for (int x = startX; x < startX + roomWidth; x++)
            {
                for (int y = startY; y < startY + roomHeight; y++)
                {
                    _maze[x, y] = false;
                }
            }

            ConnectRoomToMaze(startX, startY, roomWidth, roomHeight);
        }

        private void ConnectRoomToMaze(int startX, int startY, int roomWidth, int roomHeight)
        {
            var potentialConnectionPoints = new List<Vector2Int>();

            for (int x = startX; x < startX + roomWidth; x++)
            {
                potentialConnectionPoints.Add(new Vector2Int(x, startY - 1));
                potentialConnectionPoints.Add(new Vector2Int(x, startY + roomHeight));
            }

            for (int y = startY; y < startY + roomHeight; y++)
            {
                potentialConnectionPoints.Add(new Vector2Int(startX - 1, y));
                potentialConnectionPoints.Add(new Vector2Int(startX + roomWidth, y));
            }

            // connections from room to maze
            int numberOfConnections = UnityEngine.Random.Range(1, 3);
            for (int i = 0; i < numberOfConnections && potentialConnectionPoints.Count > 0; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, potentialConnectionPoints.Count);
                Vector2Int connection = potentialConnectionPoints[randomIndex];

                _maze[connection.x, connection.y] = false;
                potentialConnectionPoints.RemoveAt(randomIndex);
            }
        }

        private void PlaceExits()
        {
            _exits.Clear();
            for (int i = 0; i < _numberOfExits; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    int side = UnityEngine.Random.Range(0, 4);

                    Vector2Int exitPos = side switch
                    {
                        0 => new Vector2Int(UnityEngine.Random.Range(1, _width - 1), _height - 1),
                        1 => new Vector2Int(_width - 1, UnityEngine.Random.Range(1, _height - 1)),
                        2 => new Vector2Int(UnityEngine.Random.Range(1, _width - 1), 0),
                        3 => new Vector2Int(0, UnityEngine.Random.Range(1, _height - 1)),
                        < 0 or > 3 => throw new ArgumentOutOfRangeException(nameof(side)),
                    };

                    Vector2Int adjacentPos = (side) switch
                    {
                        0 => adjacentPos = new Vector2Int(exitPos.x, _height - 2),
                        1 => adjacentPos = new Vector2Int(_width - 2, exitPos.y),
                        2 => adjacentPos = new Vector2Int(exitPos.x, 1),
                        3 => adjacentPos = new Vector2Int(1, exitPos.y),
                    };

                    ExitPathway exitPathway = new ExitPathway(exitPos, adjacentPos);

                    bool tooClose = _exits.Exists(e => Vector2Int.Distance(e, exitPos) < 3);
                    if (!tooClose && IsValidExit(adjacentPos))
                    {
                        _maze[exitPos.x, exitPos.y] = false;
                        _maze[adjacentPos.x, adjacentPos.y] = false;
                        _exits.Add(exitPos);
                        break;
                    }
                }
            }
        }

        private void SpawnPlayerAtCenter()
        {
            int x = _width / 2;
            int y = _height / 2;

            if (x % 2 == 0) x--;
            if (y % 2 == 0) y--;

            if (!_maze[x, y])
            {
                Vector3 worldPos = _groundTilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
                _playerController.transform.position = worldPos;
                _playerController.Initialize(new Vector2Int(x, y), _maze, _groundTilemap, _exits);
            }
            else
            {
                Debug.LogError("Couldn't spawn player");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsValidExit(in Vector2Int p)
        {
            return p.x > 0 && p.x < _width - 1 && p.y > 0 && p.y < _height - 1;
        }

        private void PaintTiles()
        {
            _groundTilemap.ClearAllTiles();
            _wallTilemap.ClearAllTiles();
            _exitTilemap.ClearAllTiles();

            // inner interior
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                {
                    var pos = new Vector3Int(x, y, 0);
                    if (_maze[x, y])
                    {
                        TileBase selectedTile = _wallTile;

                        /*
                        // Check neighbors
                        bool up = y + 1 < height && !maze[x, y + 1];      
                        bool down = y - 1 >= 0 && !maze[x, y - 1];   
                        bool left = x - 1 >= 0 && !maze[x - 1, y];       
                        bool right = x + 1 < width && !maze[x + 1, y];    


                        if (up && !down && !left && !right && wallTop) selectedTile = wallTop;
                        else if (down && !up && !left && !right && wallBottom) selectedTile = wallBottom;
                        else if (left && !right && !up && !down && wallLeft) selectedTile = wallLeft;
                        else if (right && !left && !up && !down && wallRight) selectedTile = wallRight;


                        else if (up && left && !down && !right && wallCornerBL) selectedTile = wallCornerBL;
                        else if (up && right && !down && !left && wallCornerBR) selectedTile = wallCornerBR;
                        else if (down && left && !up && !right && wallCornerTL) selectedTile = wallCornerTL;
                        else if (down && right && !up && !left && wallCornerTR) selectedTile = wallCornerTR;
                        */

                        // Fallback
                        if (selectedTile != null)
                            _wallTilemap.SetTile(pos, selectedTile);
                    }
                    else
                    {
                        if (_exits.Contains(new Vector2Int(x, y)))
                            _exitTilemap.SetTile(pos, _exitTile);
                    }

                    _groundTilemap.SetTile(pos, _groundTiles[UnityEngine.Random.Range(0, _groundTiles.Length)]);
                }

            //exterior
            for (int x = -_tilesOutsideMazeRange; x < _width + _tilesOutsideMazeRange; x++)
            {
                for (int y = -_tilesOutsideMazeRange; y < _height + _tilesOutsideMazeRange; y++)
                {
                    var pos = new Vector3Int(x, y, 0);
                    if (_groundTiles != null && _groundTiles.Length > 0)
                    {
                        TileBase groundTile = _groundTiles[UnityEngine.Random.Range(0, _groundTiles.Length)];
                        _groundTilemap.SetTile(pos, groundTile);
                    }
                }
            }
        }


    }
}