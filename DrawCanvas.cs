using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace TheaterDaysScore {
    public class DrawCanvas : Canvas {
        private int measureHeight = 200;
        private int measureWidth = 175;
        private double noteSize = 10;

        private int drawWidth;
        private int drawHeight;

        private Pen borderPen;
        private Pen beatPen;
        private Pen halfBeatPen;
        private RenderTargetBitmap score;

        public DrawCanvas() {
            borderPen = new Pen(Colors.Black.ToUint32(), 8);
            beatPen = new Pen(Colors.Black.ToUint32(), 3);
            halfBeatPen = new Pen(Colors.Black.ToUint32(), 1);

            // Song info
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            StreamReader reader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/res/songlist.json")));
            List<Song> songs = JsonSerializer.Deserialize<List<Song>>(reader.ReadToEnd());
            int songNum = 0;

            // Rendering
            drawWidth = measureWidth;
            drawHeight = songs[songNum].measures * measureHeight;

            score = new RenderTargetBitmap(new PixelSize(measureWidth, songs[songNum].measures * measureHeight));
            using (IDrawingContextImpl ctx = score.CreateDrawingContext(null)) {
                for (int x = 0; x < songs[songNum].measures; x++) {
                    RenderMeasure(ctx, x);
                }
                foreach (Song.Note note in songs[songNum].notes) {
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

        private void RenderNote(IDrawingContextImpl ctx, Song.Note n) {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var b = new Bitmap(assets.Open(new Uri($"avares://TheaterDaysScore/res/tap.png")));
            double scale = Math.Log(n.size * 5) * noteSize;
            ctx.DrawImage(b.PlatformImpl, 1, new Rect(0, 0, b.Size.Width, b.Size.Height),
                new Rect(measureWidth / 7 * n.lane - scale / 2, drawHeight - measureHeight - measureHeight / 16 * n.beat - scale / 2, scale, scale));
            //ctx.DrawGeometry(new SolidColorBrush(Colors.Red), null, new EllipseGeometry(new Rect(0, 0, 25, 25)).PlatformImpl);
        }

        public override void Render(DrawingContext context) {
            base.Render(context);
            
            context.DrawImage(score, 1, new Rect(0, 0, score.Size.Width, score.Size.Height), new Rect(0, 0, drawWidth, drawHeight));
            this.Width = score.Size.Width;
            this.Height = score.Size.Height;
        }
    }
}