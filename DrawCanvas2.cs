using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using TheaterDaysScore.JsonModels;
using TheaterDaysScore.Models;
using TheaterDaysScore.Services;

namespace TheaterDaysScore {
    class DrawCanvas2 : Canvas {
        private int measureHeight = 200;
        private int measureWidth = 175;
        private double noteSize = 10;
        private double laneWidth;
        private double quarterBeatHeight;

        private int tickScale = 10;

        private int drawWidth;
        private int drawHeight;

        private Pen borderPen;
        private Pen beatPen;
        private Pen halfBeatPen;
        private Pen quarterBeatPen;
        private Pen holdPen;
        private Bitmap tapImg;
        private Bitmap leftImg;
        private Bitmap rightImg;
        private Bitmap upImg;
        private RenderTargetBitmap score;

        public DrawCanvas2() {
            borderPen = new Pen(Colors.Black.ToUint32(), 8);
            beatPen = new Pen(Colors.Black.ToUint32(), 3);
            halfBeatPen = new Pen(Colors.Black.ToUint32(), 1);
            quarterBeatPen = new Pen(Colors.DarkGray.ToUint32(), 1);
            holdPen = new Pen(Colors.Red.ToUint32(), Math.Log(5) * noteSize);

            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            tapImg = new Bitmap(assets.Open(new Uri($"avares://TheaterDaysScore/Assets/tap.png")));
            leftImg = new Bitmap(assets.Open(new Uri($"avares://TheaterDaysScore/Assets/left.png")));
            rightImg = new Bitmap(assets.Open(new Uri($"avares://TheaterDaysScore/Assets/right.png")));
            upImg = new Bitmap(assets.Open(new Uri($"avares://TheaterDaysScore/Assets/up.png")));
        }

        public void Draw(int songNum) {
            Song2 song = Database.DB.GetSong2();
            Song2.Difficulty difficulty = Song2.Difficulty.MillionMix;

            int laneOffset = 0;
            switch (difficulty) {
                case Song2.Difficulty.TwoMix:
                case Song2.Difficulty.TwoMixPlus:
                    laneOffset = 3;
                    break;
                case Song2.Difficulty.FourMix:
                    laneOffset = 2;
                    break;
                case Song2.Difficulty.SixMix:
                case Song2.Difficulty.MillionMix:
                case Song2.Difficulty.OverMix:
                    laneOffset = 1;
                    break;
            }

            var beats = song.Beats;
            var notes = song.Notes[difficulty];

            // Rendering
            drawWidth = measureWidth;
            //drawHeight = (int)((2*beats[beats.Count-1].Second - beats[beats.Count-2].Second) * 100);
            drawHeight = song.TotalTicks / tickScale;
            laneWidth = (double)measureWidth / 7;
            quarterBeatHeight = (double)measureHeight / 16;

            score = new RenderTargetBitmap(new PixelSize(drawWidth, drawHeight));
            using (IDrawingContextImpl ctx = score.CreateDrawingContext(null)) {
                foreach (Song2.Beat beat in beats) {
                    Song2.TimeSignature ts = song.TimeSignatureAtTick(beat.Tick);
                    RenderMeasure(ctx, beat, ts);
                }

                ctx.DrawRectangle(null, borderPen, new Rect(0, 0, drawWidth, drawHeight));

                foreach (Song2.Note2 note in notes) {
                    Song2.Note2 concurrentTapNote = notes.Find(n => n.Tick == note.Tick || n.Tick + n.HoldTicks == note.Tick);
                    if (concurrentTapNote != null) {
                        Point start = GetCenter(note.Lane + laneOffset, note.Tick);
                        float lane = (note.Tick == concurrentTapNote.Tick) ? concurrentTapNote.Lane : concurrentTapNote.EndLane;
                        Point end = GetCenter(lane + laneOffset, note.Tick);
                        ctx.DrawLine(borderPen, start, end);
                    }
                    if (note.HoldTicks > 0) {
                        Song2.Note2 concurrentReleaseNote = notes.Find(n => n.Tick == note.Tick + note.HoldTicks || n.Tick + n.HoldTicks == note.Tick + note.HoldTicks);
                        if (concurrentReleaseNote != null) {
                            Point start = GetCenter(note.EndLane + laneOffset, note.Tick + note.HoldTicks);
                            float lane = (note.Tick + note.HoldTicks == concurrentReleaseNote.Tick) ? concurrentReleaseNote.Lane : concurrentReleaseNote.EndLane;
                            Point end = GetCenter(lane + laneOffset, note.Tick + note.HoldTicks);
                            ctx.DrawLine(borderPen, start, end);
                        }
                    }
                }
                foreach (Song2.Note2 note in notes) {
                    RenderNote(ctx, note, laneOffset);
                }
            }
        }

        private void RenderMeasure(IDrawingContextImpl ctx, Song2.Beat beat, Song2.TimeSignature ts) {
            double height = drawHeight - (beat.Tick / tickScale);
            double tall = ts.TicksPerBeat / tickScale;

            // Vertical
            for (int x = 1; x < 7; x++) {
                ctx.DrawLine(quarterBeatPen, new Point(laneWidth * x, height), new Point(laneWidth * x, height - tall));
            }
            // Horizontal
            if (ts.Denominator == 4) {
                ctx.DrawLine(quarterBeatPen, new Point(0, height + tall / 2), new Point(measureWidth, height + tall / 2));
            }
            if (beat.MeasureStart) {
                ctx.DrawLine(beatPen, new Point(0, height), new Point(measureWidth, height));
            } else {
                ctx.DrawLine(halfBeatPen, new Point(0, height), new Point(measureWidth, height));
            }
        }

        private Point GetCenter(float lane, int tick) {
            return new Point(laneWidth * lane, drawHeight - (tick / tickScale));
        }

        private void RenderTap(IDrawingContextImpl ctx, float lane, int tick, Song2.Note2.InteractType type, int score) {
            Bitmap noteImg = tapImg;
            switch (type) {
                case Song2.Note2.InteractType.tap:
                    noteImg = tapImg;
                    break;
                case Song2.Note2.InteractType.leftFlick:
                    noteImg = leftImg;
                    break;
                case Song2.Note2.InteractType.rightFlick:
                    noteImg = rightImg;
                    break;
                case Song2.Note2.InteractType.upFlick:
                    noteImg = upImg;
                    break;
            }
            double noteScale = Math.Log(score * 5) * noteSize;
            double heightScale;
            double widthScale;
            if (noteImg.Size.Width > noteImg.Size.Height) {
                heightScale = noteScale;
                widthScale = heightScale * noteImg.Size.Width / noteImg.Size.Height;
            } else {
                widthScale = noteScale;
                heightScale = widthScale * noteImg.Size.Height / noteImg.Size.Width;
            }
            Point center = GetCenter(lane, tick);
            ctx.DrawBitmap(noteImg.PlatformImpl, 1, new Rect(0, 0, noteImg.Size.Width, noteImg.Size.Height),
                new Rect(center.X - widthScale / 2, center.Y - heightScale / 2, widthScale, heightScale));
        }

        private void RenderNote(IDrawingContextImpl ctx, Song2.Note2 note, int laneOffset) {
            Point center = GetCenter(note.Lane + laneOffset, note.Tick);
            if (note.Waypoints.Count > 0) {
                PathGeometry holdLine = new PathGeometry();
                StreamGeometryContext holdCtx = holdLine.Open();
                holdCtx.BeginFigure(center, false);
                foreach (Song2.Note2.Waypoint point in note.Waypoints) {
                    Point pointCenter = GetCenter(point.Lane + laneOffset, point.Tick);
                    holdCtx.LineTo(new Point(pointCenter.X, pointCenter.Y));
                }
                ctx.DrawGeometry(null, holdPen, holdLine.PlatformImpl);

                RenderTap(ctx, note.EndLane + laneOffset, note.Tick + note.HoldTicks, note.EndType, note.PointsValue);
            }

            RenderTap(ctx, note.Lane + laneOffset, note.Tick, note.Type, note.PointsValue);
        }

        public override void Render(DrawingContext context) {
            base.Render(context);

            if (score != null) {
                context.DrawImage(score, new Rect(0, 0, score.Size.Width, score.Size.Height), new Rect(0, 0, drawWidth, drawHeight));
                Width = score.Size.Width;
                Height = score.Size.Height;
            }
        }
    }
}