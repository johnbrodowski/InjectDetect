namespace InjectDetect
{
    public class TuningWeights
    {
        // Composite score blend
        public double DriftWeight { get; set; } = 0.5;

        public double IntentWeight { get; set; } = 0.0;

        public double KeywordWeight => 1.0 - DriftWeight - IntentWeight;

        // Drift score internal blend (must sum to 1.0)
        public double MaxDriftWeight { get; set; } = 0.6;

        public double AvgDriftWeight { get; set; } = 0.3;
        public double StdDevWeight { get; set; } = 0.1;

        // Decision boundaries
        public double Threshold { get; set; } = 0.15;  // CLEAN | UNCERTAIN boundary

        public double UncertaintyBand { get; set; } = 0.40; // UNCERTAIN | SUSPICIOUS boundary
                                                            // as a fraction of Threshold
                                                            // e.g. 0.40 means uncertain
                                                            // starts at Threshold * 0.40

        // Derived boundaries
        public double UncertainThreshold => Threshold * UncertaintyBand;

        public string Label =>
            $"D={DriftWeight:F2} I={IntentWeight:F2} MD={MaxDriftWeight:F2} AD={AvgDriftWeight:F2} " +
            $"SD={StdDevWeight:F2} T={Threshold:F2} UB={UncertaintyBand:F2}";

        public TuningWeights Clone() => new TuningWeights
        {
            DriftWeight = DriftWeight,
            IntentWeight = IntentWeight,
            MaxDriftWeight = MaxDriftWeight,
            AvgDriftWeight = AvgDriftWeight,
            StdDevWeight = StdDevWeight,
            Threshold = Threshold,
            UncertaintyBand = UncertaintyBand,
        };

        // Three-zone classification
        public DetectionResult Classify(double score)
        {
            if (score >= Threshold) return DetectionResult.Suspicious;
            if (score >= UncertainThreshold) return DetectionResult.Uncertain;
            return DetectionResult.Clean;
        }
    }

    public enum DetectionResult { Clean, Uncertain, Suspicious }
}