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
        private int tickScale = 10;

        private int drawWidth;
        private int drawHeight;

        private RenderTargetBitmap score;

        public DrawIntervals() {
        }

        public void Draw(Song song, Song.Difficulty difficulty, Unit unit) {
            if (song == null || unit == null) {
                return;
            }
            if (song.Notes[difficulty].Count == 0) {
                return;
            }

            // Rendering
            drawWidth = 10 * 5;
            drawHeight = song.TotalTicks / tickScale;

            double startTime = song.SkillStartTime;
            int endTicks = song.LastNoteTick;

            score = new RenderTargetBitmap(new PixelSize(drawWidth, drawHeight));
            using (IDrawingContextImpl ctx = score.CreateDrawingContext(null)) {
                // Skill interval
                int offset = 0;
                foreach (Card card in unit.Members) {
                    RenderCard(ctx, card, song, offset, startTime, endTicks);
                    offset += 10;
                }

                // Seconds bars
                int currentTicks = song.TimeToTick(startTime);
                for (int x = 0; currentTicks < endTicks; x++) {
                    Pen writePen = new Pen(Colors.Black.ToUint32(), 3);
                    double height = drawHeight - (currentTicks / tickScale);
                    ctx.DrawLine(writePen, new Point(0, height), new Point(45, height));

                    currentTicks = song.TimeToTick(startTime + x);
                }
            }
        }

        private void RenderCard(IDrawingContextImpl ctx, Card card, Song song, int offset, double startTime, int endTicks) {
            // Timing
            if (card.Skills != null) {
                foreach (Card.Skill skill in card.Skills) {
                    double currentTime = startTime + skill.Interval;
                    int currentTicks = song.TimeToTick(currentTime);
                    double currentHeight = drawHeight - (currentTicks / tickScale);
                    double endHeight = drawHeight - (endTicks / tickScale);

                    int weight = 5;
                    if (offset == 0) {
                        weight *= 2;
                    }
                    Pen writePen = new Pen(card.Color.ToUint32(), weight);
                    while (currentTicks < endTicks) {
                        if (!song.IsDuringAppeal(currentTime)) {
                            double expireHeight = drawHeight - (song.TimeToTick(currentTime + skill.Duration) / tickScale);
                            ctx.DrawLine(writePen, new Point(offset, currentHeight), new Point(offset, Math.Max(expireHeight, endHeight)));
                        }
                        currentTime += skill.Interval;
                        currentTicks = song.TimeToTick(currentTime);
                        currentHeight = drawHeight - (currentTicks / tickScale);
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