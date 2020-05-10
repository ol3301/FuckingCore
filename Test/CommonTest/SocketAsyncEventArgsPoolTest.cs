using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Common.Utilities.Pool;
using NUnit.Framework;

namespace CommonTest
{
    public class SocketAsyncEventArgsPoolTest
    {
        [Test]
        public void Test()
        {
            SocketAsyncEventArgsPool pool = new SocketAsyncEventArgsPool();
            Assert.AreEqual(0, pool.Count);
            List<SocketAsyncEventArgs> list = new List<SocketAsyncEventArgs>();
            for (int i = 0; i < 3000; ++i)
                list.Add(pool.Pop());
            Assert.AreEqual(0, pool.Count);
            foreach (var item in list)
                pool.Push(item);
            Assert.AreEqual(3000, pool.Count);
            list.Clear();
            for (int i = 0; i < 3000; ++i)
                list.Add(pool.Pop());
            Assert.AreEqual(0, pool.Count);
            foreach (var item in list)
                pool.Push(item);
            Assert.AreEqual(3000, pool.Count);
        }
    }
}