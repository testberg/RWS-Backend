using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using TranslationManagement.Api.Entities;

namespace TranslationManagement.Api.Tests.Entities
{
    public class TranslationJobEntityTests
    {
        [Fact]
        public void CurrentPrice_PriceGT_Zero_True()
        {

            //arrange
            var job = new TranslationJob { Id = Guid.NewGuid(), Price = 10 };

            // act
            var result = job.CurrentPricPrice();

            // assert
            Assert.True(result);

        }

        [Fact]
        public void CurrentPrice_PriceGT_Zero_False()
        {

            //arrange
            var job = new TranslationJob { Id = Guid.NewGuid(), Price = 0 };

            // act
            var result = job.CurrentPricPrice();

            // assert
            Assert.False(result);

        }

    }
}