using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MatchingMakingMonitor
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();

            var splitInts = Properties.Settings.Default.Overall9.Split(',')
                .Select(x => byte.Parse(x))
                .ToList();

            try
            {
                CpOverall9.SelectedColor = Color.FromRgb(splitInts[0], splitInts[1], splitInts[2]);

                splitInts = Properties.Settings.Default.Overall8.Split(',')
                    .Select(x => byte.Parse(x))
                    .ToList();

                CpOverall8.SelectedColor = Color.FromRgb(splitInts[0], splitInts[1], splitInts[2]);

                splitInts = Properties.Settings.Default.Overall7.Split(',')
                    .Select(x => byte.Parse(x))
                    .ToList();

                CpOverall7.SelectedColor = Color.FromRgb(splitInts[0], splitInts[1], splitInts[2]);

                splitInts = Properties.Settings.Default.Overall6.Split(',')
                    .Select(x => byte.Parse(x))
                    .ToList();

                CpOverall6.SelectedColor = Color.FromRgb(splitInts[0], splitInts[1], splitInts[2]);

                splitInts = Properties.Settings.Default.Overall5.Split(',')
                    .Select(x => byte.Parse(x))
                    .ToList();

                CpOverall5.SelectedColor = Color.FromRgb(splitInts[0], splitInts[1], splitInts[2]);

                splitInts = Properties.Settings.Default.Overall4.Split(',')
                    .Select(x => byte.Parse(x))
                    .ToList();

                CpOverall4.SelectedColor = Color.FromRgb(splitInts[0], splitInts[1], splitInts[2]);

                splitInts = Properties.Settings.Default.Overall3.Split(',')
                    .Select(x => byte.Parse(x))
                    .ToList();

                CpOverall3.SelectedColor = Color.FromRgb(splitInts[0], splitInts[1], splitInts[2]);

                splitInts = Properties.Settings.Default.Overall2.Split(',')
                    .Select(x => byte.Parse(x))
                    .ToList();

                CpOverall2.SelectedColor = Color.FromRgb(splitInts[0], splitInts[1], splitInts[2]);

                splitInts = Properties.Settings.Default.Overall1.Split(',')
                    .Select(x => byte.Parse(x))
                    .ToList();

                CpOverall1.SelectedColor = Color.FromRgb(splitInts[0], splitInts[1], splitInts[2]);
            } //end try
            catch (Exception ex)
            {
                Log("Exception Occurred Initializing Settings: " + ex.Message);
            } //end catch
        } //end Settings Initialize

        private void CpOverall9_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            try
            {
                if (CpOverall9.SelectedColor != null)
                {
                    Properties.Settings.Default.Overall9 = CpOverall9.SelectedColor.Value.R + "," +
                                                           CpOverall9.SelectedColor.Value.G + "," +
                                                           CpOverall9.SelectedColor.Value.B;
                    Properties.Settings.Default.Save();
                } //end if
            } //end try
            catch (Exception ex)
            {
                Log("Exception Occurred Setting Color 9 Setting: " + ex.Message);
            } //end catch
        } //end CpOverall9_OnSelectedColorChanged

        private void CpOverall8_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            try
            {
                if (CpOverall8.SelectedColor != null)
                {
                    Properties.Settings.Default.Overall8 = CpOverall8.SelectedColor.Value.R + "," +
                                                           CpOverall8.SelectedColor.Value.G + "," +
                                                           CpOverall8.SelectedColor.Value.B;
                    Properties.Settings.Default.Save();
                } //end if
            } //end try
            catch (Exception ex)
            {
                Log("Exception Occurred Setting Color 8 Setting: " + ex.Message);
            } //end catch
        } //end CpOverall8_OnSelectedColorChanged

        private void CpOverall7_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            try
            {
                if (CpOverall7.SelectedColor != null)
                {
                    Properties.Settings.Default.Overall7 = CpOverall7.SelectedColor.Value.R + "," +
                                                           CpOverall7.SelectedColor.Value.G + "," +
                                                           CpOverall7.SelectedColor.Value.B;
                    Properties.Settings.Default.Save();
                } //end if
            } //end try
            catch (Exception ex)
            {
                Log("Exception Occurred Setting Color 7 Setting: " + ex.Message);
            } //end catch
        } //end CpOverall7_OnSelectedColorChanged

        private void CpOverall6_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            try
            {
                if (CpOverall6.SelectedColor != null)
                {
                    Properties.Settings.Default.Overall6 = CpOverall6.SelectedColor.Value.R + "," +
                                                           CpOverall6.SelectedColor.Value.G + "," +
                                                           CpOverall6.SelectedColor.Value.B;
                    Properties.Settings.Default.Save();
                } //end if
            } //end try
            catch (Exception ex)
            {
                Log("Exception Occurred Setting Color 6 Setting: " + ex.Message);
            } //end catch
        } //end CpOverall6_OnSelectedColorChanged

        private void CpOverall5_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            try
            {
                if (CpOverall5.SelectedColor != null)
                {
                    Properties.Settings.Default.Overall5 = CpOverall5.SelectedColor.Value.R + "," +
                                                           CpOverall5.SelectedColor.Value.G + "," +
                                                           CpOverall5.SelectedColor.Value.B;
                    Properties.Settings.Default.Save();
                } //end if
            } //end try
            catch (Exception ex)
            {
                Log("Exception Occurred Setting Color 5 Setting: " + ex.Message);
            } //end catch
        } //end CpOverall5_OnSelectedColorChanged

        private void CpOverall4_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            try
            {
                if (CpOverall4.SelectedColor != null)
                {
                    Properties.Settings.Default.Overall4 = CpOverall4.SelectedColor.Value.R + "," +
                                                           CpOverall4.SelectedColor.Value.G + "," +
                                                           CpOverall4.SelectedColor.Value.B;
                    Properties.Settings.Default.Save();
                } //end if
            } //end try
            catch (Exception ex)
            {
                Log("Exception Occurred Setting Color 4 Setting: " + ex.Message);
            } //end catch
        } //end CpOverall4_OnSelectedColorChanged

        private void CpOverall3_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            try
            {
                if (CpOverall3.SelectedColor != null)
                {
                    Properties.Settings.Default.Overall3 = CpOverall3.SelectedColor.Value.R + "," +
                                                           CpOverall3.SelectedColor.Value.G + "," +
                                                           CpOverall3.SelectedColor.Value.B;
                    Properties.Settings.Default.Save();
                } //end if
            } //end try
            catch (Exception ex)
            {
                Log("Exception Occurred Setting Color 3 Setting: " + ex.Message);
            } //end catch
        } //end CpOverall3_OnSelectedColorChanged

        private void CpOverall2_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            try
            {
                if (CpOverall2.SelectedColor != null)
                {
                    Properties.Settings.Default.Overall2 = CpOverall2.SelectedColor.Value.R + "," +
                                                           CpOverall2.SelectedColor.Value.G + "," +
                                                           CpOverall2.SelectedColor.Value.B;
                    Properties.Settings.Default.Save();
                } //end if
            } //end try
            catch (Exception ex)
            {
                Log("Exception Occurred Setting Color 2 Setting: " + ex.Message);
            } //end catch
        } //end CpOverall2_OnSelectedColorChanged

        private void CpOverall1_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            try
            {
                if (CpOverall1.SelectedColor != null)
                {
                    Properties.Settings.Default.Overall1 = CpOverall1.SelectedColor.Value.R + "," + CpOverall1.SelectedColor.Value.G + "," + CpOverall1.SelectedColor.Value.B;
                    Properties.Settings.Default.Save();
                } //end if
            } //end try
            catch (Exception ex)
            {
                Log("Exception Occurred Setting Color 2 Setting: " + ex.Message);
            } //end catch
        } //end CpOverall1_OnSelectedColorChanged

        private void Log(string message)
        {
            try
            {
                using (var sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/Log.txt", true))
                {
                    sw.WriteLine(DateTime.Now + " - " + message);
                } //end using
            } //end try
            catch (Exception ex)
            {
                //ignore
            } //end catch
        } //end Log
    }
}
