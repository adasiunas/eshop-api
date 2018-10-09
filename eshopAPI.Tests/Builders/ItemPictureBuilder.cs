using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace eshopAPI.Tests.Builders
{
    public class ItemPictureBuilder
    {
        ItemPicture _itemPicture;
        static int _id = 1;
        public int ID { get { return _id++; } }
        public int ItemID { get; set; } = 1;
        public string Url { get; set; } = "www.google.com";

        public ItemPictureBuilder()
        {
            _itemPicture = WithDefaultValues();
        }

        public ItemPicture WithDefaultValues()
        {
            return new ItemPicture
            {
                ID = ID,
                ItemID = ItemID,
                URL = Url
            };
        }

        public ItemPicture Build()
        {
            return _itemPicture;
        }
    }
}
