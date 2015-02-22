/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Astrometry.Recognition
{
    public class ulonglong
    {
        public ulong Lo;
        public ulong Hi;

        public ulonglong(ulong hi, ulong lo)
        {
            Lo = lo;
            Hi = hi;
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to ulonglong return false.
            ulonglong p = obj as ulonglong;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return
                Lo == p.Lo &&
                Hi == p.Hi;
        }

        public bool Equals(ulonglong p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return
                Lo == p.Lo &&
                Hi == p.Hi;
        }

        public override int GetHashCode()
        {
            return (int)(Lo ^ Hi);
        }


        public static bool operator ==(ulonglong a, ulonglong b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return
                a.Lo == b.Lo &&
                a.Hi == b.Hi;
        }

        public static bool operator !=(ulonglong a, ulonglong b)
        {
            return !(a == b);
        }


    }
}
