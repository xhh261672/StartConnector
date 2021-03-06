﻿using System;
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
	public partial class FlyingBottle : UserControl
	{
        public BallState state;
        public Storyboard action;
        public int bId;
		public FlyingBottle()
		{
			this.InitializeComponent();
		}

        public void MoveBottle()
        {
            if (action != null && action.GetCurrentState() == ClockState.Active)
            {
                return;
            }
            switch(bId)
            {
                case 0:
                    BottleMostLeft();
                    break;
                case 1:
                    BottleObliqueLeft();
                    break;
                case 2:
                    BottleMiddle();
                    break;
                case 3:
                    BottleObliqueRight();
                    break;
                case 4:
                    BottleMostRight();
                    break;
                default:
                    break;
            }
        }

        private void BottleMostLeft()
        {
            action = (Storyboard)Resources["BottleMostLeft"];
            action.Begin();
        }

        private void BottleObliqueLeft()
        {
            action = (Storyboard)Resources["BottleObliqueLeft"];
            action.Begin();
        }

        private void BottleMiddle()
        {
            action = (Storyboard)Resources["BottleMiddle"];
            action.Begin();
        }

        private void BottleObliqueRight()
        {
            action = (Storyboard)Resources["BottleObliqueRight"];
            action.Begin();
        }
        private void BottleMostRight()
        {
            action = (Storyboard)Resources["BottleMostRight"];
            action.Begin();
        }

        public void RotateBottle()
        {
            ChangeBottleAngle.Angle = (ChangeBottleAngle.Angle + 10) % 360;
        }

        private void reduceScore()
        {
            GameWindow.skb.Shake();
            --Kernel.getScore;
            GameWindow.scoreText.Content = Kernel.getScore.ToString();
        }

        public void updateBottleState()
        {
            image.Source = null;
            state = BallState.NONE;
        }

        public void CalcScore()
        {
            TimeSpan actionCurTime = this.action.GetCurrentTime();
            if (state == BallState.DQUE && Kernel.InCatchScope(actionCurTime, 2.0, 2.2))
            {
                updateBottleState();
                if (bId == GameWindow.playerAngle)
                {
                    reduceScore();
                    GameWindow.playerAngle = 2;
                    bId = -1;
                }
            }
            else if (state == BallState.DQUE && Kernel.TimeLimitExsit(actionCurTime, 2.4))
            {
                updateBottleState();
            }
        }
	}
}