﻿using Microsoft.Maui.Dispatching;

namespace ControlGallery.Views.XAML
{
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

#if EXAMPLES
        [UIExample("Example")]
        public static ProgressBarDemoPage Example() => new ProgressBarDemoPage();
#endif
        ~ProgressBarDemoPage() => timer.Tick -= OnTimerTick;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += OnTimerTick;
            timer.Start();
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            progressBar.Progress += 0.01;
            if (progressBar.Progress == 1)
                timer.Stop();
        }
    }
}
