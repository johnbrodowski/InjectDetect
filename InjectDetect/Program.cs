namespace InjectDetect
{
    internal class Program
    {
        // -------------------------------------------------------------------------
        // MODE — set one to true
        // -------------------------------------------------------------------------
        private static bool TuningMode = false;

        private static bool SentenceLogMode = false;
        private static bool FineGridMode = false;
        private static bool DatasetBenchmarkMode = true;
        //private static bool InteractiveMode = false;

        private static void Main()
        {
            ApplyDefaultSettings();

            Console.WriteLine();
            Console.WriteLine("  PROMPT ANALYZER");
            Console.WriteLine($"  {SynonymNormalizer.VariantCount} synonym variants loaded.");
            Console.WriteLine($"  {TestCorpus.Prompts.Length} test prompts  " +
                              $"({TestCorpus.GetByClass(PromptClass.Clean).Length} clean, " +
                              $"{TestCorpus.GetByClass(PromptClass.Injection).Length} injection).");
            Console.WriteLine();

            if (TuningMode) RunTuner();
            else if (SentenceLogMode) RunSentenceLog();
            else if (FineGridMode) RunFineGrid();
            else if (DatasetBenchmarkMode) RunDatasetBenchmark();
            else RunInteractive();
        }

        private static void ApplyDefaultSettings()
        {
            Settings.NormalizeLeetspeak = true;
            Settings.TuningResolution = TuningResolution.TwoPass;
        }

        // ── Tuning mode ───────────────────────────────────────────────────────────
        private static void RunTuner()
        {
            int w = 90;
            string div = new string('─', w);

            Console.WriteLine("  Running auto-tuner...");
            Console.WriteLine();

            int last = -1;
            var result = AutoTuner.Run(TestCorpus.Prompts, (done, total) =>
            {
                int pct = done * 100 / total;
                if (pct != last && pct % 10 == 0)
                { Console.Write($"\r  Progress: {pct}% ({done}/{total})   "); last = pct; }
            });

            Console.WriteLine($"\r                                     ");
            Console.WriteLine();
            Console.WriteLine(div);
            Console.WriteLine("  TUNING RESULTS");
            Console.WriteLine(div);
            Console.WriteLine();

            var b = result.Best;
            Console.WriteLine("  Best settings:");
            Console.WriteLine($"    RemoveStopWords={b.RemoveStopWords}  NormalizeSynonyms={b.NormalizeSynonyms}  " +
                              $"ExpandContractions={b.ExpandContractions}  ContractExpanded={b.ContractExpanded}");
            Console.WriteLine($"    NormalizeWhitespace={b.NormalizeWhitespace}  LowercaseVariant={b.LowercaseVariant}  " +
                              $"StripPunctuation={b.StripPunctuation}  RunCombinedVariant={b.RunCombinedVariant}");
            Console.WriteLine();
            Console.WriteLine($"  Margin:     {result.Margin:F4}   Threshold: {result.Threshold:F4}");
            Console.WriteLine($"  Accuracy:   {result.Accuracy:P1}   TPR: {result.TruePositiveRate:P1}   FPR: {result.FalsePositiveRate:P1}");
            Console.WriteLine();

            for (int d = 1; d <= 17; d++)
            {
                var at = result.PromptResults.Where(p => p.Difficulty == d).ToList();
                if (!at.Any()) continue;
                int correct = at.Count(p => p.Correct);
                Console.WriteLine($"  d{d}: {correct}/{at.Count}");
                foreach (var p in at)
                {
                    string cls = p.Class == PromptClass.Injection ? "INJ" : "CLN";
                    string ok = p.Correct ? "✓" : "✗";
                    string txt = p.Text.Length > 70 ? p.Text[..67] + "..." : p.Text;
                    Console.WriteLine($"    {ok} [{cls}] {p.Score:F3}  {txt}");
                }
                Console.WriteLine();
            }

            var fail = result.PromptResults.Where(p => !p.Correct).ToList();
            if (fail.Any())
            {
                Console.WriteLine(div);
                Console.WriteLine($"  MISCLASSIFIED ({fail.Count})");
                Console.WriteLine(div);
                foreach (var p in fail)
                {
                    string lbl = p.Class == PromptClass.Injection ? "INJECTION missed" : "CLEAN flagged";
                    Console.WriteLine($"  [{lbl}] score:{p.Score:F3}  d:{p.Difficulty}");
                    Console.WriteLine($"    {p.Text}");
                    Console.WriteLine();
                }
            }
        }

        // ── Sentence log mode ─────────────────────────────────────────────────────
        private static void RunSentenceLog()
        {
            // Use threshold from a quick tuning pass, or set manually
            double threshold = 0.15;

            int w = 100;
            string div = new string('─', w);
            string div2 = new string('·', w);

            Console.WriteLine($"  Running sentence combo log (threshold={threshold:F2})...");
            Console.WriteLine();

            int last = -1;
            var logs = AutoTuner.RunSentenceLog(TestCorpus.Prompts, threshold, null, (done, total) =>
            {
                int pct = done * 100 / total;
                if (pct != last && pct % 20 == 0)
                { Console.Write($"\r  Progress: {pct}% ({done}/{total})   "); last = pct; }
            });

            Console.WriteLine($"\r                                     ");
            Console.WriteLine();

            // Summary table first
            Console.WriteLine(div);
            Console.WriteLine($"  {"Class",-6} {"d",-3} {"Flagged/Total",-15} {"Flag%",-8} {"Change%",-9} {"BestScore",-11} {"BestCombo",-20} Text");
            Console.WriteLine(div);

            foreach (var log in logs.OrderBy(l => l.Class).ThenBy(l => l.Difficulty))
            {
                string cls = log.Class == PromptClass.Injection ? "INJ" : "CLN";
                double flagPct = (double)log.FlaggedCount / log.TotalCombos * 100;
                double changePct = (double)log.DriftCount / log.TotalCombos * 100;
                string txt = log.Text.Length > 45 ? log.Text[..42] + "..." : log.Text;
                string alert = log.Class == PromptClass.Clean && flagPct > 30 ? " ⚠" :
                                  log.Class == PromptClass.Injection && flagPct < 30 ? " ✗" : "";

                Console.WriteLine($"  {cls,-6} {log.Difficulty,-3} " +
                                  $"{log.FlaggedCount}/{log.TotalCombos,-10} " +
                                  $"{flagPct,5:F1}%   {changePct,5:F1}%    " +
                                  $"{log.BestScore,-11:F3} {log.BestComboLabel,-20} {txt}{alert}");
            }

            Console.WriteLine(div);
            Console.WriteLine();

            // Overall stats
            var clean = logs.Where(l => l.Class == PromptClass.Clean).ToList();
            var injection = logs.Where(l => l.Class == PromptClass.Injection).ToList();

            double cleanFlagAvg = clean.Average(l => (double)l.FlaggedCount / l.TotalCombos * 100);
            double injFlagAvg = injection.Average(l => (double)l.FlaggedCount / l.TotalCombos * 100);

            Console.WriteLine($"  Avg flag rate — Clean: {cleanFlagAvg:F1}%   Injection: {injFlagAvg:F1}%");
            Console.WriteLine($"  Separation: {injFlagAvg - cleanFlagAvg:F1} percentage points");
            Console.WriteLine();

            // Per-difficulty flag rate
            Console.WriteLine("  Flag rate by difficulty:");
            for (int d = 1; d <= 17; d++)
            {
                var clnD = logs.Where(l => l.Class == PromptClass.Clean && l.Difficulty == d).ToList();
                var injD = logs.Where(l => l.Class == PromptClass.Injection && l.Difficulty == d).ToList();
                if (!clnD.Any() && !injD.Any()) continue;

                string clnStr = clnD.Any()
                    ? $"CLN {clnD.Average(l => (double)l.FlaggedCount / l.TotalCombos * 100):F1}%"
                    : "";
                string injStr = injD.Any()
                    ? $"INJ {injD.Average(l => (double)l.FlaggedCount / l.TotalCombos * 100):F1}%"
                    : "";
                Console.WriteLine($"    d{d}:  {clnStr,-15} {injStr}");
            }
            Console.WriteLine();

            // Worst false positives
            var worstFP = clean.OrderByDescending(l => l.FlaggedCount).Take(5).ToList();
            Console.WriteLine("  Top false positives (clean prompts most often flagged):");
            foreach (var l in worstFP)
            {
                double pct = (double)l.FlaggedCount / l.TotalCombos * 100;
                string txt = l.Text.Length > 70 ? l.Text[..67] + "..." : l.Text;
                Console.WriteLine($"    {pct,5:F1}%  {txt}");
            }
            Console.WriteLine();

            // Worst false negatives
            var worstFN = injection.OrderBy(l => l.FlaggedCount).Take(5).ToList();
            Console.WriteLine("  Top false negatives (injections least often flagged):");
            foreach (var l in worstFN)
            {
                double pct = (double)l.FlaggedCount / l.TotalCombos * 100;
                string txt = l.Text.Length > 70 ? l.Text[..67] + "..." : l.Text;
                Console.WriteLine($"    {pct,5:F1}%  {txt}");
            }

            Console.WriteLine();
            Console.WriteLine(div);
            Console.WriteLine("  BEST COMBO ANALYSIS");
            Console.WriteLine(div);
            Console.WriteLine();

            var comboFreq = injection
                .GroupBy(l => l.BestComboLabel)
                .Select(g => (Label: g.Key, Count: g.Count(), AvgScore: g.Average(l => l.BestScore)))
                .OrderByDescending(x => x.Count)
                .ToList();

            Console.WriteLine("  Combo frequency as best performer across injection prompts:");
            foreach (var (label, count, avgScore) in comboFreq.Take(8))
                Console.WriteLine($"    {count,3}x  avg score: {avgScore:F3}  [{label}]");
            Console.WriteLine();

            string topCombo = comboFreq.First().Label;
            bool sw = topCombo.Contains("SW");
            bool sy = topCombo.Contains("SY");
            bool ex = topCombo.Contains("EX");
            bool ct = topCombo.Contains("CT");
            bool ws = topCombo.Contains("WS");
            bool lc = topCombo.Contains("LC");
            bool pu = topCombo.Contains("PU");
            bool lt = topCombo.Contains("LT");
            bool cm = topCombo.Contains("CM");
            Console.WriteLine($"  Recommended settings (from [{topCombo}]):");
            Console.WriteLine($"    Settings.RemoveStopWords      = {sw};");
            Console.WriteLine($"    Settings.NormalizeSynonyms    = {sy};");
            Console.WriteLine($"    Settings.ExpandContractions   = {ex};");
            Console.WriteLine($"    Settings.ContractExpanded     = {ct};");
            Console.WriteLine($"    Settings.NormalizeWhitespace  = {ws};");
            Console.WriteLine($"    Settings.LowercaseVariant     = {lc};");
            Console.WriteLine($"    Settings.StripPunctuation     = {pu};");
            Console.WriteLine($"    Settings.NormalizeLeetspeak   = {lt};");
            Console.WriteLine($"    Settings.RunCombinedVariant   = {cm};");
            Console.WriteLine($"    // Threshold: {threshold:F2}");
        }

        // ── Fine grid mode ────────────────────────────────────────────────────────
        private static void RunFineGrid()
        {
            int w = 90;
            string div = new string('─', w);

            // Step 1: binary combo pass to find best combo
            Console.WriteLine("  Step 1: binary combo search...");
            Console.WriteLine();
            int last = -1;
            var coarseResult = AutoTuner.Run(TestCorpus.Prompts, (done, total) =>
            {
                int pct = done * 100 / total;
                if (pct != last && pct % 20 == 0)
                { Console.Write($"\r  Progress: {pct}% ({done}/{total})   "); last = pct; }
            });
            Console.WriteLine($"\r  Best combo: [{AutoTuner.ComboLabel(coarseResult.Best)}]   " +
                              $"margin: {coarseResult.Margin:F4}   acc: {coarseResult.Accuracy:P1}");
            Console.WriteLine();

            // Step 2: fine grid over weights
            Console.WriteLine("  Step 2: fine weight grid search...");
            Console.WriteLine();
            last = -1;
            var fineResult = AutoTuner.RunFineGrid(TestCorpus.Prompts, coarseResult.Best, (done, total) =>
            {
                int pct = done * 100 / total;
                if (pct != last && pct % 10 == 0)
                { Console.Write($"\r  Progress: {pct}% ({done}/{total})   "); last = pct; }
            });
            Console.WriteLine($"\r                                     ");
            Console.WriteLine();

            Console.WriteLine(div);
            Console.WriteLine("  FINE GRID RESULTS");
            Console.WriteLine(div);
            Console.WriteLine();

            var fw = fineResult.Weights;
            Console.WriteLine("  Best weights:");
            Console.WriteLine($"    DriftWeight    = {fw.DriftWeight:F2}   (Keyword = {fw.KeywordWeight:F2})");
            Console.WriteLine($"    IntentWeight   = {fw.IntentWeight:F2}");
            Console.WriteLine($"    MaxDriftWeight = {fw.MaxDriftWeight:F2}");
            Console.WriteLine($"    AvgDriftWeight = {fw.AvgDriftWeight:F2}");
            Console.WriteLine($"    StdDevWeight   = {fw.StdDevWeight:F2}");
            Console.WriteLine($"    Threshold      = {fw.Threshold:F2}   (CLEAN | UNCERTAIN boundary)");
            Console.WriteLine($"    UncertaintyBand= {fw.UncertaintyBand:F2}   (UNCERTAIN starts at {fw.UncertainThreshold:F3})");
            Console.WriteLine();
            Console.WriteLine($"  Objective:{fineResult.Margin:F4}   (coarse margin was {coarseResult.Margin:F4})");
            Console.WriteLine($"  Accuracy: {fineResult.Accuracy:P1}   TPR: {fineResult.TruePositiveRate:P1}   FPR: {fineResult.FalsePositiveRate:P1}");
            Console.WriteLine();

            Console.WriteLine("  Settings to copy into your code:");
            Console.WriteLine($"    Settings.RemoveStopWords      = {fineResult.Combo.RemoveStopWords};");
            Console.WriteLine($"    Settings.NormalizeSynonyms    = {fineResult.Combo.NormalizeSynonyms};");
            Console.WriteLine($"    Settings.ExpandContractions   = {fineResult.Combo.ExpandContractions};");
            Console.WriteLine($"    Settings.ContractExpanded     = {fineResult.Combo.ContractExpanded};");
            Console.WriteLine($"    Settings.NormalizeWhitespace  = {fineResult.Combo.NormalizeWhitespace};");
            Console.WriteLine($"    Settings.LowercaseVariant     = {fineResult.Combo.LowercaseVariant};");
            Console.WriteLine($"    Settings.StripPunctuation     = {fineResult.Combo.StripPunctuation};");
            Console.WriteLine($"    Settings.NormalizeLeetspeak   = {fineResult.Combo.NormalizeLeetspeak};");
            Console.WriteLine($"    Settings.RunCombinedVariant   = {fineResult.Combo.RunCombinedVariant};");
            Console.WriteLine($"    DriftWeight={fw.DriftWeight:F2}  IntentWeight={fw.IntentWeight:F2}  MaxDrift={fw.MaxDriftWeight:F2}  AvgDrift={fw.AvgDriftWeight:F2}  StdDev={fw.StdDevWeight:F2}");
            Console.WriteLine($"    Threshold={fw.Threshold:F2}  UncertaintyBand={fw.UncertaintyBand:F2}  (uncertain >= {fw.UncertainThreshold:F3})");
            Console.WriteLine();

            // Per-difficulty breakdown
            Console.WriteLine(div);
            Console.WriteLine("  PER-DIFFICULTY BREAKDOWN");
            Console.WriteLine(div);
            Console.WriteLine();
            for (int d = 1; d <= 17; d++)
            {
                var at = fineResult.PromptResults.Where(p => p.Difficulty == d).ToList();
                if (!at.Any()) continue;
                int correct = at.Count(p => p.Correct);
                Console.WriteLine($"  d{d}: {correct}/{at.Count}");
                foreach (var p in at)
                {
                    string cls = p.Class == PromptClass.Injection ? "INJ" : "CLN";
                    string ok = p.Correct ? "✓" : "✗";
                    string txt = p.Text.Length > 70 ? p.Text[..67] + "..." : p.Text;
                    Console.WriteLine($"    {ok} [{cls}] {p.Score:F3}  {txt}");
                }
                Console.WriteLine();
            }

            PrintFamilyBreakdown(fineResult.PromptResults);

            var fail = fineResult.PromptResults.Where(p => !p.Correct).ToList();
            Console.WriteLine(div);
            Console.WriteLine($"  MISCLASSIFIED ({fail.Count})");
            Console.WriteLine(div);
            Console.WriteLine();
            foreach (var p in fail)
            {
                string lbl = p.Expected switch
                {
                    ExpectedOutcome.MustStayClean => "CLEAN elevated",
                    ExpectedOutcome.ShouldBeSuspicious => "INJECTION not suspicious",
                    _ => "INJECTION stayed clean",
                };
                Console.WriteLine($"  [{lbl}] score:{p.Score:F3}  d:{p.Difficulty}  family:{p.Family}  detected:{p.Detection}");
                Console.WriteLine($"    {p.Text}");
                Console.WriteLine();
            }

            PrintPatternDiagnostic(fineResult.PromptResults, div);
        }

        private static void PrintPatternDiagnostic(IEnumerable<AutoTuner.PromptResult> promptResults, string div)
        {
            Console.WriteLine(div);
            Console.WriteLine("  INTENT PATTERN MATCHES (injection prompts)");
            Console.WriteLine(div);
            Console.WriteLine();

            var injections = promptResults
                .Where(p => p.Class == PromptClass.Injection)
                .OrderBy(p => p.Family.ToString())
                .ThenBy(p => p.Difficulty);

            string? lastFamily = null;
            foreach (var p in injections)
            {
                string family = p.Family.ToString();
                if (family != lastFamily)
                {
                    Console.WriteLine($"  [{family}]");
                    lastFamily = family;
                }

                var (intentScore, matched) = IntentPatternScorer.Score(p.Text);
                string patternList = matched.Length > 0
                    ? string.Join(", ", matched)
                    : "(none)";
                string ok = p.Correct ? "✓" : "✗";
                string txt = p.Text.Length > 60 ? p.Text[..57] + "..." : p.Text;
                Console.WriteLine($"    {ok} d{p.Difficulty} intent:{intentScore:F3}  [{patternList}]");
                Console.WriteLine($"       {txt}");
            }

            Console.WriteLine();
        }

        // ── Dataset benchmark mode ────────────────────────────────────────────────
        private static void RunDatasetBenchmark()
        {
            int w = 90;
            string div = new string('─', w);

            var prompts = DatasetLoader.TryLoad(out string searchedPath);
            if (prompts is null)
            {
                Console.WriteLine($"  ERROR: Dataset file not found.");
                Console.WriteLine($"  Searched from: {searchedPath}");
                Console.WriteLine($"  Place 'Prompt_INJECTION_And_Benign_DATASET.jsonl' in the project root or a parent directory.");
                return;
            }

            int clean = prompts.Count(p => p.Class == PromptClass.Clean);
            int malicious = prompts.Count(p => p.Class == PromptClass.Injection);
            Console.WriteLine($"  Dataset loaded: {prompts.Length} entries  ({clean} benign, {malicious} malicious).");
            Console.WriteLine();

            // Step 1: binary combo search
            Console.WriteLine("  Step 1: binary combo search...");
            Console.WriteLine();
            int last = -1;
            var coarseResult = AutoTuner.Run(prompts, (done, total) =>
            {
                int pct = done * 100 / total;
                if (pct != last && pct % 20 == 0)
                { Console.Write($"\r  Progress: {pct}% ({done}/{total})   "); last = pct; }
            });
            Console.WriteLine($"\r  Best combo: [{AutoTuner.ComboLabel(coarseResult.Best)}]   " +
                              $"margin: {coarseResult.Margin:F4}   acc: {coarseResult.Accuracy:P1}");
            Console.WriteLine();

            // Step 2: fine weight grid search
            Console.WriteLine("  Step 2: fine weight grid search...");
            Console.WriteLine();
            last = -1;
            var fineResult = AutoTuner.RunFineGrid(prompts, coarseResult.Best, (done, total) =>
            {
                int pct = done * 100 / total;
                if (pct != last && pct % 10 == 0)
                { Console.Write($"\r  Progress: {pct}% ({done}/{total})   "); last = pct; }
            });
            Console.WriteLine($"\r                                     ");
            Console.WriteLine();

            Console.WriteLine(div);
            Console.WriteLine("  DATASET BENCHMARK RESULTS");
            Console.WriteLine(div);
            Console.WriteLine();

            var fw = fineResult.Weights;
            Console.WriteLine("  Best weights:");
            Console.WriteLine($"    DriftWeight    = {fw.DriftWeight:F2}   (Keyword = {fw.KeywordWeight:F2})");
            Console.WriteLine($"    IntentWeight   = {fw.IntentWeight:F2}");
            Console.WriteLine($"    MaxDriftWeight = {fw.MaxDriftWeight:F2}");
            Console.WriteLine($"    AvgDriftWeight = {fw.AvgDriftWeight:F2}");
            Console.WriteLine($"    StdDevWeight   = {fw.StdDevWeight:F2}");
            Console.WriteLine($"    Threshold      = {fw.Threshold:F2}   (CLEAN | UNCERTAIN boundary)");
            Console.WriteLine($"    UncertaintyBand= {fw.UncertaintyBand:F2}   (UNCERTAIN starts at {fw.UncertainThreshold:F3})");
            Console.WriteLine();
            Console.WriteLine($"  Objective:{fineResult.Margin:F4}   (coarse margin was {coarseResult.Margin:F4})");
            Console.WriteLine($"  Accuracy: {fineResult.Accuracy:P1}   TPR: {fineResult.TruePositiveRate:P1}   FPR: {fineResult.FalsePositiveRate:P1}");
            Console.WriteLine();

            // Attack-type breakdown via PromptFamily (mapped from attack_type at load time)
            PrintFamilyBreakdown(fineResult.PromptResults);

            var fail = fineResult.PromptResults.Where(p => !p.Correct).ToList();
            Console.WriteLine(div);
            Console.WriteLine($"  MISCLASSIFIED ({fail.Count})");
            Console.WriteLine(div);
            Console.WriteLine();
            foreach (var p in fail)
            {
                string lbl = p.Expected switch
                {
                    ExpectedOutcome.MustStayClean => "BENIGN elevated",
                    ExpectedOutcome.ShouldBeSuspicious => "MALICIOUS not suspicious",
                    _ => "MALICIOUS stayed clean",
                };
                Console.WriteLine($"  [{lbl}] score:{p.Score:F3}  family:{p.Family}  detected:{p.Detection}");
                Console.WriteLine($"    {p.Text}");
                Console.WriteLine();
            }
        }

        // ── Interactive mode ──────────────────────────────────────────────────────
        private static void RunInteractive()
        {
            int w = 110;
            string div = new string('─', w);
            double threshold = 0.15;

            Console.WriteLine($"  Interactive mode (threshold={threshold:F2}). Empty line to quit.");
            Console.WriteLine();

            while (true)
            {
                Console.Write("  > ");
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) break;
                Console.WriteLine();
                AnalyzeAndPrint(input, div, threshold);
            }
        }

        private static void AnalyzeAndPrint(string prompt, string div, double threshold)
        {
            var variants = VariantPipeline.Generate(prompt);
            var pairs = variants.Select(v => (v.Label, v.Text)).ToList();
            var report = SemanticSimilarity.Analyze(pairs);

            string mostNorm = pairs.Count > 1 ? pairs.Last().Text : pairs[0].Text;
            double keyword = KeywordScorer.Score(mostNorm);
            double drift = (report.SpreadScore * 0.6) + (report.AvgDrift * 0.3) + (report.StdDev * 0.1);
            var (intentScore, matchedPatterns) = IntentPatternScorer.Score(prompt);
            double composite = (drift * 0.3) + (keyword * 0.4) + (intentScore * 0.3);

            var weights = new TuningWeights { Threshold = threshold, DriftWeight = 0.3, IntentWeight = 0.3 };
            string suspicion = weights.Classify(composite) switch
            {
                DetectionResult.Suspicious => "SUSPICIOUS ⚠",
                DetectionResult.Uncertain => "UNCERTAIN  ?",
                _ => "CLEAN      ✓",
            };

            Console.WriteLine(div);
            Console.WriteLine($"  [ORIGINAL]  {prompt}");
            for (int i = 1; i < variants.Count; i++)
                Console.WriteLine($"  {variants[i].Label,-28} drift: {report.Scores[i - 1].DriftFromOriginal:F3}");
            Console.WriteLine();
            Console.WriteLine($"  Drift: {drift:F4}   Keyword: {keyword:F4}   Intent: {intentScore:F4}   Composite: {composite:F4}");
            if (matchedPatterns.Length > 0)
                Console.WriteLine($"  Intent patterns: {string.Join(", ", matchedPatterns)}");
            Console.WriteLine($"  {suspicion}");
            Console.WriteLine();
        }

        private static void PrintFamilyBreakdown(IEnumerable<AutoTuner.PromptResult> promptResults)
        {
            Console.WriteLine("  FAMILY BREAKDOWN");
            Console.WriteLine("  Family                   Total  Correct  Uncertain+  Suspicious");
            Console.WriteLine("  ---------------------------------------------------------------");

            foreach (var group in promptResults.GroupBy(p => p.Family).OrderBy(g => g.Key.ToString()))
            {
                int total = group.Count();
                int correct = group.Count(p => p.Correct);
                int uncertainOrHigher = group.Count(p => p.Detection != DetectionResult.Clean);
                int suspicious = group.Count(p => p.Detection == DetectionResult.Suspicious);
                Console.WriteLine($"  {group.Key,-24} {total,5}  {correct,7}  {uncertainOrHigher,10}  {suspicious,10}");
            }

            Console.WriteLine();
        }
   
    
    
    
    }
}
