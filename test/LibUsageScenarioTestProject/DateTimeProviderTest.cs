using System;
using System.Collections.Generic;
using DemoDateTimeProviderLib;
using Xunit;

namespace LibUsageScenarioTestProject
{
    public class DateTimeProviderTest
    {
        [Fact]
        public void TestDemoDateTimeProvider()
        {
            var utcNow = DateTime.UtcNow;
            
            //Act
            var provider1 = new DateTimeProvider(utcNow);
            var iso8601_1st = provider1.GetIso8601Format();
            var provider2 = new DateTimeProvider(iso8601_1st);
            var iso8601_2nd = provider2.GetIso8601Format();

            //Assert
            Assert.Equal(utcNow, provider1.GetStoredDateTime(), TimeSpan.FromSeconds(0.001));
            Assert.Equal(utcNow, provider2.GetStoredDateTime(), TimeSpan.FromSeconds(0.001));
            Assert.Equal(iso8601_1st, iso8601_2nd);
            
        }
    }
}
