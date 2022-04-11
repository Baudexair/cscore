﻿using System.Threading;

namespace com.csutil {
    
    public static class ThreadSafety {

        /// <summary> Flag will be flipped to true if it was false, otherwise will return false </summary>
        public static bool FlipToTrue(ref int flag) {
            AssertV2.IsTrue(flag == 0 || flag == 1, "int was not 0 or 1 but " + flag);
            return Interlocked.CompareExchange(ref flag, 1, 0) == 0;
        }

        /// <summary> Flag will be flipped to false if it was true, otherwise will return false </summary>
        public static bool FlipToFalse(ref int flag) {
            AssertV2.IsTrue(flag == 0 || flag == 1, "int was not 0 or 1 but " + flag);
            return Interlocked.CompareExchange(ref flag, 0, 1) == 1;
        }

    }
    
}