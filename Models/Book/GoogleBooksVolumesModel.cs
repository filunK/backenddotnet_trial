using System;
using System.Collections.Generic;

namespace FilunK.backenddotnet_trial.Models.Book
{
    public class GoogleBooksVolumesModel
    {
        public string Kind { get; set; }

        public int TotalItems { get; set; }

        public GoogleBooksVolumeItemModel[] Items { get; set; }

    }
}
