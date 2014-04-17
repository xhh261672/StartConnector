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
	public partial class FlyingBall : UserControl
	{

        public static bool keepCombo = false;
        /********** DATA MEMBER **************/
        //public Image img = football1;
        public bool isClosed;
        public int eId;   // queue id
        public double xV; // x dir velocity 
        public double yV; // y dir velocity
        public BallState state;
        Storyboard action;
        public bool hasGotScore;
        /********** DATA MEMBER **************/
		public FlyingBall()
		{
			this.InitializeComponent();
		}

        public void RotateBall()
        {
            ChangeFootballAngle.Angle = (ChangeFootballAngle.Angle + 3) % 360;
        }
       

        public void CalcScore()
        {
            TimeSpan ts = this.action.GetCurrentTime();
            if (!hasGotScore && ts.TotalSeconds > 1.7 && 
                ts.TotalSeconds < 1.9)
            {
                if (eId == GameWindow.playerAngle)
                {
                    GameKernel.getScore += 1;
                    if (keepCombo)
                    {
                        GameKernel.comboCount++;
                    }
                    keepCombo = true;
                    hasGotScore = true;
                }
                else
                {
                    keepCombo = false;
                    GameKernel.maxComboCount =
                        (GameKernel.maxComboCount < GameKernel.comboCount)
                        ? GameKernel.comboCount : GameKernel.maxComboCount;
                }
                
            }
            
        }
        public void MoveBall()
        {

            if (action!=null && action.GetCurrentState()==ClockState.Active)
            {
                CalcScore();
                return;
            }
            hasGotScore = false;
            switch(eId)
            {
                case 0:
                    MoveLeftBall();
                    break;
                case 1:
                    MoveObliqueLeftBall();
                    break;
                case 2:
                    MoveMiddleBall();
                    break;
                case 3:
                    MoveObliqueRightBall();
                    break;
                case 4:
                    MoveRightBall();
                    break;
                default:
                    break;
            }
        }

        public void MoveLeftBall()
        {
            action = (Storyboard)Resources["MostLeft"];
            action.Begin();
        }

        public void MoveObliqueLeftBall()
        {
            action = (Storyboard)Resources["ObliqueLeft"];
            action.Begin();
        }
        public void MoveMiddleBall()
        {
            action = (Storyboard)Resources["Middle"];
            action.Begin();
        }

        public void MoveObliqueRightBall()
        {
            action = (Storyboard)Resources["ObliqueRight"];
            action.Begin();
        }

        public void MoveRightBall()
        {
            action = (Storyboard)Resources["MostRight"];
            action.Begin();
        }
	}
}