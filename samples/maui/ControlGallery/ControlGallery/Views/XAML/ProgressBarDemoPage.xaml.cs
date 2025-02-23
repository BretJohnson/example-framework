﻿using Microsoft.Maui.Dispatching;

namespace ControlGallery.Views.XAML
{
    [Preview(typeof(ProgressBar))]
    public partial class ProgressBarDemoPage : ContentPage
    {
        IDispatcherTimer timer;

        public ProgressBarDemoPage()
        {
            InitializeComponent();

#if EXAMPLES

            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += OnTimerTick;
            timer.Start();
#endif
        }

        ~ProgressBarDemoPage() => timer.Tick -= OnTimerTick;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += OnTimerTick;
            timer.Start();
        }

        void OnTimerTick(object? sender, EventArgs e)
        {
            progressBar.Progress += 0.01;
            if (progressBar.Progress == 1)
                timer.Stop();
        }
    }
}
