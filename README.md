# arcgis_pro_excel_export
ArcGIS Pro .NET add-in for exporting layers and tables to Excel

# Requirements
1. ArcGIS Pro 2.9
2. Visual Studio 2019 Community
  - .NET Framework 4.8
  - Microsoft.Office.Interop.Excel
  - ArcGISProSDK_29_179968
3. Microsoft Excel (desktop app)

Installer Build Process
1.  Change build mode to Release Mode
2.  Clean All
3.  Build Solution
4.  Sign Add-in: C:\Program Files\ArcGIS\Pro\bin\ArcGISSignAddIn.exe
5.  Build Installer
6.  Sign Installer (.msi) with DigiCertUtil.exe
