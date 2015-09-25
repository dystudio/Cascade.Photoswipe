using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;
using Orchard.Tags.Models;

namespace Cascade.PhotoSwipe.Models {
    public class MediaTagCloudPart : ContentPart {
        internal readonly LazyField<IList<TagCount>> _tagCountField = new LazyField<IList<TagCount>>();

        public IList<TagCount> TagCounts { get { return _tagCountField.Value; } }

        /// <summary>
        /// The number of levels or buckets used to categorize tags.
        /// Only relevent if you provide a distinct css class for each bucket.
        /// </summary>
        public int Buckets { 
            get { return this.Retrieve(r => r.Buckets, 5); }
            set { this.Store(r => r.Buckets, value); }
        }

        /// <summary>
        /// This is copied from Orchard.Tags. Dunno how it's intended to be used.
        /// </summary>
        public string Slug {
            get { return this.Retrieve(r => r.Slug); }
            set { this.Store(r => r.Slug, value); }
        }

        /// <summary>
        /// If true, place the most popular tags first.
        /// Default is to sort alphabetically.
        /// </summary>
        public bool SortByPopularity
        {
            get { return this.Retrieve(r => r.SortByPopularity); }
            set { this.Store(r => r.SortByPopularity, value); }
        }

        /// <summary>
        /// Whether to display the number of content items
        /// associated with this term, in the generated menu item text
        /// </summary>
        public bool DisplayContentCount
        {
            get { return this.Retrieve(r => r.DisplayContentCount); }
            set { this.Store(r=>r.DisplayContentCount, value); }
        }

        /// <summary>
        /// The currently selected tag. This is usually assigned the 'active' class so it's highlighted.
        /// </summary>
        public string CurrentTagName
        {
            get { return this.Retrieve(r => r.CurrentTagName); }
            set { this.Store(r => r.CurrentTagName, value); }
        }

    }
}