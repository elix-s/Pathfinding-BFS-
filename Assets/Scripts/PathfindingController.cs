using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathfindingController : MonoBehaviour
{
    private GameObject[,] _grid;  
    private int _rows = 6;
    private int _cols = 6;
    private Vector2Int _startCell = new Vector2Int(0, 0); 
    private List<Vector2Int> _currentPath = new List<Vector2Int>();  

    private void Awake()
    {
        _grid = new GameObject[_rows, _cols];

        // start initialization
        int index = 0;
        foreach (Transform child in transform)
        {
            int row = index / _cols;
            int col = index % _cols;
            _grid[row, col] = child.gameObject;
            
            if (!(row == _startCell.x && col == _startCell.y))
            {
                Button button = _grid[row, col].GetComponent<Button>();
                int r = row, c = col;
                button.onClick.AddListener(() => OnClick(r, c));
            }

            index++;
        }
        
        _grid[_startCell.x, _startCell.y].GetComponent<Image>().color = Color.red;
    }

    public void OnClick(int targetRow, int targetCol)
    {
        ClearPath();
        
        List<Vector2Int> path = FindShortestPath(_startCell, new Vector2Int(targetRow, targetCol));
        
        if (path != null)
        {
            foreach (Vector2Int cell in path)
            {
                _grid[cell.x, cell.y].GetComponent<Image>().color = Color.green;  
            }
            
            _currentPath = path;
        }
    }

    private List<Vector2Int> FindShortestPath(Vector2Int start, Vector2Int target)
    {
        // BFS algorithm
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        queue.Enqueue(start);
        cameFrom[start] = start;

        Vector2Int[] directions = {
            new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0)
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == target)
            {
                return ReconstructPath(cameFrom, target);
            }

            foreach (var direction in directions)
            {
                Vector2Int neighbor = current + direction;

                // сhecking neighboring cells
                if (IsInBoundsChecking(neighbor) && !cameFrom.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current; 
                }
            }
        }
        
        return null;
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int target)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = target;

        while (current != _startCell)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }

    private bool IsInBoundsChecking(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < _rows && cell.y >= 0 && cell.y < _cols;
    }

    private void ClearPath()
    {
        foreach (Vector2Int cell in _currentPath)
        {
            _grid[cell.x, cell.y].GetComponent<Image>().color = Color.white;
        }

        _currentPath.Clear();
    }
}
