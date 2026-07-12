// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using Newtonsoft.Json;
using osu.Game.Rulesets.Difficulty;

namespace osu.Game.Rulesets.Typer.Difficulty
{
    public class TyperPerformanceAttributes : PerformanceAttributes
    {
        /// <summary>
        /// The difficulty portion of the total performance points.
        /// </summary>
        [JsonProperty("difficulty")]
        public double Difficulty { get; set; }

        /// <summary>
        /// The accuracy portion of the total performance points.
        /// </summary>
        [JsonProperty("accuracy")]
        public double Accuracy { get; set; }

        /// <summary>
        /// The estimated unstable rate of the player, derived from their hit judgements.
        /// May be <c>null</c> if it could not be determined (e.g. no greats, or invalid hit window).
        /// </summary>
        [JsonProperty("estimated_unstable_rate")]
        public double? EstimatedUnstableRate { get; set; }

        public override IEnumerable<PerformanceDisplayAttribute> GetAttributesForDisplay()
        {
            foreach (var attribute in base.GetAttributesForDisplay())
                yield return attribute;

            yield return new PerformanceDisplayAttribute(nameof(Difficulty), "Difficulty", Difficulty);
            yield return new PerformanceDisplayAttribute(nameof(Accuracy), "Accuracy", Accuracy);
        }
    }
}
