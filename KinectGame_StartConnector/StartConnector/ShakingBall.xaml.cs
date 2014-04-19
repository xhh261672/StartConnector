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
	public partial class ShakingBall : UserControl
	{
        Storyboard sb;
		public ShakingBall()
		{
			this.InitializeComponent();
		}

        public void Shake()
        {
            if (sb != null && sb.GetCurrentState() == ClockState.Active)
                return;
            sb = (Storyboard)Resources["shaking"];
            sb.Begin();
        }
	}
}