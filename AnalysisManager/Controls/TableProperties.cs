﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Controls
{
    public partial class TableProperties : UserControl
    {
        public TableProperties()
        {
            InitializeComponent();
        }

        public void SetTableFormat(TableFormat tableFormat)
        {

        }

        public TableFormat GetTableFormat()
        {
            return new TableFormat();
        }
    }
}
