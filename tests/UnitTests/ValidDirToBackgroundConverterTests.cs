using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using App;
using Xunit;

namespace UnitTests
{
    public class ValidDirToBackgroundConverterTests
    {
        [Fact]
        public void TestConvert_True()
        {
            var converter = new ValidDirToBackgroundConverter();
            var actual = (SolidColorBrush)converter.Convert(true, typeof(bool), null, null);

            Assert.Equal(new SolidColorBrush(Colors.Transparent).Color, actual.Color);
        }

        [Fact]
        public void TestConvert_False()
        {
            var converter = new ValidDirToBackgroundConverter();
            var actual = (SolidColorBrush)converter.Convert(false, typeof(bool), null, null);

            Assert.Equal(new SolidColorBrush(Colors.Red).Color, actual.Color);
        }

        [Fact]
        public void TestConvert_Null()
        {
            var converter = new ValidDirToBackgroundConverter();
            var actual = (SolidColorBrush)converter.Convert(null, typeof(bool), null, null);

            Assert.Equal(new SolidColorBrush(Colors.Transparent).Color, actual.Color);
        }
    }
}
