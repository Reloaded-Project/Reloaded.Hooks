namespace Reloaded.Hooks.Internal
{
    /// <summary>
    /// Defines a physical address range with a minimum and maximum address.
    /// </summary>
    public struct AddressRange
    {
        public nuint StartPointer;
        public nuint EndPointer;

        public AddressRange(nuint startPointer, nuint endPointer)
        {
            StartPointer = startPointer;
            EndPointer = endPointer;
        }

        public static AddressRange FromStartAndLength(nuint start, nuint length)
        {
            return new AddressRange(start, start + length);
        }

        /// <summary>
        /// Returns true if a specified point is contained in this address range.
        /// </summary>
        public bool Contains(nuint point)
        {
            return PointInRange(ref this, point);
        }

        /// <summary>
        /// Returns true if a number "point", is between min and max of address range.
        /// </summary>
        private bool PointInRange(ref AddressRange range, nuint point)
        {
            if (point >= range.StartPointer &&
                point <= range.EndPointer)
                return true;

            return false;
        }
    }
}
