﻿using eshopAPI.Models;
using System;
using Xunit;


namespace eshopAPI.Tests.Models.OrderTests
{
    public class GetDescription
    {
        [Fact]
        public void AcceptedDescription()
        {
            OrderStatus status = OrderStatus.Accepted;
            Assert.Equal("Accepted", status.GetDescription()); 
        }

        [Fact]
        public void NonExistentDescription()
        {
            OrderStatus status = OrderStatus.Other;
            Assert.Equal("Other", status.GetDescription());
        }
    }
}
