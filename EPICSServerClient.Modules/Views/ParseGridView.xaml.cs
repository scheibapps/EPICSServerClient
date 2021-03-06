﻿using EPICSServerClient.Helpers.Data;
using EPICSServerClient.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EPICSServerClient.Modules.Views
{
    /// <summary>
    /// Interaction logic for ParseGridView.xaml
    /// </summary>
    public partial class ParseGridView : UserControl
    {
        public ParseGridView()
        {
            InitializeComponent();
        }

        private void ParseDataGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            ParseData.PopulateDataGrid(sender as DataGrid);
        }
    }
}
