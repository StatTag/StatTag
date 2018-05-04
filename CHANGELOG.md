# Releases

## v3.3.0

* Support inserting StatTag fields into Word shapes
* Improved support for file paths with apostrophes in them.
* Addressed Stata syntax issue in editor
* Addressed error when updating a field and change tracking is enabled


### Component Versions

| Component             | File Version |
| --------------------- | ------------ |
| Core                  | 3.3.0.2      |
| StatTag.R             | 3.3.0.2      |
| StatTag.SAS           | 3.3.0.2      |
| StatTag.Stata         | 3.3.0.2      |
| StatTag               | 3.3.0.2      |
| DynamicInterop        | 0.7.4.0      |
| EPPlus                | 4.1.1.0      |
| log4net               | 2.0.8.0      |
| RDotNet               | 1.7.0.0      |
| RDotNet.Graphics      | 0.1.0.0      |
| RDotNet.NativeLibrary | 1.7.0.0      |
| ScintillaNET          | 3.6.3.1      |
| ScintillaNET FindReplaceDialog | 1.4.0.0      |
| SciLexer.dll          | 3.7.4.2      |



## v3.2.0

* Receive a notification when your code files change outside of StatTag
* Import CSV files for tables
* Specify how missing/empty values in tables should be displayed
* UI changes for User Settings dialog
* New Document Properties screen
* Handle Stata system variables (e.g., _b)


### Component Versions

| Component             | File Version |
| --------------------- | ------------ |
| Core                  | 3.2.0.5      |
| StatTag.R             | 3.2.0.5      |
| StatTag.SAS           | 3.2.0.5      |
| StatTag.Stata         | 3.2.0.5      |
| StatTag               | 3.2.0.5      |
| DynamicInterop        | 0.7.4.0      |
| EPPlus                | 4.1.1.0      |
| log4net               | 2.0.8.0      |
| RDotNet               | 1.7.0.0      |
| RDotNet.Graphics      | 0.1.0.0      |
| RDotNet.NativeLibrary | 1.7.0.0      |
| ScintillaNET          | 3.6.3.1      |
| ScintillaNET FindReplaceDialog | 1.4.0.0      |
| SciLexer.dll          | 3.7.4.1      |



## v3.1.0

* Automatically set working directory before executing code files
* Insert verbatim output at the current cursor location in document
* Documented process for managing software versions
* R
    * Allow named parameters in R image commands
    * Support beta version of R
* SAS
    * Will load images and CSVs (SAS) from relative paths
    * Support SAS ODS CSV PATH parameter
* Stata
    * Support specified formatting for Stata matrix command
    * Remove extra whitespace in Stata verbatim tags
    * Improved Stata error reporting


### Component Versions

| Component             | File Version |
| --------------------- | ------------ |
| Core                  | 3.1.0.2      |
| StatTag.R             | 3.1.0.2      |
| StatTag.SAS           | 3.1.0.2      |
| StatTag.Stata         | 3.1.0.2      |
| StatTag               | 3.1.0.2      |
| DynamicInterop        | 0.7.4.0      |
| RDotNet               | 1.7.0.0      |
| RDotNet.Graphics      | 0.1.0.0      |
| RDotNet.NativeLibrary | 1.7.0.0      |
| ScintillaNET          | 3.6.3.1      |
| SciLexer.dll          | 3.7.4.1      |



## v3.0

* Support for R!
* 'Verbatim' tag type to capture console output (now the default tag type)
* Enhanced tag creation dialog
* Allow single quotes in SAS file paths
* Corrected SAS syntax highlighting in editor
* Support for inserting Stata globals and constants



## v2.3

- Address issues processing comments in Stata
- Enhance dialog box visibility on high DPI displays



## v2.2

* Expand macro/variable names for file paths in both SAS and Stata



## v2.1

* When defining tags, you can now click in the full margin in the code editor
* When opening a Word document that is linked to code files, there is now an option (turned off by default) to re-run all of the code files
* Can now use 'dis' in addition to 'display' and 'di' for tagging values in Stata
* Included a 32-bit installer for 32-bit OSes



## v2.0

 - Compatible with SAS!
 - Ability to define a filter to exclude rows/columns when inserting tables
 - Removal of the "Include row/column names" option for tables (replaced by exclusion filter)
 - Disabled warning if an instance of Stata is running
 - System information being written to the debug file (if enabled)
 - Bug fix: Code files unlinked after saving document
 - Bux fix: Extended ASCII characters (e.g., en dash) not displaying
 - Bug fix: Code editor not displaying in Windows 10
 - Bug fix: Tag field metadata not stored properly in Word 2016



## v1.0

- Compatible with Stata!