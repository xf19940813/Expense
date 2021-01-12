using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Common.Helper
{
    public static class IdHelper
    {
        public static string CreateGuid()
        {
            Guid id = Guid.NewGuid();
            return id.ToString("N").ToLower();
        }
    }
}
