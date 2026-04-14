# StatTag
StatTag is a free plug-in for conducting reproducible research and creating dynamic documents using Microsoft Word with the 
R, Stata and SAS statistical packages.

StatTag allows users to embed statistical output (estimates, tables, and figures) within Word and provides an interface to 
edit statistical code directly from Word. Statistical output can be individually or collectively updated from Word in one-click 
with a behind-the-scenes call to the statistical program. With StatTag, modification of a dataset or analysis no longer entails 
transcribing or re-copying results in to a manuscript or table. 

## Reporting Issues / Asking Questions

There are a few ways to contact the StatTag development team.

1. Email: StatTag@northwestern.edu
2. Open a GitHub issue
   1. If you have questions on a [specific code repository](https://github.com/orgs/StatTag/repositories), please feel free to open an issue within that repository.
   2. For general questions/issues, or if you are not sure which repository to open an issue under, [open an issue](https://github.com/StatTag/StatTag/issues/new/choose) under [the StatTag repo](https://github.com/StatTag/StatTag/issues).

## Contributing to StatTag
We welcome anyone that wishes to contribute to StatTag.  Please download the code and feel free to submit any proposed changes as a pull request.

## Development Setup

To compile StatTag, you will need the following:

* Visual Studio 2022 Community Edition
  * In Visual Studio Installer, select the following Workloads
    * .NET desktop development
	* Office/SharePoint development
  * From Visual Studio, install the Microsoft Visual Studio Installer Projects 2022
    * Extensions > Manage Extentions > Browse
	* Search for and install "Microsoft Visual Studio Installer"
	* Restart Visual Studio to complete installation
* Source code
  * Create a folder called "StatTag" to hold all of the code repositories
  * Clone the following repositories from GitHub into the "StatTag" folder you created
	* [StatTag/StatTag](https://github.com/StatTag/StatTag)
	* [StatTag/JupyterKernelManager](https://github.com/StatTag/JupyterKernelManager)
	* [StatTag/ScintillaNET-FindReplaceDialog](https://github.com/StatTag/ScintillaNET-FindReplaceDialog)
	* [StatTag/scintilla](https://github.com/StatTag/scintilla)
* [SAS Integration Technologies Client for Windows](https://support.sas.com/downloads/browse.htm?cat=56) (Requires a SAS login to access)


## Referencing StatTag
Please cite StatTag in any manuscripts that use it:

*Welty, L.J., Rasmussen, L.V., Baldridge, A.S., & Whitley, E. (2016). StatTag. Chicago, Illinois, United States: Galter Health Sciences Library. doi:10.18131/G36K76*
