using System;
using System.Collections.Generic;

namespace FilunK.backenddotnet_trial.Models.Book
{
    public class GoogleBooksVolumeItemModel
    {
        public string Kind { get; set; }

        public string Id { get; set; }

        public string Etag { get; set; }

        public GoogleBooksVolumeItemVolumeInfo VolumeInfo { get; set; }

    }
}
