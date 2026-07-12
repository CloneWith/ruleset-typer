using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Typer.Mods;

namespace osu.Game.Rulesets.Typer.Scoring
{
    public class TyperScoreMultiplierCalculator : ScoreMultiplierCalculator
    {
        public TyperScoreMultiplierCalculator(ScoreMultiplierContext context)
            : base(context)
        {

            #region Difficulty Reduction

            Single<TyperModEasy>(hasMultiplier: 0.5);
            Single<TyperModNoFail>(hasMultiplier: 0.5);
            Single<TyperModHalfTime>(hasMultiplier: halfTime => rateAdjustMultiplier(halfTime.SpeedChange.Value));
            Single<TyperModDaycore>(hasMultiplier: daycore => rateAdjustMultiplier(daycore.SpeedChange.Value));

            #endregion

            #region Difficulty Increase

            Single<TyperModHardRock>(hasMultiplier: hardRock => hardRock.UsesDefaultConfiguration ? 1.06 : 1);
            // Sudden Death
            // Perfect
            Single<TyperModDoubleTime>(hasMultiplier: doubleTime => rateAdjustMultiplier(doubleTime.SpeedChange.Value));
            Single<TyperModNightcore>(hasMultiplier: nightcore => rateAdjustMultiplier(nightcore.SpeedChange.Value));
            Single<TyperModHidden>(hasMultiplier: hidden => hidden.UsesDefaultConfiguration ? 1.06 : 1);
            Single<TyperModFlashlight>(hasMultiplier: flashlight => flashlight.UsesDefaultConfiguration ? 1.12 : 1);
            // Accuracy Challenge

            #endregion

            #region Conversion

            Single<TyperModDifficultyAdjust>(hasMultiplier: 0.5);
            Single<TyperModConstantSpeed>(hasMultiplier: 0.9);

            #endregion

            #region Automation

            // Autoplay
            // Cinema
            Single<TyperModRelax>(hasMultiplier: 0.1);

            #endregion

            #region Fun

            Single<ModWindUp>(hasMultiplier: 0.5);
            Single<ModWindDown>(hasMultiplier: 0.5);
            // Muted
            Single<ModAdaptiveSpeed>(hasMultiplier: 0.5);

            #endregion

        }

        private static double rateAdjustMultiplier(double speedChange)
        {
            // Round to the nearest multiple of 0.1.
            double value = (int)(speedChange * 10) / 10.0;

            // Offset back to 0.
            value -= 1;

            if (speedChange >= 1)
                return 1 + value / 5;
            else
                return 0.6 + value;
        }
    }
}
