namespace InjectDetect
{
    public enum TuningResolution
    {
        // ~500 combos — fits tight sandbox / quick iteration
        Fast,

        // ~3,000 combos — balanced, good for most machines
        Balanced,

        // ~12,000 combos — thorough, for dedicated tuning runs
        Full,

        // Two-pass: coarse sweep → tight refinement around best point
        // Adapts total work to resolution setting above
        TwoPass,
    }

    public static class GridSpec
    {
        public record Spec(
            double DwStep,   // DriftWeight step
            double MdStep,   // MaxDriftWeight step
            double TStep,    // Threshold step
            double UbStep    // UncertaintyBand step
        );

        public static Spec CoarseSpec(TuningResolution res) => res switch
        {
            TuningResolution.Fast => new(0.20, 0.20, 0.06, 0.20),
            TuningResolution.Balanced => new(0.15, 0.15, 0.04, 0.15),
            TuningResolution.Full => new(0.10, 0.10, 0.02, 0.10),
            TuningResolution.TwoPass => new(0.20, 0.20, 0.05, 0.20),
            _ => new(0.15, 0.15, 0.04, 0.15),
        };

        // Fine pass — narrow window around best point, always small steps
        public static Spec FineSpec() => new(0.04, 0.05, 0.01, 0.05);

        // Half-width of the search window around best point for two-pass fine search
        public static double Window(string param) => param switch
        {
            "dw" => 0.10,
            "md" => 0.10,
            "t" => 0.04,
            "ub" => 0.10,
            _ => 0.08,
        };
    }
}