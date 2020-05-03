using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace CommonTest
{
    public class TempTest
    {
        [Test]
        public void TestStack()
        {
            Queue<string> queue = new Queue<string>();
            
            queue.Enqueue("1");
            queue.Enqueue("2");
            
            Assert.AreEqual("1", queue.Dequeue());
            Assert.AreEqual("2", queue.Dequeue());
            Assert.AreEqual(0, queue.Count);
        }
    }
}