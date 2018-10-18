using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.Models.OrderTests
{
    public class DefaultValues
    {
        [Fact]
        public void CreateDate()
        {
            Order order = new Order();
            Assert.NotEqual(default(DateTime), order.CreateDate);
        }

        [Fact]
        public void Status()
        {
            Order order = new Order();
            Assert.Equal(OrderStatus.Accepted, order.Status);
        }
    }
}
