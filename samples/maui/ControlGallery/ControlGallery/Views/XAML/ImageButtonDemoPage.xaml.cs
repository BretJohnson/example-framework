﻿using System;
using Microsoft.Maui.Controls;

namespace ControlGallery.Views.XAML
{
    [Preview(typeof(ImageButton))]
    public partial class ImageButtonDemoPage : ContentPage
    {
        int clickTotal;

        public ImageButtonDemoPage()
        {
            InitializeComponent();
        }

        void OnImageButtonClicked(object sender, EventArgs e)
        {
            clickTotal += 1;
            label.Text = $"{clickTotal} ImageButton click{(clickTotal == 1 ? "" : "s")}";
        }
    }
}
