// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using Newtonsoft.Json;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;

namespace osu.Game.Rulesets.Typer.Difficulty
{
    public class TyperDifficultyAttributes : DifficultyAttributes
    {
        /// <summary>
        /// The difficulty corresponding to the mechanical skills in typer.
        /// This includes key-switch and stamina combined.
        /// </summary>
        public double MechanicalDifficulty { get; set; }

        /// <summary>
        /// The difficulty corresponding to the rhythm skill.
        /// </summary>
        [JsonProperty("rhythm_difficulty")]
        public double RhythmDifficulty { get; set; }

        /// <summary>
        /// The difficulty corresponding to the key-switch skill (analogous to Taiko's Colour).
        /// </summary>
        public double KeySwitchDifficulty { get; set; }

        /// <summary>
        /// The difficulty corresponding to the stamina skill.
        /// </summary>
        public double StaminaDifficulty { get; set; }

        /// <summary>
        /// The ratio of stamina difficulty from single-key streams to total stamina difficulty.
        /// A high value indicates the map's stamina difficulty comes mostly from rapid same-key presses.
        /// </summary>
        [JsonProperty("mono_stamina_factor")]
        public double MonoStaminaFactor { get; set; }

        /// <summary>
        /// The factor corresponding to the consistency of the map.
        /// Lower values indicate the map has high variance in difficulty (spiky patterns).
        /// </summary>
        [JsonProperty("consistency_factor")]
        public double ConsistencyFactor { get; set; }

        /// <summary>
        /// The number of top-weighted stamina strains, used for length bonus calculations.
        /// </summary>
        public double StaminaTopStrains { get; set; }

        public override IEnumerable<(int attributeId, object value)> ToDatabaseAttributes()
        {
            foreach (var v in base.ToDatabaseAttributes())
                yield return v;

            yield return (ATTRIB_ID_DIFFICULTY, StarRating);
            yield return (ATTRIB_ID_RHYTHM_DIFFICULTY, RhythmDifficulty);
            yield return (ATTRIB_ID_MONO_STAMINA_FACTOR, MonoStaminaFactor);
            yield return (ATTRIB_ID_CONSISTENCY_FACTOR, ConsistencyFactor);
        }

        public override void FromDatabaseAttributes(IReadOnlyDictionary<int, double> values, IBeatmapOnlineInfo onlineInfo)
        {
            base.FromDatabaseAttributes(values, onlineInfo);

            StarRating = values[ATTRIB_ID_DIFFICULTY];
            RhythmDifficulty = values[ATTRIB_ID_RHYTHM_DIFFICULTY];
            MonoStaminaFactor = values[ATTRIB_ID_MONO_STAMINA_FACTOR];
            ConsistencyFactor = values[ATTRIB_ID_CONSISTENCY_FACTOR];
        }
    }
}
