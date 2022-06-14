# Theater Days Score

This is a simple desktop app built with [Avalonia](https://www.avaloniaui.net/) and [Reactive UI](https://www.reactiveui.net/), designed to help players of the mobile game [_The iDOLM@STER Million Live! Theater Days_](https://millionlive.idolmaster.jp/theaterdays/). _Theater Days_ is a rhythm game where your score is dependent on what cards you build your unit with when playing songs. Different cards have different strengths and skills, so simulating different combinations of cards can help find the optimal unit.

This was intended to be a mostly offline version of the tools provided at [megmeg.work](https://megmeg.work/mltd/) and [MLTDApp](https://app.39m.ltd/), and all the math was built based on the formulas provided by megmeg, particularly the [appealvalue](https://megmeg.work/basic_information/formula/appealvalue/) and [score](https://megmeg.work/basic_information/formula/score/) pages.

An online connection is required to collect card and song data, at least once. Large sections of the data gathering logic are built from the pioneering work done by [OpenMLTD](https://github.com/OpenMLTD/MLTDTools), and [AssetStudio](https://github.com/Perfare/AssetStudio) is used directly as a library.

When cloning this repo, ensure that you initialize the submodules via `git submodule init`.

### Cards

Card data is retrieved on demand via [matsurihi.me](https://matsurihi.me)'s Princess [cards API](https://api.matsurihi.me/docs/#mltd-v1-cards), and cached. Simply go to the [deck building](#deck-bulding-screen) screen and select "Update" to get the latest.

### Songs

Song data is retrieved on demand via [MLTDApp](https://api.39m.ltd/api/fetch/all_song_info). The beatmaps for each song are then collected via [matsurihi.me](https://matsurihi.me)'s Princess [version API](https://api.matsurihi.me/docs/#mltd-v1-version) to build calls to the game's [asset server](https://td-assets.bn765.com), and cached. The beatmap files are identified (`scrobj_{name}.unity3d`) and data extracted (`{name}_fumen_sobj.json`).

The beatmaps visible at [hyrorre](https://million.hyrorre.com/) were used as reference to determine how to parse the info available. Currently, there is a lack of accuracy around when the countdown to the initial activation for each skills will be. The code will assume 2 seconds before the first note, which is close for most songs, but not quite right for any of them, leading to slightly miscalculated scores.

## Preview

### Scoring Screen

![](Docs/Score.png)

Select "Choose Cards" to go to the deck builder. Select the block below to go to the unit builder.

The first toggle block sets the event attribute boost, and the one below that sets the song. The preview panel on the right will show you the beat map for the selected song, as well as all the potential activation periods for each unit member's skill, colour coded to the character's image colour. The black bars of the skill activation section indicate whole seconds.

Select "Calculate" to run the score simulation.

### Deck Building Screen

![](Docs/Deck.png)

Select "Close" to return to the scoring screen.

Select "Update" to download the latest card info. "Save" will persist your deck modifications for the next time the app is opened, and "Load" will reset them to the previously saved state. Modifications made here and not saved will take effect across the other screens, but only until the app is closed.

The check under each card indicates if you posses that card. The first dropdown is for the card's master rank, while the second is for the card's skill level.

Choose "SelectAll" to set the possession check for all cards that match the current filters. "MaxRank" will set all cards matching the filters to their maximum master rank, and "MaxLevel" will do the same for the maximum skill level.

For the filters, the "All" button will either check all options for that category, or if all are already selected, will uncheck all the options. Individual options can then be toggled as desired.

### Unit Building Screen

![](Docs/Unit.png)

Select "Close" to return to the scoring screen.

Selecting each of the positions in the unit panel will bring up the list of cards that match your current filters. Positions other than the guest slot are additionally filtered to only cards you possess, as determined by the deck builder selections. The dropdown under the guest allows you to select their master rank. Once a position is selected, click on one of the cards in the filtered list to place it in that position.

For the filters, the "All" button will either check all options for that category, or if all are already selected, will uncheck all the options. Individual options can then be toggled as desired.