using Humanizer;
using MetroFramework;
using MetroFramework.Forms;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPW_Recaster
{
    public partial class InfoGui : MetroForm
    {
        #region Variables | Properties.

        private MainGui MainForm { get; set; }

        internal bool FormsChained { get; set; } = true;

        internal bool PreviewCapture { get; set; }

        #endregion Variables | Properties.

        #region Constructors | Initialization Methods.

        public InfoGui()
        {
            InitializeComponent();

            InitializeInfoGui();
        }

        public InfoGui(MainGui parent) : this() // Also calls the base constructor.
        {
            MainForm = parent;
        }

        private void InitializeInfoGui()
        {
            // Load the FormsChained value.
            FormsChained = Properties.Settings.Default.FormsChained;

            // Load the PreviewCapture value.
            chkbxPreviewCapture.Checked = Properties.Settings.Default.PreviewCapture;

            // Tooltip(s).
            toolTip.Theme = MetroThemeStyle.Dark;

            toolTip.SetToolTip(btnChainForms, "\r\nAnexar | Desanexar Formulários\r\n ");
            toolTip.SetToolTip(btnLogFolder, "\r\nAbrir Pasta de Logs\r\n ");
            toolTip.SetToolTip(numMaxRolls, "\r\nℹ\r\nTambém para de rolar\r\nquando os\r\nPerfect Elements no jogo\r\nacabam.\r\n ");
            toolTip.SetToolTip(lblMaxRolls, "\r\nℹ\r\nTambém para de rolar\r\nquando os\r\nPerfect Elements no jogo\r\nacabam.\r\n ");
            toolTip.SetToolTip(chkbxPreviewCapture, "\r\nℹ\r\nQuando marcado, apenas\r\nrealiza uma captura.\r\nNenhuma roleta será\r\nfeita no jogo.\r\n ");
        }

        #endregion Constructors | Initialization Methods.

        #region Events.

        private async void btnOcr_Click(object sender, EventArgs e)
        {
            pbLogo.Visible = false;

            if (MainForm.dgConditions.Rows.Count > 0)
            {
                if (btnOcr.Text.Contains("Iniciar"))
                {
                    if (MainForm.WindowState == FormWindowState.Minimized)
                        MainForm.WindowState = FormWindowState.Normal;

                    btnOcr.Text = "Parar";
                    btnOcr.Enabled = true;

                    #region Start Ocr.

                    // Store start timestamp.
                    MainForm.Ocr_StartTime = DateTime.Now;

                    // Request an Ocr Cancellation Token.
                    MainForm.Ocr_CancellationTokenSource = new CancellationTokenSource();
                    MainForm.Ocr_Token = MainForm.Ocr_CancellationTokenSource.Token;

                    // Do work.
                    try
                    {
                        MainForm.PrepareOcrLogic();

                        await Task.Run(() =>
                        {
                            MainForm.DoOcrLogic();

                            //Thread.Sleep(2500);
                            //AddMsg("Async Dev Test");
                            //Thread.Sleep(2500);
                        });

                        MainForm.WindowState = FormWindowState.Normal;
                        Thread.Sleep(1250);
                        MainForm.Update();
                        MainForm.Update();

                        DateTime currTime = DateTime.Now;
                        TimeSpan diffResult = currTime.Subtract(MainForm.Ocr_StartTime);

                        rTxtBoxInfo.AppendText(Environment.NewLine);
                        rTxtBoxInfo.AppendText("=========================");
                        rTxtBoxInfo.AppendText(Environment.NewLine);
                        rTxtBoxInfo.AppendText("Processo concluído.");
                        rTxtBoxInfo.AppendText(Environment.NewLine);
                        rTxtBoxInfo.AppendText("Tempo de execução: " + diffResult.Humanize(3) + ".");
                        rTxtBoxInfo.AppendText(Environment.NewLine + Environment.NewLine);

                        // Auto scroll to bottom.
                        if (chkbxAutoScrollBottom.Checked)
                        {
                            rTxtBoxInfo.SelectionStart = rTxtBoxInfo.TextLength;
                            rTxtBoxInfo.ScrollToCaret();
                        }

                        // Reset button to 'Start'.
                        btnOcr.Text = "Iniciar";
                        btnOcr.Enabled = true;
                    }
                    catch (OperationCanceledException)
                    {
                        btnOcr.Text = "⏳";
                        btnOcr.Enabled = false; // Temporarily disable.

                        DateTime currTime = DateTime.Now;
                        TimeSpan diffResult = currTime.Subtract(MainForm.Ocr_StartTime);

                        rTxtBoxInfo.AppendText(Environment.NewLine);
                        rTxtBoxInfo.AppendText("=========================");
                        rTxtBoxInfo.AppendText(Environment.NewLine);
                        rTxtBoxInfo.AppendText("Processo interrompido.");
                        rTxtBoxInfo.AppendText(Environment.NewLine);
                        rTxtBoxInfo.AppendText("Tempo de execução: " + diffResult.Humanize(3) + ".");
                        rTxtBoxInfo.AppendText(Environment.NewLine + Environment.NewLine);

                        // Auto scroll to bottom.
                        if (chkbxAutoScrollBottom.Checked)
                        {
                            rTxtBoxInfo.SelectionStart = rTxtBoxInfo.TextLength;
                            rTxtBoxInfo.ScrollToCaret();
                        }

                        btnOcr.Text = "Iniciar";
                        btnOcr.Enabled = true;

                        MainForm.WindowState = FormWindowState.Normal;
                    }
                    catch (Exception ex)
                    {
                        btnOcr.Text = "⏳";
                        btnOcr.Enabled = false; // Temporarily disable.

                        DateTime currTime = DateTime.Now;
                        TimeSpan diffResult = currTime.Subtract(MainForm.Ocr_StartTime);

                        rTxtBoxInfo.AppendText(Environment.NewLine);
                        rTxtBoxInfo.AppendText("=========================");
                        rTxtBoxInfo.AppendText(Environment.NewLine);
                        rTxtBoxInfo.AppendText(ex.GetType().Name + ": " + ex.Message);
                        rTxtBoxInfo.AppendText(Environment.NewLine);
                        rTxtBoxInfo.AppendText("Tempo de execução: " + diffResult.Humanize(3) + ".");
                        rTxtBoxInfo.AppendText(Environment.NewLine + Environment.NewLine);

                        // Auto scroll to bottom.
                        if (chkbxAutoScrollBottom.Checked)
                        {
                            rTxtBoxInfo.SelectionStart = rTxtBoxInfo.TextLength;
                            rTxtBoxInfo.ScrollToCaret();
                        }

                        if (MainForm.Ocr_CancellationTokenSource != null)
                            MainForm.Ocr_CancellationTokenSource.Cancel();

                        btnOcr.Text = "Iniciar";
                        btnOcr.Enabled = true;

                        MainForm.WindowState = FormWindowState.Normal;
                    }

                    #endregion Start Ocr.
                }
                else
                {
                    #region Stop Ocr.

                    btnOcr.Text = "⏳";
                    btnOcr.Enabled = false; // Temporarily disable. Will be re-enabled in above OperationCanceledException catch.

                    if (MainForm.Ocr_CancellationTokenSource != null)
                        MainForm.Ocr_CancellationTokenSource.Cancel();

                    #endregion Stop Ocr.
                }
            }
            else
            {
                MetroFramework.MetroMessageBox.Show(MainForm,
                                "Por favor, adicione ao menos uma condição para procurar.\n" +
                                "1. Selecione uma quantidade e estatística preferida (ex.: '2 x Channelling').\n" +
                                "2. (Opcional) Selecione quantidade(s) e estatística(s) adicionais.\n" +
                                "3. Clique no botão ➕.",
                                "", // Note
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnChainForms_Click(object sender, EventArgs e)
        {
            // Unfocus.
            lblMaxRolls.Focus();

            CheckFormsChainState();
        }

        internal void CheckFormsChainState(bool toggle = true)
        {
            if (FormsChained)
            {
                // Rechain form with parent.
                this.Resizable = false;
                this.SizeGripStyle = SizeGripStyle.Hide;
                this.Movable = false;
                this.btnChainForms.Text = "▉ 🧲 ▉";
            }
            else
            {
                // Unchain form from parent.
                this.Resizable = true;
                this.SizeGripStyle = SizeGripStyle.Show;
                this.Movable = true;
                this.btnChainForms.Text = "▉      ▉";

                FormsChained = false;
            }

            if (toggle)
            {
                if (!FormsChained)
                {
                    // Rechain form with parent.
                    this.Resizable = false;
                    this.SizeGripStyle = SizeGripStyle.Hide;
                    this.Movable = false;
                    this.btnChainForms.Text = "▉ 🧲 ▉";

                    FormsChained = true;
                }
                else
                {
                    // Unchain form from parent.
                    this.Resizable = true;
                    this.SizeGripStyle = SizeGripStyle.Show;
                    this.Movable = true;
                    this.btnChainForms.Text = "▉      ▉";

                    FormsChained = false;
                }
            }

            MainForm.SetInfoGuiSizeAndPosition();
            this.Refresh();

            // Store the FormsChained value.
            Properties.Settings.Default.FormsChained = FormsChained;
            Properties.Settings.Default.Save();
        }

        private void btnLogFolder_Click(object sender, EventArgs e)
        {
            // Unfocus.
            lblMaxRolls.Focus();

            string logPath = Tesseract.Ocr.AssemblyCodeBaseDirectory + @"\Logged";

            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(Tesseract.Ocr.AssemblyCodeBaseDirectory + @"\Logged");
            }

            System.Diagnostics.Process.Start(logPath);
        }

        private void rTxtBoxInfo_MouseClick(object sender, MouseEventArgs e)
        {
            MainForm.lblAmount.Focus();
        }

        private void InfoGui_Load(object sender, EventArgs e)
        {
            btnOcr.Focus();
        }

        private void InfoGui_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                // Store the new info window size.
                Properties.Settings.Default.InfoWindowSize = this.Size;
                Properties.Settings.Default.Save();
            }
        }

        private void InfoGui_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                // Store the new main window location.
                Properties.Settings.Default.InfoWindowLocation = this.Location;
                Properties.Settings.Default.Save();
            }
        }

        private void chkbxPreviewCapture_CheckStateChanged(object sender, EventArgs e)
        {
            if (chkbxPreviewCapture.Checked)
            {
                PreviewCapture = true;
                chkbxPreviewCapture.Text = "Pré-visualizar\n(sem rolagens | apenas OCR)";

                // Don't show number of rolls in preview mode.
                lblMaxRolls.Visible = false;
                numMaxRolls.Visible = false;
            }
            else
            {
                PreviewCapture = false;
                chkbxPreviewCapture.Text = "Pré-visualizar?";

                // Show number of rolls when not in preview mode.
                lblMaxRolls.Visible = true;
                numMaxRolls.Visible = true;
            }

            Properties.Settings.Default.PreviewCapture = PreviewCapture;
            Properties.Settings.Default.Save();

            if (MainForm != null)
            {
                MainForm.SetCaptureRegion();
            }
        }

        private void numMaxRolls_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MaxRolls = (int)numMaxRolls.Value;
            Properties.Settings.Default.Save();

            lblMaxRolls.Focus();
        }

        #endregion Events.
    }
}