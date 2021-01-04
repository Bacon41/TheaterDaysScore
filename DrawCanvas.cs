using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
        private double laneWidth;
        private double quarterBeatHeight;

        private int drawWidth;
        private int drawHeight;

        private Pen borderPen;
        private Pen beatPen;
        private Pen halfBeatPen;
        private Pen holdPen;
        private Bitmap tapImg;
        private Bitmap leftImg;
        private Bitmap rightImg;
        private Bitmap upImg;
        private RenderTargetBitmap score;

        public DrawCanvas() {
            borderPen = new Pen(Colors.Black.ToUint32(), 8);
            beatPen = new Pen(Colors.Black.ToUint32(), 3);
            halfBeatPen = new Pen(Colors.Black.ToUint32(), 1);
            holdPen = new Pen(Colors.Red.ToUint32(), Math.Log(5) * noteSize);

            // Song info
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            StreamReader reader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/res/songlist.json")));
            List<Song> songs = JsonSerializer.Deserialize<List<Song>>(reader.ReadToEnd());
            int songNum = 2;

            // Rendering
            drawWidth = measureWidth;
            drawHeight = songs[songNum].displayMeasures * measureHeight;
            laneWidth = (double)measureWidth / 7;
            quarterBeatHeight = (double)measureHeight / 16;

            tapImg = new Bitmap(assets.Open(new Uri($"avares://TheaterDaysScore/res/tap.png")));
            leftImg = new Bitmap(assets.Open(new Uri($"avares://TheaterDaysScore/res/left.png")));
            rightImg = new Bitmap(assets.Open(new Uri($"avares://TheaterDaysScore/res/right.png")));
            upImg = new Bitmap(assets.Open(new Uri($"avares://TheaterDaysScore/res/up.png")));

            score = new RenderTargetBitmap(new PixelSize(drawWidth, drawHeight));
            using (IDrawingContextImpl ctx = score.CreateDrawingContext(null)) {
                for (int x = 0; x < songs[songNum].displayMeasures; x++) {
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
            ctx.DrawLine(halfBeatPen, new Point(0, num * measureHeight + quarterBeatHeight * 2), new Point(measureWidth, num * measureHeight + quarterBeatHeight * 2));
            ctx.DrawLine(beatPen, new Point(0, num * measureHeight + quarterBeatHeight * 4), new Point(measureWidth, num * measureHeight + quarterBeatHeight * 4));
            ctx.DrawLine(halfBeatPen, new Point(0, num * measureHeight + quarterBeatHeight * 6), new Point(measureWidth, num * measureHeight + quarterBeatHeight * 6));
            ctx.DrawLine(beatPen, new Point(0, num * measureHeight + quarterBeatHeight * 8), new Point(measureWidth, num * measureHeight + quarterBeatHeight * 8));
            ctx.DrawLine(halfBeatPen, new Point(0, num * measureHeight + quarterBeatHeight * 10), new Point(measureWidth, num * measureHeight + quarterBeatHeight * 10));
            ctx.DrawLine(beatPen, new Point(0, num * measureHeight + quarterBeatHeight * 12), new Point(measureWidth, num * measureHeight + quarterBeatHeight * 12));
            ctx.DrawLine(halfBeatPen, new Point(0, num * measureHeight + quarterBeatHeight * 14), new Point(measureWidth, num * measureHeight + quarterBeatHeight * 14));
            // Vertical
            ctx.DrawLine(halfBeatPen, new Point(laneWidth, num * measureHeight), new Point(laneWidth, (num + 1) * measureHeight));
            ctx.DrawLine(halfBeatPen, new Point(laneWidth * 2, num * measureHeight), new Point(laneWidth * 2, (num + 1) * measureHeight));
            ctx.DrawLine(halfBeatPen, new Point(laneWidth * 3, num * measureHeight), new Point(laneWidth * 3, (num + 1) * measureHeight));
            ctx.DrawLine(halfBeatPen, new Point(laneWidth * 4, num * measureHeight), new Point(laneWidth * 4, (num + 1) * measureHeight));
            ctx.DrawLine(halfBeatPen, new Point(laneWidth * 5, num * measureHeight), new Point(laneWidth * 5, (num + 1) * measureHeight));
            ctx.DrawLine(halfBeatPen, new Point(laneWidth * 6, num * measureHeight), new Point(laneWidth * 6, (num + 1) * measureHeight));
        }

        private Point GetCenter(Song.Note note) {
            double xPos = laneWidth * note.lane;
            if (xPos == 0) {
                xPos = measureWidth / 2;
            }
            return new Point(xPos, drawHeight - measureHeight - quarterBeatHeight * (note.measure * 16 + note.quarterBeat));
        }

        private void RenderNote(IDrawingContextImpl ctx, Song.Note note) {
            Point center = GetCenter(note);
            if (note.waypoints != null) {
                PathGeometry holdLine = new PathGeometry();
                StreamGeometryContext holdCtx = holdLine.Open();
                holdCtx.BeginFigure(new Point(center.X, center.Y), false);
                foreach (Song.Note point in note.waypoints) {
                    Point pointCenter = GetCenter(point);
                    holdCtx.LineTo(new Point(pointCenter.X, pointCenter.Y));
                }
                ctx.DrawGeometry(null, holdPen, holdLine.PlatformImpl);
            }
            
            Bitmap noteImg = tapImg;
            switch (note.type) {
                case Song.Note.NoteType.tap:
                    noteImg = tapImg;
                    break;
                case Song.Note.NoteType.leftFlick:
                    noteImg = leftImg;
                    break;
                case Song.Note.NoteType.rightFlick:
                    noteImg = rightImg;
                    break;
                case Song.Note.NoteType.upFlick:
                    noteImg = upImg;
                    break;
            }
            double noteScale = Math.Log(note.size * 5) * noteSize;
            double heightScale;
            double widthScale;
            if (noteImg.Size.Width > noteImg.Size.Height) {
                heightScale = noteScale;
                widthScale = heightScale * noteImg.Size.Width / noteImg.Size.Height;
            } else {
                widthScale = noteScale;
                heightScale = widthScale * noteImg.Size.Height / noteImg.Size.Width;
            }
            ctx.DrawImage(noteImg.PlatformImpl, 1, new Rect(0, 0, noteImg.Size.Width, noteImg.Size.Height),
                new Rect(center.X - widthScale / 2, center.Y - heightScale / 2, widthScale, heightScale));
        }

        public override void Render(DrawingContext context) {
            base.Render(context);
            
            context.DrawImage(score, 1, new Rect(0, 0, score.Size.Width, score.Size.Height), new Rect(0, 0, drawWidth, drawHeight));
            this.Width = score.Size.Width;
            this.Height = score.Size.Height;
        }
    }
}