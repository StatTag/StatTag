using System;
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
    public sealed partial class FigureProperties : UserControl
    {
        public FigureProperties()
        {
            InitializeComponent();
            Font = UIUtility.CreateScaledFont(Font, CreateGraphics());
        }

        public void SetFigureFormat(FigureFormat figureFormat)
        {
            
        }

        public FigureFormat GetFigureFormat()
        {
            return new FigureFormat();
        }
    }
}
