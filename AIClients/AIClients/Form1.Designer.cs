namespace AIClients
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // ── Chat area ─────────────────────────────────────────────────────────────
        private ComboBox  cboProvider;
        private TextBox   txtModel;
        private TextBox   txtSystemMessage;
        private TextBox   txtPrompt;
        private RichTextBox rtbOutput;
        private Button    btnCreateSession;
        private Button    btnSend;
        private Button    btnCancel;
        private Label     lblProvider;
        private Label     lblModel;
        private Label     lblSystemMessage;
        private Label     lblPrompt;

        // ── Benchmark control ─────────────────────────────────────────────────────
        private CollapsibleGroupBox grpBenchmark;
        private ComboBox       cboRunMode;
        private NumericUpDown  nudMaxPrompts;
        private ComboBox       cboAiMode;
        private Button         btnRunBenchmark;
        private Button         btnCancelBenchmark;
        private Button         btnRetestFailed;
        private Button         btnSaveAsDefault;
        private Label          lblBenchmarkStatus;
        private ProgressBar    pbProgress;

        // ── Pipeline settings ─────────────────────────────────────────────────────
        private CollapsibleGroupBox grpPipeline;
        private CheckBox  chkRemoveStopWords;
        private CheckBox  chkNormalizeSynonyms;
        private CheckBox  chkExpandContractions;
        private CheckBox  chkContractExpanded;
        private CheckBox  chkNormalizeWhitespace;
        private CheckBox  chkLowercaseVariant;
        private CheckBox  chkStripPunctuation;
        private CheckBox  chkNormalizeLeetspeak;
        private CheckBox  chkRunCombinedVariant;
        private CheckBox  chkFilterInvisibleUnicode;
        private CheckBox  chkExtractQuotedContent;
        private CheckBox  chkDecodeBase64;
        private CheckBox  chkNumbersToWords;
        private CheckBox  chkNormalizeHomoglyphs;
        private CheckBox  chkFlagSuspectedEncoding;
        private Button    btnResetDefaults;

        // ── Tuning resolution ─────────────────────────────────────────────────────
        private CollapsibleGroupBox grpResolution;
        private ComboBox  cboResolution;

        // ── Weights & thresholds ──────────────────────────────────────────────────
        private CollapsibleGroupBox grpWeights;
        private Label         lblWeightsInfo;
        private NumericUpDown nudThreshold;
        private NumericUpDown nudUncertaintyBand;
        private NumericUpDown nudDriftWeight;
        private NumericUpDown nudIntentWeight;
        private NumericUpDown nudMaxDriftWeight;
        private NumericUpDown nudAvgDriftWeight;
        private NumericUpDown nudStdDevWeight;
        private Label         lblKeywordWeightVal;
        private Label         lblLastRunAccuracy;
        private Label         lblLastRunTpr;
        private Label         lblLastRunFpr;
        private Label         lblLastRunMargin;
        private Label         lblLastRunCombo;
        private CheckBox      chkOverrideWeights;
        private Button        btnCopyWeights;
        private Button        btnExportResults;
        private NumericUpDown nudAiWeight;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblRunModeCaption = new Label();
            lblMaxPromptsLbl = new Label();
            lblMaxHint = new Label();
            lblResCaption = new Label();
            lblTCaption = new Label();
            lblUBCaption = new Label();
            lblDWCaption = new Label();
            lblIWCaption = new Label();
            lblKWCaption = new Label();
            lblMDCaption = new Label();
            lblADCaption = new Label();
            lblSDCaption = new Label();
            lblAccCaption = new Label();
            lblTprCaption = new Label();
            lblFprCaption = new Label();
            lblMarginCaption = new Label();
            lblComboCaption = new Label();
            cboProvider = new ComboBox();
            txtModel = new TextBox();
            txtSystemMessage = new TextBox();
            txtPrompt = new TextBox();
            rtbOutput = new RichTextBox();
            btnCreateSession = new Button();
            btnSend = new Button();
            btnCancel = new Button();
            lblProvider = new Label();
            lblModel = new Label();
            lblSystemMessage = new Label();
            lblPrompt = new Label();
            grpBenchmark = new CollapsibleGroupBox();
            cboRunMode = new ComboBox();
            nudMaxPrompts = new NumericUpDown();
            lblAiModeCaption = new Label();
            cboAiMode = new ComboBox();
            btnRunBenchmark = new Button();
            btnCancelBenchmark = new Button();
            btnRetestFailed = new Button();
            lblBenchmarkStatus = new Label();
            pbProgress = new ProgressBar();
            btnSaveAsDefault = new Button();
            grpPipeline = new CollapsibleGroupBox();
            chkRemoveStopWords = new CheckBox();
            chkNormalizeSynonyms = new CheckBox();
            chkExpandContractions = new CheckBox();
            chkNormalizeLeetspeak = new CheckBox();
            chkContractExpanded = new CheckBox();
            chkFilterInvisibleUnicode = new CheckBox();
            chkNormalizeWhitespace = new CheckBox();
            chkExtractQuotedContent = new CheckBox();
            chkLowercaseVariant = new CheckBox();
            chkDecodeBase64 = new CheckBox();
            chkStripPunctuation = new CheckBox();
            chkNumbersToWords = new CheckBox();
            chkRunCombinedVariant = new CheckBox();
            chkNormalizeHomoglyphs = new CheckBox();
            btnResetDefaults = new Button();
            chkFlagSuspectedEncoding = new CheckBox();
            grpResolution = new CollapsibleGroupBox();
            cboResolution = new ComboBox();
            grpWeights = new CollapsibleGroupBox();
            lblWeightsInfo = new Label();
            nudThreshold = new NumericUpDown();
            nudUncertaintyBand = new NumericUpDown();
            lblAiWCaption = new Label();
            nudAiWeight = new NumericUpDown();
            nudDriftWeight = new NumericUpDown();
            nudIntentWeight = new NumericUpDown();
            lblKeywordWeightVal = new Label();
            nudMaxDriftWeight = new NumericUpDown();
            nudAvgDriftWeight = new NumericUpDown();
            nudStdDevWeight = new NumericUpDown();
            lblLastRunAccuracy = new Label();
            lblLastRunTpr = new Label();
            lblLastRunFpr = new Label();
            lblLastRunMargin = new Label();
            lblLastRunCombo = new Label();
            chkOverrideWeights = new CheckBox();
            btnCopyWeights = new Button();
            btnExportResults = new Button();
            button1 = new Button();
            grpBenchmark.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudMaxPrompts).BeginInit();
            grpPipeline.SuspendLayout();
            grpResolution.SuspendLayout();
            grpWeights.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudThreshold).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudUncertaintyBand).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudAiWeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDriftWeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudIntentWeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxDriftWeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudAvgDriftWeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudStdDevWeight).BeginInit();
            SuspendLayout();
            // 
            // lblRunModeCaption
            // 
            lblRunModeCaption.AutoSize = true;
            lblRunModeCaption.Location = new Point(8, 24);
            lblRunModeCaption.Name = "lblRunModeCaption";
            lblRunModeCaption.Size = new Size(41, 15);
            lblRunModeCaption.TabIndex = 0;
            lblRunModeCaption.Text = "Mode:";
            // 
            // lblMaxPromptsLbl
            // 
            lblMaxPromptsLbl.AutoSize = true;
            lblMaxPromptsLbl.Location = new Point(238, 24);
            lblMaxPromptsLbl.Name = "lblMaxPromptsLbl";
            lblMaxPromptsLbl.Size = new Size(80, 15);
            lblMaxPromptsLbl.TabIndex = 2;
            lblMaxPromptsLbl.Text = "Max prompts:";
            // 
            // lblMaxHint
            // 
            lblMaxHint.ForeColor = SystemColors.GrayText;
            lblMaxHint.Location = new Point(402, 24);
            lblMaxHint.Name = "lblMaxHint";
            lblMaxHint.Size = new Size(52, 15);
            lblMaxHint.TabIndex = 4;
            lblMaxHint.Text = "(0 = all)";
            // 
            // lblResCaption
            // 
            lblResCaption.AutoSize = true;
            lblResCaption.Location = new Point(10, 19);
            lblResCaption.Name = "lblResCaption";
            lblResCaption.Size = new Size(107, 15);
            lblResCaption.TabIndex = 0;
            lblResCaption.Text = "Tuning Resolution:";
            // 
            // lblTCaption
            // 
            lblTCaption.AutoSize = true;
            lblTCaption.Location = new Point(8, 43);
            lblTCaption.Name = "lblTCaption";
            lblTCaption.Size = new Size(63, 15);
            lblTCaption.TabIndex = 1;
            lblTCaption.Text = "Threshold:";
            // 
            // lblUBCaption
            // 
            lblUBCaption.AutoSize = true;
            lblUBCaption.Location = new Point(168, 43);
            lblUBCaption.Name = "lblUBCaption";
            lblUBCaption.Size = new Size(75, 15);
            lblUBCaption.TabIndex = 3;
            lblUBCaption.Text = "Uncert Band:";
            // 
            // lblDWCaption
            // 
            lblDWCaption.AutoSize = true;
            lblDWCaption.Location = new Point(8, 72);
            lblDWCaption.Name = "lblDWCaption";
            lblDWCaption.Size = new Size(47, 15);
            lblDWCaption.TabIndex = 7;
            lblDWCaption.Text = "Drift W:";
            // 
            // lblIWCaption
            // 
            lblIWCaption.AutoSize = true;
            lblIWCaption.Location = new Point(135, 72);
            lblIWCaption.Name = "lblIWCaption";
            lblIWCaption.Size = new Size(55, 15);
            lblIWCaption.TabIndex = 9;
            lblIWCaption.Text = "Intent W:";
            // 
            // lblKWCaption
            // 
            lblKWCaption.AutoSize = true;
            lblKWCaption.Location = new Point(267, 72);
            lblKWCaption.Name = "lblKWCaption";
            lblKWCaption.Size = new Size(70, 15);
            lblKWCaption.TabIndex = 11;
            lblKWCaption.Text = "Keyword W:";
            // 
            // lblMDCaption
            // 
            lblMDCaption.AutoSize = true;
            lblMDCaption.Location = new Point(8, 101);
            lblMDCaption.Name = "lblMDCaption";
            lblMDCaption.Size = new Size(55, 15);
            lblMDCaption.TabIndex = 13;
            lblMDCaption.Text = "MaxDrift:";
            // 
            // lblADCaption
            // 
            lblADCaption.AutoSize = true;
            lblADCaption.Location = new Point(143, 101);
            lblADCaption.Name = "lblADCaption";
            lblADCaption.Size = new Size(54, 15);
            lblADCaption.TabIndex = 15;
            lblADCaption.Text = "AvgDrift:";
            // 
            // lblSDCaption
            // 
            lblSDCaption.AutoSize = true;
            lblSDCaption.Location = new Point(275, 101);
            lblSDCaption.Name = "lblSDCaption";
            lblSDCaption.Size = new Size(47, 15);
            lblSDCaption.TabIndex = 17;
            lblSDCaption.Text = "StdDev:";
            // 
            // lblAccCaption
            // 
            lblAccCaption.AutoSize = true;
            lblAccCaption.Location = new Point(8, 133);
            lblAccCaption.Name = "lblAccCaption";
            lblAccCaption.Size = new Size(59, 15);
            lblAccCaption.TabIndex = 19;
            lblAccCaption.Text = "Accuracy:";
            // 
            // lblTprCaption
            // 
            lblTprCaption.AutoSize = true;
            lblTprCaption.Location = new Point(133, 133);
            lblTprCaption.Name = "lblTprCaption";
            lblTprCaption.Size = new Size(31, 15);
            lblTprCaption.TabIndex = 21;
            lblTprCaption.Text = "TPR:";
            // 
            // lblFprCaption
            // 
            lblFprCaption.AutoSize = true;
            lblFprCaption.Location = new Point(218, 133);
            lblFprCaption.Name = "lblFprCaption";
            lblFprCaption.Size = new Size(30, 15);
            lblFprCaption.TabIndex = 23;
            lblFprCaption.Text = "FPR:";
            // 
            // lblMarginCaption
            // 
            lblMarginCaption.AutoSize = true;
            lblMarginCaption.Location = new Point(303, 133);
            lblMarginCaption.Name = "lblMarginCaption";
            lblMarginCaption.Size = new Size(48, 15);
            lblMarginCaption.TabIndex = 25;
            lblMarginCaption.Text = "Margin:";
            // 
            // lblComboCaption
            // 
            lblComboCaption.AutoSize = true;
            lblComboCaption.Location = new Point(8, 157);
            lblComboCaption.Name = "lblComboCaption";
            lblComboCaption.Size = new Size(73, 15);
            lblComboCaption.TabIndex = 27;
            lblComboCaption.Text = "Best combo:";
            // 
            // cboProvider
            // 
            cboProvider.DropDownStyle = ComboBoxStyle.DropDownList;
            cboProvider.FormattingEnabled = true;
            cboProvider.Location = new Point(63, 3);
            cboProvider.Name = "cboProvider";
            cboProvider.Size = new Size(183, 23);
            cboProvider.TabIndex = 0;
            // 
            // txtModel
            // 
            txtModel.Location = new Point(338, 3);
            txtModel.Name = "txtModel";
            txtModel.Size = new Size(147, 23);
            txtModel.TabIndex = 1;
            // 
            // txtSystemMessage
            // 
            txtSystemMessage.Location = new Point(62, 30);
            txtSystemMessage.Name = "txtSystemMessage";
            txtSystemMessage.Size = new Size(423, 23);
            txtSystemMessage.TabIndex = 2;
            // 
            // txtPrompt
            // 
            txtPrompt.Location = new Point(61, 57);
            txtPrompt.Name = "txtPrompt";
            txtPrompt.Size = new Size(423, 23);
            txtPrompt.TabIndex = 3;
            // 
            // rtbOutput
            // 
            rtbOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbOutput.BorderStyle = BorderStyle.None;
            rtbOutput.Font = new Font("Consolas", 8.5F);
            rtbOutput.Location = new Point(5, 87);
            rtbOutput.Name = "rtbOutput";
            rtbOutput.Size = new Size(716, 415);
            rtbOutput.TabIndex = 4;
            rtbOutput.Text = "";
            // 
            // btnCreateSession
            // 
            btnCreateSession.Location = new Point(509, 7);
            btnCreateSession.Name = "btnCreateSession";
            btnCreateSession.Size = new Size(75, 23);
            btnCreateSession.TabIndex = 5;
            btnCreateSession.Text = "Create";
            btnCreateSession.UseVisualStyleBackColor = true;
            btnCreateSession.Click += btnCreateSession_Click;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(509, 34);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(75, 23);
            btnSend.TabIndex = 6;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(509, 61);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // lblProvider
            // 
            lblProvider.AutoSize = true;
            lblProvider.Location = new Point(5, 6);
            lblProvider.Name = "lblProvider";
            lblProvider.Size = new Size(51, 15);
            lblProvider.TabIndex = 3;
            lblProvider.Text = "Provider";
            // 
            // lblModel
            // 
            lblModel.AutoSize = true;
            lblModel.Location = new Point(275, 6);
            lblModel.Name = "lblModel";
            lblModel.Size = new Size(41, 15);
            lblModel.TabIndex = 2;
            lblModel.Text = "Model";
            // 
            // lblSystemMessage
            // 
            lblSystemMessage.AutoSize = true;
            lblSystemMessage.Location = new Point(4, 33);
            lblSystemMessage.Name = "lblSystemMessage";
            lblSystemMessage.Size = new Size(45, 15);
            lblSystemMessage.TabIndex = 1;
            lblSystemMessage.Text = "System";
            // 
            // lblPrompt
            // 
            lblPrompt.AutoSize = true;
            lblPrompt.Location = new Point(4, 60);
            lblPrompt.Name = "lblPrompt";
            lblPrompt.Size = new Size(47, 15);
            lblPrompt.TabIndex = 0;
            lblPrompt.Text = "Prompt";
            // 
            // grpBenchmark
            // 
            grpBenchmark.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            grpBenchmark.Controls.Add(lblRunModeCaption);
            grpBenchmark.Controls.Add(cboRunMode);
            grpBenchmark.Controls.Add(lblMaxPromptsLbl);
            grpBenchmark.Controls.Add(nudMaxPrompts);
            grpBenchmark.Controls.Add(lblMaxHint);
            grpBenchmark.Controls.Add(lblAiModeCaption);
            grpBenchmark.Controls.Add(cboAiMode);
            grpBenchmark.Controls.Add(btnRunBenchmark);
            grpBenchmark.Controls.Add(btnCancelBenchmark);
            grpBenchmark.Controls.Add(btnRetestFailed);
            grpBenchmark.Controls.Add(lblBenchmarkStatus);
            grpBenchmark.Controls.Add(pbProgress);
            grpBenchmark.Location = new Point(731, 5);
            grpBenchmark.Name = "grpBenchmark";
            grpBenchmark.Size = new Size(465, 152);
            grpBenchmark.TabIndex = 8;
            grpBenchmark.TabStop = false;
            grpBenchmark.Text = "Benchmark Control";
            // 
            // cboRunMode
            // 
            cboRunMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cboRunMode.FormattingEnabled = true;
            cboRunMode.Items.AddRange(new object[] { "Dataset Benchmark", "FineGrid", "SentenceLog", "Tuning" });
            cboRunMode.Location = new Point(52, 21);
            cboRunMode.Name = "cboRunMode";
            cboRunMode.Size = new Size(175, 23);
            cboRunMode.TabIndex = 1;
            // 
            // nudMaxPrompts
            // 
            nudMaxPrompts.Location = new Point(325, 21);
            nudMaxPrompts.Maximum = new decimal(new int[] { 50000, 0, 0, 0 });
            nudMaxPrompts.Name = "nudMaxPrompts";
            nudMaxPrompts.Size = new Size(70, 23);
            nudMaxPrompts.TabIndex = 3;
            // 
            // lblAiModeCaption
            // 
            lblAiModeCaption.AutoSize = true;
            lblAiModeCaption.Location = new Point(8, 56);
            lblAiModeCaption.Name = "lblAiModeCaption";
            lblAiModeCaption.Size = new Size(62, 15);
            lblAiModeCaption.TabIndex = 5;
            lblAiModeCaption.Text = "AI Inspect:";
            // 
            // cboAiMode
            // 
            cboAiMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cboAiMode.FormattingEnabled = true;
            cboAiMode.Items.AddRange(new object[] { "Off", "Failures only", "All prompts" });
            cboAiMode.Location = new Point(80, 53);
            cboAiMode.Name = "cboAiMode";
            cboAiMode.Size = new Size(140, 23);
            cboAiMode.TabIndex = 6;
            // 
            // btnRunBenchmark
            // 
            btnRunBenchmark.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRunBenchmark.Location = new Point(8, 80);
            btnRunBenchmark.Name = "btnRunBenchmark";
            btnRunBenchmark.Size = new Size(145, 24);
            btnRunBenchmark.TabIndex = 7;
            btnRunBenchmark.Text = "▶  Run Benchmark";
            btnRunBenchmark.UseVisualStyleBackColor = true;
            btnRunBenchmark.Click += btnRunBenchmark_Click;
            // 
            // btnCancelBenchmark
            // 
            btnCancelBenchmark.Enabled = false;
            btnCancelBenchmark.Location = new Point(160, 80);
            btnCancelBenchmark.Name = "btnCancelBenchmark";
            btnCancelBenchmark.Size = new Size(80, 24);
            btnCancelBenchmark.TabIndex = 8;
            btnCancelBenchmark.Text = "■  Stop";
            btnCancelBenchmark.UseVisualStyleBackColor = true;
            btnCancelBenchmark.Click += btnCancelBenchmark_Click;
            // 
            // btnRetestFailed
            // 
            btnRetestFailed.Enabled = false;
            btnRetestFailed.Location = new Point(247, 80);
            btnRetestFailed.Name = "btnRetestFailed";
            btnRetestFailed.Size = new Size(130, 24);
            btnRetestFailed.TabIndex = 9;
            btnRetestFailed.Text = "↻  Re-test Failed";
            btnRetestFailed.UseVisualStyleBackColor = true;
            btnRetestFailed.Click += btnRetestFailed_Click;
            // 
            // lblBenchmarkStatus
            // 
            lblBenchmarkStatus.ForeColor = Color.DimGray;
            lblBenchmarkStatus.Location = new Point(8, 112);
            lblBenchmarkStatus.Name = "lblBenchmarkStatus";
            lblBenchmarkStatus.Size = new Size(449, 15);
            lblBenchmarkStatus.TabIndex = 10;
            // 
            // pbProgress
            // 
            pbProgress.Location = new Point(8, 130);
            pbProgress.Name = "pbProgress";
            pbProgress.Size = new Size(449, 14);
            pbProgress.Style = ProgressBarStyle.Continuous;
            pbProgress.TabIndex = 11;
            // 
            // btnSaveAsDefault
            // 
            btnSaveAsDefault.Location = new Point(308, 220);
            btnSaveAsDefault.Name = "btnSaveAsDefault";
            btnSaveAsDefault.Size = new Size(148, 24);
            btnSaveAsDefault.TabIndex = 32;
            btnSaveAsDefault.Text = "Save as Default";
            btnSaveAsDefault.UseVisualStyleBackColor = true;
            btnSaveAsDefault.Click += btnSaveAsDefault_Click;
            // 
            // grpPipeline
            // 
            grpPipeline.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            grpPipeline.Controls.Add(chkRemoveStopWords);
            grpPipeline.Controls.Add(chkNormalizeSynonyms);
            grpPipeline.Controls.Add(chkExpandContractions);
            grpPipeline.Controls.Add(chkNormalizeLeetspeak);
            grpPipeline.Controls.Add(chkContractExpanded);
            grpPipeline.Controls.Add(chkFilterInvisibleUnicode);
            grpPipeline.Controls.Add(chkNormalizeWhitespace);
            grpPipeline.Controls.Add(chkExtractQuotedContent);
            grpPipeline.Controls.Add(chkLowercaseVariant);
            grpPipeline.Controls.Add(chkDecodeBase64);
            grpPipeline.Controls.Add(chkStripPunctuation);
            grpPipeline.Controls.Add(chkNumbersToWords);
            grpPipeline.Controls.Add(chkRunCombinedVariant);
            grpPipeline.Controls.Add(chkNormalizeHomoglyphs);
            grpPipeline.Controls.Add(btnResetDefaults);
            grpPipeline.Controls.Add(chkFlagSuspectedEncoding);
            grpPipeline.Location = new Point(731, 157);
            grpPipeline.Name = "grpPipeline";
            grpPipeline.Size = new Size(465, 220);
            grpPipeline.TabIndex = 9;
            grpPipeline.TabStop = false;
            grpPipeline.Text = "Pipeline Settings";
            // 
            // chkRemoveStopWords
            // 
            chkRemoveStopWords.AutoSize = true;
            chkRemoveStopWords.Location = new Point(10, 22);
            chkRemoveStopWords.Name = "chkRemoveStopWords";
            chkRemoveStopWords.Size = new Size(133, 19);
            chkRemoveStopWords.TabIndex = 0;
            chkRemoveStopWords.Text = "Remove Stop Words";
            // 
            // chkNormalizeSynonyms
            // 
            chkNormalizeSynonyms.AutoSize = true;
            chkNormalizeSynonyms.Location = new Point(237, 22);
            chkNormalizeSynonyms.Name = "chkNormalizeSynonyms";
            chkNormalizeSynonyms.Size = new Size(138, 19);
            chkNormalizeSynonyms.TabIndex = 1;
            chkNormalizeSynonyms.Text = "Normalize Synonyms";
            // 
            // chkExpandContractions
            // 
            chkExpandContractions.AutoSize = true;
            chkExpandContractions.Location = new Point(10, 45);
            chkExpandContractions.Name = "chkExpandContractions";
            chkExpandContractions.Size = new Size(135, 19);
            chkExpandContractions.TabIndex = 2;
            chkExpandContractions.Text = "Expand Contractions";
            // 
            // chkNormalizeLeetspeak
            // 
            chkNormalizeLeetspeak.AutoSize = true;
            chkNormalizeLeetspeak.Location = new Point(237, 45);
            chkNormalizeLeetspeak.Name = "chkNormalizeLeetspeak";
            chkNormalizeLeetspeak.Size = new Size(135, 19);
            chkNormalizeLeetspeak.TabIndex = 3;
            chkNormalizeLeetspeak.Text = "Normalize Leetspeak";
            // 
            // chkContractExpanded
            // 
            chkContractExpanded.AutoSize = true;
            chkContractExpanded.Location = new Point(10, 68);
            chkContractExpanded.Name = "chkContractExpanded";
            chkContractExpanded.Size = new Size(126, 19);
            chkContractExpanded.TabIndex = 4;
            chkContractExpanded.Text = "Contract Expanded";
            // 
            // chkFilterInvisibleUnicode
            // 
            chkFilterInvisibleUnicode.AutoSize = true;
            chkFilterInvisibleUnicode.Location = new Point(237, 68);
            chkFilterInvisibleUnicode.Name = "chkFilterInvisibleUnicode";
            chkFilterInvisibleUnicode.Size = new Size(145, 19);
            chkFilterInvisibleUnicode.TabIndex = 5;
            chkFilterInvisibleUnicode.Text = "Filter Invisible Unicode";
            // 
            // chkNormalizeWhitespace
            // 
            chkNormalizeWhitespace.AutoSize = true;
            chkNormalizeWhitespace.Location = new Point(10, 91);
            chkNormalizeWhitespace.Name = "chkNormalizeWhitespace";
            chkNormalizeWhitespace.Size = new Size(144, 19);
            chkNormalizeWhitespace.TabIndex = 6;
            chkNormalizeWhitespace.Text = "Normalize Whitespace";
            // 
            // chkExtractQuotedContent
            // 
            chkExtractQuotedContent.AutoSize = true;
            chkExtractQuotedContent.Location = new Point(237, 91);
            chkExtractQuotedContent.Name = "chkExtractQuotedContent";
            chkExtractQuotedContent.Size = new Size(150, 19);
            chkExtractQuotedContent.TabIndex = 7;
            chkExtractQuotedContent.Text = "Extract Quoted Content";
            // 
            // chkLowercaseVariant
            // 
            chkLowercaseVariant.AutoSize = true;
            chkLowercaseVariant.Location = new Point(10, 114);
            chkLowercaseVariant.Name = "chkLowercaseVariant";
            chkLowercaseVariant.Size = new Size(120, 19);
            chkLowercaseVariant.TabIndex = 8;
            chkLowercaseVariant.Text = "Lowercase Variant";
            // 
            // chkDecodeBase64
            // 
            chkDecodeBase64.AutoSize = true;
            chkDecodeBase64.Location = new Point(237, 114);
            chkDecodeBase64.Name = "chkDecodeBase64";
            chkDecodeBase64.Size = new Size(105, 19);
            chkDecodeBase64.TabIndex = 9;
            chkDecodeBase64.Text = "Decode Base64";
            // 
            // chkStripPunctuation
            // 
            chkStripPunctuation.AutoSize = true;
            chkStripPunctuation.Location = new Point(10, 137);
            chkStripPunctuation.Name = "chkStripPunctuation";
            chkStripPunctuation.Size = new Size(118, 19);
            chkStripPunctuation.TabIndex = 10;
            chkStripPunctuation.Text = "Strip Punctuation";
            // 
            // chkNumbersToWords
            // 
            chkNumbersToWords.AutoSize = true;
            chkNumbersToWords.Location = new Point(237, 137);
            chkNumbersToWords.Name = "chkNumbersToWords";
            chkNumbersToWords.Size = new Size(126, 19);
            chkNumbersToWords.TabIndex = 11;
            chkNumbersToWords.Text = "Numbers to Words";
            // 
            // chkRunCombinedVariant
            // 
            chkRunCombinedVariant.AutoSize = true;
            chkRunCombinedVariant.Location = new Point(10, 160);
            chkRunCombinedVariant.Name = "chkRunCombinedVariant";
            chkRunCombinedVariant.Size = new Size(145, 19);
            chkRunCombinedVariant.TabIndex = 12;
            chkRunCombinedVariant.Text = "Run Combined Variant";
            // 
            // chkNormalizeHomoglyphs
            // 
            chkNormalizeHomoglyphs.AutoSize = true;
            chkNormalizeHomoglyphs.Location = new Point(237, 160);
            chkNormalizeHomoglyphs.Name = "chkNormalizeHomoglyphs";
            chkNormalizeHomoglyphs.Size = new Size(152, 19);
            chkNormalizeHomoglyphs.TabIndex = 13;
            chkNormalizeHomoglyphs.Text = "Normalize Homoglyphs";
            // 
            // btnResetDefaults
            // 
            btnResetDefaults.Location = new Point(10, 190);
            btnResetDefaults.Name = "btnResetDefaults";
            btnResetDefaults.Size = new Size(120, 22);
            btnResetDefaults.TabIndex = 14;
            btnResetDefaults.Text = "Reset Defaults";
            btnResetDefaults.UseVisualStyleBackColor = true;
            btnResetDefaults.Click += btnResetDefaults_Click;
            // 
            // chkFlagSuspectedEncoding
            // 
            chkFlagSuspectedEncoding.AutoSize = true;
            chkFlagSuspectedEncoding.Location = new Point(237, 190);
            chkFlagSuspectedEncoding.Name = "chkFlagSuspectedEncoding";
            chkFlagSuspectedEncoding.Size = new Size(158, 19);
            chkFlagSuspectedEncoding.TabIndex = 15;
            chkFlagSuspectedEncoding.Text = "Flag Suspected Encoding";
            // 
            // grpResolution
            // 
            grpResolution.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            grpResolution.Controls.Add(lblResCaption);
            grpResolution.Controls.Add(cboResolution);
            grpResolution.Location = new Point(731, 370);
            grpResolution.Name = "grpResolution";
            grpResolution.Size = new Size(465, 50);
            grpResolution.TabIndex = 10;
            grpResolution.TabStop = false;
            grpResolution.Text = "Tuning Resolution";
            // 
            // cboResolution
            // 
            cboResolution.DropDownStyle = ComboBoxStyle.DropDownList;
            cboResolution.FormattingEnabled = true;
            cboResolution.Items.AddRange(new object[] { "TwoPass", "Fast", "Balanced", "Full" });
            cboResolution.Location = new Point(130, 16);
            cboResolution.Name = "cboResolution";
            cboResolution.Size = new Size(140, 23);
            cboResolution.TabIndex = 1;
            // 
            // grpWeights
            // 
            grpWeights.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            grpWeights.Controls.Add(lblWeightsInfo);
            grpWeights.Controls.Add(lblTCaption);
            grpWeights.Controls.Add(nudThreshold);
            grpWeights.Controls.Add(lblUBCaption);
            grpWeights.Controls.Add(nudUncertaintyBand);
            grpWeights.Controls.Add(lblAiWCaption);
            grpWeights.Controls.Add(nudAiWeight);
            grpWeights.Controls.Add(lblDWCaption);
            grpWeights.Controls.Add(nudDriftWeight);
            grpWeights.Controls.Add(lblIWCaption);
            grpWeights.Controls.Add(nudIntentWeight);
            grpWeights.Controls.Add(lblKWCaption);
            grpWeights.Controls.Add(lblKeywordWeightVal);
            grpWeights.Controls.Add(lblMDCaption);
            grpWeights.Controls.Add(nudMaxDriftWeight);
            grpWeights.Controls.Add(lblADCaption);
            grpWeights.Controls.Add(nudAvgDriftWeight);
            grpWeights.Controls.Add(lblSDCaption);
            grpWeights.Controls.Add(nudStdDevWeight);
            grpWeights.Controls.Add(lblAccCaption);
            grpWeights.Controls.Add(lblLastRunAccuracy);
            grpWeights.Controls.Add(lblTprCaption);
            grpWeights.Controls.Add(lblLastRunTpr);
            grpWeights.Controls.Add(lblFprCaption);
            grpWeights.Controls.Add(lblLastRunFpr);
            grpWeights.Controls.Add(lblMarginCaption);
            grpWeights.Controls.Add(lblLastRunMargin);
            grpWeights.Controls.Add(lblComboCaption);
            grpWeights.Controls.Add(lblLastRunCombo);
            grpWeights.Controls.Add(chkOverrideWeights);
            grpWeights.Controls.Add(btnCopyWeights);
            grpWeights.Controls.Add(btnExportResults);
            grpWeights.Controls.Add(btnSaveAsDefault);
            grpWeights.Location = new Point(731, 425);
            grpWeights.Name = "grpWeights";
            grpWeights.Size = new Size(465, 258);
            grpWeights.TabIndex = 11;
            grpWeights.TabStop = false;
            grpWeights.Text = "Weights & Thresholds";
            // 
            // lblWeightsInfo
            // 
            lblWeightsInfo.ForeColor = SystemColors.GrayText;
            lblWeightsInfo.Location = new Point(8, 18);
            lblWeightsInfo.Name = "lblWeightsInfo";
            lblWeightsInfo.Size = new Size(440, 15);
            lblWeightsInfo.TabIndex = 0;
            lblWeightsInfo.Text = "(populated after each run — auto-tune result)";
            // 
            // nudThreshold
            // 
            nudThreshold.DecimalPlaces = 4;
            nudThreshold.Increment = new decimal(new int[] { 1, 0, 0, 196608 });
            nudThreshold.Location = new Point(76, 40);
            nudThreshold.Maximum = new decimal(new int[] { 20000, 0, 0, 262144 });
            nudThreshold.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            nudThreshold.Name = "nudThreshold";
            nudThreshold.Size = new Size(82, 23);
            nudThreshold.TabIndex = 2;
            nudThreshold.Value = new decimal(new int[] { 200, 0, 0, 262144 });
            // 
            // nudUncertaintyBand
            // 
            nudUncertaintyBand.DecimalPlaces = 2;
            nudUncertaintyBand.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudUncertaintyBand.Location = new Point(252, 40);
            nudUncertaintyBand.Maximum = new decimal(new int[] { 100, 0, 0, 131072 });
            nudUncertaintyBand.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            nudUncertaintyBand.Name = "nudUncertaintyBand";
            nudUncertaintyBand.Size = new Size(65, 23);
            nudUncertaintyBand.TabIndex = 4;
            nudUncertaintyBand.Value = new decimal(new int[] { 10, 0, 0, 131072 });
            // 
            // lblAiWCaption
            // 
            lblAiWCaption.AutoSize = true;
            lblAiWCaption.Location = new Point(327, 43);
            lblAiWCaption.Name = "lblAiWCaption";
            lblAiWCaption.Size = new Size(35, 15);
            lblAiWCaption.TabIndex = 5;
            lblAiWCaption.Text = "AI W:";
            // 
            // nudAiWeight
            // 
            nudAiWeight.DecimalPlaces = 3;
            nudAiWeight.Increment = new decimal(new int[] { 5, 0, 0, 196608 });
            nudAiWeight.Location = new Point(360, 40);
            nudAiWeight.Maximum = new decimal(new int[] { 1000, 0, 0, 196608 });
            nudAiWeight.Name = "nudAiWeight";
            nudAiWeight.Size = new Size(72, 23);
            nudAiWeight.TabIndex = 6;
            nudAiWeight.Value = new decimal(new int[] { 50, 0, 0, 196608 });
            // 
            // nudDriftWeight
            // 
            nudDriftWeight.DecimalPlaces = 2;
            nudDriftWeight.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudDriftWeight.Location = new Point(60, 69);
            nudDriftWeight.Maximum = new decimal(new int[] { 100, 0, 0, 131072 });
            nudDriftWeight.Name = "nudDriftWeight";
            nudDriftWeight.Size = new Size(65, 23);
            nudDriftWeight.TabIndex = 8;
            nudDriftWeight.Value = new decimal(new int[] { 38, 0, 0, 131072 });
            // 
            // nudIntentWeight
            // 
            nudIntentWeight.DecimalPlaces = 2;
            nudIntentWeight.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudIntentWeight.Location = new Point(192, 69);
            nudIntentWeight.Maximum = new decimal(new int[] { 100, 0, 0, 131072 });
            nudIntentWeight.Name = "nudIntentWeight";
            nudIntentWeight.Size = new Size(65, 23);
            nudIntentWeight.TabIndex = 10;
            // 
            // lblKeywordWeightVal
            // 
            lblKeywordWeightVal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblKeywordWeightVal.ForeColor = Color.SteelBlue;
            lblKeywordWeightVal.Location = new Point(348, 72);
            lblKeywordWeightVal.Name = "lblKeywordWeightVal";
            lblKeywordWeightVal.Size = new Size(52, 15);
            lblKeywordWeightVal.TabIndex = 12;
            lblKeywordWeightVal.Text = "–";
            // 
            // nudMaxDriftWeight
            // 
            nudMaxDriftWeight.DecimalPlaces = 2;
            nudMaxDriftWeight.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudMaxDriftWeight.Location = new Point(68, 98);
            nudMaxDriftWeight.Maximum = new decimal(new int[] { 100, 0, 0, 131072 });
            nudMaxDriftWeight.Name = "nudMaxDriftWeight";
            nudMaxDriftWeight.Size = new Size(65, 23);
            nudMaxDriftWeight.TabIndex = 14;
            nudMaxDriftWeight.Value = new decimal(new int[] { 90, 0, 0, 131072 });
            // 
            // nudAvgDriftWeight
            // 
            nudAvgDriftWeight.DecimalPlaces = 2;
            nudAvgDriftWeight.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudAvgDriftWeight.Location = new Point(200, 98);
            nudAvgDriftWeight.Maximum = new decimal(new int[] { 100, 0, 0, 131072 });
            nudAvgDriftWeight.Name = "nudAvgDriftWeight";
            nudAvgDriftWeight.Size = new Size(65, 23);
            nudAvgDriftWeight.TabIndex = 16;
            nudAvgDriftWeight.Value = new decimal(new int[] { 7, 0, 0, 131072 });
            // 
            // nudStdDevWeight
            // 
            nudStdDevWeight.DecimalPlaces = 2;
            nudStdDevWeight.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudStdDevWeight.Location = new Point(328, 98);
            nudStdDevWeight.Maximum = new decimal(new int[] { 100, 0, 0, 131072 });
            nudStdDevWeight.Name = "nudStdDevWeight";
            nudStdDevWeight.Size = new Size(65, 23);
            nudStdDevWeight.TabIndex = 18;
            nudStdDevWeight.Value = new decimal(new int[] { 3, 0, 0, 131072 });
            // 
            // lblLastRunAccuracy
            // 
            lblLastRunAccuracy.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblLastRunAccuracy.ForeColor = Color.Green;
            lblLastRunAccuracy.Location = new Point(72, 133);
            lblLastRunAccuracy.Name = "lblLastRunAccuracy";
            lblLastRunAccuracy.Size = new Size(55, 15);
            lblLastRunAccuracy.TabIndex = 20;
            lblLastRunAccuracy.Text = "–";
            // 
            // lblLastRunTpr
            // 
            lblLastRunTpr.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblLastRunTpr.ForeColor = Color.Green;
            lblLastRunTpr.Location = new Point(163, 133);
            lblLastRunTpr.Name = "lblLastRunTpr";
            lblLastRunTpr.Size = new Size(50, 15);
            lblLastRunTpr.TabIndex = 22;
            lblLastRunTpr.Text = "–";
            // 
            // lblLastRunFpr
            // 
            lblLastRunFpr.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblLastRunFpr.ForeColor = Color.Firebrick;
            lblLastRunFpr.Location = new Point(248, 133);
            lblLastRunFpr.Name = "lblLastRunFpr";
            lblLastRunFpr.Size = new Size(50, 15);
            lblLastRunFpr.TabIndex = 24;
            lblLastRunFpr.Text = "–";
            // 
            // lblLastRunMargin
            // 
            lblLastRunMargin.Location = new Point(353, 133);
            lblLastRunMargin.Name = "lblLastRunMargin";
            lblLastRunMargin.Size = new Size(70, 15);
            lblLastRunMargin.TabIndex = 26;
            lblLastRunMargin.Text = "–";
            // 
            // lblLastRunCombo
            // 
            lblLastRunCombo.Font = new Font("Consolas", 8.5F);
            lblLastRunCombo.Location = new Point(82, 154);
            lblLastRunCombo.Name = "lblLastRunCombo";
            lblLastRunCombo.Size = new Size(375, 30);
            lblLastRunCombo.TabIndex = 28;
            lblLastRunCombo.Text = "–";
            // 
            // chkOverrideWeights
            // 
            chkOverrideWeights.AutoSize = true;
            chkOverrideWeights.Location = new Point(8, 194);
            chkOverrideWeights.Name = "chkOverrideWeights";
            chkOverrideWeights.Size = new Size(276, 19);
            chkOverrideWeights.TabIndex = 29;
            chkOverrideWeights.Text = "Override threshold for re-classification after run";
            chkOverrideWeights.CheckedChanged += chkOverrideWeights_CheckedChanged;
            // 
            // btnCopyWeights
            // 
            btnCopyWeights.Location = new Point(8, 220);
            btnCopyWeights.Name = "btnCopyWeights";
            btnCopyWeights.Size = new Size(145, 24);
            btnCopyWeights.TabIndex = 30;
            btnCopyWeights.Text = "Copy Weights";
            btnCopyWeights.UseVisualStyleBackColor = true;
            btnCopyWeights.Click += btnCopyWeights_Click;
            // 
            // btnExportResults
            // 
            btnExportResults.Location = new Point(160, 220);
            btnExportResults.Name = "btnExportResults";
            btnExportResults.Size = new Size(140, 24);
            btnExportResults.TabIndex = 31;
            btnExportResults.Text = "Export Results";
            btnExportResults.UseVisualStyleBackColor = true;
            btnExportResults.Click += btnExportResults_Click;
            // 
            // button1
            // 
            button1.Location = new Point(590, 21);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 12;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1206, 532);
            Controls.Add(button1);
            Controls.Add(lblPrompt);
            Controls.Add(lblSystemMessage);
            Controls.Add(lblModel);
            Controls.Add(lblProvider);
            Controls.Add(btnCancel);
            Controls.Add(btnSend);
            Controls.Add(btnCreateSession);
            Controls.Add(rtbOutput);
            Controls.Add(txtPrompt);
            Controls.Add(txtSystemMessage);
            Controls.Add(txtModel);
            Controls.Add(cboProvider);
            Controls.Add(grpBenchmark);
            Controls.Add(grpPipeline);
            Controls.Add(grpResolution);
            Controls.Add(grpWeights);
            Name = "Form1";
            Text = "Injection Detect — Benchmark Harness";
            Load += Form1_Load;
            grpBenchmark.ResumeLayout(false);
            grpBenchmark.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudMaxPrompts).EndInit();
            grpPipeline.ResumeLayout(false);
            grpPipeline.PerformLayout();
            grpResolution.ResumeLayout(false);
            grpResolution.PerformLayout();
            grpWeights.ResumeLayout(false);
            grpWeights.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudThreshold).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudUncertaintyBand).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudAiWeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDriftWeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudIntentWeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxDriftWeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudAvgDriftWeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudStdDevWeight).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblRunModeCaption;
        private Label lblMaxPromptsLbl;
        private Label lblMaxHint;
        private Label lblResCaption;
        private Label lblTCaption;
        private Label lblUBCaption;
        private Label lblDWCaption;
        private Label lblIWCaption;
        private Label lblKWCaption;
        private Label lblMDCaption;
        private Label lblADCaption;
        private Label lblSDCaption;
        private Label lblAccCaption;
        private Label lblTprCaption;
        private Label lblFprCaption;
        private Label lblMarginCaption;
        private Label lblComboCaption;
        private Label lblAiModeCaption;
        private Label lblAiWCaption;
        private Button button1;
    }
}
