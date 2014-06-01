using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
namespace Utility.Tests
{
    [TestClass()]
    public class RetryTests
    {
        int retries = 0;

        private bool SucceedAlways()
        {
            retries++;

            return true;
        }

        private bool SucessOnSubsequentTry()
        {
            retries++;
            if (retries == 1)
                throw new Exception("first try always fail");
            var tick = DateTime.Now.Ticks;
            var seed = int.Parse(tick.ToString().Substring(tick.ToString().Length -4, 4));
            var random = new Random(seed);
            var value = random.Next();
            var even = value % 2;

            if (even.ToString().EndsWith("0"))
                return true;
            else
                throw new Exception("fail senario in second trial as a random");
        }

        private bool FailAlways()
        {
            retries++;

            throw new Exception("first try always fail");
        }


        [TestMethod()]
        public void RetrySucceedsOnFirstTry()
        {
            retries = 0;
            var completedDelegate = Retry.Do<bool>(
                () => SucceedAlways()
                , TimeSpan.FromSeconds(1));

            Debug.WriteLine(retries.ToString());
            Assert.IsTrue(retries == 1);
        }

        [TestMethod()]
        public void RetrySucceedsOnSubsequentTry()
        {
            retries = 0;
            var completedDelegate = Retry.Do<bool>(
                () => SucessOnSubsequentTry()
                ,TimeSpan.FromSeconds(1));

            Debug.WriteLine(retries.ToString());
            Assert.IsTrue(retries > 1);
        }

        [TestMethod()]
        public void RetryFail()
        {
            retries = 0;
            try
            {
                var completedDelegate = Retry.Do<bool>(
                    () => FailAlways()
                    , TimeSpan.FromSeconds(1)
                    , 3);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            Debug.WriteLine(retries.ToString());
            Assert.IsTrue(retries == 3);
        }
    }
}
