using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace TheaterDaysScore {
    public class DrawIntervals : Canvas {
        private int measureHeight = 200;

        private int drawWidth;
        private int drawHeight;

        private Pen kotohaPen;
        private Pen serikaPen;
        private RenderTargetBitmap score;

        public DrawIntervals() {
            kotohaPen = new Pen(Color.Parse("#92cfbb").ToUint32(), 10);
            serikaPen = new Pen(Color.Parse("#ed90ba").ToUint32(), 5);

            // Song info
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            StreamReader reader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/res/songlist.json")));
            List<Song> songs = JsonSerializer.Deserialize<List<Song>>(reader.ReadToEnd());
            int songNum = 0;

            // Rendering
            drawWidth = 15;
            drawHeight = songs[songNum].measures * measureHeight;

            List<Card> cards = new List<Card>() {
                new Card(286, Types.Princess, new Card.Skill(Card.Skill.Type.comboBonus, 6, 13, 35, new int[] { 26 }, 5)),
                new Card(250, Types.Angel, new Card.Skill(Card.Skill.Type.comboBonus, 4, 7, 30, new int[] { 28 }, 6))
            };

            score = new RenderTargetBitmap(new PixelSize(drawWidth, drawHeight));
            using (IDrawingContextImpl ctx = score.CreateDrawingContext(null)) {
                int offset = 0;
                foreach (Card card in cards) {
                    RenderCard(ctx, card, songs[songNum], offset);
                    offset += 10;
                }
            }
        }

        private void RenderCard(IDrawingContextImpl ctx, Card card, Song song, int offset) {
            // Timing
            double pixelsPerSecond = measureHeight * (double)song.bpm / 60 / 4;
            double startPos = measureHeight * (song.measures - 1) - card.skill.interval * pixelsPerSecond;
            Pen writePen = kotohaPen;
            if (offset != 0) {
                writePen = serikaPen;
            }
            while (startPos > measureHeight) {
                ctx.DrawLine(writePen, new Point(offset, startPos), new Point(offset, Math.Max(measureHeight, startPos - card.skill.duration * pixelsPerSecond)));
                startPos -= card.skill.interval * pixelsPerSecond;
            }
        }

        public override void Render(DrawingContext context) {
            base.Render(context);
            
            context.DrawImage(score, 1, new Rect(0, 0, score.Size.Width, score.Size.Height), new Rect(0, 0, drawWidth, drawHeight));
            this.Width = score.Size.Width;
            this.Height = score.Size.Height;
        }
    }
}