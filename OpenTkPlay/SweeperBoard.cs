using System;
using System.Drawing;

namespace OpenTkPlay
{
   public partial class SweeperBoard : IDisposable
   {
      public delegate void OnGameStartedHandler();
      public delegate void OnGameOverHandler(bool gameWon);

      public event OnGameStartedHandler OnGameStarted;
      public event OnGameOverHandler OnGameOver;
      public bool GameOver { get; private set; }
      public bool GameWon { get; private set; }
      public int FlaggedTiles { get; private set; }
      public int TotalLeftClicks { get; private set; }
      public int TotalRightClicks { get; private set; }
      public int TotalClicks { get { return (TotalLeftClicks + TotalRightClicks); } }

      public static SweeperBoard NewBoard(RectangleF bounds, int numMines, int rows, int cols)
      {
         return new SweeperBoard()
         {
            _bounds = bounds,
            _numMines = numMines,
            _rows = rows,
            _cols = cols,
            _totalTiles = (rows * cols),
            _tiles = new Tile[rows, cols]
         };
      }

      public void UpdateBounds(RectangleF bounds)
      {
         _bounds = bounds;
      }

      public void NewGame()
      {
         Reset();
         CreateTileSet();
      }

      public void Pause()
      {
         _paused = true;
      }

      public void Resume()
      {
         _paused = false;
      }

      public void Draw()
      {
         for (int row = 0; row < _rows; row++)
         {
            for (int col = 0; col < _cols; col++)
            {
               if (_tiles[row, col] != null)
                  _tiles[row, col].Draw();
            }
         }
      }

      public void Reveal()
      {
         GameOver = true;

         for (int row = 0; row < _rows; row++)
         {
            for (int col = 0; col < _cols; col++)
            {
               _tiles[row, col].Reveal();
            }
         }

         if (OnGameOver != null)
            OnGameOver(GameWon);
      }

      public void Dispose()
      {
         Mouse.OnLeftUp -= OnLeftMouseUp;
         Mouse.OnRightUp -= OnRightMouseUp;

         Tile.OnMineDetonated -= OnTileDetonated;
         Tile.OnFlagChanged -= OnTileFlagChanged;
         Tile.OnLeftClicked -= OnTileLeftClicked;

         for (int row = 0; row < _rows; row++)
         {
            for (int col = 0; col < _cols; col++)
            {
               if (_tiles[row, col] != null)
               {
                  _tiles[row, col].Dispose();
                  _tiles[row, col] = null;
               }
            }
         }

         _tiles = null;
         _random = null;
      }
   }
}
