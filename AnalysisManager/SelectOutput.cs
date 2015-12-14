﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnalysisManager.Core.Models;

namespace AnalysisManager
{
    public partial class SelectOutput : Form
    {
        protected List<CodeFile> Files = new List<CodeFile>(); 

        public SelectOutput(List<CodeFile> files = null)
        {
            InitializeComponent();
            Files = files;
        }

        public Annotation[] GetSelectedAnnotations()
        {
            return clbOutput.CheckedItems.Cast<Annotation>().ToArray();
        }

        private void SelectOutput_Load(object sender, EventArgs e)
        {
            foreach (var file in Files)
            {
                foreach (var annotation in file.Annotations)
                {
                    clbOutput.Items.Add(annotation);
                }
            }
        }
    }
}