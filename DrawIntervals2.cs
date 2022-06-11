﻿using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using TheaterDaysScore.Models;
using TheaterDaysScore.Services;

namespace TheaterDaysScore {
    class DrawIntervals2 : Canvas {
        private int measureHeight = 200;

        private int tickScale = 10;

        private int drawWidth;
        private int drawHeight;

        private RenderTargetBitmap score;

        public DrawIntervals2() {
        }

        public void Draw(int songNum, Unit unit) {
            if (unit == null) {
                return;
            }

            Song2 song = Database.DB.GetSong2();
            Song2.Difficulty difficulty = Song2.Difficulty.MillionMix;

            var notes = song.Notes[difficulty];

            // Rendering
            drawWidth = 10 * 5;
            drawHeight = song.TotalTicks / tickScale;

            // This is close to accurate much of the time, but I can't figure out how to get the real value
            double skillStartAdjustment = 2;
            double startTime = notes[0].Second - skillStartAdjustment;
            int endTicks = notes[notes.Count - 1].Tick + notes[notes.Count - 1].HoldTicks;

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

        private void RenderCard(IDrawingContextImpl ctx, Card card, Song2 song, int offset, double startTime, int endTicks) {
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