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
using System.Linq;


namespace StartConnector
{
	
	public partial class ResultMenu : UserControl
	{
		public ResultMenu()
		{
			this.InitializeComponent();
		}

        public void ShowResult()
        {
            // static game data
            double yourRate = Kernel.CalcRate();
            int yourScore = Kernel.getScore;
            int bestScore = Kernel.bestScore;
            int yourCombos = Kernel.maxComboCount;
            int bestCombos = Kernel.bestCombos;

            this.YourScoreNumber.Content = yourScore;
            this.YourCombosNumber.Content = yourCombos;

            this.BestScoreNumber.Content = (bestScore == 0) ? 100 : bestScore;
            if (yourScore == 0) bestScore = 1;
            this.BestCombosNumber.Content = (bestCombos == 0) ? 100 : bestCombos;
            if (yourCombos == 0) bestCombos = 1;

            yourCombos = 3;
            yourScore = 2;
            bestCombos = 6;
            bestScore = 8;
            Storyboard sb = (Storyboard)Resources["ShowResult"];
            
            // Rate animation
            DoubleAnimationUsingKeyFrames yRate = sb.Children[0] as DoubleAnimationUsingKeyFrames;
            yRate.KeyFrames[1].Value = 1023.594 + (yourRate / 105.0) * 293.888;

            // Score animation
            DoubleAnimationUsingKeyFrames yScoreMoveFront = sb.Children[1] as DoubleAnimationUsingKeyFrames;
            yScoreMoveFront.KeyFrames[1].Value = (yourScore / bestScore) * (-558);
            
            DoubleAnimationUsingKeyFrames yScorePoint = sb.Children[3] as DoubleAnimationUsingKeyFrames;
            yScorePoint.KeyFrames[1].Value = (yourScore / bestScore) * (-558);
            
            DoubleAnimationUsingKeyFrames yScore = sb.Children[5] as DoubleAnimationUsingKeyFrames;
            yScore.KeyFrames[1].Value = (yourScore / bestScore) * (139.917);
            
            DoubleAnimationUsingKeyFrames yScoreLocation = sb.Children[6] as DoubleAnimationUsingKeyFrames;
            yScoreLocation.KeyFrames[1].Value = (yourScore / bestScore) * (-277.833);
            
            DoubleAnimationUsingKeyFrames yScoreNumber = sb.Children[9] as DoubleAnimationUsingKeyFrames;
            yScoreNumber.KeyFrames[1].Value =(yourScore / bestScore) * (-277.833);

            DoubleAnimationUsingKeyFrames yScoreFont = sb.Children[10] as DoubleAnimationUsingKeyFrames;
            yScoreFont.KeyFrames[1].Value = (yourScore / bestScore) * (-576);
            
            // Combos animation
            DoubleAnimationUsingKeyFrames yCombosFornt = sb.Children[2] as DoubleAnimationUsingKeyFrames;
            yCombosFornt.KeyFrames[1].Value = (yourCombos / bestCombos) * (-558);
            
            DoubleAnimationUsingKeyFrames yCombosPoint = sb.Children[4] as DoubleAnimationUsingKeyFrames;
            yCombosPoint.KeyFrames[1].Value = (yourCombos / bestCombos) * (-558);
            
            DoubleAnimationUsingKeyFrames yCombos = sb.Children[7] as DoubleAnimationUsingKeyFrames;
            yCombos.KeyFrames[1].Value = (yourCombos / bestCombos) * (139.917);
            
            DoubleAnimationUsingKeyFrames yCombosLocation = sb.Children[8] as DoubleAnimationUsingKeyFrames;
            yCombosLocation.KeyFrames[1].Value = (yourCombos / bestCombos) * (-277.833);

            DoubleAnimationUsingKeyFrames yCombosNumber = sb.Children[11] as DoubleAnimationUsingKeyFrames;
            yCombosNumber.KeyFrames[1].Value = (yourCombos / bestCombos) * (-277.833);

            DoubleAnimationUsingKeyFrames yCombosFont = sb.Children[12] as DoubleAnimationUsingKeyFrames;
            yCombosFont.KeyFrames[1].Value = (yourCombos / bestCombos) * (-576);

            sb.Begin();
		}
	}
}