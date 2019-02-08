using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace OpenTkPlay
{
   public class GameLogic
   {
      public delegate void OnFlagChangedHandler(int falggedTiles);
      public delegate void OnGameStartedHandler();
      public delegate void OnGameOverHandler(bool gameWon);

      public event OnFlagChangedHandler OnFlagChanged;
      public event OnGameStartedHandler OnGameStarted;
      public event OnGameOverHandler OnGameOver;

      private RectangleF _bounds;
      private SweeperBoard _sweeperBoard;
      private int _flaggedTiles = -1;
      private int _numMines;
      private int _rows;
      private int _cols;
      private TextWriter _win1;
      private TextWriter _win2;
      private TextWriter _win3;

      private GameLogic(RectangleF bounds, int numMines, int rows, int cols)
      {
         _bounds = bounds;
         _numMines = numMines;
         _rows = rows;
         _cols = cols;
         _sweeperBoard = SweeperBoard.NewBoard(bounds, numMines, rows, cols);

         _sweeperBoard.OnGameStarted += delegate()
         {
            if (OnGameStarted != null)
               OnGameStarted();
         };

         _sweeperBoard.OnGameOver += delegate(bool gameWon)
         {
            if (OnGameOver != null)
               OnGameOver(gameWon);
         };

         _win1 = new TextWriter(10).AddLine("\"The supreme art of war is to subdue").CreateTexture();
         _win2 = new TextWriter(10).AddLine("the enemy without fighting.\"").CreateTexture();
         _win3 = new TextWriter(8).AddLine("- Sun Tzu, The Art of War").CreateTexture();
      }

      public static GameLogic Create(RectangleF bounds, int numMines, int rows, int cols)
      {
         return new GameLogic(bounds, numMines, rows, cols);
      }

      public void UpdateBounds(RectangleF bounds)
      {
         _bounds = bounds;
         _sweeperBoard.UpdateBounds(bounds);
      }

      public void NewGame()
      {
         _flaggedTiles = -1;
         _sweeperBoard.NewGame();
      }

      public void Pause()
      {
         _sweeperBoard.Pause();
      }

      public void Resume()
      {
         _sweeperBoard.Resume();
      }

      public void Draw()
      {
         _sweeperBoard.Draw();

         if (_flaggedTiles != _sweeperBoard.FlaggedTiles)
         {
            _flaggedTiles = _sweeperBoard.FlaggedTiles;
            if (OnFlagChanged != null)
               OnFlagChanged(_sweeperBoard.FlaggedTiles);
         }

         if (_sweeperBoard.GameOver && _sweeperBoard.GameWon)
         {
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(Color.FromArgb(214, Color.White));
            GL.Vertex2(_bounds.X, _bounds.Y);
            GL.Vertex2(_bounds.X, _bounds.Height + _bounds.Y);
            GL.Vertex2(_bounds.Width, _bounds.Height + _bounds.Y);
            GL.Vertex2(_bounds.Width, _bounds.Y);
            GL.End();

            GL.PushMatrix();
            GL.Translate((_bounds.Width / 2f) - 50, _bounds.Height / 2f, 0);
            _win1.Draw();
            GL.PopMatrix();

            GL.PushMatrix();
            GL.Translate((_bounds.Width / 2f) + 75, (_bounds.Height / 2f) + 15, 0);
            _win2.Draw();
            GL.PopMatrix();

            GL.PushMatrix();
            GL.Translate((_bounds.Width / 2f) - 95, (_bounds.Height / 2f) + 50, 0);
            _win3.Draw();
            GL.PopMatrix();
         }
      }
   }
}
