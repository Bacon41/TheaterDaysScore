using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using TheaterDaysScore.Models;
using TheaterDaysScore.Services;

namespace TheaterDaysScore {
    class DrawIntervals : Canvas {
        private int measureHeight = 200;

        private int drawWidth;
        private int drawHeight;

        private RenderTargetBitmap score;

        public DrawIntervals() {
        }

        public void Draw(int songNum, Unit unit) {
            if (unit == null) {
                return;
            }

            Song song = Database.DB.GetSong(songNum);

            // Rendering
            drawWidth = 10 * 5;
            drawHeight = song.DisplayMeasures * measureHeight;

            score = new RenderTargetBitmap(new PixelSize(drawWidth, drawHeight));
            using (IDrawingContextImpl ctx = score.CreateDrawingContext(null)) {
                // Skill interval
                int offset = 0;
                foreach (Card card in unit.Members) {
                    RenderCard(ctx, card, song, offset);
                    offset += 10;
                }

                // Second bar
                for (int x = 0; x < song.Length; x++) {
                    Pen writePen = new Pen(Colors.Black.ToUint32(), 3);
                    double pixelsPerSecond = measureHeight * (double)song.BPM / 60 / 4;
                    double height = measureHeight * (song.DisplayMeasures - song.MeasuresForSkillStart + ((song.SkillStartOffset - song.Notes[0].QuarterBeat) / 16)) - x * pixelsPerSecond;
                    ctx.DrawLine(writePen, new Point(0, height), new Point(45, height));
                }
            }
        }

        private void RenderCard(IDrawingContextImpl ctx, Card card, Song song, int offset) {
            // Timing
            double pixelsPerSecond = measureHeight * (double)song.BPM / 60 / 4;
            if (card.Skills != null) {
                foreach (Card.Skill skill in card.Skills) {
                    double startPos = measureHeight * (song.DisplayMeasures - song.MeasuresForSkillStart + ((song.SkillStartOffset - song.Notes[0].QuarterBeat) / 16)) - skill.Interval * pixelsPerSecond;
                    int weight = 5;
                    if (offset == 0) {
                        weight *= 2;
                    }
                    Pen writePen = new Pen(card.Color.ToUint32(), weight);
                    double lastPos = measureHeight * (double)(16 - song.Notes.Last().QuarterBeat) / 16;
                    while (startPos > lastPos) {
                        ctx.DrawLine(writePen, new Point(offset, startPos), new Point(offset, Math.Max(lastPos, startPos - skill.Duration * pixelsPerSecond)));
                        startPos -= skill.Interval * pixelsPerSecond;
                    }
                }
            }
        }

        public override void Render(DrawingContext context) {
            base.Render(context);

            if (score != null) {
                context.DrawImage(score, new Rect(0, 0, score.Size.Width, score.Size.Height), new Rect(0, 0, drawWidth, drawHeight));
                this.Width = score.Size.Width;
                this.Height = score.Size.Height;
            }
        }
    }
}