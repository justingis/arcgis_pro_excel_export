using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System.IO;

namespace orca_export
{
    internal class btn_quick_export : Button
    {
        protected override void OnClick()
        {
            try
            {
                Type officeType = Type.GetTypeFromProgID("Excel.Application");
                if (officeType == null)
                {
                    orca_export.Properties.Settings.Default.quickExportFormat = "CSV";
                    orca_export.Properties.Settings.Default.Save();
                }

                var mapView = MapView.Active;
                if (mapView == null)
                    return;

                var selectedTOClayers = mapView.GetSelectedLayers();
                var selectedTOCtabales = mapView.GetSelectedStandaloneTables();
                FeatureLayer featureLayer = null;
                StandaloneTable standaloneTable = null;
                string name = "";
                bool layer = false;
                List<FieldDescription> fields = null;

                if (selectedTOClayers.Count > 1 || selectedTOCtabales.Count > 1)
                {
                    MessageBox.Show("Please select only 1 layer or table in the TOC");
                    return;
                }
                else if (selectedTOClayers.Count > 0 && selectedTOCtabales.Count > 0)
                {
                    MessageBox.Show("Please select only 1 layer or table in the TOC");
                    return;
                }
                else if (selectedTOClayers.Count == 0 && selectedTOCtabales.Count == 0)
                {
                    MessageBox.Show("Please select only 1 layer or table in the TOC");
                    return;
                }
                else
                {
                    MapContent currentMapContent = new MapContent();
                    currentMapContent.QuickExport = true;
                    if (selectedTOClayers.Count == 1)
                    {
                        featureLayer = (FeatureLayer)selectedTOClayers[0];
                        currentMapContent.SelectedLayer = featureLayer;
                        currentMapContent.IsLayer = true;
                        name = featureLayer.Name;
                        layer = true;
                    }
                    else if (selectedTOCtabales.Count == 1)
                    {
                        standaloneTable = selectedTOCtabales[0];
                        currentMapContent.SelectedTable = standaloneTable;
                        currentMapContent.IsTable = true;
                        name = standaloneTable.Name;
                        layer = false;
                    }

                    if (orca_export.Properties.Settings.Default.quickExportFormat == "Excel")
                    {
                        QueuedTask.Run(() =>
                        {
                            currentMapContent.Excel = true;
                            currentMapContent.DateTime = false;
                            if (orca_export.Properties.Settings.Default.saveToExcelFile)
                            {
                                System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
                                saveFileDialog1.Title = "Save output Excel file (.xlsx)";
                                saveFileDialog1.Filter = "All files (*.*)|*.*|xlsx files (*.xlsx)|*.xlsx";
                                saveFileDialog1.FilterIndex = 0;
                                saveFileDialog1.RestoreDirectory = true;
                                saveFileDialog1.FileName = name;
                                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    currentMapContent.Excel = true;
                                    currentMapContent.FileName = name;
                                    currentMapContent.FilePath = Path.GetFullPath(saveFileDialog1.FileName);
                                    if (!(currentMapContent.FilePath.Contains(".xlsx")))
                                        currentMapContent.FilePath = currentMapContent.FilePath + ".xlsx";
                                    if (layer == true)
                                    {
                                        fields = featureLayer.GetFieldDescriptions();
                                        foreach (var fieldName in fields)
                                        {
                                            if (fieldName.Name != "Shape" && fieldName.IsVisible && fieldName.Name != "SHAPE" && fieldName.Name != "shape")
                                            {
                                                currentMapContent.FieldNames.Add(fieldName.Name);
                                                if (orca_export.Properties.Settings.Default.qFieldAlias)
                                                    currentMapContent.CheckedFields.Add(fieldName.Alias);
                                                else
                                                    currentMapContent.CheckedFields.Add(fieldName.Name);
                                            }
                                        }
                                        currentMapContent.BuildAttributeTable(featureLayer, orca_export.Properties.Settings.Default.qDomainSub);
                                    }
                                    else if (layer == false)
                                    {
                                        fields = standaloneTable.GetFieldDescriptions();
                                        foreach (var fieldName in fields)
                                        {
                                            if (fieldName.IsVisible)
                                            {
                                                currentMapContent.FieldNames.Add(fieldName.Name);
                                                if (orca_export.Properties.Settings.Default.qFieldAlias)
                                                    currentMapContent.CheckedFields.Add(fieldName.Alias);
                                                else
                                                    currentMapContent.CheckedFields.Add(fieldName.Name);
                                            }
                                        }
                                        currentMapContent.BuildAttributeTable(standaloneTable, orca_export.Properties.Settings.Default.qDomainSub);
                                    }
                                    MessageBox.Show("Excel file successfully created.", "Success");
                                }
                            }
                            else
                            {
                                currentMapContent.Excel = true;
                                currentMapContent.FileName = name;
                                if (layer == true)
                                {
                                    fields = featureLayer.GetFieldDescriptions();
                                    foreach (var fieldName in fields)
                                    {
                                        if (fieldName.Name != "Shape" && fieldName.IsVisible && fieldName.Name != "SHAPE" && fieldName.Name != "shape")
                                        {
                                            currentMapContent.FieldNames.Add(fieldName.Name);
                                            if (orca_export.Properties.Settings.Default.qFieldAlias)
                                                currentMapContent.CheckedFields.Add(fieldName.Alias);
                                            else
                                                currentMapContent.CheckedFields.Add(fieldName.Name);
                                        }
                                    }
                                    currentMapContent.BuildAttributeTable(featureLayer, orca_export.Properties.Settings.Default.qDomainSub);
                                }
                                else
                                {
                                    fields = standaloneTable.GetFieldDescriptions();
                                    foreach (var fieldName in fields)
                                    {
                                        if (fieldName.IsVisible)
                                        {
                                            currentMapContent.FieldNames.Add(fieldName.Name);
                                            if (orca_export.Properties.Settings.Default.qFieldAlias)
                                                currentMapContent.CheckedFields.Add(fieldName.Alias);
                                            else
                                                currentMapContent.CheckedFields.Add(fieldName.Name);
                                        }
                                    }
                                    currentMapContent.BuildAttributeTable(standaloneTable, orca_export.Properties.Settings.Default.qDomainSub);
                                }
                            }
                            layer = false;
                            currentMapContent.Excel = false;
                        });
                    }
                    else if (orca_export.Properties.Settings.Default.quickExportFormat == "CSV")
                    {
                        QueuedTask.Run(() =>
                        {
                            currentMapContent.CSV = true;
                            currentMapContent.DateTime = false;
                            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
                            saveFileDialog1.Title = "Save output CSV file (.csv)";
                            saveFileDialog1.Filter = "All files (*.*)|*.*|csv files (*.csv)|*.csv";
                            saveFileDialog1.FilterIndex = 0;
                            saveFileDialog1.RestoreDirectory = true;
                            saveFileDialog1.FileName = name;
                            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                currentMapContent.FileName = name;
                                currentMapContent.FilePath = Path.GetFullPath(saveFileDialog1.FileName);
                                if (!(currentMapContent.FilePath.Contains(".csv")))
                                    currentMapContent.FilePath = currentMapContent.FilePath + ".csv";
                                if (layer == true)
                                {
                                    fields = featureLayer.GetFieldDescriptions();
                                    foreach (var fieldName in fields)
                                    {
                                        if (fieldName.Name != "Shape" && fieldName.IsVisible && fieldName.Name != "SHAPE" && fieldName.Name != "shape")
                                        {
                                            currentMapContent.FieldNames.Add(fieldName.Name);
                                            if (orca_export.Properties.Settings.Default.qFieldAlias)
                                                currentMapContent.CheckedFields.Add(fieldName.Alias);
                                            else
                                                currentMapContent.CheckedFields.Add(fieldName.Name);
                                        }
                                    }
                                    currentMapContent.BuildAttributeTable(featureLayer, orca_export.Properties.Settings.Default.qDomainSub);
                                }
                                else if (layer == false)
                                {
                                    fields = standaloneTable.GetFieldDescriptions();
                                    foreach (var fieldName in fields)
                                    {
                                        if (fieldName.IsVisible)
                                        {
                                            currentMapContent.FieldNames.Add(fieldName.Name);
                                            if (orca_export.Properties.Settings.Default.qFieldAlias)
                                                currentMapContent.CheckedFields.Add(fieldName.Alias);
                                            else
                                                currentMapContent.CheckedFields.Add(fieldName.Name);
                                        }
                                    }
                                    currentMapContent.BuildAttributeTable(standaloneTable, orca_export.Properties.Settings.Default.qDomainSub);
                                }
                            }
                            layer = false;
                            currentMapContent.CSV = false;
                        });
                    }
                }
            }
            catch
            {
                MessageBox.Show("Please select a valid layer or table from the TOC");
                return;
            }
        }
    }
}
