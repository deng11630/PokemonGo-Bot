﻿namespace PokemonGo.RocketAPI.Window
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.authTypeLabel = new System.Windows.Forms.Label();
            this.authTypeCb = new System.Windows.Forms.ComboBox();
            this.ptcUserLabel = new System.Windows.Forms.Label();
            this.ptcPasswordLabel = new System.Windows.Forms.Label();
            this.latLabel = new System.Windows.Forms.Label();
            this.longiLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ptcUserText = new System.Windows.Forms.TextBox();
            this.ptcPassText = new System.Windows.Forms.TextBox();
            this.latitudeText = new System.Windows.Forms.TextBox();
            this.longitudeText = new System.Windows.Forms.TextBox();
            this.razzmodeCb = new System.Windows.Forms.ComboBox();
            this.razzSettingText = new System.Windows.Forms.TextBox();
            this.transferTypeCb = new System.Windows.Forms.ComboBox();
            this.transferCpThresText = new System.Windows.Forms.TextBox();
            this.evolveAllChk = new System.Windows.Forms.CheckBox();
            this.saveBtn = new System.Windows.Forms.Button();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.FindAdressButton = new System.Windows.Forms.Button();
            this.AdressBox = new System.Windows.Forms.TextBox();
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lblHyper = new System.Windows.Forms.Label();
            this.txtHyper = new System.Windows.Forms.TextBox();
            this.txtSuper = new System.Windows.Forms.TextBox();
            this.lblSuper = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ImageSizeBox = new System.Windows.Forms.TextBox();
            this.EmailLoginText = new System.Windows.Forms.Label();
            this.CatchPokemonBox = new System.Windows.Forms.CheckBox();
            this.EmailPasswordText = new System.Windows.Forms.Label();
            this.CatchPokemonText = new System.Windows.Forms.Label();
            this.EmailPasswordBox = new System.Windows.Forms.TextBox();
            this.EmailLoginBox = new System.Windows.Forms.TextBox();
            this.transferIVThresText = new System.Windows.Forms.TextBox();
            this.TravelSpeedText = new System.Windows.Forms.Label();
            this.TravelSpeedBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lblTravelParam = new System.Windows.Forms.Label();
            this.txtTravelParam = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // authTypeLabel
            // 
            this.authTypeLabel.AutoSize = true;
            this.authTypeLabel.Location = new System.Drawing.Point(3, 7);
            this.authTypeLabel.Name = "authTypeLabel";
            this.authTypeLabel.Size = new System.Drawing.Size(63, 13);
            this.authTypeLabel.TabIndex = 0;
            this.authTypeLabel.Text = "Login Type:";
            this.authTypeLabel.Click += new System.EventHandler(this.authTypeLabel_Click);
            // 
            // authTypeCb
            // 
            this.authTypeCb.FormattingEnabled = true;
            this.authTypeCb.Items.AddRange(new object[] {
            "google",
            "Ptc"});
            this.authTypeCb.Location = new System.Drawing.Point(68, 4);
            this.authTypeCb.Name = "authTypeCb";
            this.authTypeCb.Size = new System.Drawing.Size(155, 21);
            this.authTypeCb.TabIndex = 1;
            this.authTypeCb.SelectedIndexChanged += new System.EventHandler(this.authTypeCb_SelectedIndexChanged);
            // 
            // ptcUserLabel
            // 
            this.ptcUserLabel.AutoSize = true;
            this.ptcUserLabel.Location = new System.Drawing.Point(3, 36);
            this.ptcUserLabel.Name = "ptcUserLabel";
            this.ptcUserLabel.Size = new System.Drawing.Size(58, 13);
            this.ptcUserLabel.TabIndex = 2;
            this.ptcUserLabel.Text = "Username:";
            // 
            // ptcPasswordLabel
            // 
            this.ptcPasswordLabel.AutoSize = true;
            this.ptcPasswordLabel.Location = new System.Drawing.Point(3, 62);
            this.ptcPasswordLabel.Name = "ptcPasswordLabel";
            this.ptcPasswordLabel.Size = new System.Drawing.Size(56, 13);
            this.ptcPasswordLabel.TabIndex = 3;
            this.ptcPasswordLabel.Text = "Password:";
            // 
            // latLabel
            // 
            this.latLabel.AutoSize = true;
            this.latLabel.Location = new System.Drawing.Point(3, 88);
            this.latLabel.Name = "latLabel";
            this.latLabel.Size = new System.Drawing.Size(48, 13);
            this.latLabel.TabIndex = 4;
            this.latLabel.Text = "Latitude:";
            // 
            // longiLabel
            // 
            this.longiLabel.AutoSize = true;
            this.longiLabel.Location = new System.Drawing.Point(3, 114);
            this.longiLabel.Name = "longiLabel";
            this.longiLabel.Size = new System.Drawing.Size(57, 13);
            this.longiLabel.TabIndex = 5;
            this.longiLabel.Text = "Longitude:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 140);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Razzberry Mode:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 193);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Transfer Type:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 295);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Evolve Pokemon:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 220);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "CP Threshold:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 167);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Razzberry Setting:";
            // 
            // ptcUserText
            // 
            this.ptcUserText.Location = new System.Drawing.Point(68, 33);
            this.ptcUserText.Name = "ptcUserText";
            this.ptcUserText.Size = new System.Drawing.Size(136, 20);
            this.ptcUserText.TabIndex = 11;
            // 
            // ptcPassText
            // 
            this.ptcPassText.Location = new System.Drawing.Point(68, 62);
            this.ptcPassText.Name = "ptcPassText";
            this.ptcPassText.Size = new System.Drawing.Size(136, 20);
            this.ptcPassText.TabIndex = 12;
            // 
            // latitudeText
            // 
            this.latitudeText.Location = new System.Drawing.Point(104, 85);
            this.latitudeText.Name = "latitudeText";
            this.latitudeText.ReadOnly = true;
            this.latitudeText.Size = new System.Drawing.Size(119, 20);
            this.latitudeText.TabIndex = 13;
            // 
            // longitudeText
            // 
            this.longitudeText.Location = new System.Drawing.Point(104, 111);
            this.longitudeText.Name = "longitudeText";
            this.longitudeText.ReadOnly = true;
            this.longitudeText.Size = new System.Drawing.Size(119, 20);
            this.longitudeText.TabIndex = 14;
            // 
            // razzmodeCb
            // 
            this.razzmodeCb.FormattingEnabled = true;
            this.razzmodeCb.Items.AddRange(new object[] {
            "probability",
            "cp"});
            this.razzmodeCb.Location = new System.Drawing.Point(104, 137);
            this.razzmodeCb.Name = "razzmodeCb";
            this.razzmodeCb.Size = new System.Drawing.Size(119, 21);
            this.razzmodeCb.TabIndex = 15;
            // 
            // razzSettingText
            // 
            this.razzSettingText.Location = new System.Drawing.Point(104, 164);
            this.razzSettingText.Name = "razzSettingText";
            this.razzSettingText.Size = new System.Drawing.Size(119, 20);
            this.razzSettingText.TabIndex = 16;
            // 
            // transferTypeCb
            // 
            this.transferTypeCb.FormattingEnabled = true;
            this.transferTypeCb.Items.AddRange(new object[] {
            "None",
            "CP",
            "IV",
            "Leave Strongest",
            "Duplicate",
            "IV Duplicate",
            "All"});
            this.transferTypeCb.Location = new System.Drawing.Point(104, 190);
            this.transferTypeCb.Name = "transferTypeCb";
            this.transferTypeCb.Size = new System.Drawing.Size(119, 21);
            this.transferTypeCb.TabIndex = 17;
            this.transferTypeCb.SelectedIndexChanged += new System.EventHandler(this.transferTypeCb_SelectedIndexChanged);
            // 
            // transferCpThresText
            // 
            this.transferCpThresText.Location = new System.Drawing.Point(104, 220);
            this.transferCpThresText.Name = "transferCpThresText";
            this.transferCpThresText.Size = new System.Drawing.Size(100, 20);
            this.transferCpThresText.TabIndex = 18;
            this.transferCpThresText.TextChanged += new System.EventHandler(this.transferCpThresText_TextChanged);
            // 
            // evolveAllChk
            // 
            this.evolveAllChk.AutoSize = true;
            this.evolveAllChk.Location = new System.Drawing.Point(104, 295);
            this.evolveAllChk.Name = "evolveAllChk";
            this.evolveAllChk.Size = new System.Drawing.Size(15, 14);
            this.evolveAllChk.TabIndex = 19;
            this.evolveAllChk.UseVisualStyleBackColor = true;
            this.evolveAllChk.CheckedChanged += new System.EventHandler(this.evolveAllChk_CheckedChanged);
            // 
            // saveBtn
            // 
            this.saveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.saveBtn.Location = new System.Drawing.Point(0, 578);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(229, 95);
            this.saveBtn.TabIndex = 20;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // gMapControl1
            // 
            this.gMapControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl1.BackColor = System.Drawing.SystemColors.Info;
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(3, 16);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 2;
            this.gMapControl1.MinZoom = 2;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(810, 625);
            this.gMapControl1.TabIndex = 22;
            this.gMapControl1.Zoom = 0D;
            this.gMapControl1.Load += new System.EventHandler(this.gMapControl1_Load);
            this.gMapControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.FindAdressButton);
            this.groupBox1.Controls.Add(this.AdressBox);
            this.groupBox1.Controls.Add(this.trackBar);
            this.groupBox1.Controls.Add(this.gMapControl1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(238, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(816, 679);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Location";
            // 
            // FindAdressButton
            // 
            this.FindAdressButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.FindAdressButton.Location = new System.Drawing.Point(680, 647);
            this.FindAdressButton.Name = "FindAdressButton";
            this.FindAdressButton.Size = new System.Drawing.Size(130, 26);
            this.FindAdressButton.TabIndex = 25;
            this.FindAdressButton.Text = "Find Location";
            this.FindAdressButton.UseVisualStyleBackColor = true;
            this.FindAdressButton.Click += new System.EventHandler(this.FindAdressButton_Click_1);
            // 
            // AdressBox
            // 
            this.AdressBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AdressBox.Location = new System.Drawing.Point(6, 651);
            this.AdressBox.Name = "AdressBox";
            this.AdressBox.Size = new System.Drawing.Size(668, 20);
            this.AdressBox.TabIndex = 25;
            // 
            // trackBar
            // 
            this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar.BackColor = System.Drawing.SystemColors.Info;
            this.trackBar.Location = new System.Drawing.Point(768, 16);
            this.trackBar.Name = "trackBar";
            this.trackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar.Size = new System.Drawing.Size(45, 104);
            this.trackBar.TabIndex = 25;
            this.trackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtTravelParam);
            this.panel1.Controls.Add(this.lblTravelParam);
            this.panel1.Controls.Add(this.comboBox2);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.lblHyper);
            this.panel1.Controls.Add(this.txtHyper);
            this.panel1.Controls.Add(this.txtSuper);
            this.panel1.Controls.Add(this.lblSuper);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.ImageSizeBox);
            this.panel1.Controls.Add(this.EmailLoginText);
            this.panel1.Controls.Add(this.CatchPokemonBox);
            this.panel1.Controls.Add(this.EmailPasswordText);
            this.panel1.Controls.Add(this.CatchPokemonText);
            this.panel1.Controls.Add(this.EmailPasswordBox);
            this.panel1.Controls.Add(this.EmailLoginBox);
            this.panel1.Controls.Add(this.transferIVThresText);
            this.panel1.Controls.Add(this.TravelSpeedText);
            this.panel1.Controls.Add(this.TravelSpeedBox);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.authTypeLabel);
            this.panel1.Controls.Add(this.authTypeCb);
            this.panel1.Controls.Add(this.ptcUserLabel);
            this.panel1.Controls.Add(this.ptcPasswordLabel);
            this.panel1.Controls.Add(this.saveBtn);
            this.panel1.Controls.Add(this.latLabel);
            this.panel1.Controls.Add(this.evolveAllChk);
            this.panel1.Controls.Add(this.longiLabel);
            this.panel1.Controls.Add(this.transferCpThresText);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.transferTypeCb);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.razzSettingText);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.razzmodeCb);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.longitudeText);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.latitudeText);
            this.panel1.Controls.Add(this.ptcUserText);
            this.panel1.Controls.Add(this.ptcPassText);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(9, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(229, 679);
            this.panel1.TabIndex = 26;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "All in range",
            "All without saving",
            "All with saving",
            "No Pokéstop",
            "X Pokéstop"});
            this.comboBox2.Location = new System.Drawing.Point(104, 394);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(119, 21);
            this.comboBox2.TabIndex = 40;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 397);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(69, 13);
            this.label9.TabIndex = 39;
            this.label9.Text = "Travel mode:";
            // 
            // lblHyper
            // 
            this.lblHyper.AutoSize = true;
            this.lblHyper.Location = new System.Drawing.Point(3, 371);
            this.lblHyper.Name = "lblHyper";
            this.lblHyper.Size = new System.Drawing.Size(86, 13);
            this.lblHyper.TabIndex = 38;
            this.lblHyper.Text = "CP for Hyperball:";
            // 
            // txtHyper
            // 
            this.txtHyper.Location = new System.Drawing.Point(104, 368);
            this.txtHyper.Name = "txtHyper";
            this.txtHyper.Size = new System.Drawing.Size(119, 20);
            this.txtHyper.TabIndex = 37;
            // 
            // txtSuper
            // 
            this.txtSuper.Location = new System.Drawing.Point(104, 342);
            this.txtSuper.Name = "txtSuper";
            this.txtSuper.Size = new System.Drawing.Size(119, 20);
            this.txtSuper.TabIndex = 36;
            // 
            // lblSuper
            // 
            this.lblSuper.AutoSize = true;
            this.lblSuper.Location = new System.Drawing.Point(3, 345);
            this.lblSuper.Name = "lblSuper";
            this.lblSuper.Size = new System.Drawing.Size(86, 13);
            this.lblSuper.TabIndex = 35;
            this.lblSuper.Text = "CP for Superball:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "IV",
            "CP"});
            this.comboBox1.Location = new System.Drawing.Point(104, 315);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(119, 21);
            this.comboBox1.TabIndex = 34;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 318);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 13);
            this.label8.TabIndex = 33;
            this.label8.Text = "Pokeball mode:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 249);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 13);
            this.label7.TabIndex = 32;
            this.label7.Text = "PokeUi Image Size:";
            // 
            // ImageSizeBox
            // 
            this.ImageSizeBox.Location = new System.Drawing.Point(104, 246);
            this.ImageSizeBox.Name = "ImageSizeBox";
            this.ImageSizeBox.Size = new System.Drawing.Size(119, 20);
            this.ImageSizeBox.TabIndex = 31;
            // 
            // EmailLoginText
            // 
            this.EmailLoginText.AutoSize = true;
            this.EmailLoginText.Location = new System.Drawing.Point(3, 36);
            this.EmailLoginText.Name = "EmailLoginText";
            this.EmailLoginText.Size = new System.Drawing.Size(35, 13);
            this.EmailLoginText.TabIndex = 27;
            this.EmailLoginText.Text = "Email:";
            // 
            // CatchPokemonBox
            // 
            this.CatchPokemonBox.AutoSize = true;
            this.CatchPokemonBox.Location = new System.Drawing.Point(104, 272);
            this.CatchPokemonBox.Name = "CatchPokemonBox";
            this.CatchPokemonBox.Size = new System.Drawing.Size(15, 14);
            this.CatchPokemonBox.TabIndex = 26;
            this.CatchPokemonBox.UseVisualStyleBackColor = true;
            this.CatchPokemonBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // EmailPasswordText
            // 
            this.EmailPasswordText.AutoSize = true;
            this.EmailPasswordText.Location = new System.Drawing.Point(3, 62);
            this.EmailPasswordText.Name = "EmailPasswordText";
            this.EmailPasswordText.Size = new System.Drawing.Size(56, 13);
            this.EmailPasswordText.TabIndex = 28;
            this.EmailPasswordText.Text = "Password:";
            // 
            // CatchPokemonText
            // 
            this.CatchPokemonText.AutoSize = true;
            this.CatchPokemonText.Location = new System.Drawing.Point(3, 272);
            this.CatchPokemonText.Name = "CatchPokemonText";
            this.CatchPokemonText.Size = new System.Drawing.Size(86, 13);
            this.CatchPokemonText.TabIndex = 25;
            this.CatchPokemonText.Text = "Catch Pokemon:";
            this.CatchPokemonText.Click += new System.EventHandler(this.label7_Click);
            // 
            // EmailPasswordBox
            // 
            this.EmailPasswordBox.Location = new System.Drawing.Point(68, 62);
            this.EmailPasswordBox.Name = "EmailPasswordBox";
            this.EmailPasswordBox.Size = new System.Drawing.Size(155, 20);
            this.EmailPasswordBox.TabIndex = 30;
            // 
            // EmailLoginBox
            // 
            this.EmailLoginBox.Location = new System.Drawing.Point(68, 33);
            this.EmailLoginBox.Name = "EmailLoginBox";
            this.EmailLoginBox.Size = new System.Drawing.Size(155, 20);
            this.EmailLoginBox.TabIndex = 29;
            // 
            // transferIVThresText
            // 
            this.transferIVThresText.Location = new System.Drawing.Point(104, 220);
            this.transferIVThresText.Name = "transferIVThresText";
            this.transferIVThresText.Size = new System.Drawing.Size(119, 20);
            this.transferIVThresText.TabIndex = 24;
            this.transferIVThresText.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // TravelSpeedText
            // 
            this.TravelSpeedText.AutoSize = true;
            this.TravelSpeedText.Location = new System.Drawing.Point(1, 490);
            this.TravelSpeedText.Name = "TravelSpeedText";
            this.TravelSpeedText.Size = new System.Drawing.Size(102, 13);
            this.TravelSpeedText.TabIndex = 23;
            this.TravelSpeedText.Text = "Travel Speed km/h:";
            // 
            // TravelSpeedBox
            // 
            this.TravelSpeedBox.Location = new System.Drawing.Point(104, 487);
            this.TravelSpeedBox.Name = "TravelSpeedBox";
            this.TravelSpeedBox.Size = new System.Drawing.Size(119, 20);
            this.TravelSpeedBox.TabIndex = 22;
            this.TravelSpeedBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 220);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "IV Threshold:";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // lblTravelParam
            // 
            this.lblTravelParam.AutoSize = true;
            this.lblTravelParam.Location = new System.Drawing.Point(3, 424);
            this.lblTravelParam.Name = "lblTravelParam";
            this.lblTravelParam.Size = new System.Drawing.Size(37, 13);
            this.lblTravelParam.TabIndex = 41;
            this.lblTravelParam.Text = "Param";
            // 
            // txtTravelParam
            // 
            this.txtTravelParam.Location = new System.Drawing.Point(104, 421);
            this.txtTravelParam.Name = "txtTravelParam";
            this.txtTravelParam.Size = new System.Drawing.Size(119, 20);
            this.txtTravelParam.TabIndex = 42;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1063, 697);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(630, 360);
            this.Name = "SettingsForm";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label authTypeLabel;
        private System.Windows.Forms.ComboBox authTypeCb;
        private System.Windows.Forms.Label ptcUserLabel;
        private System.Windows.Forms.Label ptcPasswordLabel;
        private System.Windows.Forms.Label latLabel;
        private System.Windows.Forms.Label longiLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox ptcUserText;
        private System.Windows.Forms.TextBox ptcPassText;
        private System.Windows.Forms.TextBox latitudeText;
        private System.Windows.Forms.TextBox longitudeText;
        private System.Windows.Forms.ComboBox razzmodeCb;
        private System.Windows.Forms.TextBox razzSettingText;
        private System.Windows.Forms.ComboBox transferTypeCb;
        private System.Windows.Forms.TextBox transferCpThresText;
        private System.Windows.Forms.CheckBox evolveAllChk;
        private System.Windows.Forms.Button saveBtn;
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TrackBar trackBar;
        private CueTextBox textBoxAddress;
        private System.Windows.Forms.TextBox TravelSpeedBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label TravelSpeedText;
        private System.Windows.Forms.TextBox transferIVThresText;
        private System.Windows.Forms.TextBox AdressBox;
        private System.Windows.Forms.Button FindAdressButton;
        private System.Windows.Forms.CheckBox CatchPokemonBox;
        private System.Windows.Forms.Label CatchPokemonText;
        private System.Windows.Forms.Label EmailLoginText;
        private System.Windows.Forms.Label EmailPasswordText;
        private System.Windows.Forms.TextBox EmailLoginBox;
        private System.Windows.Forms.TextBox EmailPasswordBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox ImageSizeBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblHyper;
        private System.Windows.Forms.TextBox txtHyper;
        private System.Windows.Forms.TextBox txtSuper;
        private System.Windows.Forms.Label lblSuper;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtTravelParam;
        private System.Windows.Forms.Label lblTravelParam;
    }
}
