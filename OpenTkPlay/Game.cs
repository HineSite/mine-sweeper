using System;
using System.Drawing;
using System.IO;
using System.Timers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenTkPlay
{
   class Game : GameWindow
   {
      private const int NUMBER_OF_MINES = 50;
      private const int BOARD_ROWS = 18;
      private const int BOARD_COLS = 18;
      private const int HEADER_BOX_INNER_WIDTH = 60;
      private const int HEADER_BOX_INNER_HEIGHT = 30;
      private const int HEADER_BOX_BORDER_WIDTH = 2;
      private const int HEADER_SEPARATOR_HEIGHT = 2;
      private const int HEADER_HEIGHT = (HEADER_SEPARATOR_HEIGHT + HEADER_BOX_BORDER_WIDTH + HEADER_BOX_INNER_HEIGHT);

      private RectangleF _clientBounds;
      private RectangleF _gameBounds;
      private GameLogic _gameLogic;
      private Button _newGameButton;
      private Button _pauseGameButton;
      private Number _mines1;
      private Number _mines10;
      private Number _mines100;
      private Number _timer1;
      private Number _timer10;
      private Number _timer100;
      private Timer _timer;
      private int _eleapsedTime;
      private bool _gameOver = true;
      private bool _gamePaused = false;
      private TextWriter _pausedWriter = new TextWriter(15);

      public Game()
      {
         ClientSize = new Size(400, 400 + HEADER_HEIGHT);
         WindowBorder = OpenTK.WindowBorder.Fixed;

         /*string icoLocy = "./FormIcon.ico";
         byte[] bytes = File.ReadAllBytes(icoLocy);

         int count = 0;
         foreach (byte bite in bytes)
         {
            Console.Write(bite + ", ");
            count++;

            if (count % 30 == 0)
               Console.WriteLine("");
         }*/

         byte[] icoBytes = {
            0, 0, 1, 0, 1, 0, 16, 16, 0, 0, 1, 0, 32, 0, 104, 4, 0, 0, 22, 0, 0, 0, 40, 0, 0, 0, 16, 0, 0, 0,
            32, 0, 0, 0, 1, 0, 32, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0, 0, 0,
            176, 22, 18, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0, 0, 0, 0, 0,
            0, 0, 176, 22, 18, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0, 0, 0, 0, 0,
            0, 0, 0, 0, 176, 22, 18, 255, 176, 22, 18, 255, 176, 22, 18, 255, 176, 22, 18, 255, 176, 22, 18, 255, 176, 22, 18, 255, 176, 22,
            18, 255, 176, 22, 18, 255, 176, 22, 18, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 176, 22, 18, 255, 176, 22, 18, 0, 176, 22, 18, 0, 0, 0,0, 0, 176, 22, 18, 255, 176, 22,
            18, 0, 176, 22, 18, 0, 176, 22, 18, 0, 176, 22, 18, 255, 0, 0, 0, 0, 0, 0, 0, 0,0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 176, 22, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 176, 22,
            18, 255, 0, 0, 0, 0, 176, 22, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 176, 22, 18, 255, 176, 22, 18, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 176, 22, 18, 255, 0, 0, 0, 0, 176, 22, 18, 0, 0, 0, 0, 0, 176, 22, 18, 255, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 176, 22, 18, 255, 176, 22, 18, 255, 176, 22,
            18, 255, 176, 22, 18, 255, 176, 22, 18, 255, 176, 22, 18, 255, 176, 22, 18, 255,176, 22, 18, 255, 176, 22, 18, 255, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 176, 22,18, 255, 176, 22,
            18, 0, 176, 22, 18, 0, 176, 22, 18, 0, 176, 22, 18, 0, 176, 22, 18, 0, 176, 22,18, 0, 176, 22, 18, 0, 176, 22, 18, 255,
            11, 11, 186, 255, 11, 11, 186, 255, 11, 11, 186, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0,0, 0, 0, 11, 11, 186, 178, 12, 11,
            184, 255, 11, 11, 186, 72, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 11, 186, 0, 176, 22, 18, 0, 0, 0, 0, 0,
            176, 22, 18, 0, 0, 0, 0, 0, 11, 11, 186, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 11, 186, 29, 11, 11,
            186, 191, 0, 0, 0, 0, 176, 22, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0, 0, 176, 22, 18, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 11, 186, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0,0, 0, 0, 11, 11,
            186, 142, 11, 11, 186, 72, 0, 0, 0, 0, 176, 22, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            176, 22, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 11, 186, 255, 11, 11, 186, 39, 0, 0, 0, 0, 11, 11,
            186, 47, 11, 11, 186, 179, 0, 0, 0, 0, 176, 22, 18, 0, 176, 22, 18, 0, 176, 22,18, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            176, 22, 18, 0, 176, 22, 18, 0, 176, 22, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 11,186, 255, 11, 11, 186, 197, 11, 11,
            186, 167, 11, 11, 186, 144, 11, 11, 186, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 11,186, 255, 11, 11,
            186, 27, 11, 11, 186, 190, 11, 11, 186, 35, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 11,
            186, 255, 0, 0, 0, 0, 11, 11, 186, 28, 11, 11, 186, 191, 11, 11, 186, 34, 0, 0,0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 11, 11, 186, 255, 0, 0, 0, 0, 0, 0, 0, 0, 11, 11, 186, 29, 11, 11, 186, 191, 11, 11, 186, 34, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 11, 11, 186, 255, 11, 11, 186, 255, 11, 11, 186, 255, 0, 0, 0, 0, 11, 11,186, 255, 11, 11, 186, 255, 11, 11, 186, 255,
            11, 11, 186, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 254, 254, 0, 0, 254, 0, 0, 0, 254, 238, 0, 0, 255, 239, 0, 0,254, 238, 0, 0, 254, 0, 0, 0,
            254, 254, 0, 0, 28, 255, 0, 0, 189, 255, 0, 0, 187, 255, 0, 0, 187, 255, 0, 0, 135, 255, 0, 0, 175, 255, 0, 0, 183, 255,
            0, 0, 187, 255, 0, 0, 16, 255, 0, 0
         };

         //Bitmap bitmap = 
         using (Stream stream = new MemoryStream(icoBytes))
         {
            Icon = new Icon(stream);
         }



         /*string icoLoc = "./FormIcon.ico";
         if (File.Exists(icoLoc))
            Icon = new Icon(icoLoc);*/

         GL.MatrixMode(MatrixMode.Projection);
         GL.LoadIdentity();
         GL.Ortho(0.0f, ClientSize.Width, ClientSize.Height, 0.0f, 0.0f, 1.0f); // Left, Right, Bottom, Top

         UpdateBounds();

         Load += OnLoad;
         Resize += OnWindowResize;
         UpdateFrame += OnFrameUpdate;
         RenderFrame += OnFrameRender;
         Move += OnWindowMove;

         Run(60.0); // Run the game at 60 updates per second
      }

      public void OnWindowResize(object sender, EventArgs e)
      {
         GL.Viewport(0, 0, Width, Height);

         UpdateBounds();
      }

      public void OnWindowMove(object sender, EventArgs e)
      {
         GL.Viewport(0, 0, Width, Height);

         UpdateBounds();
      }

      public void UpdateBounds()
      {
         int borderWidth = ((Bounds.Width - ClientRectangle.Width) / 2);
         int titleHeight = (Bounds.Height - ClientRectangle.Height - borderWidth);

         _clientBounds = new RectangleF(Bounds.X + borderWidth, Bounds.Y + titleHeight, ClientRectangle.Width, ClientRectangle.Height);
         _gameBounds = new RectangleF(0, HEADER_HEIGHT, ClientRectangle.Width, ClientRectangle.Height - HEADER_HEIGHT);

         //if (_gameLogic != null)
            //_gameLogic.UpdateBounds(_gameBounds);
      }

      public void OnLoad(object sender, EventArgs e)
      {
         // setup settings, load textures, sounds
         VSync = VSyncMode.On;

         _gameLogic = GameLogic.Create(_gameBounds, NUMBER_OF_MINES, BOARD_ROWS, BOARD_COLS);

         { // Set up game controls
            int width = 75;
            int height = 20;
            int widthHalf = (int)Math.Round(width / 2f);
            int heightHalf = (int)Math.Round(height / 2f);

            _newGameButton = new Button("New Game", new RectangleF(((ClientSize.Width / 2.0f) - width - 5), ((HEADER_BOX_INNER_HEIGHT / 2.0f) - heightHalf), width, height));
            _pauseGameButton = new Button("Pause", new RectangleF(((ClientSize.Width / 2.0f) + 5), ((HEADER_BOX_INNER_HEIGHT / 2.0f) - heightHalf), width, height));

            _newGameButton.OnClick += delegate()
            {
               NewGame();
            };

            _pauseGameButton.OnClick += delegate()
            {
               if (!_gameOver)
               {
                  if (_timer.Enabled)
                  {
                     // Disable!
                     _timer.Enabled = false;
                     _gamePaused = true;
                     _gameLogic.Pause();

                     _pauseGameButton.UpdateText("Resume");

                  }
                  else
                  {
                     // Enable!
                     _timer.Enabled = true;
                     _gamePaused = false;
                     _gameLogic.Resume();

                     _pauseGameButton.UpdateText("Pause");
                  }
               }
            };
         }

         { // Setup game timer
            _timer = new Timer(1000);
            _timer.Elapsed += delegate(object senderT, ElapsedEventArgs eT)
            {
               UpdateTimer(_eleapsedTime++);
            };

            _gameLogic.OnGameStarted += delegate()
            {
               _gameOver = false;
               _timer.Start();
            };

            _gameLogic.OnGameOver += delegate(bool gameWon)
            {
               _timer.Stop();
               _gameOver = true;
            };
         }

         { // Set up mine counter
            int width = ((HEADER_BOX_INNER_WIDTH - 20) / 3); // 20 for left and right margins
            int height = (HEADER_BOX_INNER_HEIGHT - 10); // 10 for top and bottom margin
            int leastX = (HEADER_BOX_BORDER_WIDTH + HEADER_BOX_INNER_WIDTH - width - 5); // 5 for right margin
            int leastY = (HEADER_BOX_BORDER_WIDTH + 5); // 5 for top margin

            _mines1 = new Number(0, new RectangleF(leastX, leastY, width, height), Color.Navy);
            _mines10 = new Number(0, new RectangleF(leastX -= (width + 5), leastY, width, height), Color.Navy);
            _mines100 = new Number(0, new RectangleF(leastX -= (width + 5), leastY, width, height), Color.Navy);


            _gameLogic.OnFlagChanged += UpdateFlagCounter;
         }

         { // Set up timer and counter
            int width = ((HEADER_BOX_INNER_WIDTH - 20) / 3); // 20 for left and right margins
            int height = (HEADER_BOX_INNER_HEIGHT - 10); // 10 for top and bottom margin
            int leastX = (ClientSize.Width - HEADER_BOX_BORDER_WIDTH - 5 - width); // 5 for right margin
            int leastY = (HEADER_BOX_BORDER_WIDTH + 5); // 5 for top margin

            _timer1 = new Number(0, new RectangleF(leastX, leastY, width, height), Color.Navy);
            _timer10 = new Number(0, new RectangleF(leastX -= (width + 5), leastY, width, height), Color.Navy);
            _timer100 = new Number(0, new RectangleF(leastX -= (width + 5), leastY, width, height), Color.Navy);
         }

         base.KeyUp += (object senderJo, KeyboardKeyEventArgs eJo) =>
         {
            switch (eJo.Key)
            {
               case Key.F5:
               {
                  NewGame();

                  break;
               }
            }
         };

         base.KeyDown += (object senderJo, KeyboardKeyEventArgs eJo) =>
         {
            switch (eJo.Key)
            {
               case Key.Escape:
               {
                  Exit();

                  break;
               }
            }
         };

         _pausedWriter.AddLine("Game PauseD:!");
         _pausedWriter.CreateTexture();

         NewGame();
      }

      private void NewGame()
      {
         _gamePaused = false;
         _eleapsedTime = 0;
         UpdateTimer(0);

         if (_timer != null)
         {
            _timer.Stop();
         }

         _gameLogic.NewGame();
      }

      public void UpdateFlagCounter(int flaggedTiles)
      {
         int remaining = (NUMBER_OF_MINES - flaggedTiles);
         string padding = (remaining < 0 ? "00" : "000");
         char[] chars = remaining.ToString(padding).ToCharArray();

         if (chars.Length != 3)
            return;

         _mines100.DisplayNumber = chars[0] - 48;
         _mines10.DisplayNumber = chars[1] - 48;
         _mines1.DisplayNumber = chars[2] - 48;
      }

      public void UpdateTimer(int time)
      {
         char[] chars = time.ToString("000").ToCharArray();

         if (chars.Length != 3)
            return;

         _timer100.DisplayNumber = chars[0] - 48;
         _timer10.DisplayNumber = chars[1] - 48;
         _timer1.DisplayNumber = chars[2] - 48;
      }

      public void OnFrameUpdate(object sender, FrameEventArgs e)
      {
         if (!Focused)
            return;

         OpenTkPlay.Mouse.Update(_clientBounds);
      }

      public void DrawHeader()
      {
         // Background
         GL.Begin(PrimitiveType.Quads);
         GL.Color3(Color.White);
         GL.Vertex2(0.0f, 0.0f);
         GL.Vertex2(0.0f, HEADER_HEIGHT);
         GL.Vertex2(ClientSize.Width, HEADER_HEIGHT);
         GL.Vertex2(ClientSize.Width, 0.0f);
         GL.End();

         _newGameButton.Draw();
         _pauseGameButton.Draw();

         _mines1.Draw();
         _mines10.Draw();
         _mines100.Draw();

         _timer1.Draw();
         _timer10.Draw();
         _timer100.Draw();

         { // Header Game seperator
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.LightGray);
            GL.Vertex2(0.0f, HEADER_HEIGHT);
            GL.Vertex2(ClientSize.Width, HEADER_HEIGHT);

            GL.Color3(Color.DarkGray);
            GL.Vertex2(0.0f, HEADER_HEIGHT - 1.0f);
            GL.Vertex2(ClientSize.Width, HEADER_HEIGHT - 1.0f);

            GL.Color3(Color.LightGray);
            GL.Vertex2(0.0f + HEADER_BOX_BORDER_WIDTH + HEADER_BOX_INNER_WIDTH + HEADER_BOX_BORDER_WIDTH, HEADER_HEIGHT - 2);
            GL.Vertex2(ClientSize.Width - (HEADER_BOX_BORDER_WIDTH + HEADER_BOX_INNER_WIDTH + HEADER_BOX_BORDER_WIDTH), HEADER_HEIGHT - 2);
            GL.End();
         }

         { // Left Side Box
            { // Left Accent
               GL.Begin(PrimitiveType.Lines);
               GL.Color3(Color.LightGray);
               GL.Vertex2(1.0f, HEADER_BOX_BORDER_WIDTH - 1.0f);
               GL.Vertex2(1.0f, HEADER_HEIGHT);

               GL.Color3(Color.DarkGray);
               GL.Vertex2(2.0f, HEADER_BOX_BORDER_WIDTH - 1.0f);
               GL.Vertex2(2.0f, HEADER_HEIGHT - HEADER_SEPARATOR_HEIGHT);
               GL.End();
            }

            { // Top
               GL.Begin(PrimitiveType.Lines);
               GL.Color3(Color.LightGray);
               GL.Vertex2(0.0f, 1.0f);
               GL.Vertex2(0.0f + HEADER_BOX_BORDER_WIDTH + HEADER_BOX_INNER_WIDTH + HEADER_BOX_BORDER_WIDTH, 0.0f);

               GL.Color3(Color.DarkGray);
               GL.Vertex2(1.0f, 2.0f);
               GL.Vertex2(0.0f + HEADER_BOX_BORDER_WIDTH + HEADER_BOX_INNER_WIDTH + HEADER_BOX_BORDER_WIDTH, 1.0f);
               GL.End();
            }

            { // Right Accent
               GL.Begin(PrimitiveType.Lines);
               GL.Color3(Color.DarkGray);
               GL.Vertex2(1.0f + HEADER_BOX_INNER_WIDTH + HEADER_BOX_BORDER_WIDTH, HEADER_BOX_BORDER_WIDTH - 1.0f);
               GL.Vertex2(1.0f + HEADER_BOX_INNER_WIDTH + HEADER_BOX_BORDER_WIDTH, HEADER_HEIGHT - HEADER_SEPARATOR_HEIGHT);

               GL.Color3(Color.LightGray);
               GL.Vertex2(2.0f + HEADER_BOX_INNER_WIDTH + HEADER_BOX_BORDER_WIDTH, HEADER_BOX_BORDER_WIDTH - 1.0f);
               GL.Vertex2(2.0f + HEADER_BOX_INNER_WIDTH + HEADER_BOX_BORDER_WIDTH, HEADER_HEIGHT - 2.0f);
               GL.End();
            }
         }

         { // Right Side Box
            int leftStart = (ClientSize.Width - HEADER_BOX_BORDER_WIDTH - HEADER_BOX_INNER_WIDTH - HEADER_BOX_BORDER_WIDTH);

            { // Left Accent
               GL.Begin(PrimitiveType.Lines);
               GL.Color3(Color.LightGray);
               GL.Vertex2(1.0f + leftStart, HEADER_BOX_BORDER_WIDTH - 1.0f);
               GL.Vertex2(1.0f + leftStart, HEADER_HEIGHT - 2.0f);

               GL.Color3(Color.DarkGray);
               GL.Vertex2(2.0f + leftStart, HEADER_BOX_BORDER_WIDTH);
               GL.Vertex2(2.0f + leftStart, HEADER_HEIGHT - 1.0f);
               GL.End();
            }

            { // Top
               GL.Begin(PrimitiveType.Lines);
               GL.Color3(Color.LightGray);
               GL.Vertex2(0.0f + leftStart, 1.0f);
               GL.Vertex2(0.0f + leftStart + HEADER_BOX_BORDER_WIDTH + HEADER_BOX_INNER_WIDTH + HEADER_BOX_BORDER_WIDTH, 0.0f);

               GL.Color3(Color.DarkGray);
               GL.Vertex2(1.0f + leftStart, 2.0f);
               GL.Vertex2(0.0f + leftStart + HEADER_BOX_BORDER_WIDTH + HEADER_BOX_INNER_WIDTH + HEADER_BOX_BORDER_WIDTH, 1.0f);
               GL.End();
            }

            { // Right Accent
               GL.Begin(PrimitiveType.Lines);
               GL.Color3(Color.DarkGray);
               GL.Vertex2(1.0f + leftStart + HEADER_BOX_INNER_WIDTH + HEADER_BOX_BORDER_WIDTH, HEADER_BOX_BORDER_WIDTH);
               GL.Vertex2(1.0f + leftStart + HEADER_BOX_INNER_WIDTH + HEADER_BOX_BORDER_WIDTH, HEADER_HEIGHT - HEADER_SEPARATOR_HEIGHT);

               GL.Color3(Color.LightGray);
               GL.Vertex2(2.0f + leftStart + HEADER_BOX_INNER_WIDTH + HEADER_BOX_BORDER_WIDTH, HEADER_BOX_BORDER_WIDTH - 1.0f);
               GL.Vertex2(2.0f + leftStart + HEADER_BOX_INNER_WIDTH + HEADER_BOX_BORDER_WIDTH, HEADER_HEIGHT - HEADER_SEPARATOR_HEIGHT + 1.0f);
               GL.End();
            }
         }
      }

      public void OnFrameRender(object sender, FrameEventArgs e)
      {
         GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.AccumBufferBit);
         GL.Enable(EnableCap.Blend);
         GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
         GL.Enable(EnableCap.LineSmooth);

         // Background
         GL.Begin(PrimitiveType.Quads);
         GL.Color3(Color.LightGray);
         GL.Vertex2(0.0f, 0.0f);
         GL.Vertex2(0.0f, ClientSize.Height);
         GL.Vertex2(ClientSize.Width, ClientSize.Height);
         GL.Vertex2(ClientSize.Width, 0.0f);
         GL.End();

         _gameLogic.Draw();

         DrawHeader();

         {// Draw Game Paused Display
            if (_gamePaused)
            {
               GL.Begin(PrimitiveType.Quads);
               GL.Color4(Color.FromArgb(214, Color.White));
               GL.Vertex2(_gameBounds.X, _gameBounds.Y);
               GL.Vertex2(_gameBounds.X, _gameBounds.Height + _gameBounds.Y);
               GL.Vertex2(_gameBounds.Width, _gameBounds.Height + _gameBounds.Y);
               GL.Vertex2(_gameBounds.Width, _gameBounds.Y);
               GL.End();

               GL.PushMatrix();

               GL.Translate(_gameBounds.Width / 2f, _gameBounds.Height / 2f, 0);

               _pausedWriter.Draw();

               GL.PopMatrix();
            }
         }

         SwapBuffers();
      }
   }
}
