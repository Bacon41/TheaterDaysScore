﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        private List<Song> songs;
        private List<Card> allCards;
        private List<Idol> idols;

        private RenderTargetBitmap score;

        public DrawIntervals() {
            // Song info
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            StreamReader reader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/res/songlist.json")));
            songs = JsonSerializer.Deserialize<List<Song>>(reader.ReadToEnd());

            StreamReader cardReader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/res/cardlist.json")));
            allCards = JsonSerializer.Deserialize<List<Card>>(cardReader.ReadToEnd());

            StreamReader idolReader = new StreamReader(assets.Open(new Uri($"avares://TheaterDaysScore/res/idollist.json")));
            idols = JsonSerializer.Deserialize<List<Idol>>(idolReader.ReadToEnd());
        }

        public void Draw(int songNum, int[] cardIds) {
            Song song = songs[songNum];
            // Rendering
            drawWidth = 10 * 5;
            drawHeight = song.displayMeasures * measureHeight;

            List<Card> cards = new List<Card>();

            for (int x = 0; x < 5; x++) {
                Card nextCard = allCards.Find(card => card.id == cardIds[x]);
                nextCard.colour = idols.Find(idol => idol.id == nextCard.idolId).colour;
                cards.Add(nextCard);
            }

            score = new RenderTargetBitmap(new PixelSize(drawWidth, drawHeight));
            using (IDrawingContextImpl ctx = score.CreateDrawingContext(null)) {
                int offset = 0;
                foreach (Card card in cards) {
                    RenderCard(ctx, card, song, offset);
                    offset += 10;
                }
                for (int x = 0; x < song.songLength; x++) {
                    Pen writePen = new Pen(Colors.Black.ToUint32(), 3);
                    double pixelsPerSecond = measureHeight * (double)songs[songNum].bpm / 60 / 4;
                    double height = measureHeight * (song.displayMeasures - song.measuresForSkillStart + ((song.skillStartOffset - song.notes[0].quarterBeat) / 16)) - x * pixelsPerSecond;
                    ctx.DrawLine(writePen, new Point(0, height), new Point(45, height));
                }
            }
        }

        private void RenderCard(IDrawingContextImpl ctx, Card card, Song song, int offset) {
            // Timing
            double pixelsPerSecond = measureHeight * (double)song.bpm / 60 / 4;
            double startPos = measureHeight * (song.displayMeasures - song.measuresForSkillStart + ((song.skillStartOffset - song.notes[0].quarterBeat) / 16)) - card.skill[0].interval * pixelsPerSecond;
            int weight = 5;
            if (offset == 0) {
                weight *= 2;
            }
            Pen writePen = new Pen(Color.Parse("#" + card.colour).ToUint32(), weight);
            double lastPos = measureHeight * (double)(16 - song.notes.Last().quarterBeat) / 16;
            while (startPos > lastPos) {
                ctx.DrawLine(writePen, new Point(offset, startPos), new Point(offset, Math.Max(lastPos, startPos - card.skill[0].duration * pixelsPerSecond)));
                startPos -= card.skill[0].interval * pixelsPerSecond;
            }
        }

        public override void Render(DrawingContext context) {
            base.Render(context);

            if (score != null) {
                context.DrawImage(score, 1, new Rect(0, 0, score.Size.Width, score.Size.Height), new Rect(0, 0, drawWidth, drawHeight));
                this.Width = score.Size.Width;
                this.Height = score.Size.Height;
            }
        }
    }
}