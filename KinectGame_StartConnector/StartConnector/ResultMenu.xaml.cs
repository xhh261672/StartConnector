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
            //对静态数据进行修改
            this.BestScoreNumber.Content = 100;
            this.YourScoreNumber.Content = 100;
            this.BestCombosNumber.Content = 100;
            this.YourCombosNumber.Content = 100;
            Storyboard sb = (Storyboard)Resources["ShowResult"];
            //更改Rate的动画
            DoubleAnimationUsingKeyFrames yRate = sb.Children[0] as DoubleAnimationUsingKeyFrames;
            yRate.KeyFrames[1].Value = 1217.482;/*value = 1023.594+(yourRate/105)*293.888*/

            //更改Score动画
            DoubleAnimationUsingKeyFrames yScoreFornt = sb.Children[1] as DoubleAnimationUsingKeyFrames;
            yScoreFornt.KeyFrames[1].Value = -279;/*value=(yourScore/bestScore)*(-558)*/
            
            DoubleAnimationUsingKeyFrames yScorePoint = sb.Children[3] as DoubleAnimationUsingKeyFrames;
            yScorePoint.KeyFrames[1].Value = -279;/*value=(yourScore/bestScore)*(-558)*/
            
            DoubleAnimationUsingKeyFrames yScore = sb.Children[5] as DoubleAnimationUsingKeyFrames;
            yScore.KeyFrames[1].Value = 69.771;/*value=(yourScore/bestScore)*(139.917)*/
            
            DoubleAnimationUsingKeyFrames yScoreLocation = sb.Children[6] as DoubleAnimationUsingKeyFrames;
            yScoreLocation.KeyFrames[1].Value = -138.541;//value=(yourScore/bestScore)*(-277.833)
            
            DoubleAnimationUsingKeyFrames yScoreNumber = sb.Children[9] as DoubleAnimationUsingKeyFrames;
            yScoreNumber.KeyFrames[1].Value = -279;//value=(yourScore/bestScore)*(-277.833)

            DoubleAnimationUsingKeyFrames yScoreFont = sb.Children[10] as DoubleAnimationUsingKeyFrames;
            yScoreFont.KeyFrames[1].Value = -576;//value=(yourScore/bestScore)*(-576)
            
            //更改combos动画
            DoubleAnimationUsingKeyFrames yCombosFornt = sb.Children[2] as DoubleAnimationUsingKeyFrames;
            yCombosFornt.KeyFrames[1].Value = -558;//value=(yourScore/bestScore)*(-558)
            
            DoubleAnimationUsingKeyFrames yCombosPoint = sb.Children[4] as DoubleAnimationUsingKeyFrames;
            yCombosPoint.KeyFrames[1].Value = -558;//value=(yourScore/bestScore)*(-558)
            
            DoubleAnimationUsingKeyFrames yCombos = sb.Children[7] as DoubleAnimationUsingKeyFrames;
            yCombos.KeyFrames[1].Value = -139.917;//value=(yourScore/bestScore)*(139.917)
            
            DoubleAnimationUsingKeyFrames yCombosLocation = sb.Children[8] as DoubleAnimationUsingKeyFrames;
            yCombosLocation.KeyFrames[1].Value = -277;//value=(yourScore/bestScore)*(-277.833)

            DoubleAnimationUsingKeyFrames yCombosNumber = sb.Children[11] as DoubleAnimationUsingKeyFrames;
            yCombosNumber.KeyFrames[1].Value = -279;//value=(yourScore/bestScore)*(-277.833)

            DoubleAnimationUsingKeyFrames yCombosFont = sb.Children[12] as DoubleAnimationUsingKeyFrames;
            yCombosFont.KeyFrames[1].Value = -576;//value=(yourScore/bestScore)*(-576)

            //播放showRate的动画
            sb.Begin();
		}
	}
}