using System;
using System.Collections.Generic;

namespace FilunK.backenddotnet_trial.Models.Book
{
    public class GoogleBooksVolumeItemVolumeInfo
    {
        public string Title { get; set; }

        public string[] Authors { get; set; }

        public string PublishedDate { get; set; }

        public string Description { get; set; }

        public GoogleBooksVolumeItemIndustryIdentifier[] IndustryIdentifiers { get; set; }

        public int PageCount { get; set; }
    }
}
