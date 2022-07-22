using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Core.Data;
using System.IO;
using ArcGIS.Desktop.Core.Geoprocessing;
using System.IO.Compression;
using ArcGIS.Desktop.Catalog;

namespace orca_export
{
    public partial class OrcaMainMenu : Form
    {
        Settings settings = new Settings();
        MapContent currentMapContent = new MapContent();
        List<FieldDescription> fieldNames;
        bool sortAscending = false;
        bool sortDescending = false;
        string name;
        FeatureLayer shapeFileLayer2;

        public OrcaMainMenu()
        {
            InitializeComponent();
        }

        private void OrcaMainMenu_Load(object sender, EventArgs e)
        {
            Type officeType = Type.GetTypeFromProgID("Excel.Application");
            if (officeType == null)
            {
                btnExcel.Enabled = false;
            }
            else
            {
                btnExcel.Enabled = true;
            }
            SetToolTips();
        }

        private void SetToolTips()
        {
            ttipMainMenu.SetToolTip(this.btnExcel, "Export to Excel");
            ttipMainMenu.SetToolTip(this.btnSettings, "Application Settings");
            ttipMainMenu.SetToolTip(this.btnCSV, "Export to CSV");
            ttipMainMenu.SetToolTip(this.btnSHP, "Export to Shapefile");
            ttipMainMenu.SetToolTip(this.btnFileGDB, "Export to file geodatabase");
            ttipMainMenu.SetToolTip(this.btnUp, "Move selected field up");
            ttipMainMenu.SetToolTip(this.btnDown, "Move selected field down");
            ttipMainMenu.SetToolTip(this.btnFieldSort, "Sort fields alphabetically");
            ttipMainMenu.SetToolTip(this.txtName, "Optional name for output dataset"); //dosent work
        }

        private void ResetTextBoxText(TextBox inputTextBox,string defaultText)
        {
            if (string.IsNullOrWhiteSpace(inputTextBox.Text))
            {
                inputTextBox.Text = defaultText;
                inputTextBox.ForeColor = Color.Gray;
            }
        }

        private void ClearTextBox(TextBox inputTextBox, string defaultText)
        {
            if (inputTextBox.Text == defaultText)
            {
                inputTextBox.Clear();
                inputTextBox.ForeColor = Color.Black;
            }
        }

        private void txtName_MouseClick(object sender, MouseEventArgs e)
        {
            ClearTextBox(txtName, "Name (Optional)");
        }

        private void txtName_Leave(object sender, EventArgs e)
        {
            ResetTextBoxText(txtName, "Name (Optional)");
        }


        private ArrayList GetFeatureLayers()
        {
            ArrayList currentMapLayers = new ArrayList();
            currentMapLayers = currentMapContent.GetFeatureLayers(chkVisible.Checked, chkLayers.Checked);
            return currentMapLayers;
        }
        private ArrayList GetStandAloneTables()
        {
            ArrayList currentStandAloneTables = new ArrayList();
            currentStandAloneTables = currentMapContent.GetStandAloneTables(chkTables.Checked);
            return currentStandAloneTables;
        }


        private void PopulateLayerDrowndown(ComboBox inputComboBox)
        {
            inputComboBox.Items.Clear();
            ArrayList featureLayers = GetFeatureLayers();
            foreach (var layer in featureLayers)
            {
                inputComboBox.Items.Add(layer);
            }

            ArrayList standAloneTables = GetStandAloneTables();
            foreach (var table in standAloneTables)
            {
                inputComboBox.Items.Add(table);
            }
        }

        private void cboMapContent_DropDown(object sender, EventArgs e)
        {
            PopulateLayerDrowndown(cboMapContent);
            ResizeComboBox(sender, e);
        }

        private void SetSelectedMapContent()
        {
            try
            {
                currentMapContent.SelectedLayer = (FeatureLayer)cboMapContent.SelectedItem;
                currentMapContent.IsLayer = true;
                currentMapContent.IsTable = false;
            }
            catch
            {
                currentMapContent.SelectedTable = (StandaloneTable)cboMapContent.SelectedItem;
                currentMapContent.IsTable = true;
                currentMapContent.IsLayer = false;
            }

            if (currentMapContent.IsTable)
                btnSHP.Enabled = false;
            else
                btnSHP.Enabled = true;
        }

        private void cboMapContent_SelectedIndexChanged(object sender, EventArgs e)
        {
            QueuedTask.Run(() =>
            {
                SetSelectedMapContent();
                sortAscending = false;
                sortDescending = false;
                AddFieldsToChkBox(sortAscending, sortDescending);
            });
        }

        private void RemoveUnwantedFields(List<FieldDescription> inputCurrentFields) //specifies field types to not include such as Geometry type
        {
            ArrayList fieldTypesToRemove = new ArrayList();
            fieldTypesToRemove.Add("Geometry");
            for (int i=0; i < inputCurrentFields.Count; i++)
            {
                string dataType = inputCurrentFields[i].Type.ToString();
                if (fieldTypesToRemove.Contains(inputCurrentFields[i].Type.ToString()))
                {
                    inputCurrentFields.Remove(inputCurrentFields[i]);
                }
            }
        }

        private void SelectAllFields(CheckedListBox inputCheckListBox)
        {
            if (inputCheckListBox.SelectedIndex == 0)
            {
                if (inputCheckListBox.GetItemCheckState(0).ToString() == "Checked")
                {
                    for (int v = 1; v < inputCheckListBox.Items.Count; v++)
                    {
                        inputCheckListBox.SetItemChecked(v, true);
                    }
                }
                else
                {
                    for (int v = 1; v < inputCheckListBox.Items.Count; v++)
                    {
                        inputCheckListBox.SetItemChecked(v, false);
                    }
                }

            }
        }

        private void AddFieldsToChkBox(bool inputSortAscending, bool inputSortDescending)
        {
            currentMapContent.FieldNames.Clear();
            chkListBoxFields.Items.Clear();
            chkListBoxFields.Sorted = false;
            chkListBoxFields.Items.Add("<Select All>");
            chkListBoxFields.SetItemChecked(0, true);
            int fieldIndex = 1;
            fieldNames = currentMapContent.GetSelectedContentFields();
            RemoveUnwantedFields(fieldNames);
            List<FieldDescription> fieldNames2 = new List<FieldDescription>();
            if (inputSortAscending == true || inputSortDescending == true)
            {
                ArrayList sortedFieldNames = new ArrayList();

                foreach (var field in fieldNames)
                {
                    if (chkFieldAlias.Checked == true)
                    {
                        sortedFieldNames.Add(field.Alias);
                    }
                    else
                        sortedFieldNames.Add(field.Name);
                }

                sortedFieldNames.Sort();
                if (inputSortDescending == true)
                    sortedFieldNames.Reverse();

                foreach (string field_name in sortedFieldNames)
                {
                    foreach (var field in fieldNames)
                    {
                        if (chkFieldAlias.Checked == true)
                        {
                            if (field_name == field.Alias)
                            {
                                fieldNames2.Add(field);
                            }
                        }
                        else if (chkFieldAlias.Checked == false)
                        {
                            if (field_name == field.Name)
                            {
                                fieldNames2.Add(field);
                            }
                        }
                    }
                }
            }
            else
            {
                fieldNames2 = fieldNames;
            }
            

            foreach (var field in fieldNames2)
            {
                if (chkFieldAlias.Checked == true)
                {
                    chkListBoxFields.Items.Add(field.Alias);
                    currentMapContent.FieldNames.Add(field.Name);
                }
                else
                {
                    chkListBoxFields.Items.Add(field.Name);
                    currentMapContent.FieldNames.Add(field.Name);
                }

                if (field.IsVisible == true)
                    chkListBoxFields.SetItemChecked(fieldIndex, true);
                fieldIndex += 1;
            }


        }

        public void ResizeComboBox(object sender, System.EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            int width = senderComboBox.DropDownWidth;
            Graphics g = senderComboBox.CreateGraphics();
            Font font = senderComboBox.Font;
            int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                ? SystemInformation.VerticalScrollBarWidth : 0;
            int newWidth;

            foreach (var s in ((ComboBox)sender).Items) //changed string to var
            {
                newWidth = (int)g.MeasureString(s.ToString(), font).Width //added s.ToSTring() instead of just s
                    + vertScrollBarWidth;
                if (width < newWidth)
                {
                    width = newWidth;
                }
            }
            senderComboBox.DropDownWidth = width;
        }


        private void chkListBoxFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectAllFields(chkListBoxFields);
            //MessageBox.Show(currentMapContent.FieldNames[chkListBoxFields.SelectedIndex - 1]);
            //currentMapContent.FieldNames.RemoveAt(chkListBoxFields.SelectedIndex-1);

            //MessageBox.Show((chkListBoxFields.SelectedIndex).ToString());
            //decrementer++;
            //currentMapContent.FieldNames.RemoveAt(chkListBoxFields.SelectedIndex - decrementer);
        }


        private void SelectAllDoubleClick(CheckedListBox inputCheckListBox)
        {
            if (inputCheckListBox.GetItemCheckState(0).ToString() == "Checked")
            {
                for (int w = 0; w < inputCheckListBox.Items.Count; w++)
                {
                    inputCheckListBox.SetItemChecked(w, true);
                }
            }

            if (inputCheckListBox.GetItemCheckState(0).ToString() == "Unchecked")
            {
                for (int w = 0; w < inputCheckListBox.Items.Count; w++)
                {
                    inputCheckListBox.SetItemChecked(w, false);
                }
            }
        }

        private void chkListBoxFields_DoubleClick(object sender, EventArgs e)
        {
            SelectAllDoubleClick(chkListBoxFields);
        }

        

        private void btnFieldSort_Click(object sender, EventArgs e)
        {
            if (cboMapContent.SelectedItem == null)
            {
                return;
            }
            QueuedTask.Run(() =>
            {
                if (sortDescending == true)
                {
                    AddFieldsToChkBox(false, true);
                    sortDescending = false;
                }
                else
                {
                    AddFieldsToChkBox(true, false);
                    sortDescending = true;
                }
            });
        }


        public void MoveItem(int direction, CheckedListBox inputChecklistBox, IList<string> inputFieldNames)
        {
            inputChecklistBox.Sorted = false;
            if (inputChecklistBox.SelectedItem == null || inputChecklistBox.SelectedIndex < 1)
                return;

            int newIndex = inputChecklistBox.SelectedIndex + direction;

            if (newIndex < 1 || newIndex >= inputChecklistBox.Items.Count)
                return;

            object selected = inputChecklistBox.SelectedItem;
            var item = inputFieldNames[inputChecklistBox.SelectedIndex-1];

            inputFieldNames.RemoveAt(inputChecklistBox.SelectedIndex-1);
            inputChecklistBox.Items.Remove(selected);

            inputFieldNames.Insert(newIndex-1, item);
            inputChecklistBox.Items.Insert(newIndex, selected);

            inputChecklistBox.SetSelected(newIndex, true);
            inputChecklistBox.SetItemChecked(newIndex, true);
        }

       
        private void btnDown_Click(object sender, EventArgs e)
        {
            MoveItem(1, chkListBoxFields, currentMapContent.FieldNames);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            MoveItem(-1, chkListBoxFields, currentMapContent.FieldNames);
        }

        private void chkFieldAlias_CheckedChanged(object sender, EventArgs e)
        {
            QueuedTask.Run(() =>
            {
                UseFieldAlias(chkListBoxFields, chkFieldAlias);
            });
        }

        private void UseFieldAlias(CheckedListBox inputCheckListBox, CheckBox inputCheckBox)
        {
            if (inputCheckListBox.Items.Count == 0)
                return;

            inputCheckListBox.Items.Clear();
            inputCheckListBox.Sorted = false;
            AddFieldsToChkBox(false, false);
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            this.Focus();
            QueuedTask.Run(() =>
            {
                if (chkListBoxFields.Items.Count == 0)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please select a layer or table from the dropdown menu.", "Error");
                    this.UseWaitCursor = false;
                    this.BringToFront();
                    this.Focus();
                    return;
                }
                if (chkListBoxFields.CheckedItems.Count == 0)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please check at least 1 field from the Fields checklist box.", "Error");
                    this.UseWaitCursor = false;
                    this.BringToFront();
                    this.Focus();
                    return;
                }
                currentMapContent.Excel = true;
                currentMapContent.DateTime = chkDateTimeStamp.Checked;
                if (orca_export.Properties.Settings.Default.saveToExcelFile)
                {
                    saveFileDialog1.Title = "Save output Excel file (.xlsx)";
                    saveFileDialog1.Filter = "All files (*.*)|*.*|xlsx files (*.xlsx)|*.xlsx";
                    saveFileDialog1.FilterIndex = 0;
                    saveFileDialog1.RestoreDirectory = true;
                    name = txtName.Text;
                    if (string.IsNullOrWhiteSpace(name) || name == "Name (Optional)")
                    {
                        if (name.Contains(".xlsx"))
                            name = cboMapContent.SelectedItem.ToString();
                        else
                            name = cboMapContent.SelectedItem.ToString() + ".xlsx";
                    }
                    else
                    {
                        if (name.Contains(".xlsx"))
                            name = txtName.Text;
                        else
                            name = txtName.Text + ".xlsx";
                    }

                    saveFileDialog1.FileName = name;
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        SetCheckedFields(chkListBoxFields); //sets the checked fields in the MapContent object
                        currentMapContent.Excel = true;
                        currentMapContent.FileName = name;
                        currentMapContent.FilePath = Path.GetFullPath(saveFileDialog1.FileName);
                        if (!(currentMapContent.FilePath.Contains(".xlsx")))
                            currentMapContent.FilePath = currentMapContent.FilePath + ".xlsx";

                        currentMapContent.FieldCheckListBox = chkListBoxFields;
                        currentMapContent.BuildAttributeTable(cboMapContent.SelectedItem, chkDomainSub.Checked);
                        //MessageBox.Show("Excel file successfully created.", "Success");
                        ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Excel file successfully created.", "Success");
                    }
                    else
                    {
                        this.UseWaitCursor = false;
                        this.Focus();
                        return;
                    }
                }
                else
                {
                    SetCheckedFields(chkListBoxFields); //sets the checked fields in the MapContent object
                    currentMapContent.FileName = txtName.Text;
                    currentMapContent.FieldCheckListBox = chkListBoxFields;
                    currentMapContent.BuildAttributeTable(cboMapContent.SelectedItem, chkDomainSub.Checked);
                }
                this.UseWaitCursor = false;
                this.Focus();
            });
        }

        private void SetCheckedFields(CheckedListBox inputCheckListBox)
        {
            currentMapContent.CheckedFields.Clear();
            for (int i=1; i < inputCheckListBox.Items.Count; i++)
            {
                if (inputCheckListBox.GetItemChecked(i))
                {
                    currentMapContent.CheckedFields.Add(inputCheckListBox.Items[i]);
                }
            }
        }

        private void btnCSV_Click(object sender, EventArgs e)
        {
          
            this.UseWaitCursor = true;
            this.Focus();
            QueuedTask.Run(() =>
            {
                if (chkListBoxFields.Items.Count == 0)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please select a layer or table from the dropdown menu.", "Error");
                    this.UseWaitCursor = false;
                    this.BringToFront();
                    this.Focus();
                    return;
                }
                if (chkListBoxFields.CheckedItems.Count == 0)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please check at least 1 field from the Fields checklist box.", "Error");
                    this.UseWaitCursor = false;
                    this.BringToFront();
                    this.Focus();
                    return;
                }
                currentMapContent.FieldAlias = chkFieldAlias.Checked;
                currentMapContent.DomainSub = chkDomainSub.Checked;
                saveFileDialog1.Title = "Save output CSV file (.csv)";
                saveFileDialog1.Filter = "All files (*.*)|*.*|csv files (*.csv)|*.csv";
                saveFileDialog1.FilterIndex = 0;
                saveFileDialog1.RestoreDirectory = true;
                name = txtName.Text;
                if (string.IsNullOrWhiteSpace(name) || name == "Name (Optional)")
                {
                    if (name.Contains(".csv"))
                        name = cboMapContent.SelectedItem.ToString();
                    else
                        name = cboMapContent.SelectedItem.ToString() + ".csv";
                }
                else
                {
                    if (name.Contains(".csv"))
                        name = txtName.Text;
                    else
                        name = txtName.Text + ".csv";
                }

                saveFileDialog1.FileName = name;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    SetCheckedFields(chkListBoxFields); //sets the checked fields in the MapContent object
                    currentMapContent.CSV = true;
                    currentMapContent.FileName = name;
                    currentMapContent.FilePath = Path.GetFullPath(saveFileDialog1.FileName);
                    if (!(currentMapContent.FilePath.Contains(".csv")))
                        currentMapContent.FilePath = currentMapContent.FilePath + ".csv";

                    currentMapContent.FieldCheckListBox = chkListBoxFields;
                    currentMapContent.BuildAttributeTable(cboMapContent.SelectedItem, chkDomainSub.Checked);
                }
                else
                {
                    this.UseWaitCursor = false;
                    this.Focus();
                    this.BringToFront();
                    return;
                }
                this.UseWaitCursor = false;
                this.Focus();
                this.BringToFront();
            });
        }

        private void btnSHP_Click(object sender, EventArgs e)
        {
            QueuedTask.Run(() =>
            {
                this.UseWaitCursor = true;
                this.Focus();
                if (chkListBoxFields.Items.Count == 0)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please select a layer or table from the dropdown menu.", "Error");
                    this.UseWaitCursor = false;
                    this.BringToFront();
                    this.Focus();
                    return;
                }
                if (chkListBoxFields.CheckedItems.Count == 0)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please check at least 1 field from the Fields checklist box.", "Error");
                    this.UseWaitCursor = false;
                    this.BringToFront();
                    this.Focus();
                    return;
                }
                currentMapContent.FieldAlias = chkFieldAlias.Checked;
                currentMapContent.DomainSub = chkDomainSub.Checked;

                saveFileDialog1.Title = "Save output Shapefile file (.shp)";
                saveFileDialog1.Filter = "All files (*.*)|*.*|shp files (*.shp)|*.shp";
                saveFileDialog1.FilterIndex = 0;

                currentMapContent.FileName = txtName.Text;

                saveFileDialog1.RestoreDirectory = true;
                name = txtName.Text;
                string dropFields = "";
                if (string.IsNullOrWhiteSpace(name) || name == "Name (Optional)")
                {
                    if (name.Contains(".shp"))
                    {
                        //name = cboMapContent.SelectedItem.ToString();
                        string preName = cboMapContent.SelectedItem.ToString().Replace(".shp","");
                        name = preName.Substring(preName.LastIndexOf('.'));
                        name = name.Replace(".", "");
                        name = name + ".shp";
                    }
                    else
                    {

                        string preName = cboMapContent.SelectedItem.ToString();
                        if (preName.Contains("."))
                        {
                            name = preName.Substring(preName.LastIndexOf('.'));
                            name = name.Replace(".", "");
                        }
                        else
                            name = preName;
                        name = name + ".shp";
                        //name = cboMapContent.SelectedItem.ToString() + ".shp";
                    }
                }
                else
                {
                    if (name.Contains(".shp"))
                        name = txtName.Text;
                    else
                        name = txtName.Text + ".shp";
                }

                saveFileDialog1.FileName = name;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    currentMapContent.SHP = true;
                    currentMapContent.CSV = true;
                    SetCheckedFields(chkListBoxFields); //sets the checked fields in the MapContent object

                    currentMapContent.FilePath = Path.GetFullPath(saveFileDialog1.FileName);
                    
                    if (!(currentMapContent.FilePath.Contains(".shp")))
                        currentMapContent.FilePath = currentMapContent.FilePath + ".shp";

                    string outLocation = Path.GetDirectoryName(currentMapContent.FilePath);
                    string oldName = currentMapContent.SelectedLayer.Name + ".shp";
                    string newName = Path.GetFileName(currentMapContent.FilePath);
                    string stageName = newName.Replace(".shp", "");

                    if (oldName != newName)
                    {
                        if (stageName.Contains("."))
                        {
                            ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Shapefiles cannnot contain periods ('.') in the name", "Warning");
                            this.UseWaitCursor = false;
                            this.Focus();
                            this.BringToFront();
                            return;
                        }

                        currentMapContent.SelectedLayer.SetName(newName);
                        var valueArray = Geoprocessing.MakeValueArray(currentMapContent.SelectedLayer, outLocation);
                        var gpresult1 = Geoprocessing.ExecuteToolAsync("FeatureClassToShapefile_conversion", valueArray);
                        oldName = oldName.Replace(".shp", "");
                        currentMapContent.SelectedLayer.SetName(oldName);
                    }
                    else
                    {
                        if (stageName.Contains("."))
                        {
                            ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Shapefiles cannnot contain periods ('.') in the name", "Warning");
                            this.UseWaitCursor = false;
                            this.Focus();
                            this.BringToFront();
                            return;
                        }
                        var valueArray = Geoprocessing.MakeValueArray(currentMapContent.SelectedLayer, outLocation);
                        var gpresult1 = Geoprocessing.ExecuteToolAsync("FeatureClassToShapefile_conversion", valueArray);
                    }

                    Map map = MapView.Active.Map;
                    string uriShp = currentMapContent.FilePath;
                    FeatureLayer shapeFileLayer = LayerFactory.Instance.CreateFeatureLayer(new Uri(uriShp), map); // throwing the error
                    foreach (var field in shapeFileLayer.GetFieldDescriptions())
                    {
                        if (field.Name != "Shape" && field.Name != "FID" && field.Name != "Shape_Leng" && field.Name != "Shape_Area")
                            dropFields += field.Name + ";";
                    }

                    var valueArray2 = Geoprocessing.MakeValueArray(shapeFileLayer, "zOFID", "LONG", 9, "", "", "refcode", "NULLABLE", "REQUIRED");
                    var gpresult2 = Geoprocessing.ExecuteToolAsync("AddField_management", valueArray2);

                    valueArray2 = Geoprocessing.MakeValueArray(shapeFileLayer, dropFields);
                    gpresult2 = Geoprocessing.ExecuteToolAsync("DeleteField_management", valueArray2);

                    currentMapContent.FileName = currentMapContent.FileName.Replace(".shp", ".csv");// do we need this?
                    currentMapContent.FilePath = currentMapContent.FilePath.Replace(".shp", ".csv");
                    currentMapContent.FieldCheckListBox = chkListBoxFields;
                    currentMapContent.BuildAttributeTable(cboMapContent.SelectedItem, chkDomainSub.Checked);

                    valueArray2 = Geoprocessing.MakeValueArray(shapeFileLayer, "FID", currentMapContent.FilePath, "zOrder");
                    gpresult2 = Geoprocessing.ExecuteToolAsync("AddJoin_management", valueArray2);

                    valueArray2 = Geoprocessing.MakeValueArray(shapeFileLayer, outLocation, "zscratch_" + shapeFileLayer.Name);
                    gpresult2 = Geoprocessing.ExecuteToolAsync("FeatureClassToFeatureClass_conversion", valueArray2);

                    string originalShpPath = currentMapContent.FilePath.Replace(".csv", ".shp");

                    valueArray2 = Geoprocessing.MakeValueArray(originalShpPath);
                    gpresult2 = Geoprocessing.ExecuteToolAsync("Delete_management", valueArray2);

                    valueArray2 = Geoprocessing.MakeValueArray("zscratch_" + shapeFileLayer.Name, "zOFID;zOrder");
                    gpresult2 = Geoprocessing.ExecuteToolAsync("DeleteField_management", valueArray2);

                    string newPath = outLocation + "\\" + "zscratch_" + shapeFileLayer.Name + ".shp";

                    valueArray2 = Geoprocessing.MakeValueArray(newPath, shapeFileLayer.Name);
                    gpresult2 = Geoprocessing.ExecuteToolAsync("Rename_management", valueArray2);


                    foreach (var layer in map.Layers)
                    {
                        if (layer.Name == "zscratch_" + shapeFileLayer.Name)
                        {
                            map.RemoveLayer(layer);
                            break;
                        }
                    }

                    if (orca_export.Properties.Settings.Default.zipSHP == true)
                    {
                        string shapeFileName = shapeFileLayer.Name;
                        string oldFileSource = outLocation + "\\" + shapeFileName;
                        string newFileSource = outLocation + "\\" + shapeFileName + "\\" + shapeFileName;
                        File.Delete(currentMapContent.FilePath);
                        var newDir = Directory.CreateDirectory(outLocation + "\\" + shapeFileName);

                        File.Move(oldFileSource + ".CPG", newFileSource + ".CPG");
                        File.Move(oldFileSource + ".dbf", newFileSource + ".dbf");
                        File.Move(oldFileSource + ".prj", newFileSource + ".prj");
                        File.Move(oldFileSource + ".sbn", newFileSource + ".sbn");
                        File.Move(oldFileSource + ".sbx", newFileSource + ".sbx");
                        File.Move(oldFileSource + ".shp.xml", newFileSource + ".shp.xml");
                        File.Move(oldFileSource + ".shx", newFileSource + ".shx");
                        File.Move(oldFileSource + ".shp", newFileSource + ".shp");

                        var zipFile = outLocation + "\\" + shapeFileName + ".zip";
                        var files = Directory.GetFiles(outLocation + "\\" + shapeFileName);

                        using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create))
                        {
                            foreach (var fPath in files)
                            {
                                archive.CreateEntryFromFile(fPath, Path.GetFileName(fPath));
                            }
                        }

                        newDir.Delete(true);
                    }
                    else
                    {
                        File.Delete(currentMapContent.FilePath);
                        shapeFileLayer2 = LayerFactory.Instance.CreateFeatureLayer(new Uri(originalShpPath), map);
                    }
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Shapefile successfully created.","Success");
                }
                else
                {
                    this.UseWaitCursor = false;
                    this.Focus();
                    this.BringToFront();
                    return;
                }
                currentMapContent.SHP = false;
                currentMapContent.CSV = false;
                File.Delete(Path.GetDirectoryName(currentMapContent.FilePath) + "\\schema.ini");
                this.UseWaitCursor = false;
                this.Focus();
                this.BringToFront();
            });
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            settings.ShowDialog();
        }

        private void btnFileGDB_Click(object sender, EventArgs e)
        {
            if (chkListBoxFields.Items.Count == 0)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please select a layer or table from the dropdown menu.", "Error");
                this.UseWaitCursor = false;
                this.BringToFront();
                this.Focus();
                return;
            }
            if (chkListBoxFields.CheckedItems.Count == 0)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please check at least 1 field from the Fields checklist box.", "Error");
                this.UseWaitCursor = false;
                this.BringToFront();
                this.Focus();
                return;
            }
            this.Focus();
            this.UseWaitCursor = true;
            currentMapContent.FieldAlias = chkFieldAlias.Checked;
            currentMapContent.DomainSub = chkDomainSub.Checked;

            SaveItemDialog saveDialog = new SaveItemDialog(); // can either create a new gdb or save feature class or table to existing
            saveDialog.Title = "Browse for Geodatabase";
            if (saveDialog.ShowDialog() == true)
            {
                this.Focus();
                currentMapContent.SHP = true;
                currentMapContent.CSV = true;
                SetCheckedFields(chkListBoxFields); //sets the checked fields in the MapContent object
                currentMapContent.FilePath = Path.GetFullPath(saveDialog.FilePath);
                string outLocation = Path.GetDirectoryName(currentMapContent.FilePath);
                if (!outLocation.EndsWith(".gdb"))
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Feature class or table must be saved to a file geodatabase (.gdb)");
                    this.UseWaitCursor = false;
                    this.Focus();
                    return;
                }

                string dropFields = "";
                string oldName = "";
                string newName = "";
                string newLayerName = "";

                if (currentMapContent.IsLayer)
                {
                    oldName = currentMapContent.SelectedLayer.Name;
                }
                else if (currentMapContent.IsTable)
                {
                    oldName = currentMapContent.SelectedTable.Name;
                }
                newName = Path.GetFileName(currentMapContent.FilePath);
                QueuedTask.Run(() =>
                {
                    Map map = MapView.Active.Map;
                    string uriShp = currentMapContent.FilePath;
                    if (oldName != newName)
                    {
                        if (currentMapContent.IsLayer)
                        {
                            currentMapContent.SelectedLayer.SetName(newName);
                            var valueArray = Geoprocessing.MakeValueArray(currentMapContent.SelectedLayer, outLocation + "\\" + newName);
                            var gpresult1 = Geoprocessing.ExecuteToolAsync("CopyFeatures_management", valueArray);
                            currentMapContent.SelectedLayer.SetName(oldName);
                            newLayerName = newName;
                        }
                        else if (currentMapContent.IsTable)
                        {
                            currentMapContent.SelectedTable.SetName(newName);
                            var valueArray = Geoprocessing.MakeValueArray(currentMapContent.SelectedTable, outLocation, newName);
                            var gpresult1 = Geoprocessing.ExecuteToolAsync("TableToGeodatabase_conversion", valueArray);
                            currentMapContent.SelectedTable.SetName(oldName);
                            StandaloneTableFactory.Instance.CreateStandaloneTable(new Uri(uriShp), map);
                            newLayerName = newName;
                        }
                    }
                    else
                    {
                        if(currentMapContent.IsLayer)
                        {
                            var valueArray = Geoprocessing.MakeValueArray(currentMapContent.SelectedLayer, outLocation + "\\" + oldName);
                            var gpresult1 = Geoprocessing.ExecuteToolAsync("CopyFeatures_management", valueArray);
                            newLayerName = oldName;
                        }
                        else if (currentMapContent.IsTable)
                        {
                            var valueArray = Geoprocessing.MakeValueArray(currentMapContent.SelectedTable, outLocation, oldName);
                            var gpresult1 = Geoprocessing.ExecuteToolAsync("TableToGeodatabase_conversion", valueArray);
                            StandaloneTableFactory.Instance.CreateStandaloneTable(new Uri(uriShp), map);
                            newLayerName = oldName;
                        }
                       
                    }
                    FeatureLayer gdbLayer = null;
                    StandaloneTable gdbTable = null;
                    
                    if (currentMapContent.IsLayer)
                    {
                        foreach (var layer in map.Layers)
                        {
                            if (layer.Name == newLayerName)
                            {
                                gdbLayer = (FeatureLayer)layer;
                                break;
                            }
                        }
                    }
                    else if (currentMapContent.IsTable)
                    {
                        foreach (var table in map.StandaloneTables)
                        {
                            
                            if (table.Name == newLayerName)
                            {
                                gdbTable = table;
                                break;
                            }
                        }
                    }

                    if (currentMapContent.IsLayer)
                    {
                        foreach (var field in gdbLayer.GetFieldDescriptions())
                        {
                            if (field.Name != "Shape" && field.Name != "OBJECTID" && field.Name != "Shape_Length" && field.Name != "Shape_Area")
                                dropFields += field.Name + ";";
                        }
                        var valueArray2 = Geoprocessing.MakeValueArray(gdbLayer, dropFields);
                        var gpresult2 = Geoprocessing.ExecuteToolAsync("DeleteField_management", valueArray2);

                        valueArray2 = Geoprocessing.MakeValueArray(gdbLayer, "zOFID", "LONG", 9, "", "", "zOFID", "NULLABLE", "REQUIRED");
                        gpresult2 = Geoprocessing.ExecuteToolAsync("AddField_management", valueArray2);

                        valueArray2 = Geoprocessing.MakeValueArray(gdbLayer, "zOFID", "autoIncrement()", "PYTHON3", "rec=-1\ndef autoIncrement(): \n global rec \n pStart = 1  \n pInterval = 1 \n if (rec " + "== 0):  \n  rec = pStart  \n else:  \n  rec += pInterval  \n return rec");
                        gpresult2 = Geoprocessing.ExecuteToolAsync("CalculateField_management", valueArray2);

                        string csvFile = outLocation.Replace(".gdb", "_zzz.csv");
                        currentMapContent.FilePath = csvFile;
                        currentMapContent.FieldCheckListBox = chkListBoxFields;

                        currentMapContent.BuildAttributeTable(cboMapContent.SelectedItem, chkDomainSub.Checked);
                        valueArray2 = Geoprocessing.MakeValueArray(gdbLayer, "zOFID", currentMapContent.FilePath, "zOrder");
                        gpresult2 = Geoprocessing.ExecuteToolAsync("AddJoin_management", valueArray2);

                        valueArray2 = Geoprocessing.MakeValueArray(gdbLayer, outLocation, "zscratch_" + gdbLayer.Name);
                        gpresult2 = Geoprocessing.ExecuteToolAsync("FeatureClassToFeatureClass_conversion", valueArray2);

                        valueArray2 = Geoprocessing.MakeValueArray(outLocation + "\\" + gdbLayer.Name);
                        gpresult2 = Geoprocessing.ExecuteToolAsync("Delete_management", valueArray2);

                        valueArray2 = Geoprocessing.MakeValueArray("zscratch_" + gdbLayer.Name, "zOFID;zOrder");
                        gpresult2 = Geoprocessing.ExecuteToolAsync("DeleteField_management", valueArray2);

                        string newPath = outLocation + "\\" + "zscratch_" + gdbLayer.Name;

                        foreach (var layer in map.Layers)
                        {
                            if (layer.Name == ("zscratch_" + gdbLayer.Name))
                            {
                                //layer.SetName(gdbLayer.Name);
                                map.RemoveLayer(layer);
                                break;
                            }
                        }
                        valueArray2 = Geoprocessing.MakeValueArray(newPath, gdbLayer.Name);
                        gpresult2 = Geoprocessing.ExecuteToolAsync("Rename_management", valueArray2);
                        FeatureLayer finalLayer = LayerFactory.Instance.CreateFeatureLayer(new Uri(uriShp), map);
                        ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Geodatabase feature class successfully created.", "Success");
                    }
                    else if (currentMapContent.IsTable)
                    {
                        var valueArray2 = Geoprocessing.MakeValueArray(gdbTable, "zOFID", "LONG", 9, "", "", "zOFID", "NULLABLE", "REQUIRED");
                        var gpresult2 = Geoprocessing.ExecuteToolAsync("AddField_management", valueArray2);
                        foreach (var field in gdbTable.GetFieldDescriptions())
                        {
                            if (field.Name != "zOFID" && field.Name != "OBJECTID")
                                dropFields += field.Name + ";";
                        }
                        valueArray2 = Geoprocessing.MakeValueArray(gdbTable, dropFields);
                        gpresult2 = Geoprocessing.ExecuteToolAsync("DeleteField_management", valueArray2);

                        valueArray2 = Geoprocessing.MakeValueArray(gdbTable, "zOFID", "autoIncrement()", "PYTHON3", "rec=-1\ndef autoIncrement(): \n global rec \n pStart = 1  \n pInterval = 1 \n if (rec " + "== 0):  \n  rec = pStart  \n else:  \n  rec += pInterval  \n return rec");
                        gpresult2 = Geoprocessing.ExecuteToolAsync("CalculateField_management", valueArray2);

                        string csvFile = outLocation.Replace(".gdb", ".csv");
                        currentMapContent.FilePath = csvFile;
                        currentMapContent.FieldCheckListBox = chkListBoxFields;
                        currentMapContent.BuildAttributeTable(cboMapContent.SelectedItem, chkDomainSub.Checked);

                        valueArray2 = Geoprocessing.MakeValueArray(gdbTable, "zOFID", currentMapContent.FilePath, "zOrder");
                        gpresult2 = Geoprocessing.ExecuteToolAsync("AddJoin_management", valueArray2);

                        valueArray2 = Geoprocessing.MakeValueArray(gdbTable, outLocation, "zscratch_" + gdbTable.Name);
                        gpresult2 = Geoprocessing.ExecuteToolAsync("TableToTable_conversion", valueArray2);

                        valueArray2 = Geoprocessing.MakeValueArray(outLocation + "\\" + gdbTable.Name);
                        gpresult2 = Geoprocessing.ExecuteToolAsync("Delete_management", valueArray2);

                        valueArray2 = Geoprocessing.MakeValueArray("zscratch_" + gdbTable.Name, "zOFID;zOrder");
                        gpresult2 = Geoprocessing.ExecuteToolAsync("DeleteField_management", valueArray2);

                        string newPath = outLocation + "\\" + "zscratch_" + gdbTable.Name;

                        foreach (var table in map.StandaloneTables)
                        {
                            if (table.Name == ("zscratch_" + gdbTable.Name))
                            {
                                map.RemoveStandaloneTable(table);
                                break;
                            }
                        }

                        valueArray2 = Geoprocessing.MakeValueArray(newPath, gdbTable.Name);
                        gpresult2 = Geoprocessing.ExecuteToolAsync("Rename_management", valueArray2);
                        StandaloneTable finalTable = StandaloneTableFactory.Instance.CreateStandaloneTable(new Uri(uriShp), map);
                        ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Geodatabase table successfully created.", "Success");
                    }
                    File.Delete(currentMapContent.FilePath);
                    File.Delete(Path.GetDirectoryName(currentMapContent.FilePath) + "\\schema.ini");
                    currentMapContent.SHP = false;
                    currentMapContent.CSV = false;
                    this.UseWaitCursor = false;
                    this.Focus();
                    this.BringToFront();
                });
            }
            else
            {
                this.UseWaitCursor = false;
                this.Focus();
                return;
            }
        }

        private void pic_ArcMapTools_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.processor-gis.com");
        }

        private void pic_ArcMapTools_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void pic_ArcMapTools_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }
    }
}
