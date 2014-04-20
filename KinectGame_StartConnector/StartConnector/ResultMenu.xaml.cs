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
using System.IO;
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


        public int getBest(string temp)
        {
            int bestscore = 0;
            FileStream F = new FileStream(temp + ".dat", FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader sr = new StreamReader(F, System.Text.Encoding.UTF8);
            string bestValue = sr.ReadLine();
            if (bestValue != null)
            {
                bestscore = int.Parse(bestValue);
            }
            //sr.Flush();
            sr.Close();
            F.Close();
            return bestscore;
        }
        public void setBest(int bestscore, string temp)
        {
            FileStream F = new FileStream(temp + ".dat", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter st = new StreamWriter(F, System.Text.Encoding.UTF8);
            st.WriteLine(bestscore.ToString());
            st.Flush();
            st.Close();
            F.Close();
        }
        public int getBestScore(int yourscore)
        {
            int bestscore = getBest("score");
            if (bestscore == 0)
            {
                setBest(yourscore, "score");
            }
            else if (bestscore < yourscore)
            {
                setBest(yourscore, "score");
            }
            bestscore = getBest("score");
            return bestscore;
        }
        public int getBestCombos(int yourcombos)
        {
            int bestcombos = getBest("combos");
            if (bestcombos == 0)
            {
                setBest(yourcombos, "combos");
            }
            else if (bestcombos < yourcombos)
            {
                setBest(yourcombos, "combos");
            }
            bestcombos = getBest("combos");
            return bestcombos;
        }
        public void ShowResult()
        {
            hasShowed = true;

            double yourRate = Kernel.CalcRate();
            double yourScore = Kernel.getScore;
            double bestScore = getBestScore(Kernel.getScore);
            double bestCombos = getBestCombos(Kernel.maxComboCount);
            double yourCombos = Kernel.maxComboCount;


            // Content and Text setting
            this.BestScoreNumber.Content = bestScore;
            this.YourScoreNumber.Content = yourScore;
            this.BestCombosNumber.Content = bestCombos;
            this.YourCombosNumber.Content = yourCombos;
            this.RatValue.Content = yourRate.ToString() + "%";


            Storyboard sb = (Storyboard)Resources["ShowResult"];
            // Rate
            DoubleAnimationUsingKeyFrames yRate = sb.Children[0] as DoubleAnimationUsingKeyFrames;
            yRate.KeyFrames[1].Value = 1023.594 + (yourRate / 105) * 293.888;//1217.482;/*value = 

            // Score
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
            
            // Combos
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

            // ShowRate
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