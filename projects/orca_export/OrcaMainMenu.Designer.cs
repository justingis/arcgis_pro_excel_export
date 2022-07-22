namespace orca_export
{
    partial class OrcaMainMenu
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrcaMainMenu));
            this.Panel_MainMenu = new System.Windows.Forms.Panel();
            this.btnFieldSort = new System.Windows.Forms.Button();
            this.txtName = new System.Windows.Forms.TextBox();
            this.chkVisible = new System.Windows.Forms.CheckBox();
            this.cboMapContent = new System.Windows.Forms.ComboBox();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.chkDateTimeStamp = new System.Windows.Forms.CheckBox();
            this.chkDomainSub = new System.Windows.Forms.CheckBox();
            this.chkFieldAlias = new System.Windows.Forms.CheckBox();
            this.btnFileGDB = new System.Windows.Forms.Button();
            this.btnSHP = new System.Windows.Forms.Button();
            this.btnCSV = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.lblFields = new System.Windows.Forms.Label();
            this.chkListBoxFields = new System.Windows.Forms.CheckedListBox();
            this.chkTables = new System.Windows.Forms.CheckBox();
            this.chkLayers = new System.Windows.Forms.CheckBox();
            this.pic_ArcMapTools = new System.Windows.Forms.PictureBox();
            this.ttipMainMenu = new System.Windows.Forms.ToolTip(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.Panel_MainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ArcMapTools)).BeginInit();
            this.SuspendLayout();
            // 
            // Panel_MainMenu
            // 
            this.Panel_MainMenu.BackColor = System.Drawing.Color.White;
            this.Panel_MainMenu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel_MainMenu.Controls.Add(this.btnFieldSort);
            this.Panel_MainMenu.Controls.Add(this.txtName);
            this.Panel_MainMenu.Controls.Add(this.chkVisible);
            this.Panel_MainMenu.Controls.Add(this.cboMapContent);
            this.Panel_MainMenu.Controls.Add(this.btnDown);
            this.Panel_MainMenu.Controls.Add(this.btnUp);
            this.Panel_MainMenu.Controls.Add(this.btnSettings);
            this.Panel_MainMenu.Controls.Add(this.chkDateTimeStamp);
            this.Panel_MainMenu.Controls.Add(this.chkDomainSub);
            this.Panel_MainMenu.Controls.Add(this.chkFieldAlias);
            this.Panel_MainMenu.Controls.Add(this.btnFileGDB);
            this.Panel_MainMenu.Controls.Add(this.btnSHP);
            this.Panel_MainMenu.Controls.Add(this.btnCSV);
            this.Panel_MainMenu.Controls.Add(this.btnExcel);
            this.Panel_MainMenu.Controls.Add(this.lblFields);
            this.Panel_MainMenu.Controls.Add(this.chkListBoxFields);
            this.Panel_MainMenu.Controls.Add(this.chkTables);
            this.Panel_MainMenu.Controls.Add(this.chkLayers);
            this.Panel_MainMenu.Controls.Add(this.pic_ArcMapTools);
            this.Panel_MainMenu.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Panel_MainMenu.Location = new System.Drawing.Point(8, 7);
            this.Panel_MainMenu.Name = "Panel_MainMenu";
            this.Panel_MainMenu.Size = new System.Drawing.Size(659, 456);
            this.Panel_MainMenu.TabIndex = 2;
            // 
            // btnFieldSort
            // 
            this.btnFieldSort.BackColor = System.Drawing.Color.White;
            this.btnFieldSort.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFieldSort.BackgroundImage")));
            this.btnFieldSort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFieldSort.Location = new System.Drawing.Point(447, 369);
            this.btnFieldSort.Name = "btnFieldSort";
            this.btnFieldSort.Size = new System.Drawing.Size(25, 25);
            this.btnFieldSort.TabIndex = 55;
            this.btnFieldSort.UseVisualStyleBackColor = false;
            this.btnFieldSort.Click += new System.EventHandler(this.btnFieldSort_Click);
            // 
            // txtName
            // 
            this.txtName.ForeColor = System.Drawing.SystemColors.GrayText;
            this.txtName.Location = new System.Drawing.Point(225, 402);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(216, 20);
            this.txtName.TabIndex = 23;
            this.txtName.Text = "Name (Optional)";
            this.txtName.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtName_MouseClick);
            this.txtName.Leave += new System.EventHandler(this.txtName_Leave);
            // 
            // chkVisible
            // 
            this.chkVisible.AutoSize = true;
            this.chkVisible.Checked = true;
            this.chkVisible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVisible.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkVisible.Location = new System.Drawing.Point(156, 19);
            this.chkVisible.Name = "chkVisible";
            this.chkVisible.Size = new System.Drawing.Size(56, 17);
            this.chkVisible.TabIndex = 22;
            this.chkVisible.Text = "Visible";
            this.chkVisible.UseVisualStyleBackColor = true;
            // 
            // cboMapContent
            // 
            this.cboMapContent.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cboMapContent.FormattingEnabled = true;
            this.cboMapContent.Location = new System.Drawing.Point(13, 45);
            this.cboMapContent.Name = "cboMapContent";
            this.cboMapContent.Size = new System.Drawing.Size(199, 21);
            this.cboMapContent.TabIndex = 21;
            this.cboMapContent.Text = "Select layer or table";
            this.cboMapContent.DropDown += new System.EventHandler(this.cboMapContent_DropDown);
            this.cboMapContent.SelectedIndexChanged += new System.EventHandler(this.cboMapContent_SelectedIndexChanged);
            // 
            // btnDown
            // 
            this.btnDown.BackColor = System.Drawing.Color.White;
            this.btnDown.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnDown.BackgroundImage")));
            this.btnDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDown.Location = new System.Drawing.Point(447, 218);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(30, 30);
            this.btnDown.TabIndex = 20;
            this.btnDown.UseVisualStyleBackColor = false;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.BackColor = System.Drawing.Color.White;
            this.btnUp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnUp.BackgroundImage")));
            this.btnUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnUp.Location = new System.Drawing.Point(447, 181);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(30, 30);
            this.btnUp.TabIndex = 19;
            this.btnUp.UseVisualStyleBackColor = false;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.BackColor = System.Drawing.Color.White;
            this.btnSettings.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSettings.BackgroundImage")));
            this.btnSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSettings.Location = new System.Drawing.Point(628, 3);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(26, 24);
            this.btnSettings.TabIndex = 18;
            this.btnSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btnSettings.UseVisualStyleBackColor = false;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // chkDateTimeStamp
            // 
            this.chkDateTimeStamp.AutoSize = true;
            this.chkDateTimeStamp.Location = new System.Drawing.Point(515, 353);
            this.chkDateTimeStamp.Name = "chkDateTimeStamp";
            this.chkDateTimeStamp.Size = new System.Drawing.Size(108, 17);
            this.chkDateTimeStamp.TabIndex = 14;
            this.chkDateTimeStamp.Text = "Date Time Stamp";
            this.chkDateTimeStamp.UseVisualStyleBackColor = true;
            // 
            // chkDomainSub
            // 
            this.chkDomainSub.AutoSize = true;
            this.chkDomainSub.Checked = true;
            this.chkDomainSub.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDomainSub.Location = new System.Drawing.Point(515, 317);
            this.chkDomainSub.Name = "chkDomainSub";
            this.chkDomainSub.Size = new System.Drawing.Size(122, 30);
            this.chkDomainSub.TabIndex = 13;
            this.chkDomainSub.Text = "Use domain and \r\nsubtype descriptions";
            this.chkDomainSub.UseVisualStyleBackColor = true;
            // 
            // chkFieldAlias
            // 
            this.chkFieldAlias.AutoSize = true;
            this.chkFieldAlias.Checked = true;
            this.chkFieldAlias.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFieldAlias.Location = new System.Drawing.Point(515, 281);
            this.chkFieldAlias.Name = "chkFieldAlias";
            this.chkFieldAlias.Size = new System.Drawing.Size(108, 30);
            this.chkFieldAlias.TabIndex = 12;
            this.chkFieldAlias.Text = "Use field alias as \r\ncolumn header";
            this.chkFieldAlias.UseVisualStyleBackColor = true;
            this.chkFieldAlias.CheckedChanged += new System.EventHandler(this.chkFieldAlias_CheckedChanged);
            // 
            // btnFileGDB
            // 
            this.btnFileGDB.BackColor = System.Drawing.Color.LightGray;
            this.btnFileGDB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFileGDB.Location = new System.Drawing.Point(565, 207);
            this.btnFileGDB.Name = "btnFileGDB";
            this.btnFileGDB.Size = new System.Drawing.Size(72, 48);
            this.btnFileGDB.TabIndex = 11;
            this.btnFileGDB.Text = "File GDB";
            this.btnFileGDB.UseVisualStyleBackColor = false;
            this.btnFileGDB.Click += new System.EventHandler(this.btnFileGDB_Click);
            // 
            // btnSHP
            // 
            this.btnSHP.BackColor = System.Drawing.Color.Khaki;
            this.btnSHP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSHP.Location = new System.Drawing.Point(565, 153);
            this.btnSHP.Name = "btnSHP";
            this.btnSHP.Size = new System.Drawing.Size(72, 48);
            this.btnSHP.TabIndex = 10;
            this.btnSHP.Text = "SHP";
            this.btnSHP.UseVisualStyleBackColor = false;
            this.btnSHP.Click += new System.EventHandler(this.btnSHP_Click);
            // 
            // btnCSV
            // 
            this.btnCSV.BackColor = System.Drawing.Color.White;
            this.btnCSV.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCSV.Location = new System.Drawing.Point(565, 99);
            this.btnCSV.Name = "btnCSV";
            this.btnCSV.Size = new System.Drawing.Size(72, 48);
            this.btnCSV.TabIndex = 9;
            this.btnCSV.Text = "CSV";
            this.btnCSV.UseVisualStyleBackColor = false;
            this.btnCSV.Click += new System.EventHandler(this.btnCSV_Click);
            // 
            // btnExcel
            // 
            this.btnExcel.BackColor = System.Drawing.Color.LightGreen;
            this.btnExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExcel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnExcel.Location = new System.Drawing.Point(565, 45);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(72, 48);
            this.btnExcel.TabIndex = 8;
            this.btnExcel.Text = "Excel";
            this.btnExcel.UseVisualStyleBackColor = false;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // lblFields
            // 
            this.lblFields.AutoSize = true;
            this.lblFields.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFields.Location = new System.Drawing.Point(302, 21);
            this.lblFields.Name = "lblFields";
            this.lblFields.Size = new System.Drawing.Size(34, 13);
            this.lblFields.TabIndex = 7;
            this.lblFields.Text = "Fields";
            // 
            // chkListBoxFields
            // 
            this.chkListBoxFields.CheckOnClick = true;
            this.chkListBoxFields.FormattingEnabled = true;
            this.chkListBoxFields.Location = new System.Drawing.Point(225, 45);
            this.chkListBoxFields.Name = "chkListBoxFields";
            this.chkListBoxFields.Size = new System.Drawing.Size(216, 349);
            this.chkListBoxFields.TabIndex = 6;
            this.chkListBoxFields.SelectedIndexChanged += new System.EventHandler(this.chkListBoxFields_SelectedIndexChanged);
            this.chkListBoxFields.DoubleClick += new System.EventHandler(this.chkListBoxFields_DoubleClick);
            // 
            // chkTables
            // 
            this.chkTables.AutoSize = true;
            this.chkTables.Checked = true;
            this.chkTables.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTables.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkTables.Location = new System.Drawing.Point(82, 19);
            this.chkTables.Name = "chkTables";
            this.chkTables.Size = new System.Drawing.Size(58, 17);
            this.chkTables.TabIndex = 5;
            this.chkTables.Text = "Tables";
            this.chkTables.UseVisualStyleBackColor = true;
            // 
            // chkLayers
            // 
            this.chkLayers.AutoSize = true;
            this.chkLayers.Checked = true;
            this.chkLayers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLayers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkLayers.Location = new System.Drawing.Point(13, 19);
            this.chkLayers.Name = "chkLayers";
            this.chkLayers.Size = new System.Drawing.Size(57, 17);
            this.chkLayers.TabIndex = 4;
            this.chkLayers.Text = "Layers";
            this.chkLayers.UseVisualStyleBackColor = true;
            // 
            // pic_ArcMapTools
            // 
            this.pic_ArcMapTools.Image = ((System.Drawing.Image)(resources.GetObject("pic_ArcMapTools.Image")));
            this.pic_ArcMapTools.Location = new System.Drawing.Point(534, 411);
            this.pic_ArcMapTools.Name = "pic_ArcMapTools";
            this.pic_ArcMapTools.Size = new System.Drawing.Size(117, 39);
            this.pic_ArcMapTools.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pic_ArcMapTools.TabIndex = 0;
            this.pic_ArcMapTools.TabStop = false;
            this.pic_ArcMapTools.Click += new System.EventHandler(this.pic_ArcMapTools_Click);
            this.pic_ArcMapTools.MouseEnter += new System.EventHandler(this.pic_ArcMapTools_MouseEnter);
            this.pic_ArcMapTools.MouseLeave += new System.EventHandler(this.pic_ArcMapTools_MouseLeave);
            // 
            // OrcaMainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(675, 470);
            this.Controls.Add(this.Panel_MainMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "OrcaMainMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Orca Export";
            this.Load += new System.EventHandler(this.OrcaMainMenu_Load);
            this.Panel_MainMenu.ResumeLayout(false);
            this.Panel_MainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ArcMapTools)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_MainMenu;
        private System.Windows.Forms.CheckBox chkVisible;
        private System.Windows.Forms.ComboBox cboMapContent;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.CheckBox chkDateTimeStamp;
        private System.Windows.Forms.CheckBox chkDomainSub;
        private System.Windows.Forms.CheckBox chkFieldAlias;
        private System.Windows.Forms.Button btnFileGDB;
        private System.Windows.Forms.Button btnSHP;
        private System.Windows.Forms.Button btnCSV;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Label lblFields;
        private System.Windows.Forms.CheckBox chkTables;
        private System.Windows.Forms.CheckBox chkLayers;
        private System.Windows.Forms.PictureBox pic_ArcMapTools;
        private System.Windows.Forms.ToolTip ttipMainMenu;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.CheckedListBox chkListBoxFields;
        internal System.Windows.Forms.Button btnFieldSort;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}