using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace TheaterDaysScore {
    public class DrawCanvas : Canvas {
        private int measureHeight = 200;
        private int measureWidth = 175;
        private double noteSize = 25/2;

        private int numMeasures;
        private Pen borderPen;
        private Pen beatPen;
        private Pen halfBeatPen;
        private RenderTargetBitmap score;

        public class Note {
            public int beat;
            public int size;
            public int column;

            public Note(int b, int s, int c) {
                beat = b;
                size = s;
                column = c;
            }
        }

        public DrawCanvas() {
            borderPen = new Pen(Colors.Black.ToUint32(), 8);
            beatPen = new Pen(Colors.Black.ToUint32(), 3);
            halfBeatPen = new Pen(Colors.Black.ToUint32(), 1);

            // Song info
            numMeasures = 4;
            List<Note> notes = new List<Note>() {
                new Note(0, 2, 2),
                new Note(0, 2, 5),
            };

            // Rendering
            score = new RenderTargetBitmap(new PixelSize(measureWidth, numMeasures * measureHeight));
            using (IDrawingContextImpl ctx = score.CreateDrawingContext(null)) {
                for (int x = 0; x < numMeasures; x++) {
                    RenderMeasure(ctx, x);
                }
                foreach (Note note in notes) {
                    RenderNote(ctx, note);
                }
            }
        }

        private void RenderMeasure(IDrawingContextImpl ctx, int num) {
            // Border
            ctx.DrawRectangle(borderPen, new Rect(0, num * measureHeight, measureWidth, measureHeight));
            // Horizontal
            ctx.DrawLine(halfBeatPen, new Point(0, num * measureHeight + measureHeight / 8), new Point(measureWidth, num * measureHeight + measureHeight / 8));
            ctx.DrawLine(beatPen, new Point(0, num * measureHeight + measureHeight / 4), new Point(measureWidth, num * measureHeight + measureHeight / 4));
            ctx.DrawLine(halfBeatPen, new Point(0, num * measureHeight + measureHeight / 8 * 3), new Point(measureWidth, num * measureHeight + measureHeight / 8 * 3));
            ctx.DrawLine(beatPen, new Point(0, num * measureHeight + measureHeight / 2), new Point(measureWidth, num * measureHeight + measureHeight / 2));
            ctx.DrawLine(halfBeatPen, new Point(0, num * measureHeight + measureHeight / 8 * 5), new Point(measureWidth, num * measureHeight + measureHeight / 8 * 5));
            ctx.DrawLine(beatPen, new Point(0, num * measureHeight + measureHeight / 4 * 3), new Point(measureWidth, num * measureHeight + measureHeight / 4 * 3));
            ctx.DrawLine(halfBeatPen, new Point(0, num * measureHeight + measureHeight / 8 * 7), new Point(measureWidth, num * measureHeight + measureHeight / 8 * 7));
            // Vertical
            ctx.DrawLine(beatPen, new Point(measureWidth / 7, num * measureHeight), new Point(measureWidth / 7, (num + 1) * measureHeight));
            ctx.DrawLine(beatPen, new Point(measureWidth / 7 * 2, num * measureHeight), new Point(measureWidth / 7 * 2, (num + 1) * measureHeight));
            ctx.DrawLine(beatPen, new Point(measureWidth / 7 * 3, num * measureHeight), new Point(measureWidth / 7 * 3, (num + 1) * measureHeight));
            ctx.DrawLine(beatPen, new Point(measureWidth / 7 * 4, num * measureHeight), new Point(measureWidth / 7 * 4, (num + 1) * measureHeight));
            ctx.DrawLine(beatPen, new Point(measureWidth / 7 * 5, num * measureHeight), new Point(measureWidth / 7 * 5, (num + 1) * measureHeight));
            ctx.DrawLine(beatPen, new Point(measureWidth / 7 * 6, num * measureHeight), new Point(measureWidth / 7 * 6, (num + 1) * measureHeight));
        }

        private void RenderNote(IDrawingContextImpl ctx, Note n) {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var b = new Bitmap(assets.Open(new Uri($"avares://TheaterDaysScore/res/tap.png")));
            double scale = n.size * noteSize;
            ctx.DrawImage(b.PlatformImpl, 1, new Rect(0, 0, b.Size.Width, b.Size.Height),
                new Rect(measureWidth / 7 * n.column - scale / 2, (numMeasures - 1) * measureHeight + measureHeight / 8 * n.beat - scale / 2, scale, scale));
            //ctx.DrawGeometry(new SolidColorBrush(Colors.Red), null, new EllipseGeometry(new Rect(0, 0, 25, 25)).PlatformImpl);
        }

        public override void Render(DrawingContext context) {
            base.Render(context);
            
            context.DrawImage(score, 1, new Rect(0, 0, score.Size.Width, score.Size.Height), new Rect(0, 0, measureWidth, numMeasures * measureHeight));
            this.Width = score.Size.Width;
            this.Height = score.Size.Height;
        }
    }
}