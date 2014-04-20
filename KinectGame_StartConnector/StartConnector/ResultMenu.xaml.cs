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
	/// <summary>
	/// ResultMenu.xaml 的交互逻辑
	/// </summary>
	public partial class ResultMenu : UserControl
	{
        public static bool hasShowed = false;
		public ResultMenu()
		{
			this.InitializeComponent();
		}
        public void ShowResult()
        {
            hasShowed = true;
            //对静态数据进行修改

            double yourRate = 0.33;
            double yourScore = 4;
            double bestScore = 30;
            double bestCombos = 10;
            double yourCombos = 3;
            
            this.BestScoreNumber.Content = bestScore;
            this.YourScoreNumber.Content = yourScore;
            this.BestCombosNumber.Content = bestCombos;
            this.YourCombosNumber.Content = yourCombos;
            this.RatValue.Content = yourRate.ToString() + "%";


            Storyboard sb = (Storyboard)Resources["ShowResult"];
            //更改Rate的动画
            DoubleAnimationUsingKeyFrames yRate = sb.Children[0] as DoubleAnimationUsingKeyFrames;
            yRate.KeyFrames[1].Value = 1023.594 + (yourRate / 1.05) * 293.888;//1217.482;/*value = 

            //更改Score动画
            DoubleAnimationUsingKeyFrames yScoreFornt = sb.Children[1] as DoubleAnimationUsingKeyFrames;
            yScoreFornt.KeyFrames[1].Value = (yourScore / bestScore) * (-558);//-279;/*value=;
            
            DoubleAnimationUsingKeyFrames yScorePoint = sb.Children[3] as DoubleAnimationUsingKeyFrames;
            yScorePoint.KeyFrames[1].Value = (yourScore / bestScore) * (-558);
            
            DoubleAnimationUsingKeyFrames yScore = sb.Children[5] as DoubleAnimationUsingKeyFrames;
            yScore.KeyFrames[1].Value = (yourScore / bestScore) * (139.917);
            
            DoubleAnimationUsingKeyFrames yScoreLocation = sb.Children[6] as DoubleAnimationUsingKeyFrames;
            yScoreLocation.KeyFrames[1].Value = (yourScore / bestScore) * (-277.833);
            
            DoubleAnimationUsingKeyFrames yScoreNumber = sb.Children[9] as DoubleAnimationUsingKeyFrames;
            yScoreNumber.KeyFrames[1].Value = (yourScore / bestScore) * (-277.833);

            DoubleAnimationUsingKeyFrames yScoreFont = sb.Children[10] as DoubleAnimationUsingKeyFrames;
            yScoreFont.KeyFrames[1].Value = (yourScore / bestScore) * (-576);
            
            //更改combos动画
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

            //播放showRate的动画
            sb.Begin();
		}
        public void CloseMenu()
        {
            hasShowed = false;
            Storyboard sb = (Storyboard)Resources["CloseMenu"];
            sb.Begin();
        }
	}
}