// -----------------------------------------------------------------------
// <copyright file="SystemConstants.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SystemConstants
    {
        public const int    NULL     = -1;

        public const char   DELIMITER   = '|';

        public const string START_TAG   = "[{";
        public const string END_TAG     = "}]";
        public const string LINE_BREAK  = "\n";

        public const int    KILOBYTE    = 1024;
        public const int    MEGABYTE    = 1048576;
        public const int    GIGABYTE    = 1073741824;
        public const long   TERABYTE    = 1099511627776;
        public const long   PETABYTE    = 1125899906842624;

        public const int    HASH_PRIME  = 29;

        public const long   NO_START_DATE   = 0;
        public const long   NO_EXPIRY_DATE  = 999999;

        public DateTime MIN_DATE = new DateTime();
    }

    public enum Status
    {
        Ok = 0,
        Error = 1,
        NoMatch = 2,
    }
    
}
