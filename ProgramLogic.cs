using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace EPW_Recaster
{
    public partial class MainGui
    {
        #region Variables | Properties.

        // For logging purposes.
        private Bitmap img;

        // To be used by Tesseract (OCR).
        private Bitmap upscaledImg;

        private string upscaledImgPath = Tesseract.Ocr.TempCacheDirectory + @"\tmp.png";

        private int CaptureRegionWidth { get; set; } = 0;
        private int CaptureRegionHeight { get; set; } = 0;

        private Point CaptureRegionUpperLeftPoint { get; set; } = new Point();

        private Point RetainClickPoint { get; set; } = new Point();

        private Point NewClickPoint { get; set; } = new Point();

        private Point ReproduceClickPoint { get; set; } = new Point();

        private int AwaitIngameReproduceButtonAvailable { get; set; } = 1750; // Time it takes for the in-game reproduce button to become available again.
        private int AwaitIngameStatsRolled { get; set; } = 1250; // Time it takes for the in-game stats to be rolled.
        private int AwaitAcceptRejectAction { get; set; } = 1750; // Time to wait before accepting/rejecting a roll.

        #endregion Variables | Properties.

        #region Methods.

        private System.Drawing.Bitmap CaptureRegion()
        {
            Bitmap tmpBmp = new Bitmap(CaptureRegionWidth, (int)Math.Round(CaptureRegionHeight * CaptureRegionHeightClipping));
            Graphics g;
            g = Graphics.FromImage(tmpBmp);
            g.CopyFromScreen(CaptureRegionUpperLeftPoint, new Point(0, 0), new Size(CaptureRegionWidth, (int)Math.Round(CaptureRegionHeight * CaptureRegionHeightClipping)));
            return (System.Drawing.Bitmap)tmpBmp;
        }

        internal void PrepareOcrLogic()
        {
            // Clear info text.
            AddMsg();

            // Set default color (in case needed in code later on).
            DefaultColor = InfoGui.rTxtBoxInfo.SelectionColor;

            if (!Directory.Exists(Tesseract.Ocr.AssemblyCodeBaseDirectory + @"\Logged"))
            {
                Directory.CreateDirectory(Tesseract.Ocr.AssemblyCodeBaseDirectory + @"\Logged");
            }

            if (!Directory.Exists(Tesseract.Ocr.TempCacheDirectory))
            {
                Directory.CreateDirectory(Tesseract.Ocr.TempCacheDirectory);
            }

            // Set the capture region data before minimizing form.
            Size size = seeThroughRegion.ClientSize;

            if (!InfoGui.PreviewCapture)
            {
                // Non-Preview Capture : Right Half Capture only.
                CaptureRegionWidth = size.Width / 2;
                CaptureRegionHeight = size.Height;
                CaptureRegionUpperLeftPoint = seeThroughRegion.PointToScreen(new Point(size.Width / 2, 0));
            }
            else
            {
                // Preview Capture : Full Width Capture.
                CaptureRegionWidth = size.Width;
                CaptureRegionHeight = size.Height;
                CaptureRegionUpperLeftPoint = seeThroughRegion.PointToScreen(new Point(0, 0));
            }

            RetainClickPoint = btnRetain.PointToScreen(
                new Point(
                    (int)Math.Round(0.50F * btnRetain.Width),
                    (int)Math.Round(0.50F * btnRetain.Height)
                    )
                );

            NewClickPoint = btnNew.PointToScreen(
                new Point(
                    (int)Math.Round(0.50F * btnNew.Width),
                    (int)Math.Round(0.50F * btnNew.Height)
                    )
                );

            ReproduceClickPoint = btnReproduce.PointToScreen(
                new Point(
                    (int)Math.Round(0.50F * btnReproduce.Width),
                    (int)Math.Round(0.50F * btnReproduce.Height)
                    )
                );
            // ---

            this.WindowState = FormWindowState.Minimized;
            InfoGui.WindowState = FormWindowState.Normal;

            SetParamsFromCfg();
        }

        private void SetParamsFromCfg()
        {
            if (File.Exists(Tesseract.Ocr.AssemblyCodeBaseDirectory + @"\Config\Params.cfg"))
            {
                // Load cfg file containing specific parameters.
                string[] parameters = File.ReadAllLines(Tesseract.Ocr.AssemblyCodeBaseDirectory + @"\Config\Params.cfg");
                foreach (string line in parameters)
                {
                    if (!line.Contains('#')) // Ignore custom comments.
                    {
                        // Split by pipe character '|'.
                        string[] split = line.Split('|');

                        if (split.Count() == 2)
                        {
                            // Await In-Game Reproduce Button Available.
                            if (split[0].ToLower().Contains("Await In-Game Reproduce".ToLower()))
                            {
                                if (Int32.TryParse(Regex.Match(split[1].Trim(), @"\d+").Value, out int cfgValue))
                                {
                                    this.AwaitIngameReproduceButtonAvailable = cfgValue;
                                }
                            }

                            // Await In-Game Stats Rolled.
                            if (split[0].ToLower().Contains("Await In-Game Stats".ToLower()))
                            {
                                if (Int32.TryParse(Regex.Match(split[1].Trim(), @"\d+").Value, out int cfgValue))
                                {
                                    this.AwaitIngameStatsRolled = cfgValue;
                                }
                            }

                            // Await Accept/Reject Action Time.
                            if (split[0].ToLower().Contains("Await Accept/Reject".ToLower()))
                            {
                                if (Int32.TryParse(Regex.Match(split[1].Trim(), @"\d+").Value, out int cfgValue))
                                {
                                    this.AwaitAcceptRejectAction = cfgValue;
                                }
                            }
                        }
                    }
                }
            }

            SetLanguageFromCfg();
        }

        private void SetLanguageFromCfg()
        {
            string langCfgPath = Tesseract.Ocr.AssemblyCodeBaseDirectory + @"\Config\Language.cfg";

            if (File.Exists(langCfgPath))
            {
                string[] lines = File.ReadAllLines(langCfgPath);
                foreach (string line in lines)
                {
                    if (!line.Contains('#')) // Ignore custom comments.
                    {
                        string[] split = line.Split('|');
                        if (split.Count() == 2 && split[0].ToLower().Contains("language"))
                        {
                            Tesseract.Ocr.Language = split[1].Trim().ToLower();
                        }
                    }
                }
            }

            EnsureTrainedData(Tesseract.Ocr.Language);
        }

        private void SaveLanguageToCfg(string language)
        {
            string langCfgPath = Tesseract.Ocr.AssemblyCodeBaseDirectory + @"\Config\Language.cfg";
            string[] contents = {
                "# =================================================================",
                "# OCR language for Tesseract",
                "# Supported options:",
                "#   por - Portuguese (Brazil) (default)",
                "#   eng - English (US)",
                "# =================================================================",
                $"Language | {language}"
            };

            Directory.CreateDirectory(Path.GetDirectoryName(langCfgPath));
            File.WriteAllLines(langCfgPath, contents);
        }

        private void ToggleLanguage()
        {
            string newLang = Tesseract.Ocr.Language == "eng" ? "por" : "eng";
            Tesseract.Ocr.Language = newLang;
            EnsureTrainedData(newLang);
            SaveLanguageToCfg(newLang);
            btnSwitchLanguage.Text = newLang == "eng" ? "ENUS" : "PTBR";
        }

        private void EnsureTrainedData(string language)
        {
            string tessdataDir = Path.Combine(Tesseract.Ocr.AssemblyCodeBaseDirectory, Tesseract.Ocr.VersionPath, "tessdata");
            string trainedDataPath = Path.Combine(tessdataDir, $"{language}.traineddata");

            if (File.Exists(trainedDataPath))
            {
                return;
            }

            Directory.CreateDirectory(tessdataDir);
            string url = $"https://github.com/tesseract-ocr/tessdata/raw/main/{language}.traineddata";

            using (var httpClient = new HttpClient())
            using (var response = httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result)
            {
                response.EnsureSuccessStatusCode();
                using (var stream = response.Content.ReadAsStreamAsync().Result)
                using (var fileStream = File.Create(trainedDataPath))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        internal void DoOcrLogic()
        {
            // ===========
            // == SETUP ==
            // ===========

            #region Initial roll setup.

            AddMsg(); // An empty msg clears (rich) text box.

            bool keepRunning = true;

            int nrRolls = 0;
            int maxNrRolls = (int)InfoGui.numMaxRolls.Value;

            bool uniqueStatWarningShown = false;

            // History of captured text in order to stop after x times same result.
            int capturedTextHistoryCapacity = 3; // Store/Compare X times.
            Queue<string> capturedTextHistory = new Queue<string>(capturedTextHistoryCapacity);

            //Thread.Sleep(500);

            // [DEVNOTE] Define here so a new folder won't be created each minute (at start only).
            string logPathWithoutExtension = Tesseract.Ocr.AssemblyCodeBaseDirectory + @"\Logged\" + DateTime.Now.ToString("yyyy-dd-MM") + " (Started " + DateTime.Now.ToString("HH-mm") + ")";

            // Get conditions for this preview or batch roll.
            List<Condition> conditions = new List<Condition>();

            // Get conditions for this preview or batch roll.
            List<ConditionListEntry> conditionListEntries = new List<ConditionListEntry>();

            if (dgConditions.InvokeRequired)
            {
                dgConditions.Invoke(new MethodInvoker(delegate
                {
                    foreach (DataGridViewRow row in dgConditions.Rows)
                    {
                        conditionListEntries.Add((ConditionListEntry)row.Tag);
                    }
                }));
            }
            else
            {
                foreach (DataGridViewRow row in dgConditions.Rows)
                {
                    conditionListEntries.Add((ConditionListEntry)row.Tag);
                }
            }

            #endregion Initial roll setup.

            // ===============
            // == ROLL LOOP ==
            // ===============

            #region Main roll loop.

            while (keepRunning)
            {
                // ========================
                // == CURRENT ROLL SETUP ==
                // ========================

                #region Current roll setup.

                Ocr_Token.ThrowIfCancellationRequested();

                img = CaptureRegion();
                upscaledImg = (System.Drawing.Bitmap)ResizeImage(img, scaleFactor: 1.5);

                string imgPath;
                string outputPath;

                Equipment currEquipment = new Equipment();

                bool validEntry = false;

                if (!InfoGui.PreviewCapture)
                {
                    string currTimeStamp = DateTime.Now.ToString("yyyy-dd-MM") + " (" + DateTime.Now.ToString("HH-mm-ss") + ")";

                    imgPath = logPathWithoutExtension + @"\" + currTimeStamp + ".png";
                    outputPath = logPathWithoutExtension + @"\" + currTimeStamp; // [DEVNOTE] Tesseract does not want .txt or any extension to be added to outputPath.
                }
                else
                {
                    // Overwrite logPathWithoutExtension.
                    logPathWithoutExtension = Tesseract.Ocr.AssemblyCodeBaseDirectory + @"\Logged";

                    imgPath = logPathWithoutExtension + @"\" + "Preview Capture.png";
                    outputPath = logPathWithoutExtension + @"\" + "Preview Capture"; // [DEVNOTE] Tesseract does not want .txt or any extension to be added to outputPath.
                }

                if (!Directory.Exists(logPathWithoutExtension))
                {
                    Directory.CreateDirectory(logPathWithoutExtension);
                }

                img.Save(imgPath, System.Drawing.Imaging.ImageFormat.Png);
                upscaledImg.Save(upscaledImgPath, System.Drawing.Imaging.ImageFormat.Png);

                string rawCapturedText = Tesseract.Ocr.GetText(
                    /*imgPath: imgPath,*/
                    imgPath: upscaledImgPath,
                    outputPath: outputPath
                    );

                currEquipment.OcrText = rawCapturedText;

                #region Queue handling.

                // Add most recent capture.
                capturedTextHistory.Enqueue(currEquipment.OcrText);

                // Remove oldest captured text if history extends predefined capacity.
                if (capturedTextHistory.Count > capturedTextHistoryCapacity)
                {
                    capturedTextHistory.Dequeue();
                }

                #endregion Queue handling.

                #region (Temporarily) Cache the config short terms.

                List<string[]> cfgTerms = new List<string[]>();

                // Load cfg file containing terms and temp cache/store all terms found.
                foreach (string term in File.ReadAllLines(Tesseract.Ocr.AssemblyCodeBaseDirectory + @"\Config\Stats.cfg"))
                {
                    if (!term.Contains('#')) // Ignore custom comments.
                    {
                        // Split by pipe character '|'.
                        string[] splitTerm = term.Split('|');

                        if (splitTerm.Count() == 2)
                        {
                            cfgTerms.Add(new string[] { splitTerm[0].Trim(), splitTerm[1].Trim() }); // splitTerm[0] = long term | splitTerm[1] = short term
                        }
                    }
                }

                #endregion (Temporarily) Cache the config short terms.

                // Display in info box.
                AddMsg();

                #endregion Current roll setup.

                // ===================
                // == VALIDATE ROLL ==
                // ===================

                #region Validate roll.

                if (!String.IsNullOrEmpty(currEquipment.OcrText))
                {
                    if (currEquipment.OcrText.Count() >= 20) // Consider as containing stats when char count > 20.
                    {
                        try
                        {
                            //AddMsg(currEquipment.OcrText);

                            List<Stat> blueStats = currEquipment.BlueStats;

                            if (blueStats.Count() >= 4)
                            {
                                if (currEquipment.IsWeapon)
                                    AddMsg(new RtMessage("[ Estatísticas Azuis de Arma Detectadas ]", bold: true));
                                else
                                    AddMsg(new RtMessage("[ Estatísticas Azuis de Equipamento Detectadas ]", bold: true));

                                foreach (Stat blueStat in blueStats)
                                {
                                    AddMsg(new RtMessage("⮩ " + blueStat.FormattedStat, color: BlueStatColor, indent: 3));

                                    if (!uniqueStatWarningShown)
                                    {
                                        // Check if the stat contains 'Purify'...
                                        // If it does, give the user a warning about missing stat(s).
                                        if (blueStat.Identifier.Contains("Purify"))
                                        {
                                            DialogResult userChoice = MessageBox.Show(
                                                "Uma estatística 'Purify' foi detectada.\r\n" +
                                                "\r\n" +
                                                "Sem qualquer alteração nos arquivos do jogo (configs.pck),\r\n" +
                                                "isso resultará em avaliação e tratamento incorretos das condições " +
                                                "sempre que essa estatística for rolada, pois a janela do jogo precisa ser rolada " +
                                                "para que todas as estatísticas sejam legíveis (fora do escopo para " + Application.ProductName + ").\r\n" +
                                                "\r\n" +
                                                "======\r\n" +
                                                "Em resumo:\r\n" +
                                                "======\r\n" +
                                                "Se estatísticas de descrição longa não forem corrigidas,\r\nalgumas rolagens podem deixar de aceitar uma rolagem possivelmente válida.\r\n" +
                                                "\r\n" +
                                                "Continuar rolando?",
                                                "[ AVISO IMPORTANTE ]",
                                                MessageBoxButtons.OKCancel,
                                                MessageBoxIcon.Warning
                                                );

                                            uniqueStatWarningShown = true; // Only show once.

                                            if (userChoice == DialogResult.Cancel)
                                            {
                                                Ocr_CancellationTokenSource.Cancel();
                                            }
                                        }
                                    }
                                }

                                AddMsg("=========================");

                                validEntry = true;

                                // Increase roll counter if the current capture was a valid one.
                                nrRolls++;
                            }
                            else
                            {
                                AddMsg(new RtMessage("[ Equipamento Não Identificável ]", color: RedLightColor, bold: true));
                                AddMsg(
                                    "  => Esta rolagem não será avaliada/tratada."
                                    );
                            }
                        }
                        catch/*(Exception e)*/
                        {
                            //MessageBox.Show(e.ToString());
                            AddMsg(currEquipment.OcrText);
                        }
                    }
                    else
                    {
                        AddMsg("Nenhuma informação de rolagem válida detectada (ainda).");
                    }
                }
                else
                {
                    //AddMsg("No text found in region.");
                    AddMsg("Nenhuma informação de rolagem válida detectada (ainda).");
                }

                #endregion Validate roll.

                // ======================
                // == JUDGE CONDITIONS ==
                // ======================

                #region Judge conditions.

                // Check history first, stop if needed.
                // [DEVNOTE] Only check when history is filled entirely.
                if (!InfoGui.PreviewCapture & capturedTextHistory.Count == capturedTextHistoryCapacity && capturedTextHistory.Distinct().Count() == 1)
                {
                    AddMsg(); // Clear info box first.

                    AddMsg(new RtMessage("O processo de rolagem foi interrompido.", bold: true));
                    AddMsg("( Não parece necessário rolar mais. )" + Environment.NewLine);
                    AddMsg("Ou:" + Environment.NewLine +
                        "- o número de Perfect Elements no jogo" + Environment.NewLine +
                        "  foi esgotado" + Environment.NewLine +
                        "- as condições já foram" + Environment.NewLine +
                        "  atendidas e aceitas" + Environment.NewLine +
                        "- o cliente do jogo desconectou" + Environment.NewLine +
                        "- a ferramenta não conseguiu ler corretamente as estatísticas roladas" + Environment.NewLine +
                        "  > verifique os limites da região de captura" + Environment.NewLine +
                        "  > verifique se a ferramenta está sobrepondo" + Environment.NewLine +
                        "    o cliente do jogo");
                    AddMsg("=========================");

                    string doneSound = Tesseract.Ocr.AssemblyCodeBaseDirectory + @"\Media\Sounds\Done.wav";
                    if (File.Exists(doneSound)) // !InfoGui.PreviewCapture already checked above.
                    {
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(doneSound);
                        player.Play();
                    }

                    //return; // [DEVNOTE] Needs to reach #Rolls display. Instead:
                    keepRunning = false;
                }

                // ~ else continue checking ...

                bool conditionMet = false;

                if (validEntry)
                {
                    #region Match Condition(s).

                    for (int i = 0; i < conditionListEntries.Count(); i++)
                    {
                        //AddMsg("=========================");
                        AddMsg(new RtMessage("[ Verificando Entrada da Lista de Condições " + (i + 1) + " ]", bold: true));

                        conditionMet = true;

                        // =====================
                        // A. Fixed Amount Mode.
                        // =====================

                        #region Fixed Amount Mode.

                        foreach (Condition currCondition in conditionListEntries[i])
                        {
                            if (currCondition.Amount > 0)
                            {
                                int hits = currEquipment.MatchStat(currCondition.ShortTerm);

                                AddMsg(new RtMessage(
                                    message:
                                    (hits >= Convert.ToInt32(currCondition.Amount) ? "✓" : "❌") + " '" + currCondition.LongTerm + "': " + hits + " / " + currCondition.Amount,
                                    color:
                                    hits >= Convert.ToInt32(currCondition.Amount) ? GreenLightColor : OrangeLightColor,
                                    indent: 3
                                ));

                                if (conditionMet) // Only when at least one condition has already been met.
                                {
                                    conditionMet = (hits >= Convert.ToInt32(currCondition.Amount)); // Check if current condition has been met.
                                }
                            }
                        }

                        #endregion Fixed Amount Mode.

                        // ==============
                        // B. Combo Mode.
                        // ==============

                        #region Combo Mode.

                        string allowedComboStats = "";

                        foreach (Condition currCondition in conditionListEntries[i])
                        {
                            if (currCondition.Amount == 0)
                            {
                                int hits = currEquipment.MatchStat(currCondition.ShortTerm);

                                AddMsg(new RtMessage(
                                    message:
                                    (hits > 0 ? "✓" : "❌") + " '" + currCondition.LongTerm + "': " + hits,
                                    color:
                                    hits > 0 ? GreenLightColor : OrangeLightColor,
                                    indent: 3
                                ));

                                if (conditionMet) // Only when at least one condition has already been met.
                                {
                                    conditionMet = (hits > 0); // Check if current condition has been met.

                                    // Check if the current condition has been met.
                                    // [DEVNOTE] conditionMet check not necessary here.
                                    // Would break if set to false above and wouldn't enter the next step anyway.
                                    allowedComboStats += currCondition.ShortTerm + " | ";
                                }
                            }
                        }

                        // -----------------------------------------------
                        // Combo mode can not have any other stat combined
                        // other than the ones in the condition entry.
                        // -----------------------------------------------

                        // Check current equipment stats against each config short term.
                        // [DEVNOTE] If allowedComboStats isn't an empty string at this point,
                        //           current condition list entry is a combo.
                        //           Additionally, check if current condition has (still) been met.
                        if (!string.IsNullOrEmpty(allowedComboStats) & conditionMet)
                        {
                            foreach (string[] currConfigTerm in cfgTerms)
                            {
                                // If not an allowed stat.
                                if (!allowedComboStats.ToLower().Contains(currConfigTerm[1].ToLower()))
                                {
                                    int hits = currEquipment.MatchStat(currConfigTerm[1]);

                                    // If a non allowed stat has been found in current equipment.
                                    if (hits > 0)
                                    {
                                        conditionMet = false;
                                    }
                                }

                                // Break the foreach loop if a non allowed stat was found
                                // and notify.
                                if (!conditionMet)
                                {
                                    AddMsg(new RtMessage(
                                        message:
                                        "❌ Estatística de combo não permitida detectada:",
                                        color:
                                        OrangeLightColor,
                                        indent: 3
                                    ));
                                    AddMsg(new RtMessage(
                                         message:
                                         "'" + currConfigTerm[0] + "'",
                                         color:
                                         OrangeLightColor,
                                         indent: 7
                                     ));

                                    break;
                                }
                            }
                        }

                        #endregion Combo Mode.

                        if (conditionMet)
                        {
                            break;
                        }
                    }

                    // ---------------------
                    // LOG RESULTS/DECISION.
                    // ---------------------

                    AddMsg("=========================");
                    if (conditionListEntries.Count() > 1)
                    {
                        AddMsg(new RtMessage(
                            message:
                            "• " + (conditionMet ? "Condição atendida." : "Nenhuma condição atendida."),
                            color:
                            conditionMet ? GreenLightColor : OrangeLightColor
                            ));
                    }
                    else
                    {
                        AddMsg(new RtMessage(
                            message:
                            "• " + (conditionMet ? "Condição atendida." : "Condição não foi atendida."),
                            color:
                            conditionMet ? GreenLightColor : OrangeLightColor
                            ));
                    }
                    if (!InfoGui.PreviewCapture)
                    {
                        if (conditionMet)
                        {
                            AddMsg(new RtMessage("=> Aceitando novos atributos.", color: GreenLightColor, indent: 3));
                        }
                        else
                        {
                            AddMsg(new RtMessage("=> Mantendo atributos antigos.", indent: 3));
                        }
                    }

                    #endregion Match Condition(s).
                }

                if (!InfoGui.PreviewCapture)
                {
                    AddMsg("• Número de rolagens: " + nrRolls.ToString() + " / " + maxNrRolls.ToString() + ".");
                }

                #endregion Judge conditions.

                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

                // [DEVNOTE] Invoke due to threading (else not in sync).
                // OVERWRITE TESSERACT RESULT FILE.
                InfoGui.rTxtBoxInfo.Invoke((MethodInvoker)(() =>
                    File.WriteAllText(outputPath + ".txt",
                        "[ OCR Bruto ]" + Environment.NewLine +
                        rawCapturedText + Environment.NewLine +
                        "=========================" + Environment.NewLine +
                        "[ OCR Otimizado Personalizado ]" + Environment.NewLine +
                        currEquipment.OcrText.TrimEnd() + Environment.NewLine +
                        "=========================" + Environment.NewLine +
                        InfoGui.rTxtBoxInfo.Text,
                        System.Text.Encoding.UTF8
                        )
                ));

                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

                // =============
                // == ACTIONS ==
                // =============

                #region Perform actions based on matching decision.

                if (!InfoGui.PreviewCapture & keepRunning) // [DEVNOTE] keepRunning could have been set to false in history check.
                {
                    // Wait for a bit before rejecting or accepting current roll.
                    Thread.Sleep(AwaitAcceptRejectAction);

                    Ocr_Token.ThrowIfCancellationRequested();

                    if (conditionMet)
                    {
                        // Accept the new attributes.
                        MoveMouse((uint)NewClickPoint.X, (uint)NewClickPoint.Y);
                        DoLeftMouseClick((uint)NewClickPoint.X, (uint)NewClickPoint.Y);

                        string successSound = Tesseract.Ocr.AssemblyCodeBaseDirectory + @"\Media\Sounds\Success.wav";
                        if (File.Exists(successSound)) // !InfoGui.PreviewCapture already checked above.
                        {
                            System.Media.SoundPlayer player = new System.Media.SoundPlayer(successSound);
                            player.Play();
                        }

                        keepRunning = false;
                    }
                    else
                    {
                        // Keep the old attributes.
                        MoveMouse((uint)RetainClickPoint.X, (uint)RetainClickPoint.Y);
                        DoLeftMouseClick((uint)RetainClickPoint.X, (uint)RetainClickPoint.Y);

                        // Reproduce?
                        if (nrRolls == maxNrRolls) // Check if not at allowed rolls.
                        {
                            AddMsg(Environment.NewLine + Environment.NewLine + "Número máximo de rolagens alcançado. Parado.");

                            string doneSound = Tesseract.Ocr.AssemblyCodeBaseDirectory + @"\Media\Sounds\Done.wav";
                            if (File.Exists(doneSound)) // !InfoGui.PreviewCapture already checked above.
                            {
                                System.Media.SoundPlayer player = new System.Media.SoundPlayer(doneSound);
                                player.Play();
                            }

                            keepRunning = false;
                        }
                        else
                        {
                            MoveMouse((uint)ReproduceClickPoint.X, (uint)ReproduceClickPoint.Y);
                            // Additional wait until in-game reproduce button becomes available again.
                            Thread.Sleep(AwaitIngameReproduceButtonAvailable); // 1500 = measured | 1500+ = safer value.
                            DoLeftMouseClick((uint)ReproduceClickPoint.X, (uint)ReproduceClickPoint.Y);
                            // Additional wait until ingame stats have been rolled.
                            Thread.Sleep(AwaitIngameStatsRolled);
                        }
                    }
                }
                else // if PreviewCapture = true;
                {
                    keepRunning = false;
                }

                #endregion Perform actions based on matching decision.
            }

            #endregion Main roll loop.
        }

        private System.Drawing.Image ResizeImage(System.Drawing.Image imgToResize, double scaleFactor = 2)
        {
            //Get the image current width.
            int sourceWidth = imgToResize.Width;
            //Get the image current height.
            int sourceHeight = imgToResize.Height;

            // Recalculate width & height based on given scale factor.
            int destWidth = (int)(sourceWidth * scaleFactor);
            int destHeight = (int)(sourceHeight * scaleFactor);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Draw image with new width and height.
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Image)b;
        }

        public void DoLeftMouseClick(uint posX, uint posY)
        {
            // Call the imported function with the cursor's current position.
            //uint X = (uint)Cursor.Position.X;
            //uint Y = (uint)Cursor.Position.Y;

            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, posX, posY, 0, 0); // Current app will lose focus.
            Thread.Sleep(50);

            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, posX, posY, 0, 0); // Actual click.
            Thread.Sleep(50);

            // Refocus InfoGui (in order for Esc key capture to work without having to do a keyboard hook).
            //if (InfoGui.InvokeRequired)
            //{
            //    InfoGui.BeginInvoke(new MethodInvoker(delegate
            //    {
            //        InfoGui.Focus();
            //    }));
            //}
            //else
            //{
            //    InfoGui.Focus();
            //}
        }

        //public void MoveMouse(uint posX, uint posY)
        //{
        //    this.Cursor = new Cursor(Cursor.Current.Handle);
        //    Cursor.Position = new Point((int)posX, (int)posY);

        //    Thread.Sleep(50);
        //}

        #region Low Level Methods.

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        // Mouse actions.
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;

        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        public void MoveMouse(uint posX, uint posY)
        {
            SetCursorPos((int)posX, (int)posY);
            Thread.Sleep(50);
        }

        #endregion Low Level Methods.

        #endregion Methods.
    }
}