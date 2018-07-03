namespace Segplan.ReplicateCustomField.App
{
    partial class frmPrincipal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbPrincipal = new System.Windows.Forms.TabControl();
            this.tbOrigem = new System.Windows.Forms.TabPage();
            this.gbOptions = new System.Windows.Forms.GroupBox();
            this.ckOpcoes = new System.Windows.Forms.CheckedListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnConectSource = new System.Windows.Forms.Button();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSourceUser = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPassSource = new System.Windows.Forms.TextBox();
            this.tbDestino = new System.Windows.Forms.TabPage();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnTarget = new System.Windows.Forms.Button();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTargetUser = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTargetPwd = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAnterior = new System.Windows.Forms.Button();
            this.btnAvancar = new System.Windows.Forms.Button();
            this.tbPrincipal.SuspendLayout();
            this.tbOrigem.SuspendLayout();
            this.gbOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tbDestino.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbPrincipal
            // 
            this.tbPrincipal.Controls.Add(this.tbOrigem);
            this.tbPrincipal.Controls.Add(this.tbDestino);
            this.tbPrincipal.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbPrincipal.Location = new System.Drawing.Point(0, 0);
            this.tbPrincipal.Name = "tbPrincipal";
            this.tbPrincipal.SelectedIndex = 0;
            this.tbPrincipal.Size = new System.Drawing.Size(370, 122);
            this.tbPrincipal.TabIndex = 0;
            // 
            // tbOrigem
            // 
            this.tbOrigem.Controls.Add(this.gbOptions);
            this.tbOrigem.Controls.Add(this.pictureBox1);
            this.tbOrigem.Controls.Add(this.btnConectSource);
            this.tbOrigem.Controls.Add(this.txtSource);
            this.tbOrigem.Controls.Add(this.label1);
            this.tbOrigem.Controls.Add(this.label3);
            this.tbOrigem.Controls.Add(this.txtSourceUser);
            this.tbOrigem.Controls.Add(this.label4);
            this.tbOrigem.Controls.Add(this.txtPassSource);
            this.tbOrigem.Location = new System.Drawing.Point(4, 22);
            this.tbOrigem.Name = "tbOrigem";
            this.tbOrigem.Padding = new System.Windows.Forms.Padding(3);
            this.tbOrigem.Size = new System.Drawing.Size(362, 96);
            this.tbOrigem.TabIndex = 0;
            this.tbOrigem.Text = "Origem";
            this.tbOrigem.UseVisualStyleBackColor = true;
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.ckOpcoes);
            this.gbOptions.Location = new System.Drawing.Point(17, 99);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(332, 96);
            this.gbOptions.TabIndex = 20;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Opções:";
            this.gbOptions.Visible = false;
            // 
            // ckOpcoes
            // 
            this.ckOpcoes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ckOpcoes.FormattingEnabled = true;
            this.ckOpcoes.Items.AddRange(new object[] {
            "Project Custom Fields",
            "Task Custom Fields",
            "Create and Populate LookupTables"});
            this.ckOpcoes.Location = new System.Drawing.Point(3, 16);
            this.ckOpcoes.Name = "ckOpcoes";
            this.ckOpcoes.Size = new System.Drawing.Size(326, 77);
            this.ckOpcoes.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Segplan.ReplicateCustomField.App.Properties.Resources.Action_ok_icon;
            this.pictureBox1.Location = new System.Drawing.Point(311, 67);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(38, 21);
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // btnConectSource
            // 
            this.btnConectSource.Location = new System.Drawing.Point(247, 67);
            this.btnConectSource.Name = "btnConectSource";
            this.btnConectSource.Size = new System.Drawing.Size(58, 21);
            this.btnConectSource.TabIndex = 18;
            this.btnConectSource.Text = "Logon";
            this.btnConectSource.UseVisualStyleBackColor = true;
            this.btnConectSource.Click += new System.EventHandler(this.btnConectSource_Click);
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(17, 29);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(332, 20);
            this.txtSource.TabIndex = 11;
            this.txtSource.Text = "http://epmprod01/pwa";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Project Server Url:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Username";
            // 
            // txtSourceUser
            // 
            this.txtSourceUser.Location = new System.Drawing.Point(17, 67);
            this.txtSourceUser.Name = "txtSourceUser";
            this.txtSourceUser.Size = new System.Drawing.Size(109, 20);
            this.txtSourceUser.TabIndex = 13;
            this.txtSourceUser.Text = "segplan\\svcsp_farm";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(129, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Password";
            // 
            // txtPassSource
            // 
            this.txtPassSource.Location = new System.Drawing.Point(132, 67);
            this.txtPassSource.Name = "txtPassSource";
            this.txtPassSource.PasswordChar = '*';
            this.txtPassSource.Size = new System.Drawing.Size(109, 20);
            this.txtPassSource.TabIndex = 15;
            this.txtPassSource.Text = "M$3pm@2k12";
            // 
            // tbDestino
            // 
            this.tbDestino.Controls.Add(this.pictureBox2);
            this.tbDestino.Controls.Add(this.btnTarget);
            this.tbDestino.Controls.Add(this.txtTarget);
            this.tbDestino.Controls.Add(this.label2);
            this.tbDestino.Controls.Add(this.label5);
            this.tbDestino.Controls.Add(this.txtTargetUser);
            this.tbDestino.Controls.Add(this.label6);
            this.tbDestino.Controls.Add(this.txtTargetPwd);
            this.tbDestino.Location = new System.Drawing.Point(4, 22);
            this.tbDestino.Name = "tbDestino";
            this.tbDestino.Padding = new System.Windows.Forms.Padding(3);
            this.tbDestino.Size = new System.Drawing.Size(362, 96);
            this.tbDestino.TabIndex = 1;
            this.tbDestino.Text = "Destino";
            this.tbDestino.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Segplan.ReplicateCustomField.App.Properties.Resources.Action_ok_icon;
            this.pictureBox2.Location = new System.Drawing.Point(311, 67);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(38, 21);
            this.pictureBox2.TabIndex = 27;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // btnTarget
            // 
            this.btnTarget.Location = new System.Drawing.Point(247, 67);
            this.btnTarget.Name = "btnTarget";
            this.btnTarget.Size = new System.Drawing.Size(58, 21);
            this.btnTarget.TabIndex = 26;
            this.btnTarget.Text = "Logon";
            this.btnTarget.UseVisualStyleBackColor = true;
            this.btnTarget.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtTarget
            // 
            this.txtTarget.Location = new System.Drawing.Point(17, 29);
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.Size = new System.Drawing.Size(332, 20);
            this.txtTarget.TabIndex = 21;
            this.txtTarget.Text = "http://epmdesenv/pwa";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Project Server Url:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Username";
            // 
            // txtTargetUser
            // 
            this.txtTargetUser.Location = new System.Drawing.Point(17, 67);
            this.txtTargetUser.Name = "txtTargetUser";
            this.txtTargetUser.Size = new System.Drawing.Size(109, 20);
            this.txtTargetUser.TabIndex = 23;
            this.txtTargetUser.Text = "segplan\\svcsp_farm";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(129, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "Password";
            // 
            // txtTargetPwd
            // 
            this.txtTargetPwd.Location = new System.Drawing.Point(132, 67);
            this.txtTargetPwd.Name = "txtTargetPwd";
            this.txtTargetPwd.PasswordChar = '*';
            this.txtTargetPwd.Size = new System.Drawing.Size(109, 20);
            this.txtTargetPwd.TabIndex = 25;
            this.txtTargetPwd.Text = "M$3pm@2k12";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnAnterior);
            this.panel1.Controls.Add(this.btnAvancar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 126);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(370, 35);
            this.panel1.TabIndex = 1;
            // 
            // btnAnterior
            // 
            this.btnAnterior.Enabled = false;
            this.btnAnterior.Image = global::Segplan.ReplicateCustomField.App.Properties.Resources.left;
            this.btnAnterior.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAnterior.Location = new System.Drawing.Point(195, 3);
            this.btnAnterior.Name = "btnAnterior";
            this.btnAnterior.Size = new System.Drawing.Size(75, 29);
            this.btnAnterior.TabIndex = 1;
            this.btnAnterior.Text = "Anterior";
            this.btnAnterior.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAnterior.UseVisualStyleBackColor = true;
            this.btnAnterior.Click += new System.EventHandler(this.btnAnterior_Click);
            // 
            // btnAvancar
            // 
            this.btnAvancar.Enabled = false;
            this.btnAvancar.Image = global::Segplan.ReplicateCustomField.App.Properties.Resources.right;
            this.btnAvancar.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAvancar.Location = new System.Drawing.Point(276, 3);
            this.btnAvancar.Name = "btnAvancar";
            this.btnAvancar.Size = new System.Drawing.Size(78, 29);
            this.btnAvancar.TabIndex = 0;
            this.btnAvancar.Text = "Avançar";
            this.btnAvancar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAvancar.UseVisualStyleBackColor = true;
            this.btnAvancar.Click += new System.EventHandler(this.btnAvancar_Click);
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 161);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tbPrincipal);
            this.Name = "frmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Migração para Project PWA";
            this.tbPrincipal.ResumeLayout(false);
            this.tbOrigem.ResumeLayout(false);
            this.tbOrigem.PerformLayout();
            this.gbOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tbDestino.ResumeLayout(false);
            this.tbDestino.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbPrincipal;
        private System.Windows.Forms.TabPage tbOrigem;
        private System.Windows.Forms.TabPage tbDestino;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSourceUser;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPassSource;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnConectSource;
        private System.Windows.Forms.GroupBox gbOptions;
        private System.Windows.Forms.CheckedListBox ckOpcoes;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button btnTarget;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTargetUser;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTargetPwd;
        private System.Windows.Forms.Button btnAnterior;
        private System.Windows.Forms.Button btnAvancar;
    }
}