using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace TheaterDaysScore {

    public enum Types {
        Princess,
        Fairy,
        Angel,
        All,
        EX,
    };

    class Card {
        public int id;
        public Types type;
        public SkillTypes skill;

        public enum SkillTypes {
            filler,
            scoreUp,
            comboBonus,
            lifeRestore,
            damageGuard,
            comboProtect,
            judgementBoost,
            doubleBoost,
            multiUp,
            overClock,
            overRondo,
        };

        public Card(int i, Types t, SkillTypes s) {
            id = i;
            type = t;
            skill = s;

            // https://storage.matsurihi.me/mltd/card/017kth0054_0_a.png
        }
    }

    class Calculator {
        public Calculator() {
        }

        public int GetScore() {
            Song song = new Song("Blue Symphony", Types.Fairy, 140, 66, 16, 555, 21, 1, 77.25f);

            //Card guest = new Card(256, Types.Fairy);
            Card center = new Card(286, Types.Princess, Card.SkillTypes.comboBonus);
            //Card mem1 = new Card(250, Types.Angel);
            //Card mem2 = new Card(391, Types.Princess);
            //Card mem3 = new Card(269, Types.Fairy);
            //Card mem4 = new Card(178, Types.Princess);

            int totalAppeal = 320000;

            float baseScore = totalAppeal * (33f + song.level) / 20;
            float notesAndHolds = song.noteWeight + 2 * song.holdLength;

            float scoreScale = 0.7f * baseScore / notesAndHolds;
            float comboScale = 0.3f * baseScore / (2 * song.noteCount - 66);

            return (int)comboScale;
        }
    }
}
