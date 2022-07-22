using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace orca_export
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void SetCancelButton(Button myCancelBtn)
        {
            this.CancelButton = myCancelBtn;
        }

        private void btnSaveAppSettings_Click(object sender, EventArgs e)
        {
            orca_export.Properties.Settings.Default.zipSHP = chkZipSHP.Checked;
            orca_export.Properties.Settings.Default.openInExcel = radioOpenInExcel.Checked;
            orca_export.Properties.Settings.Default.saveToExcelFile = radioSaveToExcel.Checked;
            orca_export.Properties.Settings.Default.formatAsText = chkFormatNumText.Checked;
            orca_export.Properties.Settings.Default.quickExportFormat = cboQuickExportFormat.SelectedItem.ToString();
            orca_export.Properties.Settings.Default.q_DateTimeStamp = chkQDateTime.Checked;
            orca_export.Properties.Settings.Default.qFieldAlias = chkQFieldAlias.Checked;
            orca_export.Properties.Settings.Default.qDomainSub = chkQDomainSub.Checked;


            orca_export.Properties.Settings.Default.Save();
            this.Close();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            Type officeType = Type.GetTypeFromProgID("Excel.Application");
            if (officeType == null)
            {
                panel2.Enabled = false;
                cboQuickExportFormat.Items.Remove("Excel");
                cboQuickExportFormat.SelectedItem = "CSV";
            }
            else
            {
                panel2.Enabled = true;
                cboQuickExportFormat.SelectedItem = orca_export.Properties.Settings.Default.quickExportFormat;
            }

            if (cboQuickExportFormat.Text == "CSV")
            {
                chkQDateTime.Enabled = false;
                chkQDateTime.Checked = false;
            }
            else
            {
                chkQDateTime.Enabled = true;
            }

            chkZipSHP.Checked = orca_export.Properties.Settings.Default.zipSHP;
            radioOpenInExcel.Checked = orca_export.Properties.Settings.Default.openInExcel;
            radioSaveToExcel.Checked = orca_export.Properties.Settings.Default.saveToExcelFile;
            //cboQuickExportFormat.SelectedItem = Properties.Settings.Default.quickExportFormat;
            chkFormatNumText.Checked = orca_export.Properties.Settings.Default.formatAsText;
            chkQDateTime.Checked = orca_export.Properties.Settings.Default.q_DateTimeStamp;
            chkQFieldAlias.Checked = orca_export.Properties.Settings.Default.qFieldAlias;
            chkQDomainSub.Checked = orca_export.Properties.Settings.Default.qDomainSub;
            SetCancelButton(btnCancel);
        }

        private void cboQuickExportFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboQuickExportFormat.Text == "CSV")
            {
                chkQDateTime.Enabled = false;
                chkQDateTime.Checked = false;
            }
            else
                chkQDateTime.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
