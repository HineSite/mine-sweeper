using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace OpenTkPlay
{
   class Number
   {
      public int DisplayNumber { get; set; }

      private RectangleF _bounds;
      private Color _color = Color.Black;

      public Number(int number, RectangleF bounds, Color color)
      {
         DisplayNumber = number;
         _bounds = bounds;
         _color = color;
      }

      public void Draw()
      {
         float coordWidth = (_bounds.Width / 2f);
         float coordHeight = (_bounds.Height / 2f);

         GL.PushMatrix();

         GL.Enable(EnableCap.LineSmooth);

         GL.Translate(_bounds.X + coordWidth, _bounds.Y + coordHeight, 0);

         // Top
         if (DisplayNumber == 0 ||DisplayNumber == 2 ||DisplayNumber == 3 ||DisplayNumber == 5 ||DisplayNumber == 6 ||DisplayNumber == 7 || DisplayNumber == 8 || DisplayNumber == 9)
         {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(_color);
            GL.Vertex2(-coordWidth, -coordHeight);
            GL.Vertex2(coordWidth, -coordHeight);
            GL.End();
         }

         // Bottom
         if (DisplayNumber == 0 || DisplayNumber == 2 || DisplayNumber == 3 || DisplayNumber == 5 || DisplayNumber == 6 || DisplayNumber == 8)
         {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(_color);
            GL.Vertex2(-coordWidth, coordHeight);
            GL.Vertex2(coordWidth, coordHeight);
            GL.End();
         }

         // Top Left
         if (DisplayNumber == 0 || DisplayNumber == 4 || DisplayNumber == 5 || DisplayNumber == 6 || DisplayNumber == 8 || DisplayNumber == 9)
         {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(_color);
            GL.Vertex2(-coordWidth, -coordHeight);
            GL.Vertex2(-coordWidth, 0);
            GL.End();
         }

         // Bottom Left
         if (DisplayNumber == 0 || DisplayNumber == 2  || DisplayNumber == 6 || DisplayNumber == 8)
         {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(_color);
            GL.Vertex2(-coordWidth, 0);
            GL.Vertex2(-coordWidth, coordHeight);
            GL.End();
         }

         // Top Right
         if (DisplayNumber == 0 || DisplayNumber == 1 || DisplayNumber == 2 || DisplayNumber == 3 || DisplayNumber == 4 || DisplayNumber == 7 || DisplayNumber == 8 || DisplayNumber == 9)
         {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(_color);
            GL.Vertex2(coordWidth, -coordHeight);
            GL.Vertex2(coordWidth, 0);
            GL.End();
         }

         // Botom Right
         if (DisplayNumber == 0 || DisplayNumber == 1|| DisplayNumber == 3 || DisplayNumber == 4 || DisplayNumber == 5 || DisplayNumber == 6 || DisplayNumber == 7 || DisplayNumber == 8 || DisplayNumber == 9)
         {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(_color);
            GL.Vertex2(coordWidth, 0);
            GL.Vertex2(coordWidth, coordHeight);
            GL.End();
         }

         // Center
         if (DisplayNumber == 2 || DisplayNumber == 3 || DisplayNumber == 4 || DisplayNumber == 5 || DisplayNumber == 6 || DisplayNumber == 8 || DisplayNumber == 9 || DisplayNumber < 0)
         {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(_color);
            GL.Vertex2(-coordWidth, 0);
            GL.Vertex2(coordWidth, 0);
            GL.End();
         }

         GL.Disable(EnableCap.LineSmooth);

         GL.PopMatrix();
      }
   }
}
