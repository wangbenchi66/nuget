using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class ResultTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue(true);
        }
    }

    public readonly struct Result<TSuccess, TError>
    {
        public bool IsSuccess => !IsError;
        public bool IsError { get; }
        private readonly TSuccess? _success;
        private readonly TError? _error;
        private Result(TSuccess success)
        {
            this.IsError = false;
            _success = success;
            _error = default;
        }
        private Result(TError error)
        {
            this.IsError = true;
            _error = error;
            _success = default;
        }
        public static implicit operator Result<TSuccess, TError>(TSuccess success) => new(success);
        public static implicit operator Result<TSuccess, TError>(TError error) => new(error);
        public TResult Match<TResult>(
            Func<TSuccess, TResult> success,
            Func<TError, TResult> error) =>
            !IsError ? success(_success!) : error(_error!);
    }
}