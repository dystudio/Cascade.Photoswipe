using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cascade.PhotoSwipe.Models
{
    public class UseragentRedirectSettingsPart : ContentPart
    {
        /// <summary>
        /// If this text is present anywhere in the useragent string then
        /// a redirection will occur.
        /// </summary>
        public string UseragentKeyword
        {
            get { return this.Retrieve(r=>r.UseragentKeyword);}
            set { this.Store(r => r.UseragentKeyword, value); }
        }

        /// <summary>
        /// The component of the url to be replaced eg: /show
        /// </summary>
        public string Path
        {
            get { return this.Retrieve(r => r.Path); }
            set { this.Store(r => r.Path, value); }
        }

        /// <summary>
        /// The replacement path eg: /mobile-show
        /// </summary>
        public string ReplacementPath
        {
            get { return this.Retrieve(r => r.ReplacementPath); }
            set { this.Store(r => r.ReplacementPath, value); }
        }
    }
}