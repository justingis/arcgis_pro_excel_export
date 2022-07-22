using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Core;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System.Windows.Forms;
using System.Collections;
using System.Data;
using System.IO;
using ArcGIS.Desktop.Core.Geoprocessing;

namespace orca_export
{
    class MapContent
    {
        private FeatureLayer selectedLayer;
        private StandaloneTable selectedTable;
        private bool isLayer;
        private bool isTable;
        private ArrayList checkedFields = new ArrayList();
        IList<string> fieldNames = new List<string>();
        private object[,] attributeTable;
        private CheckedListBox fieldCheckListBox;
        private bool excel = false;
        private bool csv = false;
        bool shp = false;
        private string fileName;
        int featureIdCounter = 0;
        string filePath;
        private bool dateTime;
        private bool quickExport = false;
        IList<Field> outputFields = new List<Field>();
        private bool fieldAlias = false;
        private bool domainSub = false;
        private string subtypeField = "";

        public bool DomainSub
        {
            get
            {
                return domainSub;
            }
            set
            {
                domainSub = value;
            }
        }
        public bool FieldAlias
        {
            get
            {
                return fieldAlias;
            }
            set
            {
                fieldAlias = value;
            }
        }
        public bool QuickExport
        {
            get
            {
                return quickExport;
            }
            set
            {
                quickExport = value;
            }
        }

        public bool DateTime
        {
            get
            {
                return dateTime;
            }
            set
            {
                dateTime = value;
            }
        }
        public bool SHP
        {
            get
            {
                return shp;
            }
            set
            {
                shp = value;
            }
        }

        public string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
            }
        }

        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
            }
        }

        public bool Excel
        {
            get
            {
                return excel;
            }
            set
            {
                excel = value;
            }
        }

        public bool CSV
        {
            get
            {
                return csv;
            }
            set
            {
                csv = value;
            }
        }

        public CheckedListBox FieldCheckListBox
        {
            get
            {
                return fieldCheckListBox;
            }
            set
            {
                fieldCheckListBox = value;
            }
        }
        public FeatureLayer SelectedLayer
        {
            get
            {
                return selectedLayer;
            }
            set
            {
                selectedLayer = value;
            }
        }
        public StandaloneTable SelectedTable
        {
            get
            {
                return selectedTable;
            }
            set
            {
                selectedTable = value;
            }
        }
        public bool IsLayer
        {
            get
            {
                return isLayer;
            }
            set
            {
                isLayer = value;
            }
        }
        public bool IsTable
        {
            get
            {
                return isTable;
            }
            set
            {
                isTable = value;
            }
        }

        public ArrayList CheckedFields
        {
            get
            {
                return checkedFields;
            }
            set
            {
                checkedFields = value;
            }
        }

        public IList<string> FieldNames
        {
            get
            {
                return fieldNames;
            }
            set
            {
                fieldNames = value;
            }
        }

        public ArrayList GetFeatureLayers(bool visible, bool showLayers)
        {
            ArrayList layerNames = new ArrayList();
            var currentMapLayers = MapView.Active.Map.GetLayersAsFlattenedList();
            
            string layerType;
            foreach (var featureLayer in currentMapLayers)
            {
                layerType = featureLayer.GetType().ToString();
                if (layerType == "ArcGIS.Desktop.Mapping.FeatureLayer")
                {
                    if (showLayers == true)
                    {
                        if (featureLayer.IsVisible == true && visible == true)
                        {
                            layerNames.Add(featureLayer);
                        }
                        else if (visible == false)
                        {
                            layerNames.Add(featureLayer);
                        }
                    }
                }
            }
            return layerNames;
        }

        public ArrayList GetStandAloneTables(bool showTables)
        {
            ArrayList tableNames = new ArrayList();
            var currentTables = MapView.Active.Map.StandaloneTables;
            foreach (var table in currentTables)
            {
                if (showTables == true)
                    tableNames.Add(table);
            }
            return tableNames;
        }

        public List<FieldDescription> GetSelectedContentFields()
        {
            List <FieldDescription> fields = null;
            if (isLayer == true)
            {
                fields = selectedLayer.GetFieldDescriptions();
            }
            else if (isTable == true)
            {
                fields = selectedTable.GetFieldDescriptions();
                
            }
            return fields;
        }

        private void BuildData2Darray(FeatureLayer inputFeatureLayer, IList<string> inputFieldList, bool subDomain)
        {
            QueuedTask.Run(() =>
            {
                try
                {
                    outputFields.Clear();
                    featureIdCounter = 0;
                    int fieldCount = 0;
                    int fieldIndex = 0;
                    int selectionCount = 0;
                    Domain fieldDomain;
                    CodedValueDomain codedValueDomain;
                    IReadOnlyList<Field> fields;
                    object attributeValue;
                    string domainValue;
                    IReadOnlyList<Subtype> subtype1 = null;
                    string dataStore;
                    string subtype1_field = "";
                    SortedList<object, string> codedValuePairs;
                    selectionCount = inputFeatureLayer.GetSelection().GetCount();
                    dataStore = inputFeatureLayer.GetFeatureClass().GetDatastore().ToString();
                    
                    if (dataStore == "ArcGIS.Core.Data.Geodatabase")
                    {
                        subtype1_field = inputFeatureLayer.GetTable().GetDefinition().GetSubtypeField();
                        subtypeField = subtype1_field;
                        subtype1 = inputFeatureLayer.GetTable().GetDefinition().GetSubtypes();
                    }

                    if (selectionCount > 0)
                    {
                        RowCursor fieldsCursor = inputFeatureLayer.Search();
                        fieldsCursor.MoveNext();
                        fields = fieldsCursor.Current.GetFields();
                        attributeTable = new object[inputFeatureLayer.GetSelection().GetCount(), inputFieldList.Count];
                        using (RowCursor rc = inputFeatureLayer.GetSelection().Search())
                        {
                            while (rc.MoveNext())
                            {
                                for (int i = 1; i < fieldCheckListBox.Items.Count; i++)
                                {
                                    if (fieldCheckListBox.GetItemChecked(i))
                                    {
                                        fieldIndex = rc.Current.FindField(inputFieldList[i - 1]);
                                        outputFields.Add(fields[fieldIndex]);
                                        if (subDomain)
                                        {
                                            if (fields[fieldIndex].Name == subtype1_field)
                                            {
                                                if (rc.Current[fieldIndex] != null)
                                                {
                                                    int subTypeCode = Convert.ToInt32(attributeValue = rc.Current[fieldIndex]);
                                                    for (int j = 0; j < subtype1.Count; j++)
                                                    {
                                                        if (subtype1[j].GetCode() == subTypeCode)
                                                        {
                                                            attributeTable[featureIdCounter, fieldCount] = subtype1[j].GetName();
                                                        }
                                                    }
                                                }

                                                fieldCount += 1;
                                            }
                                            else
                                            {
                                                fieldDomain = fields[fieldIndex].GetDomain();
                                                if (fieldDomain != null)
                                                {
                                                    codedValueDomain = (CodedValueDomain)fieldDomain;
                                                    codedValuePairs = codedValueDomain.GetCodedValuePairs();
                                                    attributeValue = rc.Current[fieldIndex];
                                                    if (attributeValue != null)
                                                    {
                                                        try
                                                        {
                                                            domainValue = codedValuePairs[attributeValue];
                                                            attributeTable[featureIdCounter, fieldCount] = domainValue;
                                                            fieldCount += 1;
                                                        }
                                                        catch
                                                        {
                                                            attributeTable[featureIdCounter, fieldCount] = attributeValue;
                                                            fieldCount += 1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        attributeTable[featureIdCounter, fieldCount] = attributeValue;
                                                        fieldCount += 1;
                                                    }
                                                }
                                                else
                                                {
                                                    attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                                    fieldCount += 1;
                                                }
                                            }
                                        }

                                        else
                                        {
                                            attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                            fieldCount += 1;
                                        }
                                    }
                                }
                                fieldCount = 0;
                                featureIdCounter += 1;
                            }
                        }
                    }
                    else
                    {
                        RowCursor fieldsCursor = inputFeatureLayer.Search();
                        fieldsCursor.MoveNext();
                        fields = fieldsCursor.Current.GetFields();
                        attributeTable = new object[inputFeatureLayer.GetTable().GetCount(), inputFieldList.Count];
                        using (RowCursor rc = inputFeatureLayer.Search())
                        {
                            while (rc.MoveNext())
                            {
                                for (int i=1; i < fieldCheckListBox.Items.Count; i++)
                                {
                                    if (fieldCheckListBox.GetItemChecked(i))
                                    {
                                        fieldIndex = rc.Current.FindField(inputFieldList[i - 1]);
                                        outputFields.Add(fields[fieldIndex]);
                                        if (subDomain)
                                        {
                                            if (fields[fieldIndex].Name == subtype1_field)
                                            {
                                                if (rc.Current[fieldIndex] != null)
                                                {
                                                    int subTypeCode = Convert.ToInt32(attributeValue = rc.Current[fieldIndex]);
                                                    for (int j = 0; j < subtype1.Count; j++)
                                                    {
                                                        if (subtype1[j].GetCode() == subTypeCode)
                                                        {
                                                            attributeTable[featureIdCounter, fieldCount] = subtype1[j].GetName();
                                                        }
                                                    }
                                                }
                                                
                                                fieldCount += 1;
                                            }
                                            else
                                            {
                                                fieldDomain = fields[fieldIndex].GetDomain();
                                                if (fieldDomain != null)
                                                {
                                                    codedValueDomain = (CodedValueDomain)fieldDomain;
                                                    codedValuePairs = codedValueDomain.GetCodedValuePairs();
                                                    attributeValue = rc.Current[fieldIndex];
                                                    if (attributeValue != null)
                                                    {
                                                        try
                                                        {
                                                            domainValue = codedValuePairs[attributeValue];
                                                            attributeTable[featureIdCounter, fieldCount] = domainValue;
                                                            fieldCount += 1;
                                                        }
                                                        catch
                                                        {
                                                            attributeTable[featureIdCounter, fieldCount] = attributeValue;
                                                            fieldCount += 1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        attributeTable[featureIdCounter, fieldCount] = attributeValue;
                                                        fieldCount += 1;
                                                    }
                                                }
                                                else
                                                {
                                                    attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                                    fieldCount += 1;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                            fieldCount += 1;
                                        }
                                    }
                                }
                                fieldCount = 0;
                                featureIdCounter += 1;
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(fileName) || fileName == "Name (Optional)")
                    {
                        fileName = selectedLayer.Name;
                    }

                    if (excel == true)
                    {
                        Excel excelExport = new Excel();
                        if (fileName.Length > 31)
                            fileName = fileName.Substring(0, 31);
                        excelExport.ExportToExcel(dateTime, fileName, checkedFields, attributeTable, filePath);
                        excel = false;
                    }
                    else if (csv == true)
                    {
                        CreateCSVfile();
                        csv = false;
                    }
                }
                catch (Exception ex)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message);
                }
            });
        }
        private void BuildData2Darray(StandaloneTable inputStandAloneTable, IList<string> inputFieldList, bool subDomain)
        {
            QueuedTask.Run(() =>
            {
                try
                {
                    outputFields.Clear();
                    featureIdCounter = 0;
                    int fieldCount = 0;
                    int fieldIndex = 0;
                    int selectionCount = 0;
                    Domain fieldDomain;
                    CodedValueDomain codedValueDomain;
                    IReadOnlyList<Field> fields;
                    object attributeValue;
                    string domainValue;
                    IReadOnlyList<Subtype> subtype1 = null;
                    string dataStore;
                    string subtype1_field = "";
                    SortedList<object, string> codedValuePairs;
                    selectionCount = inputStandAloneTable.GetSelection().GetCount();

                    dataStore = inputStandAloneTable.GetTable().GetDatastore().ToString();

                    if (dataStore == "ArcGIS.Core.Data.Geodatabase")
                    {
                        subtype1_field = inputStandAloneTable.GetTable().GetDefinition().GetSubtypeField();
                        subtypeField = subtype1_field;
                        subtype1 = inputStandAloneTable.GetTable().GetDefinition().GetSubtypes();
                    }
                    if (selectionCount > 0)
                    {
                        RowCursor fieldsCursor = inputStandAloneTable.Search();
                        fieldsCursor.MoveNext();
                        fields = fieldsCursor.Current.GetFields();
                        attributeTable = new object[inputStandAloneTable.GetSelection().GetCount(), inputFieldList.Count];
                        using (RowCursor rc = inputStandAloneTable.GetSelection().Search())
                        {
                            while (rc.MoveNext())
                            {
                                for (int i = 1; i < fieldCheckListBox.Items.Count; i++)
                                {
                                    if (fieldCheckListBox.GetItemChecked(i))
                                    {
                                        fieldIndex = rc.Current.FindField(inputFieldList[i - 1]);
                                        outputFields.Add(fields[fieldIndex]);
                                        if (subDomain)
                                        {
                                            if (fields[fieldIndex].Name == subtype1_field)
                                            {
                                                if (rc.Current[fieldIndex] != null)
                                                {
                                                    int subTypeCode = Convert.ToInt32(attributeValue = rc.Current[fieldIndex]);
                                                    for (int j = 0; j < subtype1.Count; j++)
                                                    {
                                                        if (subtype1[j].GetCode() == subTypeCode)
                                                        {
                                                            attributeTable[featureIdCounter, fieldCount] = subtype1[j].GetName();
                                                        }
                                                    }
                                                }

                                                fieldCount += 1;
                                            }
                                            else
                                            {
                                                fieldDomain = fields[fieldIndex].GetDomain();
                                                if (fieldDomain != null)
                                                {
                                                    codedValueDomain = (CodedValueDomain)fieldDomain;
                                                    codedValuePairs = codedValueDomain.GetCodedValuePairs();
                                                    attributeValue = rc.Current[fieldIndex];
                                                    if (attributeValue != null)
                                                    {
                                                        try
                                                        {
                                                            domainValue = codedValuePairs[attributeValue];
                                                            attributeTable[featureIdCounter, fieldCount] = domainValue;
                                                            fieldCount += 1;
                                                        }
                                                        catch
                                                        {
                                                            attributeTable[featureIdCounter, fieldCount] = attributeValue;
                                                            fieldCount += 1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        attributeTable[featureIdCounter, fieldCount] = attributeValue;
                                                        fieldCount += 1;
                                                    }
                                                }
                                                else
                                                {
                                                    attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                                    fieldCount += 1;
                                                }
                                            }
                                        }

                                        else
                                        {
                                            attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                            fieldCount += 1;
                                        }

                                    }
                                }
                                fieldCount = 0;
                                featureIdCounter += 1;
                            }
                        }
                    }
                    else
                    {
                        RowCursor fieldsCursor = inputStandAloneTable.Search();
                        fieldsCursor.MoveNext();
                        fields = fieldsCursor.Current.GetFields();
 
                        attributeTable = new object[inputStandAloneTable.GetTable().GetCount(), inputFieldList.Count];
                        using (RowCursor rc = inputStandAloneTable.Search())
                        {
                            while (rc.MoveNext())
                            {
                                for (int i = 1; i < fieldCheckListBox.Items.Count; i++)
                                {
                                    if (fieldCheckListBox.GetItemChecked(i))
                                    {
                                        fieldIndex = rc.Current.FindField(inputFieldList[i - 1]);
                                        outputFields.Add(fields[fieldIndex]);
                                        if (subDomain)
                                        {
                                            if (fields[fieldIndex].Name == subtype1_field)
                                            {
                                                if (rc.Current[fieldIndex] != null)
                                                {
                                                    int subTypeCode = Convert.ToInt32(attributeValue = rc.Current[fieldIndex]);
                                                    for (int j = 0; j < subtype1.Count; j++)
                                                    {
                                                        if (subtype1[j].GetCode() == subTypeCode)
                                                        {
                                                            attributeTable[featureIdCounter, fieldCount] = subtype1[j].GetName();
                                                        }
                                                    }
                                                }

                                                fieldCount += 1;
                                            }
                                            else
                                            {
                                                fieldDomain = fields[fieldIndex].GetDomain();
                                                if (fieldDomain != null)
                                                {
                                                    codedValueDomain = (CodedValueDomain)fieldDomain;
                                                    codedValuePairs = codedValueDomain.GetCodedValuePairs();
                                                    attributeValue = rc.Current[fieldIndex];
                                                    if (attributeValue != null)
                                                    {
                                                        try
                                                        {
                                                            domainValue = codedValuePairs[attributeValue];
                                                            attributeTable[featureIdCounter, fieldCount] = domainValue;
                                                            fieldCount += 1;
                                                        }
                                                        catch
                                                        {
                                                            attributeTable[featureIdCounter, fieldCount] = attributeValue;
                                                            fieldCount += 1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        attributeTable[featureIdCounter, fieldCount] = attributeValue;
                                                        fieldCount += 1;
                                                    }
                                                }
                                                else
                                                {
                                                    attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                                    fieldCount += 1;
                                                }
                                            }
                                        }

                                        else
                                        {
                                            attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                            fieldCount += 1;
                                        }

                                    }
                                }
                                fieldCount = 0;
                                featureIdCounter += 1;
                            }
                        }
                    }
                    if (string.IsNullOrWhiteSpace(fileName) || fileName == "Name (Optional)")
                    {
                        fileName = selectedTable.Name;
                    }

                    if (excel == true)
                    {
                        Excel excelExport = new Excel();
                        if (fileName.Length > 31)
                            fileName = fileName.Substring(0, 31);
                        excelExport.ExportToExcel(dateTime, fileName, checkedFields, attributeTable, filePath);
                        excel = false;
                    }
                    else if (csv == true)
                    {
                        CreateCSVfile();
                        csv = false;
                    }
                }
                catch (Exception ex)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message);
                }
            });
        }

        public void BuildAttributeTable(object inputObject, bool subDomain)
        {
            string inputType;
            string featureLayer = "ArcGIS.Desktop.Mapping.FeatureLayer";
            string standAloneTable = "ArcGIS.Desktop.Mapping.StandaloneTable";
            inputType = inputObject.GetType().ToString();
            if (inputObject.GetType().ToString() == featureLayer)
            {
                if (quickExport == true)
                {
                    BuildData2Darray2(selectedLayer, fieldNames, subDomain);
                }
                else
                {
                    BuildData2Darray(selectedLayer, fieldNames, subDomain);
                }
            }
            else if (inputObject.GetType().ToString() == standAloneTable)
            {
                if (quickExport == true)
                {
                    BuildData2Darray2(selectedTable, fieldNames, subDomain);
                }
                else
                {
                    BuildData2Darray(selectedTable, fieldNames, subDomain);
                }
            }
            else
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("some sort of error has occured...");
        }



        private void CreateSchemaINI()
        {
            fileName = Path.GetFileName(filePath);
            string lineText = "[" + fileName + "]" + Environment.NewLine;
            string outputPath = Path.GetDirectoryName(filePath);
            outputPath = outputPath + "\\schema.ini";
            string fieldType = "";
            string width = "";
            string fieldName = "";
            lineText += "Format=CSVDelimited" + Environment.NewLine;
            lineText += "ColNameHeader=True" + Environment.NewLine;
            int z = 1;

            for (int i = 0; i < checkedFields.Count; i++)
            {
                fieldType = outputFields[i].FieldType.ToString();
                if (fieldType == "String")
                    width = outputFields[i].Length.ToString();

                if (fieldType == "String")
                    fieldType = "Text Width " + width;
                else if (fieldType == "OID")
                    fieldType = "Long";
                else if (fieldType == "Date")
                    fieldType = "Text";
                else if (fieldType == "Integer")
                    fieldType = "Long";
                else if (fieldType == "SmallInteger")
                    fieldType = "Long";
                else if (fieldType == "ObjectID") //???
                    fieldType = "Long";
                else if (fieldType == "GlobalID")
                    fieldType = "Text";
                
                fieldName = checkedFields[i].ToString();

                if (domainSub == true)
                {
                    if (outputFields[i].Name == subtypeField)
                    {
                        fieldType = "Text";
                    }
                }
                fieldName = fieldName.Replace(" ", "_");
                lineText += "Col" + z.ToString() + "=" + fieldName + " " + fieldType + Environment.NewLine;
                z++;
            }
            File.WriteAllText(outputPath, lineText);
        }

        private void CreateCSVfile()
        {
            string delimiter = ",";
            int length = featureIdCounter;
            StringBuilder sb = new StringBuilder();
            string line = "";
            string csvValue = "";

            for (int i = 0; i < checkedFields.Count; i++ )
            {
                if (i == checkedFields.Count - 1)
                {
                    line += string.Join(delimiter, checkedFields[i]);
                }
                else
                {
                    line += string.Join(delimiter, checkedFields[i] + ",");
                }

            }
            if (shp == true)
                line += string.Join(delimiter, "," + "zOrder"); // added new column for order
            sb.AppendLine(string.Join(delimiter, line));

            for (int i = 0; i < length; i++)
            {
                line = "";
                for (int j = 0; j < checkedFields.Count; j++)
                {
                    if (j == checkedFields.Count-1)
                    {
                        if (attributeTable[i, j] == null)
                            attributeTable[i, j] = "";
                        if (attributeTable[i, j].ToString().Contains("\""))
                            attributeTable[i, j] = attributeTable[i, j].ToString().Replace("\"", "\"\"");
                        csvValue = "\"" + attributeTable[i, j].ToString() + "\"";
                        line += string.Join(delimiter, csvValue);
                        if (shp == true)
                            line += ("," + string.Join(delimiter, i));
                    }
                    else
                    {
                        if (attributeTable[i, j] == null)
                            attributeTable[i, j] = "";
                        if (attributeTable[i, j].ToString().Contains("\""))
                            attributeTable[i, j] = attributeTable[i, j].ToString().Replace("\"", "\"\"");


                        csvValue = "\"" + attributeTable[i, j].ToString() + "\"";
                        line += string.Join(delimiter, csvValue + ",");
                    }
                }
                sb.AppendLine(string.Join(delimiter, line));
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.Default);
            
            csv = false;
            CreateSchemaINI();
            if (shp == false)
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("CSV file successfully created.", "Success");
        }

        private void BuildData2Darray2(FeatureLayer inputFeatureLayer, IList<string> inputFieldList, bool subDomain)
        {
            QueuedTask.Run(() =>
            {
                try
                {
                    featureIdCounter = 0;
                    int fieldCount = 0;
                    int fieldIndex = 0;
                    int selectionCount = 0;
                    Domain fieldDomain;
                    CodedValueDomain codedValueDomain;
                    IReadOnlyList<Field> fields;
                    object attributeValue;
                    string domainValue;
                    IReadOnlyList<Subtype> subtype1 = null;
                    string dataStore;
                    string subtype1_field = "";
                    SortedList<object, string> codedValuePairs;
                    selectionCount = inputFeatureLayer.GetSelection().GetCount();
                    dataStore = inputFeatureLayer.GetFeatureClass().GetDatastore().ToString();

                    if (dataStore == "ArcGIS.Core.Data.Geodatabase")
                    {
                        subtype1_field = inputFeatureLayer.GetTable().GetDefinition().GetSubtypeField();
                        subtypeField = subtype1_field;
                        subtype1 = inputFeatureLayer.GetTable().GetDefinition().GetSubtypes();
                    }

                    if (selectionCount > 0)
                    {
                        RowCursor fieldsCursor = inputFeatureLayer.Search();
                        fieldsCursor.MoveNext();
                        fields = fieldsCursor.Current.GetFields();
                        attributeTable = new object[inputFeatureLayer.GetSelection().GetCount(), inputFieldList.Count];
                        using (RowCursor rc = inputFeatureLayer.GetSelection().Search())
                        {
                            while (rc.MoveNext())
                            {
                                for (int i = 1; i < fieldNames.Count + 1; i++)
                                {
                                    fieldIndex = rc.Current.FindField(inputFieldList[i - 1]);
                                    if (subDomain)
                                    {
                                        if (fields[fieldIndex].Name == subtype1_field)
                                        {
                                            if (rc.Current[fieldIndex] != null)
                                            {
                                                int subTypeCode = Convert.ToInt32(attributeValue = rc.Current[fieldIndex]);
                                                for (int j = 0; j < subtype1.Count; j++)
                                                {
                                                    if (subtype1[j].GetCode() == subTypeCode)
                                                    {
                                                        attributeTable[featureIdCounter, fieldCount] = subtype1[j].GetName();
                                                    }
                                                }
                                            }

                                            fieldCount += 1;
                                        }
                                        else
                                        {
                                            fieldDomain = fields[fieldIndex].GetDomain();
                                            if (fieldDomain != null)
                                            {
                                                codedValueDomain = (CodedValueDomain)fieldDomain;
                                                codedValuePairs = codedValueDomain.GetCodedValuePairs();
                                                attributeValue = rc.Current[fieldIndex];
                                                if (attributeValue != null)
                                                {
                                                    domainValue = codedValuePairs[attributeValue];
                                                    attributeTable[featureIdCounter, fieldCount] = domainValue;
                                                    fieldCount += 1;
                                                }
                                                else
                                                {
                                                    attributeTable[featureIdCounter, fieldCount] = attributeValue;
                                                    fieldCount += 1;
                                                }
                                            }
                                            else
                                            {
                                                attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                                fieldCount += 1;
                                            }
                                        }
                                    }

                                    else
                                    {
                                        attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                        fieldCount += 1;
                                    }
                                }
                                fieldCount = 0;
                                featureIdCounter += 1;
                            }
                        }
                    }
                    else
                    {
                        RowCursor fieldsCursor = inputFeatureLayer.Search();
                        fieldsCursor.MoveNext();
                        fields = fieldsCursor.Current.GetFields();
                        attributeTable = new object[inputFeatureLayer.GetTable().GetCount(), inputFieldList.Count];
                        using (RowCursor rc = inputFeatureLayer.Search())
                        {
                            while (rc.MoveNext())
                            {
                                for (int i = 1; i < fieldNames.Count+1; i++)
                                {
                                    fieldIndex = rc.Current.FindField(inputFieldList[i - 1]);
                                    if (subDomain)
                                    {
                                        if (fields[fieldIndex].Name == subtype1_field)
                                        {
                                            if (rc.Current[fieldIndex] != null)
                                            {
                                                int subTypeCode = Convert.ToInt32(attributeValue = rc.Current[fieldIndex]);
                                                for (int j = 0; j < subtype1.Count; j++)
                                                {
                                                    if (subtype1[j].GetCode() == subTypeCode)
                                                    {
                                                        attributeTable[featureIdCounter, fieldCount] = subtype1[j].GetName();
                                                    }
                                                }
                                            }

                                            fieldCount += 1;
                                        }
                                        else
                                        {

                                            fieldDomain = fields[fieldIndex].GetDomain();
                                            if (fieldDomain != null)
                                            {
                                                codedValueDomain = (CodedValueDomain)fieldDomain;
                                                codedValuePairs = codedValueDomain.GetCodedValuePairs();
                                                attributeValue = rc.Current[fieldIndex];
                                                if (attributeValue != null)
                                                {
                                                    domainValue = codedValuePairs[attributeValue];
                                                    attributeTable[featureIdCounter, fieldCount] = domainValue;
                                                    fieldCount += 1;
                                                }
                                                else
                                                {
                                                    attributeTable[featureIdCounter, fieldCount] = attributeValue;
                                                    fieldCount += 1;
                                                }
                                            }
                                            else
                                            {
                                                attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                                fieldCount += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                        fieldCount += 1;
                                    }
                                }
                                fieldCount = 0;
                                featureIdCounter += 1;
                            }
                        }
                    }

                    fileName = selectedLayer.Name;
                    if (excel == true)
                    {
                        Excel excelExport = new Excel();
                        if (fileName.Length > 31)
                            fileName = fileName.Substring(0, 31);
                        excelExport.ExportToExcel(orca_export.Properties.Settings.Default.q_DateTimeStamp, fileName, checkedFields, attributeTable, filePath);
                        excel = false;
                    }
                    else if (csv == true)
                    {
                        CreateCSVfile();
                        csv = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private void BuildData2Darray2(StandaloneTable inputStandAloneTable, IList<string> inputFieldList, bool subDomain)
        {
            QueuedTask.Run(() =>
            {
                try
                {
                    featureIdCounter = 0;
                    int fieldCount = 0;
                    int fieldIndex = 0;
                    int selectionCount = 0;
                    Domain fieldDomain;
                    CodedValueDomain codedValueDomain;
                    IReadOnlyList<Field> fields;
                    object attributeValue;
                    string domainValue;
                    IReadOnlyList<Subtype> subtype1 = null;
                    string dataStore;
                    string subtype1_field = "";
                    SortedList<object, string> codedValuePairs;
                    selectionCount = inputStandAloneTable.GetSelection().GetCount();

                    dataStore = inputStandAloneTable.GetTable().GetDatastore().ToString();

                    if (dataStore == "ArcGIS.Core.Data.Geodatabase")
                    {
                        subtype1_field = inputStandAloneTable.GetTable().GetDefinition().GetSubtypeField();
                        subtypeField = subtype1_field;
                        subtype1 = inputStandAloneTable.GetTable().GetDefinition().GetSubtypes();
                    }
                    if (selectionCount > 0)
                    {
                        RowCursor fieldsCursor = inputStandAloneTable.Search();
                        fieldsCursor.MoveNext();
                        fields = fieldsCursor.Current.GetFields();
                        attributeTable = new object[inputStandAloneTable.GetSelection().GetCount(), inputFieldList.Count];
                        using (RowCursor rc = inputStandAloneTable.GetSelection().Search())
                        {
                            while (rc.MoveNext())
                            {
                                for (int i = 1; i < fieldNames.Count + 1; i++)
                                {
                                    fieldIndex = rc.Current.FindField(inputFieldList[i - 1]);
                                    if (subDomain)
                                    {
                                        if (fields[fieldIndex].Name == subtype1_field)
                                        {
                                            if (rc.Current[fieldIndex] != null)
                                            {
                                                int subTypeCode = Convert.ToInt32(attributeValue = rc.Current[fieldIndex]);
                                                for (int j = 0; j < subtype1.Count; j++)
                                                {
                                                    if (subtype1[j].GetCode() == subTypeCode)
                                                    {
                                                        attributeTable[featureIdCounter, fieldCount] = subtype1[j].GetName();
                                                    }
                                                }
                                            }

                                            fieldCount += 1;
                                        }
                                        else
                                        {
                                            fieldDomain = fields[fieldIndex].GetDomain();
                                            if (fieldDomain != null)
                                            {
                                                codedValueDomain = (CodedValueDomain)fieldDomain;
                                                codedValuePairs = codedValueDomain.GetCodedValuePairs();
                                                attributeValue = rc.Current[fieldIndex];
                                                if (attributeValue != null)
                                                {
                                                    domainValue = codedValuePairs[attributeValue];
                                                    attributeTable[featureIdCounter, fieldCount] = domainValue;
                                                    fieldCount += 1;
                                                }
                                                else
                                                {
                                                    attributeTable[featureIdCounter, fieldCount] = attributeValue;
                                                    fieldCount += 1;
                                                }
                                            }
                                            else
                                            {
                                                attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                                fieldCount += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                        fieldCount += 1;
                                    }
                                }
                                fieldCount = 0;
                                featureIdCounter += 1;
                            }
                        }
                    }
                    else
                    {
                        RowCursor fieldsCursor = inputStandAloneTable.Search();
                        fieldsCursor.MoveNext();
                        fields = fieldsCursor.Current.GetFields();
                        attributeTable = new object[inputStandAloneTable.GetTable().GetCount(), inputFieldList.Count];
                        using (RowCursor rc = inputStandAloneTable.Search())
                        {
                            while (rc.MoveNext())
                            {
                                for (int i = 1; i < fieldNames.Count + 1; i++)
                                {
                                    fieldIndex = rc.Current.FindField(inputFieldList[i - 1]);
                                    if (subDomain)
                                    {
                                        if (fields[fieldIndex].Name == subtype1_field)
                                        {
                                            if (rc.Current[fieldIndex] != null)
                                            {
                                                int subTypeCode = Convert.ToInt32(attributeValue = rc.Current[fieldIndex]);
                                                for (int j = 0; j < subtype1.Count; j++)
                                                {
                                                    if (subtype1[j].GetCode() == subTypeCode)
                                                    {
                                                        attributeTable[featureIdCounter, fieldCount] = subtype1[j].GetName();
                                                    }
                                                }
                                            }

                                            fieldCount += 1;
                                        }
                                        else
                                        {
                                            fieldDomain = fields[fieldIndex].GetDomain();
                                            if (fieldDomain != null)
                                            {
                                                codedValueDomain = (CodedValueDomain)fieldDomain;
                                                codedValuePairs = codedValueDomain.GetCodedValuePairs();
                                                attributeValue = rc.Current[fieldIndex];
                                                if (attributeValue != null)
                                                {
                                                    domainValue = codedValuePairs[attributeValue];
                                                    attributeTable[featureIdCounter, fieldCount] = domainValue;
                                                    fieldCount += 1;
                                                }
                                                else
                                                {
                                                    attributeTable[featureIdCounter, fieldCount] = attributeValue;
                                                    fieldCount += 1;
                                                }
                                            }
                                            else
                                            {
                                                attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                                fieldCount += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        attributeTable[featureIdCounter, fieldCount] = rc.Current[fieldIndex];
                                        fieldCount += 1;
                                    }
                                }
                                fieldCount = 0;
                                featureIdCounter += 1;
                            }
                        }
                    }
                    if (string.IsNullOrWhiteSpace(fileName) || fileName == "Name (Optional)")
                    {
                        fileName = selectedTable.Name;
                    }

                    if (excel == true)
                    {
                        Excel excelExport = new Excel();
                        if (fileName.Length > 31)
                            fileName = fileName.Substring(0, 31);
                        excelExport.ExportToExcel(orca_export.Properties.Settings.Default.q_DateTimeStamp, fileName, checkedFields, attributeTable, filePath);
                        excel = false;
                    }
                    else if (csv == true)
                    {
                        CreateCSVfile();
                        csv = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }
    }
}
