using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace OpenTkPlay
{
   class Tile : IDisposable
   {
      public delegate void OnMineDetonatedHandler(Tile detonatedTile);
      public delegate void OnFlagChangedHandler(Tile tile, bool newState);
      public delegate void OnLeftClickedHandler(Tile tile);

      public static event OnMineDetonatedHandler OnMineDetonated
      {
         add
         {
            _events["OnMineDetonated"] = (OnMineDetonatedHandler)_events["OnMineDetonated"] + value;
         }

         remove
         {
            _events["OnMineDetonated"] = (OnMineDetonatedHandler)_events["OnMineDetonated"] - value;
         }
      }
      public static event OnFlagChangedHandler OnFlagChanged
      {
         add
         {
            _events["OnFlagChanged"] = (OnFlagChangedHandler)_events["OnFlagChanged"] + value;
         }

         remove
         {
            _events["OnFlagChanged"] = (OnFlagChangedHandler)_events["OnFlagChanged"] - value;
         }
      }
      /// <summary>
      ///  Will only be called if this tile is not mined and has not already been clicked.
      /// </summary>
      public static event OnLeftClickedHandler OnLeftClicked
      {
         add
         {
            _events["OnLeftClicked"] = (OnLeftClickedHandler)_events["OnLeftClicked"] + value;
         }

         remove
         {
            _events["OnLeftClicked"] = (OnLeftClickedHandler)_events["OnLeftClicked"] - value;
         }
      }

      private static Hashtable _events = new Hashtable();

      public Tile[] Siblings
      {
         get
         {
            return _siblings;
         }

         set
         {
            _siblings = value;

            foreach (Tile tile in value)
            {
               if (tile.Mined)
                  _minedSiblings++;
            }
         }
      }
      public bool Active
      {
         get
         {
            return (!_clicked && !_flagged) || (_flagged && !Mined);
         }
      }

      public bool Mined { get; set; }
      public Point GridPosition { get; set; }

      private RectangleF _bounds;
      private bool _clicked = false;
      private bool _flagged = false;
      private bool _detonated = false;
      private bool _revealed = false;
      private int _minedSiblings = 0;
      private Tile[] _siblings = null;
      private bool _disposed = false;

      private Tile() { }

      public static Tile Create(RectangleF bounds, Point gridPosition)
      {
         Tile tile = new Tile();

         tile.UpdateBounds(bounds);
         tile.GridPosition = gridPosition;

         return tile;
      }

      ~Tile()
      {
         Dispose();
      }

      public void Dispose()
      {
         _disposed = true;
      }

      public Tile UpdateBounds(RectangleF bounds)
      {
         _bounds = bounds;

         return this;
      }

      public void RightClick()
      {
         if (_revealed || _clicked || _detonated)
            return;

         _flagged = !_flagged;

         if (_events.ContainsKey("OnFlagChanged"))
            ((OnFlagChangedHandler)_events["OnFlagChanged"])(this, _flagged);
      }

      public void LeftClick()
      {
         if (_revealed || _clicked || _flagged || _detonated)
            return;

         if (Mined)
         {
            if (_events.ContainsKey("OnMineDetonated"))
               ((OnMineDetonatedHandler)_events["OnMineDetonated"])(this);

            _detonated = true;
         }
         else
         {
            _clicked = true;

            if (_events.ContainsKey("OnLeftClicked"))
               ((OnLeftClickedHandler)_events["OnLeftClicked"])(this);

            List<Tile> allSibs = new List<Tile>();

            foreach (Tile tile in Siblings)
            {
               if (!tile.Mined && tile._minedSiblings == 0)
               {
                  //tile.Click(); <- I'm pretty sure this is a bug?
                  allSibs.AddRange(tile.Siblings);
               }
            }

            // Why do we do this here, when we could do this up there?
            foreach (Tile tile in allSibs)
            {
               if (!tile.Mined)
                  tile.LeftClick();
            }
         }
      }

      public void Reveal()
      {
         _revealed = true;
      }

      public void Draw()
      {
         float coordWidthOrg = (_bounds.Width / 2f);
         float coordHeightOrg = (_bounds.Height / 2f);
         float coordWidth = coordWidthOrg;
         float coordHeight = coordHeightOrg;

         Color innerColor = (_clicked ? Color.LightGray : Color.White);

         if (_clicked && _minedSiblings > 0)
            innerColor = ColorTranslator.FromHtml("#dddddd");

         //if (Mined) // Cheater!
            //innerColor = Color.Orange;

         if (_revealed && Mined && !_flagged)
            innerColor = Color.DarkRed;

         GL.PushMatrix();

         GL.Translate(_bounds.X + coordWidthOrg, _bounds.Y + coordHeightOrg, 0);

         GL.Begin(PrimitiveType.Quads);
         GL.Color3(innerColor);
         GL.Vertex2(-coordWidth, coordHeight);
         GL.Vertex2(-coordWidth, -coordHeight);
         GL.Vertex2(coordWidth, -coordHeight);
         GL.Vertex2(coordWidth, coordHeight);
         GL.End();

         GL.PopMatrix();

         if (_flagged)
         {
            Flag flag = new Flag(_bounds);
            flag.Draw();

            if (_revealed && !Mined)
            {
               GL.PushMatrix();

               GL.Translate(_bounds.X + coordWidthOrg, _bounds.Y + coordHeightOrg, 0);

               GL.Begin(PrimitiveType.Lines);
               GL.Color3(Color.DarkRed);
               GL.Vertex2(-coordWidthOrg, coordHeightOrg);
               GL.Vertex2(coordWidthOrg, -coordHeightOrg);

               GL.Vertex2(-coordWidthOrg, -coordHeightOrg);
               GL.Vertex2(coordWidthOrg, coordHeightOrg);
               GL.End();

               GL.PopMatrix();
            }
         }

         if (_clicked && _minedSiblings > 0)
         {
            float numWidth = (_bounds.Width / 3.0f);
            float numHeight = (_bounds.Height / 2.5f);
            Color numColor = Color.Blue;

            switch (_minedSiblings)
            {
               case 2:
               {
                  numColor = Color.Green;

                  break;
               }

               case 3:
               {
                  numColor = Color.Red;

                  break;
               }

               case 4:
               {
                  numColor = Color.DarkBlue;

                  break;
               }

               case 5:
               {
                  numColor = Color.Black;

                  break;
               }

               case 6:
               {
                  numColor = Color.Turquoise;

                  break;
               }

               case 7:
               {
                  numColor = Color.DarkGreen;

                  break;
               }

               case 8:
               {
                  numColor = Color.DarkGray;

                  break;
               }
            }

            Number number = new Number(_minedSiblings, new RectangleF(_bounds.X + (coordWidth / 2f), _bounds.Y + (coordHeight / 2f), numWidth, numHeight), numColor);
            number.Draw();
         }
      }
   }
}
