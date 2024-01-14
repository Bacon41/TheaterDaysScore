using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DynamicData;
using ReactiveUI;
using System.Reactive.Disposables;
using TheaterDaysScore.JsonModels;
using TheaterDaysScore.Models;
using TheaterDaysScore.ViewModels;

namespace TheaterDaysScore.Views {
    public class DeckBuilderView : ReactiveUserControl<DeckBuilderViewModel> {

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
        private CheckBox lnkCheck => this.FindControl<CheckBox>("categoryLnk");
        private CheckBox pstCheck => this.FindControl<CheckBox>("categoryPST");
        private CheckBox colleCheck => this.FindControl<CheckBox>("categoryColle");
        private CheckBox premPickCheck => this.FindControl<CheckBox>("categoryPremPick");
        private CheckBox annCheck => this.FindControl<CheckBox>("categoryAnn");
        private CheckBox prCheck => this.FindControl<CheckBox>("categoryPR");
        private CheckBox otherCheck => this.FindControl<CheckBox>("categoryOther");

        private CheckBox centerBoostVocalCheck => this.FindControl<CheckBox>("centerBoostVocal");
        private CheckBox centerBoostDanceCheck => this.FindControl<CheckBox>("centerBoostDance");
        private CheckBox centerBoostVisualCheck => this.FindControl<CheckBox>("centerBoostVisual");
        private CheckBox centerBoostAllCheck => this.FindControl<CheckBox>("centerBoostAll");
        private CheckBox centerBoostLifeCheck => this.FindControl<CheckBox>("centerBoostLife");
        private CheckBox centerBoostProcCheck => this.FindControl<CheckBox>("centerBoostProc");
        private CheckBox centerBoostNoneCheck => this.FindControl<CheckBox>("centerBoostNone");

        private CheckBox centerReqPrincessCheck => this.FindControl<CheckBox>("centerReqPrincess");
        private CheckBox centerReqFairyCheck => this.FindControl<CheckBox>("centerReqFairy");
        private CheckBox centerReqAngelCheck => this.FindControl<CheckBox>("centerReqAngel");
        private CheckBox centerReqAllCheck => this.FindControl<CheckBox>("centerReqAll");
        private CheckBox centerReqNoneCheck => this.FindControl<CheckBox>("centerReqNone");

        private CheckBox scoreCheck => this.FindControl<CheckBox>("skillScore");
        private CheckBox overClockCheck => this.FindControl<CheckBox>("skillOverClock");
        private CheckBox fusionScoreCheck => this.FindControl<CheckBox>("skillFusionScore");
        private CheckBox comboCheck => this.FindControl<CheckBox>("skillCombo");
        private CheckBox overRondoCheck => this.FindControl<CheckBox>("skillOverRondo");
        private CheckBox fusionComboCheck => this.FindControl<CheckBox>("skillFusionCombo");
        private CheckBox doubleBoostCheck => this.FindControl<CheckBox>("skillDoubleBoost");
        private CheckBox doubleEffectCheck => this.FindControl<CheckBox>("skillDoubleEffect");
        private CheckBox lifeSkillCheck => this.FindControl<CheckBox>("skillLife");
        private CheckBox damageGuardCheck => this.FindControl<CheckBox>("skillDamageGuard");
        private CheckBox comboProtectCheck => this.FindControl<CheckBox>("skillComboProtect");
        private CheckBox judgementBoostCheck => this.FindControl<CheckBox>("skillJudgementBoost");
        private CheckBox multiUpCheck => this.FindControl<CheckBox>("skillMultiUp");
        private CheckBox multiBonusCheck => this.FindControl<CheckBox>("skillMultiBonus");
        private CheckBox noneSkillCheck => this.FindControl<CheckBox>("skillNone");

        public DeckBuilderView() {
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
                    return set.Contains(Card.Categories.PermanentGacha);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.PermanentGacha);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.limCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.LimitedGacha);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.LimitedGacha);
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
                this.Bind(ViewModel, vm => vm.Categories, v => v.lnkCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.Linkage);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.Linkage);
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
                this.Bind(ViewModel, vm => vm.Categories, v => v.prCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.PR);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.PR);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Categories, v => v.otherCheck.IsChecked, set => {
                    return set.Contains(Card.Categories.Other);
                }, isChecked => {
                    SetCategory(isChecked, Card.Categories.Other);
                    return ViewModel.Categories;
                }).DisposeWith(disposables);

                // Center boost filters
                this.Bind(ViewModel, vm => vm.CenterBoostTypes, v => v.centerBoostVocalCheck.IsChecked, set => {
                    return set.Contains(CardData.CenterEffect.Type.vocalUp);
                }, isChecked => {
                    SetCenterBoost(isChecked, CardData.CenterEffect.Type.vocalUp);
                    return ViewModel.CenterBoostTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterBoostTypes, v => v.centerBoostDanceCheck.IsChecked, set => {
                    return set.Contains(CardData.CenterEffect.Type.danceUp);
                }, isChecked => {
                    SetCenterBoost(isChecked, CardData.CenterEffect.Type.danceUp);
                    return ViewModel.CenterBoostTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterBoostTypes, v => v.centerBoostVisualCheck.IsChecked, set => {
                    return set.Contains(CardData.CenterEffect.Type.visualUp);
                }, isChecked => {
                    SetCenterBoost(isChecked, CardData.CenterEffect.Type.visualUp);
                    return ViewModel.CenterBoostTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterBoostTypes, v => v.centerBoostAllCheck.IsChecked, set => {
                    return set.Contains(CardData.CenterEffect.Type.allUp);
                }, isChecked => {
                    SetCenterBoost(isChecked, CardData.CenterEffect.Type.allUp);
                    return ViewModel.CenterBoostTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterBoostTypes, v => v.centerBoostLifeCheck.IsChecked, set => {
                    return set.Contains(CardData.CenterEffect.Type.lifeUp);
                }, isChecked => {
                    SetCenterBoost(isChecked, CardData.CenterEffect.Type.lifeUp);
                    return ViewModel.CenterBoostTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterBoostTypes, v => v.centerBoostProcCheck.IsChecked, set => {
                    return set.Contains(CardData.CenterEffect.Type.skillActivationUp);
                }, isChecked => {
                    SetCenterBoost(isChecked, CardData.CenterEffect.Type.skillActivationUp);
                    return ViewModel.CenterBoostTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterBoostTypes, v => v.centerBoostNoneCheck.IsChecked, set => {
                    return set.Contains(CardData.CenterEffect.Type.none);
                }, isChecked => {
                    SetCenterBoost(isChecked, CardData.CenterEffect.Type.none);
                    return ViewModel.CenterBoostTypes;
                }).DisposeWith(disposables);

                // Center requirement filters
                this.Bind(ViewModel, vm => vm.CenterReqTypes, v => v.centerReqPrincessCheck.IsChecked, set => {
                    return set.Contains(Types.Princess);
                }, isChecked => {
                    SetCenterReq(isChecked, Types.Princess);
                    return ViewModel.CenterReqTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterReqTypes, v => v.centerReqFairyCheck.IsChecked, set => {
                    return set.Contains(Types.Fairy);
                }, isChecked => {
                    SetCenterReq(isChecked, Types.Fairy);
                    return ViewModel.CenterReqTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterReqTypes, v => v.centerReqAngelCheck.IsChecked, set => {
                    return set.Contains(Types.Angel);
                }, isChecked => {
                    SetCenterReq(isChecked, Types.Angel);
                    return ViewModel.CenterReqTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterReqTypes, v => v.centerReqAllCheck.IsChecked, set => {
                    return set.Contains(Types.All);
                }, isChecked => {
                    SetCenterReq(isChecked, Types.All);
                    return ViewModel.CenterReqTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CenterReqTypes, v => v.centerReqNoneCheck.IsChecked, set => {
                    return set.Contains(Types.None);
                }, isChecked => {
                    SetCenterReq(isChecked, Types.None);
                    return ViewModel.CenterReqTypes;
                }).DisposeWith(disposables);

                // Skill filters
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.scoreCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.scoreBonus);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.scoreBonus);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.overClockCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.overClock);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.overClock);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.fusionScoreCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.fusionScore);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.fusionScore);
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
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.fusionComboCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.fusionCombo);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.fusionCombo);
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
                    return set.Contains(CardData.Skill.Type.healer);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.healer);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.damageGuardCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.lifeGuard);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.lifeGuard);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.comboProtectCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.comboGuard);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.comboGuard);
                    return ViewModel.SkillTypes;
                }).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SkillTypes, v => v.judgementBoostCheck.IsChecked, set => {
                    return set.Contains(CardData.Skill.Type.perfectLock);
                }, isChecked => {
                    SetSkill(isChecked, CardData.Skill.Type.perfectLock);
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

                // Initial filtering on boot
                ViewModel.FilterCards();
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

        private void SetCenterBoost(bool? isChecked, CardData.CenterEffect.Type center) {
            if (isChecked ?? true) {
                ViewModel.CenterBoostTypes.Add(center);
            } else {
                ViewModel.CenterBoostTypes.Remove(center);
            }
            ViewModel.FilterCards();
        }

        private void SetCenterReq(bool? isChecked, Types center) {
            if (isChecked ?? true) {
                ViewModel.CenterReqTypes.Add(center);
            } else {
                ViewModel.CenterReqTypes.Remove(center);
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
