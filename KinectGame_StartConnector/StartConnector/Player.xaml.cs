using System;
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

	public partial class Player : UserControl
	{
        //bool DoingAction = false;
        Storyboard Action;
		public Player()
		{
			this.InitializeComponent();
		}

        public void ControlAction(int key)
        {
            //DoingAction && 
            if (Action!=null && (Action.GetCurrentState()==ClockState.Active))
            {
                //Action.Stop();
                //DoingAction = false;
                return;
            }
            
            switch(key)
            {
                case 1:
                    //DoingAction = true;
                    Action = (Storyboard)Resources["CatchMostLeft"];
                    Action.Begin();
                    break;
                case 2:
                    //DoingAction = true;
                    Action = (Storyboard)Resources["CatchObliqueLeft"];
                    Action.Begin();
                    break;
                case 3:
                    //DoingAction = true;
                    Action = (Storyboard)Resources["CatchMiddle"];
                    Action.Begin();
                    break;
                case 4:
                    //DoingAction = true;
                    Action = (Storyboard)Resources["CatchObliqueRight"];
                    Action.Begin();
                    break;
                case 5:
                    //DoingAction = true;
                    Action = (Storyboard)Resources["CatchMostRight"];
                    Action.Begin();
                    break;
                default:
                    break;
            }
        }
	}
}