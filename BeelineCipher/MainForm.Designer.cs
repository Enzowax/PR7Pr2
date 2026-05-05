using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BeelineCipher
{
    partial class MainForm
    {
        private IContainer components = null;
        private GroupBox grpInput;
        private Label lblInput;
        private TextBox txtInput;
        private Label lblRows;
        private NumericUpDown numRows;
        private GroupBox grpActions;
        private Button btnEncrypt;
        private Button btnDecrypt;
        private Button btnVisualize;
        private Button btnClear;
        private GroupBox grpOutput;
        private Label lblOutput;
        private TextBox txtOutput;
        private GroupBox grpVisualization;
        private TextBox txtVisualization;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private ToolTip toolTip;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();

            this.grpInput = new GroupBox();
            this.lblInput = new Label();
            this.txtInput = new TextBox();
            this.lblRows = new Label();
            this.numRows = new NumericUpDown();

            this.grpActions = new GroupBox();
            this.btnEncrypt = new Button();
            this.btnDecrypt = new Button();
            this.btnVisualize = new Button();
            this.btnClear = new Button();

            this.grpOutput = new GroupBox();
            this.lblOutput = new Label();
            this.txtOutput = new TextBox();

            this.grpVisualization = new GroupBox();
            this.txtVisualization = new TextBox();

            this.statusStrip = new StatusStrip();
            this.statusLabel = new ToolStripStatusLabel();

            this.toolTip = new ToolTip(this.components);

            ((ISupportInitialize)(this.numRows)).BeginInit();
            this.grpInput.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.grpOutput.SuspendLayout();
            this.grpVisualization.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();

            // grpInput
            this.grpInput.Controls.Add(this.lblInput);
            this.grpInput.Controls.Add(this.txtInput);
            this.grpInput.Controls.Add(this.lblRows);
            this.grpInput.Controls.Add(this.numRows);
            this.grpInput.Location = new Point(12, 12);
            this.grpInput.Name = "grpInput";
            this.grpInput.Size = new Size(820, 130);
            this.grpInput.TabIndex = 0;
            this.grpInput.TabStop = false;
            this.grpInput.Text = "Входные данные";

            // lblInput
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new Point(15, 25);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new Size(85, 15);
            this.lblInput.TabIndex = 0;
            this.lblInput.Text = "Исходный текст:";

            // txtInput
            this.txtInput.Location = new Point(15, 45);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.ScrollBars = ScrollBars.Vertical;
            this.txtInput.Size = new Size(640, 70);
            this.txtInput.TabIndex = 1;
            this.txtInput.Font = new Font("Consolas", 9.75F);

            // lblRows
            this.lblRows.AutoSize = true;
            this.lblRows.Location = new Point(680, 25);
            this.lblRows.Name = "lblRows";
            this.lblRows.Size = new Size(120, 15);
            this.lblRows.TabIndex = 2;
            this.lblRows.Text = "Количество строк:";

            // numRows
            this.numRows.Location = new Point(680, 45);
            this.numRows.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numRows.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            this.numRows.Value = new decimal(new int[] { 3, 0, 0, 0 });
            this.numRows.Name = "numRows";
            this.numRows.Size = new Size(120, 23);
            this.numRows.TabIndex = 3;
            this.numRows.Font = new Font("Segoe UI", 10F);

            // grpActions
            this.grpActions.Controls.Add(this.btnEncrypt);
            this.grpActions.Controls.Add(this.btnDecrypt);
            this.grpActions.Controls.Add(this.btnVisualize);
            this.grpActions.Controls.Add(this.btnClear);
            this.grpActions.Location = new Point(12, 148);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new Size(820, 70);
            this.grpActions.TabIndex = 1;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Действия";

            // btnEncrypt
            this.btnEncrypt.Location = new Point(15, 25);
            this.btnEncrypt.Name = "btnEncrypt";
            this.btnEncrypt.Size = new Size(180, 32);
            this.btnEncrypt.TabIndex = 0;
            this.btnEncrypt.Text = "Зашифровать";
            this.btnEncrypt.UseVisualStyleBackColor = true;
            this.btnEncrypt.Click += new System.EventHandler(this.BtnEncrypt_Click);

            // btnDecrypt
            this.btnDecrypt.Location = new Point(205, 25);
            this.btnDecrypt.Name = "btnDecrypt";
            this.btnDecrypt.Size = new Size(180, 32);
            this.btnDecrypt.TabIndex = 1;
            this.btnDecrypt.Text = "Расшифровать";
            this.btnDecrypt.UseVisualStyleBackColor = true;
            this.btnDecrypt.Click += new System.EventHandler(this.BtnDecrypt_Click);

            // btnVisualize
            this.btnVisualize.Location = new Point(395, 25);
            this.btnVisualize.Name = "btnVisualize";
            this.btnVisualize.Size = new Size(180, 32);
            this.btnVisualize.TabIndex = 2;
            this.btnVisualize.Text = "Показать зигзаг";
            this.btnVisualize.UseVisualStyleBackColor = true;
            this.btnVisualize.Click += new System.EventHandler(this.BtnVisualize_Click);

            // btnClear
            this.btnClear.Location = new Point(620, 25);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new Size(180, 32);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Очистить";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);

            // grpOutput
            this.grpOutput.Controls.Add(this.lblOutput);
            this.grpOutput.Controls.Add(this.txtOutput);
            this.grpOutput.Location = new Point(12, 224);
            this.grpOutput.Name = "grpOutput";
            this.grpOutput.Size = new Size(820, 110);
            this.grpOutput.TabIndex = 2;
            this.grpOutput.TabStop = false;
            this.grpOutput.Text = "Результат";

            // lblOutput
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new Point(15, 25);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new Size(150, 15);
            this.lblOutput.TabIndex = 0;
            this.lblOutput.Text = "Текст после операции:";

            // txtOutput
            this.txtOutput.Location = new Point(15, 45);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = ScrollBars.Vertical;
            this.txtOutput.Size = new Size(785, 55);
            this.txtOutput.TabIndex = 1;
            this.txtOutput.Font = new Font("Consolas", 9.75F);

            // grpVisualization
            this.grpVisualization.Controls.Add(this.txtVisualization);
            this.grpVisualization.Location = new Point(12, 340);
            this.grpVisualization.Name = "grpVisualization";
            this.grpVisualization.Size = new Size(820, 200);
            this.grpVisualization.TabIndex = 3;
            this.grpVisualization.TabStop = false;
            this.grpVisualization.Text = "Визуализация заполнения зигзагом";

            // txtVisualization
            this.txtVisualization.Location = new Point(15, 25);
            this.txtVisualization.Multiline = true;
            this.txtVisualization.Name = "txtVisualization";
            this.txtVisualization.ReadOnly = true;
            this.txtVisualization.ScrollBars = ScrollBars.Both;
            this.txtVisualization.Size = new Size(785, 165);
            this.txtVisualization.TabIndex = 0;
            this.txtVisualization.Font = new Font("Consolas", 11F);
            this.txtVisualization.WordWrap = false;

            // statusStrip
            this.statusStrip.Items.AddRange(new ToolStripItem[] { this.statusLabel });
            this.statusStrip.Location = new Point(0, 555);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new Size(844, 22);
            this.statusStrip.TabIndex = 4;

            // statusLabel
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new Size(60, 17);
            this.statusLabel.Text = "Готово";

            // MainForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(844, 577);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.grpVisualization);
            this.Controls.Add(this.grpOutput);
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.grpInput);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Шифр Билайна — Двухлинейный (Rail Fence) | Вариант 13";

            ((ISupportInitialize)(this.numRows)).EndInit();
            this.grpInput.ResumeLayout(false);
            this.grpInput.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.grpOutput.ResumeLayout(false);
            this.grpOutput.PerformLayout();
            this.grpVisualization.ResumeLayout(false);
            this.grpVisualization.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
