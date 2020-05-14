﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ColorPicker.ColorPickingFunctionality;

namespace ColorPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TransparentWindow _transparentWindow = new TransparentWindow();
        private DispatcherTimer _updateTimer = new DispatcherTimer();
        private Color _previousColor;

        public MainWindow()
        {
            InitializeComponent();
            ConfigureTransparentWindow();
            ConfigureUpdateTimer();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
            ActivateColorSelectionMode();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _transparentWindow.Close();
            base.OnClosing(e);
        }

        private void ConfigureTransparentWindow()
        {
            _transparentWindow.AddActionCallback(ActionBroker.ActionTypes.Click, OnTransparentScreenClick);
            _transparentWindow.AddActionCallback(ActionBroker.ActionTypes.Escape, OnTransparentScreenEscape);
        }

        private void ConfigureUpdateTimer()
        {
            _updateTimer.Tick += UpdateCurrentColor;
            _updateTimer.Interval = new TimeSpan(1000);
        }

        private void OnTransparentScreenClick(object sender, EventArgs e)
        {
            SetColor(PixelColorFinder.GetColorUnderCursor());
            DeactivateColorSelectionMode();
        }

        private void OnTransparentScreenEscape(object sender, EventArgs e)
        {
            SetColor(_previousColor);
            DeactivateColorSelectionMode();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                OnTransparentScreenEscape(sender, e);
            }
        }

        private void OnNewColorButtonClick(object sender, EventArgs e)
        {
            if (NewColorButton.IsChecked ?? false)
            {
                ActivateColorSelectionMode();
            }
            else
            {
                DeactivateColorSelectionMode();
            }
        }

        private void UpdateCurrentColor(object sender, EventArgs e)
        {
            if (NewColorButton.IsChecked ?? false)
            {
                SetColor(PixelColorFinder.GetColorUnderCursor());
            }
        }

        private void SetColor(Color color)
        {
            ColorPreviewRectangle.Fill = new SolidColorBrush(color);
            RgbTextBox.Text = $"{color.R}, {color.G}, {color.B}";
            HexTextBox.Text = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        private void ActivateColorSelectionMode()
        {
            _previousColor = (ColorPreviewRectangle.Fill as SolidColorBrush).Color;
            NewColorButton.IsChecked = true;
            _transparentWindow.Show();
            _updateTimer.Start();
        }

        private void DeactivateColorSelectionMode()
        {
            NewColorButton.IsChecked = false;
            _transparentWindow.Hide();
            _updateTimer.Stop();
        }
    }
}
