using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using OpenTK.Graphics.OpenGL;

namespace OpenTkPlay
{
   class TextWriter
   {
      private Font _font;
      private Bitmap _bitmap;
      private int _textureId;
      private List<Line> _textLines = new List<Line>();
      private bool _created = false;

      private class Line
      {
         public string Text = "";
         public Size Size;

         public Line(string text)
         {
            Text = text;
         }
      }

      ~TextWriter()
      {
         Dispose();
      }

      public TextWriter(int fontEms) : this(new Font(new FontFamily("Segoe UI"), fontEms)) { }

      public TextWriter(Font font)
      {
         _font = font;
      }

      private void Update()
      {
         if (_created)
            return;

         _created = true;

         int drawX = 0;
         int drawY = 0;
         int totalHeight = 0;
         int maxWidth = 0;
         _bitmap = new Bitmap(1, 1);
         Graphics g = Graphics.FromImage(_bitmap);

         {
            SizeF sizeF;

            foreach (Line line in _textLines)
            {
               sizeF = g.MeasureString(line.Text, _font);
               line.Size = new Size((int)Math.Ceiling(sizeF.Width) + 0, (int)Math.Ceiling(sizeF.Height) + 0);
               totalHeight += line.Size.Height;

               if (line.Size.Width > maxWidth)
                  maxWidth = line.Size.Width;
            }
         }

         g.Flush();
         _bitmap.Dispose();
         g.Dispose();

         _bitmap = new Bitmap(maxWidth, totalHeight);
         g = Graphics.FromImage(_bitmap);

         // Ensure the best possible quality rendering
         g.SmoothingMode = SmoothingMode.AntiAlias;                  // The smoothing mode specifies whether lines, curves, and the edges of filled areas use smoothing (also called antialiasing). One exception is that path gradient brushes do not obey the smoothing mode. Areas filled using a PathGradientBrush are rendered the same way (aliased) regardless of the SmoothingMode property.
         g.InterpolationMode = InterpolationMode.HighQualityBicubic; // The interpolation mode determines how intermediate values between two endpoints are calculated.
         g.PixelOffsetMode = PixelOffsetMode.HighQuality;            // Use this property to specify either higher quality, slower rendering, or lower quality, faster rendering of the contents of this Graphics object.
         g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;   // This one is important

         foreach (Line line in _textLines)
         {
            g.DrawString(line.Text, _font, Brushes.Black, new RectangleF(drawX, drawY, line.Size.Width, line.Size.Height));
            drawY += line.Size.Height;
         }

         g.Flush();
      }

      public TextWriter AddLine(string textLine)
      {
         _textLines.Add(new Line(textLine));

         return this;
      }

      public void Clear()
      {
         try
         {
            if (_bitmap != null)
               _bitmap.Dispose();

            _textLines.Clear();

            _created = false;
         }
         catch { }
      }

      public TextWriter CreateTexture()
      {
         Update();

         int textureId;

         GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Replace); //Important, or wrong color on some computers
         GL.GenTextures(1, out textureId);
         GL.BindTexture(TextureTarget.Texture2D, textureId);

         BitmapData data = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

         GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
         GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

         GL.Finish();

         _bitmap.UnlockBits(data);

         _textureId = textureId;

         return this;
      }

      public void Dispose()
      {
         Clear();
      }

      public void Draw()
      {
         if (_bitmap == null)
            return;

         float widthHalf = (_bitmap.Width / 2f);
         float heightHalf = (_bitmap.Height / 2f);

         GL.PushMatrix();

         GL.Translate(-widthHalf, -heightHalf, 0);
         
         GL.Enable(EnableCap.Blend);
         GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
         GL.Enable(EnableCap.Texture2D);
         GL.BindTexture(TextureTarget.Texture2D, _textureId);


         GL.Begin(PrimitiveType.Quads);
         GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
         GL.TexCoord2(1, 0); GL.Vertex2(_bitmap.Width, 0);
         GL.TexCoord2(1, 1); GL.Vertex2(_bitmap.Width, _bitmap.Height);
         GL.TexCoord2(0, 1); GL.Vertex2(0, _bitmap.Height);
         GL.End();

         GL.Disable(EnableCap.Blend);
         GL.Disable(EnableCap.Texture2D);


         GL.PopMatrix();
      }
   }
}
