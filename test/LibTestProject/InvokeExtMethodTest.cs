using System;
using GranDen.CallExtMethodLib;
using Iso8601ExtMethodLib;
using Xunit;

namespace LibTestProject
{
    public class InvokeExtMethodTest
    {
        [Fact]
        public void InvokeToIso8601StringExtMethodTest()
        {
            //Arrange
            var utcNow = DateTime.UtcNow;
            var helper = new ExtMethodInvoker("Iso8601ExtMethodLib");
            var normalInvocationResult = utcNow.ToIso8601String(true);

            //Act
            var result = helper.Invoke<string>("ToIso8601String", utcNow, true);

            //Assert
            Assert.Equal(normalInvocationResult, result);
        }

        [Fact]
        public void InvokeFromIso8601StringExtMethodTest()
        {
            //Arrange
            var utcNow = DateTime.UtcNow;
            var helper = new ExtMethodInvoker("Iso8601ExtMethodLib");
            var iso8601String = utcNow.ToIso8601String(true);

            //Act
            var result = helper.Invoke<DateTime>("FromIso8601String", iso8601String);

            //Assert
            Assert.Equal(utcNow, result, TimeSpan.FromSeconds(0.001));

        }
    }
}
