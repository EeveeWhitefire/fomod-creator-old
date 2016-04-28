﻿using System;
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

namespace Module.InfoEditor.View
{
    /// <summary>
    /// Логика взаимодействия для InfoEditorView.xaml
    /// </summary>
    public partial class InfoEditorView : UserControl
    {
        public InfoEditorView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ViewModel.InfoEditorViewModel).ModuleInformation.Author = "***ИЗМЕНЕНИЯ ВНЕ МОДЕЛИ***";
        }
    }
}
