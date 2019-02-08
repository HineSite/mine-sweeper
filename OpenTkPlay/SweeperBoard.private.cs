using System;
using System.Drawing;
using System.Linq;

namespace OpenTkPlay
{
   public partial class SweeperBoard
   {
      private Tile[,] _tiles;
      private Random _random = new Random();
      private RectangleF _bounds;
      private int _numMines;
      private int _rows;
      private int _cols;
      private int _totalTiles;
      private float _tileWidth;
      private float _tileHeight;
      private float _tileGap = 1;
      private Point _noMinePos;
      private bool _paused = false;

      private SweeperBoard()
      {
         Mouse.OnLeftUp += OnLeftMouseUp;
         Mouse.OnRightUp += OnRightMouseUp;

         Tile.OnMineDetonated += OnTileDetonated;
         Tile.OnFlagChanged += OnTileFlagChanged;
         Tile.OnLeftClicked += OnTileLeftClicked;
      }

      ~SweeperBoard()
      {
         Dispose();
      }

      private Tile GetTileAtMousePos(PointF mousePos)
      {
         if (_bounds.IntersectsWith(mousePos))
         {
            PointF adjustedPos = mousePos.Offset(-_bounds.X, -_bounds.Y);
            int col = (int)(adjustedPos.X / (_tileWidth + _tileGap));
            int row = (int)(adjustedPos.Y / (_tileHeight + _tileGap));

            if (row >= 0 && row < _rows && col >= 0 && col < _cols)
               return _tiles[row, col];
         }

         return null;
      }

      private void OnLeftMouseUp(MouseEventArgs mouseEventArgs)
      {
         if (_paused || GameOver)
            return;

         Tile tile = GetTileAtMousePos(mouseEventArgs.WindowPos);
         if (tile != null)
         {
            if (TotalLeftClicks == 0)
            {
               MineField(_numMines, tile.GridPosition);

               if (OnGameStarted != null)
                  OnGameStarted();
            }

            TotalLeftClicks++;

            tile.LeftClick();
         }
      }

      private void OnRightMouseUp(MouseEventArgs mouseEventArgs)
      {
         if (_paused || GameOver)
            return;

         if (TotalLeftClicks == 0)
            return;

         Tile tile = GetTileAtMousePos(mouseEventArgs.WindowPos);
         if (tile != null)
         {
            TotalRightClicks++;

            tile.RightClick();
         }
      }

      private void OnTileDetonated(Tile tile)
      {
         GameWon = false;
         Reveal();
      }

      private void OnTileFlagChanged(Tile tile, bool newState)
      {
         if (newState)
         {
            FlaggedTiles++;
         }
         else
         {
            FlaggedTiles--;
         }

         CheckForWin();
      }

      private void OnTileLeftClicked(Tile tile)
      {
         TotalLeftClicks++;
         CheckForWin();
      }

      private void CheckForWin()
      {
         if (IsCleared())
         {
            GameOver = true;
            GameWon = true;

            if (OnGameOver != null)
               OnGameOver(GameWon);
         }
      }

      private bool IsCleared()
      {
         if (FlaggedTiles != _numMines)
            return false;

         for (int row = 0; row < _rows; row++)
         {
            for (int col = 0; col < _cols; col++)
            {
               if (_tiles[row, col].Active)
                  return false;
            }
         }

         return true;
      }

      private void Reset()
      {
         GameOver = false;
         GameWon = false;
         FlaggedTiles = 0;
         TotalLeftClicks = 0;
         TotalRightClicks = 0;
         _paused = false;

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
      }

      private void CreateTileSet()
      {
         _tileWidth = ((_bounds.Width / (float)_cols) - _tileGap);
         _tileHeight = ((_bounds.Height / (float)_rows) - _tileGap);
         float widthHalf = (_tileWidth / 2);
         float heightHalf = (_tileHeight / 2);
         float xPos = _bounds.X;
         float yPos = _bounds.Y;

         for (int row = 0; row < _rows; row++)
         {
            xPos = _bounds.X;

            for (int col = 0; col < _cols; col++)
            {
               _tiles[row, col] = Tile.Create(new RectangleF(xPos, yPos, _tileWidth, _tileHeight), new Point(row, col));

               xPos += (_tileWidth + _tileGap);
            }

            yPos += (_tileHeight + _tileGap);
         }
      }

      public void MineField(int numMines, Point avoidTile)
      {
         int row = 0;
         int col = 0;

         while (numMines != 0)
         {
            row = _random.Next(0, _rows);
            col = _random.Next(0, _cols);

            if (!_tiles[row, col].Mined && (row != avoidTile.X || col != avoidTile.Y))
            {
               _tiles[row, col].Mined = true;

               numMines--;
            }
         }

         PopulateSiblings();
      }

      private void PopulateSiblings()
      {
         Tile[] siblings = null;
         int cols = (_cols - 1);
         int rows = (_rows - 1);

         for (int row = 0; row < _rows; row++)
         {
            for (int col = 0; col < _cols; col++)
            {
               siblings = new Tile[8];
               siblings[0] = (row > 0 && col > 0) ? _tiles[row - 1, col - 1] : null;
               siblings[1] = (row > 0) ? _tiles[row - 1, col - 0] : null;
               siblings[2] = (row > 0 && col < cols) ? _tiles[row - 1, col + 1] : null;
               siblings[3] = (col < cols) ? _tiles[row - 0, col + 1] : null;
               siblings[4] = (row < rows && col < cols) ? _tiles[row + 1, col + 1] : null;
               siblings[5] = (row < rows) ? _tiles[row + 1, col - 0] : null;
               siblings[6] = (row < cols && col > 0) ? _tiles[row + 1, col - 1] : null;
               siblings[7] = (col > 0) ? _tiles[row - 0, col - 1] : null;

               _tiles[row, col].Siblings = (siblings.Where(o => o != null).ToArray());
            }
         }
      }
   }
}
