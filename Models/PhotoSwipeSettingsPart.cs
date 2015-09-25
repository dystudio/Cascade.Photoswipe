using Orchard.ContentManagement;

namespace Cascade.PhotoSwipe.Models
{
    public class PhotoSwipeSettingsPart : ContentPart
    {
        // NOTE: Width and height settings for bothe expanded and thumbs are calculated to preserve the image proportions:
        // One or both of height and width must be non-zero
        // If one is set then the other is calculated from the image propotions
        // If both are set then the image is cropped. ShowHideOpacity is recommended in this case.
        // For expanded images the dimensions are 'max' dimensions. Viewport size is the limiting factor.

        /////////////// Basic Image settings ///////////////
        // Expanded image
        public int Width
        {
            get { return this.Retrieve(r => r.Width, 800); }
            set { this.Store(r => r.Width, value); }
        }
        public int Height
        {
            get { return this.Retrieve(r => r.Height, 600); }
            set { this.Store(r => r.Height, value); }
        }
        public int Quality
        {
            get { return this.Retrieve(r => r.Quality, 60); }
            set { this.Store(r => r.Quality, value); }
        }
        public string ScaleMode
        {
            get { return this.Retrieve(r => r.ScaleMode, "fit"); }
            set { this.Store(r => r.ScaleMode, value); }
        }
        public bool ShowLightboxCaptions
        {
            get { return this.Retrieve(r => r.ShowLightboxCaptions, true); }
            set { this.Store(r => r.ShowLightboxCaptions, value); }
        }
        public bool ShowLightboxTitles
        {
            get { return this.Retrieve(r => r.ShowLightboxTitles, true); }
            set { this.Store(r => r.ShowLightboxTitles, value); }
        }

        // Thumbnails
        public int ThumbWidth
        {
            get { return this.Retrieve(r => r.ThumbWidth, 100); }
            set { this.Store(r => r.ThumbWidth, value); }
        }
        public int ThumbHeight
        {
            get { return this.Retrieve(r => r.ThumbHeight, 67); }
            set { this.Store(r => r.ThumbHeight, value); }
        }
        public int ThumbnailQuality
        {
            get { return this.Retrieve(r => r.ThumbnailQuality, 60); }
            set { this.Store(r => r.ThumbnailQuality, value); }
        }
        public bool ShowThumbCaptions
        {
            get { return this.Retrieve(r => r.ShowThumbCaptions, true); }
            set { this.Store(r => r.ShowThumbCaptions, value); }
        }
        public bool ShowThumbTitles
        {
            get { return this.Retrieve(r => r.ShowThumbTitles, true); }
            set { this.Store(r => r.ShowThumbTitles, value); }
        }
        /////////////////// Swipe Options ///////////////////////////
        //showAnimationDuration
        public int OpenSpeed
        {
            get { return this.Retrieve(r => r.OpenSpeed, 500); }
            set { this.Store(r => r.OpenSpeed, value); }
        }
        //hideAnimationDuration
        public int CloseSpeed
        {
            get { return this.Retrieve(r => r.CloseSpeed, 500); }
            set { this.Store(r => r.CloseSpeed, value); }
        }

        public int PreloadBefore
        {
            get { return this.Retrieve(r => r.PreloadBefore, 1); }
            set { this.Store(r => r.PreloadBefore, value); }
        }
        public int PreloadAfter
        {
            get { return this.Retrieve(r => r.PreloadAfter, 2); }
            set { this.Store(r => r.PreloadAfter, value); }
        }

        public double BackgroundOpacity
        {
            get { return this.Retrieve(r => r.BackgroundOpacity, 1.0); }
            set { this.Store(r => r.BackgroundOpacity, value); }
        }
        public bool Loop
        {
            get { return this.Retrieve(r => r.Loop, true); }
            set { this.Store(r => r.Loop, value); }
        }
        public bool CloseOnScroll
        {
            get { return this.Retrieve(r => r.CloseOnScroll, true); }
            set { this.Store(r => r.CloseOnScroll, value); }
        }

        public bool CloseOnVerticalDrag
        {
            get { return this.Retrieve(r => r.CloseOnVerticalDrag, true); }
            set { this.Store(r => r.CloseOnVerticalDrag, value); }
        }
        public bool ShowHideOpacity
        {
            get { return this.Retrieve(r => r.ShowHideOpacity, false); }
            set { this.Store(r => r.ShowHideOpacity, value); }
        }

        ////////////////////// Cascade Swipe UI Options ////////////////////////
        public int TimeToIdle
        {
            get { return this.Retrieve(r => r.TimeToIdle, 4000); }
            set { this.Store(r => r.TimeToIdle, value); }
        }
        public int TimeToIdleOutside
        {
            get { return this.Retrieve(r => r.TimeToIdleOutside, 1000); }
            set { this.Store(r => r.TimeToIdleOutside, value); }
        }
        public bool ShowZoom
        {
            get { return this.Retrieve(r => r.ShowZoom, false); }
            set { this.Store(r => r.ShowZoom, value); }
        }
        public bool ShowShare
        {
            get { return this.Retrieve(r => r.ShowShare, false); }
            set { this.Store(r => r.ShowShare, value); }
        }
        public bool ShowFullscreen
        {
            get { return this.Retrieve(r => r.ShowFullscreen, false); }
            set { this.Store(r => r.ShowFullscreen, value); }
        }
    }
}