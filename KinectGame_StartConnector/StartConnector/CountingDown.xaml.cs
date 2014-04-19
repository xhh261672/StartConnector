using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StartConnector
{
	public partial class CountingDown : UserControl
	{
		public CountingDown()
		{
			this.InitializeComponent();
		}

        public void SignalLight()
        {
            Storyboard sb = (Storyboard)Resources["signalLight"];
            sb.Begin();
        }
	}
}