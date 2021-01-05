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

        private RenderTargetBitmap score;

        public DrawIntervals() {
            // Song info
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            StreamReader reader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/res/songlist.json")));
            List<Song> songs = JsonSerializer.Deserialize<List<Song>>(reader.ReadToEnd());
            int songNum = 1;

            // Rendering
            drawWidth = 10 * 5;
            drawHeight = songs[songNum].displayMeasures * measureHeight;

            List<Card> cards = new List<Card>() {
                new Card(409, "#fd99e1", Types.Fairy, null, new Card.Skill(Card.Skill.Type.comboBonus, 6, 11, 30, new int[] { 28 }, 10)),
                new Card(368, "#454341", Types.Fairy, null, new Card.Skill(Card.Skill.Type.scoreUp, 5, 10, 30, new int[] { 30 }, 12)),
                new Card(868, "#bee3e3", Types.Fairy, null, new Card.Skill(Card.Skill.Type.multiUp, 6, 11, 30, new int[] { 32 }, 10)),
                new Card(159, "#f19557", Types.Fairy, null, new Card.Skill(Card.Skill.Type.scoreUp, 7, 13, 30, new int[] { 30 }, 12)),
                new Card(432, "#01a860", Types.Fairy, null, new Card.Skill(Card.Skill.Type.comboBonus, 5, 9, 30, new int[] { 26 }, 12))
            };

            score = new RenderTargetBitmap(new PixelSize(drawWidth, drawHeight));
            using (IDrawingContextImpl ctx = score.CreateDrawingContext(null)) {
                int offset = 0;
                foreach (Card card in cards) {
                    RenderCard(ctx, card, songs[songNum], offset);
                    offset += 10;
                }
                for (int x = 0; x < songs[songNum].songLength; x++) {
                    Pen writePen = new Pen(Colors.Black.ToUint32(), 3);
                    double pixelsPerSecond = measureHeight * (double)songs[songNum].bpm / 60 / 4;
                    double height = measureHeight * (songs[songNum].displayMeasures - .625) - x * pixelsPerSecond;
                    ctx.DrawLine(writePen, new Point(0, height), new Point(45, height));
                }
            }
        }

        private void RenderCard(IDrawingContextImpl ctx, Card card, Song song, int offset) {
            // Timing
            double pixelsPerSecond = measureHeight * (double)song.bpm / 60 / 4;
            double startPos = measureHeight * (song.displayMeasures - .625) - card.skill.interval * pixelsPerSecond;
            int weight = 5;
            if (offset == 0) {
                weight *= 2;
            }
            Pen writePen = new Pen(Color.Parse(card.colour).ToUint32(), weight);
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