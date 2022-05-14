using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DynamicData;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using TheaterDaysScore.JsonModels;
using TheaterDaysScore.Models;
using TheaterDaysScore.Services;
using TheaterDaysScore.ViewModels;

namespace TheaterDaysScore.Views {
    public class UnitBuilderView : ReactiveUserControl<UnitBuilderViewModel> {

        private CheckBox idolCheck => this.FindControl<CheckBox>("idolFilter");
        private ComboBox idolChoice => this.FindControl<ComboBox>("idolChoice");

        private CheckBox ssrCheck => this.FindControl<CheckBox>("raritySSR");
        private CheckBox srCheck => this.FindControl<CheckBox>("raritySR");
        private CheckBox rCheck => this.FindControl<CheckBox>("rarityR");
        private CheckBox nCheck => this.FindControl<CheckBox>("rarityN");

        private CheckBox princessCheck => this.FindControl<CheckBox>("typePrincess");
        private CheckBox fairyCheck => this.FindControl<CheckBox>("typeFairy");
        private CheckBox angelCheck => this.FindControl<CheckBox>("typeAngel");
        private CheckBox exCheck => this.FindControl<CheckBox>("typeEX");

        private CheckBox permCheck => this.FindControl<CheckBox>("categoryPerm");
        private CheckBox limCheck => this.FindControl<CheckBox>("categoryLim");
        private CheckBox shsCheck => this.FindControl<CheckBox>("categorySHS");
        private CheckBox fesCheck => this.FindControl<CheckBox>("categoryFes");
        private CheckBox pstCheck => this.FindControl<CheckBox>("categoryPST");
        private CheckBox colleCheck => this.FindControl<CheckBox>("categoryColle");
        private CheckBox premPickCheck => this.FindControl<CheckBox>("categoryPremPick");
        private CheckBox annCheck => this.FindControl<CheckBox>("categoryAnn");
        private CheckBox otherCheck => this.FindControl<CheckBox>("categoryOther");

        private CheckBox vocalCheck => this.FindControl<CheckBox>("centerVocal");
        private CheckBox danceCheck => this.FindControl<CheckBox>("centerDance");
        private CheckBox visualCheck => this.FindControl<CheckBox>("centerVisual");
        private CheckBox allCheck => this.FindControl<CheckBox>("centerAll");
        private CheckBox lifeCheck => this.FindControl<CheckBox>("centerLife");
        private CheckBox procCheck => this.FindControl<CheckBox>("centerProc");
        private CheckBox noneCheck => this.FindControl<CheckBox>("centerNone");

        private CheckBox scoreCheck => this.FindControl<CheckBox>("skillScore");
        private CheckBox overClockCheck => this.FindControl<CheckBox>("skillOverClock");
        private CheckBox comboCheck => this.FindControl<CheckBox>("skillCombo");
        private CheckBox overRondoCheck => this.FindControl<CheckBox>("skillOverRondo");
        private CheckBox doubleBoostCheck => this.FindControl<CheckBox>("skillDoubleBoost");
        private CheckBox doubleEffectCheck => this.FindControl<CheckBox>("skillDoubleEffect");
        private CheckBox lifeSkillCheck => this.FindControl<CheckBox>("skillLife");
        private CheckBox damageGuardCheck => this.FindControl<CheckBox>("skillDamageGuard");
        private CheckBox comboProtectCheck => this.FindControl<CheckBox>("skillComboProtect");
        private CheckBox judgementBoostCheck => this.FindControl<CheckBox>("skillJudgementBoost");
        private CheckBox multiUpCheck => this.FindControl<CheckBox>("skillMultiUp");
        private CheckBox multiBonusCheck => this.FindControl<CheckBox>("skillMultiBonus");
        private CheckBox noneSkillCheck => this.FindControl<CheckBox>("skillNone");

        private ListBox placementChoice => this.FindControl<ListBox>("placementChoice");
        private ListBox cardChoice => this.FindControl<ListBox>("cardChoice");

        public UnitBuilderView() {
            this.InitializeComponent();

            this.WhenActivated(disposables => {
                // Idol filters
                this.Bind(ViewModel, vm => vm.FilterIdol, v => v.idolCheck.IsChecked, set => {
                    return set;
                }, isChecked => {
                    ViewModel.FilterCards();
                    return ViewModel.FilterIdol;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedIdol, v => v.idolChoice.SelectedIndex, idx => {
                    return idx;
                }, isChecked => {
                    ViewModel.FilterCards();
                    return ViewModel.SelectedIdol;
                }).DisposeWith(disposables);

                // Rarity filters
                this.Bind(ViewModel, vm => vm.Rarities, v => v.ssrCheck.IsChecked, set => {
                    return set.Contains(CardData.Rarities.SSR);
                }, isChecked => {
                    SetRarity(isChecked, CardData.Rarities.SSR);
                    return ViewModel.Rarities;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Rarities, v => v.srCheck.IsChecked, set => {
                    return set.Contains(CardData.Rarities.SR);
                }, isChecked => {
                    SetRarity(isChecked, CardData.Rarities.SR);
                    return ViewModel.Rarities;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Rarities, v => v.rCheck.IsChecked, set => {
                    return set.Contains(CardData.Rarities.R);
                }, isChecked => {
                    SetRarity(isChecked, CardData.Rarities.R);
                    return ViewModel.Rarities;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Rarities, v => v.nCheck.IsChecked, set => {
                    return set.Contains(CardData.Rarities.N);
                }, isChecked => {
                    SetRarity(isChecked, CardData.Rarities.N);
                    return ViewModel.Rarities;
                }).DisposeWith(disposables);

                // Type filters
                this.Bind(ViewModel, vm => vm.Types, v => v.princessCheck.IsChecked, set => {
                    return set.Contains(Types.Princess);
                }, isChecked => {
                    SetType(isChecked, Types.Princess);
                    return ViewModel.Types;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Types, v => v.fairyCheck.IsChecked, set => {
                    return set.Contains(Types.Fairy);
                }, isChecked => {
                    SetType(isChecked, Types.Fairy);
                    return ViewModel.Types;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Types, v => v.angelCheck.IsChecked, set => {
                    return set.Contains(Types.Angel);
                }, isChecked => {
                    SetType(isChecked, Types.Angel);
                    return ViewModel.Types;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Types, v => v.exCheck.IsChecked, set => {
                    return set.Contains(Types.EX);
                }, isChecked => {
                    SetType(isChecked, Types.EX);
                    return ViewModel.Types;
                }).DisposeWith(disposables);

                // Category filters
                this.Bind(ViewModel, vm => vm.Categories, v => v.permCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.PermanentGasha);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.PermanentGasha);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.limCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.LimitedGasha);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.LimitedGasha);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.shsCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.SHSGasha);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.SHSGasha);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.fesCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.Fes);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.Fes);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.pstCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.PST);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.PST);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.colleCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.MiliColle);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.MiliColle);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.premPickCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.PremiumPickup);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.PremiumPickup);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.annCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.Anniversary);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.Anniversary);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.otherCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.Other);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.Other);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);

                // Center filters
                this.Bind(ViewModel, vm => vm.CenterTypes, v => v.vocalCheck.IsChecked, set => {
                    return set.Contains(CardData.CenterEffect.Type.vocalUp);
                }, isChecked => {
                    SetCenter(isChecked, CardData.CenterEffect.Type.vocalUp);
                    return ViewModel.CenterTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterTypes, v => v.danceCheck.IsChecked, set => {
                    return set.Contains(CardData.CenterEffect.Type.danceUp);
                }, isChecked => {
                    SetCenter(isChecked, CardData.CenterEffect.Type.danceUp);
                    return ViewModel.CenterTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterTypes, v => v.visualCheck.IsChecked, set => {
                    return set.Contains(CardData.CenterEffect.Type.visualUp);
                }, isChecked => {
                    SetCenter(isChecked, CardData.CenterEffect.Type.visualUp);
                    return ViewModel.CenterTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterTypes, v => v.allCheck.IsChecked, set => {
                    return set.Contains(CardData.CenterEffect.Type.allUp);
                }, isChecked => {
                    SetCenter(isChecked, CardData.CenterEffect.Type.allUp);
                    return ViewModel.CenterTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterTypes, v => v.lifeCheck.IsChecked, set => {
                    return set.Contains(CardData.CenterEffect.Type.lifeUp);
                }, isChecked => {
                    SetCenter(isChecked, CardData.CenterEffect.Type.lifeUp);
                    return ViewModel.CenterTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterTypes, v => v.procCheck.IsChecked, set => {
                    return set.Contains(CardData.CenterEffect.Type.skillBoost);
                }, isChecked => {
                    SetCenter(isChecked, CardData.CenterEffect.Type.skillBoost);
                    return ViewModel.CenterTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterTypes, v => v.noneCheck.IsChecked, set => {
                    return set.Contains(CardData.CenterEffect.Type.none);
                }, isChecked => {
                    SetCenter(isChecked, CardData.CenterEffect.Type.none);
                    return ViewModel.CenterTypes;
                }).DisposeWith(disposables);

                // Skill filters
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.scoreCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.scoreUp);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.scoreUp);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.overClockCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.overClock);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.overClock);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.comboCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.comboBonus);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.comboBonus);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.overRondoCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.overRondo);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.overRondo);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.doubleBoostCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.doubleBoost);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.doubleBoost);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.doubleEffectCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.doubleEffect);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.doubleEffect);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.lifeSkillCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.lifeRestore);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.lifeRestore);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.damageGuardCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.damageGuard);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.damageGuard);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.comboProtectCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.comboProtect);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.comboProtect);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.judgementBoostCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.judgementBoost);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.judgementBoost);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.multiUpCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.multiUp);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.multiUp);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.multiBonusCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.multiBonus);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.multiBonus);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.noneSkillCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.none);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.none);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);

                // Card selection
                this.WhenAnyValue(x => x.ViewModel.PlacementIndex)
                    .Subscribe(x => ViewModel.FilterCards())
                    .DisposeWith(disposables);
                cardChoice.GetObservable(ListBox.SelectedItemProperty)
                    .Subscribe(x => ViewModel.SetCard(x as Card))
                    .DisposeWith(disposables);

                // Guest rank selection
                this.WhenAnyValue(x => x.ViewModel.GuestRank)
                    .Subscribe(x => ViewModel.SetUnit())
                    .DisposeWith(disposables);
                this.WhenAnyValue(x => x.ViewModel.Guest)
                    .Subscribe(x => {
                        if (x == "") {
                            return;
                        }
                        int oldRank = ViewModel.GuestRank;
                        ViewModel.GuestRanks = Database.DB.GetCard(x).MasterRanks;
                        if (oldRank >= ViewModel.GuestRanks.Count) {
                            oldRank = ViewModel.GuestRanks.Count - 1;
                        }
                        ViewModel.GuestRank = oldRank;
                    })
                    .DisposeWith(disposables);

                ViewModel.FilterCards();
                ViewModel.SetUnit();
            });
        }

        private void SetRarity(bool? isChecked, CardData.Rarities rarity) {
            if (isChecked ?? true) {
                ViewModel.Rarities.Add(rarity);
            } else {
                ViewModel.Rarities.Remove(rarity);
            }
            ViewModel.FilterCards();
        }

        private void SetType(bool? isChecked, Types type) {
            if (isChecked ?? true) {
                ViewModel.Types.Add(type);
            } else {
                ViewModel.Types.Remove(type);
            }
            ViewModel.FilterCards();
        }

        private void SetCategory(bool? isChecked, Card.Categories category) {
            if (isChecked ?? true) {
                ViewModel.Categories.Add(category);
            } else {
                ViewModel.Categories.Remove(category);
            }
            ViewModel.FilterCards();
        }

        private void SetCenter(bool? isChecked, CardData.CenterEffect.Type center) {
            if (isChecked ?? true) {
                ViewModel.CenterTypes.Add(center);
            } else {
                ViewModel.CenterTypes.Remove(center);
            }
            ViewModel.FilterCards();
        }

        private void SetSkill(bool? isChecked, CardData.Skill.Type skill) {
            if (isChecked ?? true) {
                ViewModel.SkillTypes.Add(skill);
            } else {
                ViewModel.SkillTypes.Remove(skill);
            }
            ViewModel.FilterCards();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
