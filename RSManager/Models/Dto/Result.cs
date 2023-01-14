using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Models.Dto
{
    internal class Result<T>
    {
        public T Item { get; private set; }
        public Error Error { get; private set; }
        public bool IsSuccess { get; private set; }

        private Result() { }        

        public static Result<T> Success(T item)
        {
            return new Result<T>() { Item = item, IsSuccess = true };
        }

        public static Result<T> Failure(Error error)
        {
            return new Result<T>() { Error = error };
        }
    }
}
