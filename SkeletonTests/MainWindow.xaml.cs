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
using System.Globalization;
using System.Windows.Shapes;

namespace TicTacToe
{
    using System.Diagnostics;
    using System.Threading;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public partial class MainWindow : Window
    {
        string currentPlayer;
        Button[] cells;

        public MainWindow()
        {
            InitializeComponent();
            this.cells = new Button[] {
                this.cell00, this.cell01, this.cell02,
                this.cell10, this.cell11, this.cell12,
                this.cell20, this.cell21, this.cell22
            };

            foreach(Button cell in this.cells)
            {
                cell.Click += cell_Click;
            }
            NewGame();
        }

        void NewGame()
        { 
        
        }
        void cell_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            if (button.Content != null){
                return;
            }
            button.Content = currentPlayer;
            if ()
        }
        {

        }
    }
}
