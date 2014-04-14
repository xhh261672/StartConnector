﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Media.Animation;

namespace BallTrackPath
{
	/// <summary>
	/// RotateBall.xaml 的交互逻辑
	/// </summary>
	public partial class RotateBall : UserControl
	{
		public RotateBall()
		{
			this.InitializeComponent();
		}

        public void play()
        {
            Storyboard sb = (Storyboard)Resources["MoveFromLeftToCenter"];
            sb.Begin();
        }
	}
}