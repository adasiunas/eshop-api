using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace eshopAPI.Tests.Builders
{
    public class ItemPictureBuilder
    {
        public static int LastId { get { return _id - 1; } }

        public int ID { get { return _id++; } }
        public int ItemID { get; set; } = 1;
        public string Url { get; set; } = "www.google.com";

        static int _id = 1;
        ItemPicture _itemPicture;

        public ItemPictureBuilder()
        {
            _itemPicture = WithDefaultValues();
        }

        ItemPicture WithDefaultValues()
        {
            return new ItemPicture
            {
                ItemID = ItemID,
                URL = Url
            };
        }

        public ItemPicture Build()
        {
            _itemPicture.ID = ID;
            return _itemPicture;
        }
    }
}
