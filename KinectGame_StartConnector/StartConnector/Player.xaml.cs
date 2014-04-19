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


namespace StartConnector
{
	public partial class Player : UserControl
	{
        Storyboard Action;
		public Player()
		{
            InitializeComponent();
        }

        public void ControlAction(int key)
        {
            //DoingAction && 
            if (Action!=null && (Action.GetCurrentState()==ClockState.Active))
                return;
            
            GameWindow.playerAngle = key;
            
            switch(key)
            {
                case 0:
                    Action = (Storyboard)Resources["CatchMostLeft"];
                    Action.Begin();
                    break;
                case 1:
                    Action = (Storyboard)Resources["CatchObliqueLeft"];
                    Action.Begin();
                    break;
                case 2:
                    Action = (Storyboard)Resources["CatchMiddle"];
                    Action.Begin();
                    break;
                case 3:
                    Action = (Storyboard)Resources["CatchObliqueRight"];
                    Action.Begin();
                    break;
                case 4:
                    Action = (Storyboard)Resources["CatchMostRight"];
                    Action.Begin();
                    break;
                case 5:
                    Action = (Storyboard)Resources["SwipeRightHand"];
                    Action.Begin();
                    break;
                default:
                    break;
            }
        }
	}
}